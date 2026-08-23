using Application.SeedWork;
using Application.SeedWork.Errors;
using Broker.Messages;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Application.StudyTemplates.Core.Commands;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Services;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;
using RabbitMq.Library.QuickStart.Abstractions;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.Core;

public sealed class StudyTemplateCommandsHandler(
    IMessageBus busService,
    IStudyTemplateRepository repository,
    IUnitOfWork unitOfWork,
    StudyTemplateVersioningService domainService) : ICommandsHandler
{
    public async Task<Result<StudyTemplate, ApplicationError>> CreateAsync(
        CreateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return new DomainRuleViolation(descResult.GetError());
        }

        var revResult = Revision.Create(command.Revision);
        if (revResult.IsFailure)
        {
            return new DomainRuleViolation(revResult.GetError());
        }

        var newTemplate = new StudyTemplate(nameResult.GetValue(), descResult.GetValue(), revResult.GetValue());

        return await SaveNewAsync(newTemplate, cancellationToken);
    }

    public async Task<Result<StudyTemplate, ApplicationError>> UpdateAsync(
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
                return new DomainRuleViolation(nameResult.GetError());
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return new DomainRuleViolation(descResult.GetError());
            }

            description = descResult.GetValue();
        }

        var effectiveName = name ?? template.Name;
        var effectiveDescription = description ?? template.Description;

        var updateResult = template.UpdatePartial(effectiveName, effectiveDescription);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<StudyTemplate, ApplicationError>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult;
        }

        var template = templateResult.GetValue();

        if (!Status.TryParse(statusCommand, out var newStatus) || newStatus is null)
        {
            return new ValidationError($"Unknown status '{statusCommand}'.");
        }

        var changeResult = template.ChangeStatus(newStatus);
        if (changeResult.IsFailure)
        {
            return new DomainRuleViolation(changeResult.GetError());
        }

        if (newStatus != Status.Active)
        {
            return await SaveChangesAsync(template, cancellationToken);
        }

        var message = new StudyTemplatePublishedMessage(template.Id.Value, template.Name.Value,
            template.Description.Value ?? string.Empty, template.Revision.Value, template.InputParameters
                .Where(p => !p.IsDeleted)
                .Select(p => new InputParameterMessage(p.Id.Value, p.Name.Value, p.Description.Value, p.AliasName.Value,
                    p.Specification.MinValue, p.Specification.MaxValue))
                .ToList(), template.ResultDefinitions
                .Where(r => !r.IsDeleted)
                .Select(r => new ResultDefinitionMessage(r.Id.Value, r.ResultInstance, r.UnitId.Value,
                    r.Specification.MinValue, r.Specification.MaxValue))
                .ToList(), template.CalculationRules
                .Where(c => !c.IsDeleted)
                .Select(c => new CalculationRuleMessage(c.Id.Value, c.Name.Value, c.Description.Value ?? string.Empty,
                    c.FormulaExpression.Value, c.ResultDefinitionId.Value))
                .ToList());

        await busService.SendAsync(message, cancellationToken);

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> DeleteAsync(
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
            return new DomainRuleViolation(deleteResult.GetError());
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to delete StudyTemplate: {ex.Message}");
        }

        return new None();
    }

    public async Task<Result<Guid, ApplicationError>> CreateRevisionAsync(
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
            return new DomainRuleViolation(revisionResult.GetError());
        }

        var createResult = domainService.CreateNewRevision(originalResult.GetValue(), revisionResult.GetValue());
        if (createResult.IsFailure)
        {
            return new DomainRuleViolation(createResult.GetError());
        }

        var creationResult = createResult.GetValue();

        var saveResult = await SaveNewAsync(creationResult, cancellationToken);
        return saveResult.IsFailure ? saveResult.CastFailure<Guid>() : creationResult.Id.Value;
    }

    private async Task<Result<StudyTemplate, ApplicationError>> GetTemplateForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(id), cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template with id '{id}' not found.");
        }

        return template;
    }

    private async Task<Result<StudyTemplate, ApplicationError>> SaveNewAsync(
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
            return new PersistenceError($"Failed to save StudyTemplate: {ex.Message}");
        }
    }

    private async Task<Result<StudyTemplate, ApplicationError>> SaveChangesAsync(
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
            return new PersistenceError($"Failed to save StudyTemplate: {ex.Message}");
        }
    }
}
