using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters;

public sealed class InputParameterCommandsHandler(
    IStudyTemplateRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
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
            return nameResult.CastFailure<Guid>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<Guid>();
        }

        var aliasResult = AliasName.Create(command.AliasName);
        if (aliasResult.IsFailure)
        {
            return aliasResult.CastFailure<Guid>();
        }

        var specification = Specification.Create(command.MinValue, command.MaxValue);
        if (specification.IsFailure)
        {
            return specification.CastFailure<Guid>();
        }

        var addResult = templateResult.GetValue()
            .AddInputParameter(nameResult.GetValue(), descResult.GetValue(), aliasResult.GetValue(),
                specification.GetValue());
        if (addResult.IsFailure)
        {
            return addResult.CastFailure<Guid>();
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : Result<Guid, Exception>.Success(addResult.GetValue()
                .Id.Value);
    }

    public async Task<Result<None, Exception>> RemoveAsync(
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
            return removeResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
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
                return nameResult.CastFailure<None>();
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return descResult.CastFailure<None>();
            }

            description = descResult.GetValue();
        }

        AliasName? aliasName = null;
        if (command.AliasName is not null)
        {
            var aliasResult = AliasName.Create(command.AliasName);
            if (aliasResult.IsFailure)
            {
                return aliasResult.CastFailure<None>();
            }

            aliasName = aliasResult.GetValue();
        }

        var updateResult = templateResult.GetValue()
            .UpdateInputParameter(new InputParameterId(parameterId), name, description, aliasName, command.MinValue,
                command.MaxValue);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
