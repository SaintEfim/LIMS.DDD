using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;

public sealed class InputParameterCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> AddInputParameterAsync(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        return await Name.Create(command.Name)
            .Bind(name => Description.Create(command.Description)
                .Map(description => (name, description)))
            .Bind(tuple => AliasName.Create(command.AliasName)
                .Map(aliasName => (tuple.name, tuple.description, aliasName)))
            .Bind(tuple => studyTemplate.AddInputParameter(tuple.name, tuple.description, tuple.aliasName,
                new Specification(command.MinValue, command.MaxValue)))
            .Bind(async result =>
            {
                try
                {
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(result.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(new Exception($"Failed to save parameter: {ex.Message}",
                        ex));
                }
            });
    }

    public async Task<Result<Exception>> RemoveInputParameterAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var removeResult = studyTemplate.RemoveInputParameter(new InputParameterId(parameterId));

        if (removeResult.IsFailure)
        {
            return Result<Exception>.Failure(removeResult.Error!);
        }

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to remove parameter: {ex.Message}", ex));
        }
    }

    public async Task<Result<Exception>> UpdateInputParameterAsync(
        Guid studyTemplateId,
        Guid parameterId,
        UpdateInputParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));

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
        {
            specification = new Specification(command.MinValue, command.MaxValue);
        }

        var updateResult = studyTemplate.UpdateInputParameter(new InputParameterId(parameterId), name, description,
            aliasName, specification);

        if (updateResult.IsFailure) return updateResult;

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to update InputParameter: {ex.Message}", ex));
        }
    }
}
