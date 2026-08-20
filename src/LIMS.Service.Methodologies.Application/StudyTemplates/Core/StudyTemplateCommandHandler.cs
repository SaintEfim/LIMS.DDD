using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using Guides.Messages;
using LIMS.Service.Methodologies.Application.StudyTemplates.Core.Commands;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Services;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;
using RabbitMq.Library.QuickStart.Abstractions;
using Revision = LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects.Revision;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.Core;

public sealed class StudyTemplateCommandsHandler(
    IMessageBus busService,
    IStudyTemplateRepository repository,
    IUnitOfWork unitOfWork,
    StudyTemplateVersioningService domainService)
{
    public async Task<Result<StudyTemplate, Exception>> CreateAsync(
        CreateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<StudyTemplate>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<StudyTemplate>();
        }

        var revResult = Revision.Create(command.Revision);
        if (revResult.IsFailure)
        {
            return revResult.CastFailure<StudyTemplate>();
        }

        var createResult = StudyTemplate.Create(nameResult.GetValue(), descResult.GetValue(), revResult.GetValue());
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<StudyTemplate>();
        }

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult;
        }

        var template = templateResult.GetValue();

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure)
            {
                return nameResult.CastFailure<StudyTemplate>();
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return descResult.CastFailure<StudyTemplate>();
            }

            description = descResult.GetValue();
        }

        var effectiveName = name ?? template.Name;
        var effectiveDescription = description ?? template.Description;

        var updateResult = template.UpdatePartial(effectiveName, effectiveDescription);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<StudyTemplate>();
        }

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);

        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<StudyTemplate>();
        }

        var template = templateResult.GetValue();

        if (!Status.TryParse(statusCommand, out var newStatus) || newStatus is null)
        {
            return new InvalidOperationException($"Unknown status '{statusCommand}'.");
        }

        var changeResult = template.ChangeStatus(newStatus);

        if (changeResult.IsFailure)
        {
            return changeResult.CastFailure<StudyTemplate>();
        }

        if (newStatus != Status.Active)
        {
            return await SaveChangesAsync(template, cancellationToken);
        }

        var message = new StudyTemplatePublishedMessage(Id: template.Id.Value, Name: template.Name.Value,
            Description: template.Description.Value ?? string.Empty, Revision: template.Revision.Value,
            InputParameters: template.InputParameters
                .Where(p => !p.IsDeleted)
                .Select(p => new InputParameterMessage(p.Id.Value, p.Name.Value, p.Description.Value, p.AliasName.Value,
                    p.Specification.MinValue, p.Specification.MaxValue))
                .ToList(), ResultDefinitions: template.ResultDefinitions
                .Where(r => !r.IsDeleted)
                .Select(r => new ResultDefinitionMessage(r.Id.Value, r.ResultInstance, r.UnitId.Value,
                    r.Specification.MinValue, r.Specification.MaxValue))
                .ToList(), CalculationRules: template.CalculationRules
                .Where(c => !c.IsDeleted)
                .Select(c => new CalculationRuleMessage(c.Id.Value, c.Name.Value, c.Description.Value ?? string.Empty,
                    c.FormulaExpression.Value, c.ResultDefinitionId.Value))
                .ToList());

        await busService.SendAsync(message, cancellationToken);

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<None, Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var template = templateResult.GetValue();

        var deleteResult = template.Delete();
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new None();
    }

    public async Task<Result<Guid, Exception>> CreateRevisionAsync(
        Guid originalId,
        CreateStudyTemplateRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var originalResult = await GetTemplateForChangeAsync(originalId, cancellationToken);
        if (originalResult.IsFailure)
        {
            return originalResult.CastFailure<Guid>();
        }

        var revisionResult = Revision.Create(command.NewRevision);
        if (revisionResult.IsFailure)
        {
            return revisionResult.CastFailure<Guid>();
        }

        var createResult = domainService.CreateNewRevision(originalResult.GetValue(), revisionResult.GetValue());
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<Guid>();
        }

        var creationResult = createResult.GetValue();

        var saveResult = await SaveNewAsync(creationResult, cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : creationResult.Id.Value;
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(id), cancellationToken);
        return template is null
            ? new KeyNotFoundException($"StudyTemplate with id {id} not found.")
            : template;
    }

    private async Task<Result<StudyTemplate, Exception>> SaveNewAsync(
        StudyTemplate template,
        CancellationToken cancellationToken = default)
    {
        try
        {
            repository.Add(template);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return template;
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save StudyTemplate: {ex.Message}", ex);
        }
    }

    private async Task<Result<StudyTemplate, Exception>> SaveChangesAsync(
        StudyTemplate template,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return template;
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save StudyTemplate: {ex.Message}", ex);
        }
    }
}
