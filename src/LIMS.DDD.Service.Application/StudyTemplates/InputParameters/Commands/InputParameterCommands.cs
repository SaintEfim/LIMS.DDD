using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;

public sealed class InputParameterCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Guid, Exception>.Failure(templateResult.Error!);

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure) return Result<Guid, Exception>.Failure(nameResult.Error!);

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure) return Result<Guid, Exception>.Failure(descResult.Error!);

        var aliasResult = AliasName.Create(command.AliasName);
        if (aliasResult.IsFailure) return Result<Guid, Exception>.Failure(aliasResult.Error!);

        var specification = Specification.Create(command.MinValue, command.MaxValue);
        if (specification.IsFailure) return Result<Guid, Exception>.Failure(specification.Error!);

        var addResult = templateResult.GetValue()
            .AddInputParameter(nameResult.GetValue(), descResult.GetValue(), aliasResult.GetValue(),
                specification.GetValue());
        if (addResult.IsFailure) return Result<Guid, Exception>.Failure(addResult.Error!);

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(addResult.GetValue()
                .Id.Value);
    }

    public async Task<Result<Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var removeResult = templateResult.GetValue()
            .RemoveInputParameter(new InputParameterId(parameterId));
        if (removeResult.IsFailure) return Result<Exception>.Failure(removeResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid parameterId,
        UpdateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure) return Result<Exception>.Failure(nameResult.Error!);
            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return Result<Exception>.Failure(descResult.Error!);
            description = descResult.GetValue();
        }

        AliasName? aliasName = null;
        if (command.AliasName is not null)
        {
            var aliasResult = AliasName.Create(command.AliasName);
            if (aliasResult.IsFailure) return Result<Exception>.Failure(aliasResult.Error!);
            aliasName = aliasResult.GetValue();
        }

        Specification? specification = null;
        if (command.MaxValue is not null || command.MinValue is not null)
        {
            var specificationResult = Specification.Create(command.MinValue, command.MaxValue);
            if (specificationResult.IsFailure) return Result<Exception>.Failure(specificationResult.Error!);
            specification = specificationResult.GetValue();
        }

        var updateResult = templateResult.GetValue()
            .UpdateInputParameter(new InputParameterId(parameterId), name, description, aliasName, specification);
        if (updateResult.IsFailure) return Result<Exception>.Failure(updateResult.Error!);

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

    private async Task<Result<Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
