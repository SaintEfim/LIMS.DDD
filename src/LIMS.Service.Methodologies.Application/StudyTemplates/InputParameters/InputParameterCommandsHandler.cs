using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters.Commands;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;

public sealed class InputParameterCommandsHandler(IStudyTemplateRepository repository, IUnitOfWork unitOfWork)
    : ICommandsHandler
{
    public async Task<Result<Guid, ApplicationError>> CreateAsync(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<Guid>();
        }

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

        var aliasResult = AliasName.Create(command.AliasName);
        if (aliasResult.IsFailure)
        {
            return new DomainRuleViolation(aliasResult.GetError());
        }

        var specification = Specification.Create(command.MinValue, command.MaxValue);
        if (specification.IsFailure)
        {
            return new DomainRuleViolation(specification.GetError());
        }

        var addResult = templateResult.GetValue()
            .AddInputParameter(nameResult.GetValue(), descResult.GetValue(), aliasResult.GetValue(),
                specification.GetValue());
        if (addResult.IsFailure)
        {
            return new DomainRuleViolation(addResult.GetError());
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : addResult.GetValue()
                .Id.Value;
    }

    public async Task<Result<None, ApplicationError>> RemoveAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var removeResult = templateResult.GetValue()
            .RemoveInputParameter(new InputParameterId(parameterId));
        if (removeResult.IsFailure)
        {
            return new DomainRuleViolation(removeResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
        Guid studyTemplateId,
        Guid parameterId,
        UpdateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

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

        AliasName? aliasName = null;
        if (command.AliasName is not null)
        {
            var aliasResult = AliasName.Create(command.AliasName);
            if (aliasResult.IsFailure)
            {
                return new DomainRuleViolation(aliasResult.GetError());
            }

            aliasName = aliasResult.GetValue();
        }

        var updateResult = templateResult.GetValue()
            .UpdateInputParameter(new InputParameterId(parameterId), name, description, aliasName, command.MinValue,
                command.MaxValue);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, ApplicationError>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template with id '{studyTemplateId}' not found.");
        }

        return template;
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }
}
