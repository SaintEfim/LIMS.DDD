using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;
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

        var specification = new Specification(command.MinValue, command.MaxValue);

        var addResult = templateResult.Value!.AddInputParameter(
            nameResult.Value, descResult.Value, aliasResult.Value, specification);
        if (addResult.IsFailure) return Result<Guid, Exception>.Failure(addResult.Error!);

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(addResult.Value!.Id.Value);
    }

    public async Task<Result<Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var removeResult = templateResult.Value!.RemoveInputParameter(new InputParameterId(parameterId));
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
            name = nameResult.Value;
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return Result<Exception>.Failure(descResult.Error!);
            description = descResult.Value;
        }

        AliasName? aliasName = null;
        if (command.AliasName is not null)
        {
            var aliasResult = AliasName.Create(command.AliasName);
            if (aliasResult.IsFailure) return Result<Exception>.Failure(aliasResult.Error!);
            aliasName = aliasResult.Value;
        }

        Specification? specification = null;
        if (command.MinValue is not null || command.MaxValue is not null)
            specification = new Specification(command.MinValue, command.MaxValue);

        var updateResult = templateResult.Value!.UpdateInputParameter(
            new InputParameterId(parameterId), name, description, aliasName, specification);
        if (updateResult.IsFailure) return Result<Exception>.Failure(updateResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid studyTemplateId, CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<Exception>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(
                new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
