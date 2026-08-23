using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed class StudyTemplateSnapshotCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyTemplateSnapshotRepository snapshotRepository) : ICommandsHandler
{
    public async Task<Result<StudyTemplateSnapshot, ApplicationError>> CreateAsync(
        CreateStudyTemplateSnapshotCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateId = new StudyTemplateId(command.Id);

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var descriptionResult = Description.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            return new DomainRuleViolation(descriptionResult.GetError());
        }

        var revisionResult = Revision.Create(command.Revision);
        if (revisionResult.IsFailure)
        {
            return new DomainRuleViolation(revisionResult.GetError());
        }

        var inputParametersResult = MapInputParameters(templateId, command.InputParameters);
        if (inputParametersResult.IsFailure)
        {
            return inputParametersResult.CastFailure<StudyTemplateSnapshot>();
        }

        var resultDefinitionsResult = MapResultDefinitions(templateId, command.ResultDefinitions);
        if (resultDefinitionsResult.IsFailure)
        {
            return resultDefinitionsResult.CastFailure<StudyTemplateSnapshot>();
        }

        var calculationRulesResult = MapCalculationRules(templateId, command.CalculationRules);
        if (calculationRulesResult.IsFailure)
        {
            return calculationRulesResult.CastFailure<StudyTemplateSnapshot>();
        }

        var snapshot = new StudyTemplateSnapshot(templateId, revisionResult.GetValue(), nameResult.GetValue(),
            descriptionResult.GetValue(), inputParametersResult.GetValue(), resultDefinitionsResult.GetValue(),
            calculationRulesResult.GetValue());

        return await SaveAsync(snapshot, cancellationToken);
    }

    private static Result<IReadOnlyList<InputParameterSnapshot>, ApplicationError> MapInputParameters(
        StudyTemplateId templateId,
        IReadOnlyList<InputParameterDto> dtos)
    {
        var snapshots = new List<InputParameterSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            var nameResult = Name.Create(dto.Name);
            if (nameResult.IsFailure)
            {
                return new DomainRuleViolation(nameResult.GetError());
            }

            var descriptionResult = Description.Create(dto.Description);
            if (descriptionResult.IsFailure)
            {
                return new DomainRuleViolation(descriptionResult.GetError());
            }

            var aliasResult = AliasName.Create(dto.AliasName);
            if (aliasResult.IsFailure)
            {
                return new DomainRuleViolation(aliasResult.GetError());
            }

            var specificationResult = Specification.Create(dto.SpecMin, dto.SpecMax);
            if (specificationResult.IsFailure)
            {
                return new DomainRuleViolation(specificationResult.GetError());
            }

            var snapshot = new InputParameterSnapshot(new InputParameterId(dto.Id), templateId, nameResult.GetValue(),
                descriptionResult.GetValue(), aliasResult.GetValue(), specificationResult.GetValue());

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private static Result<IReadOnlyList<ResultDefinitionSnapshot>, ApplicationError> MapResultDefinitions(
        StudyTemplateId templateId,
        IReadOnlyList<CreateResultDefinitionCommand> dtos)
    {
        var snapshots = new List<ResultDefinitionSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            if (dto.UnitId is null)
            {
                return new ValidationError($"Result definition '{dto.ResultInstance}' has no unit specified.");
            }

            var specificationResult = Specification.Create(dto.SpecMin, dto.SpecMax);
            if (specificationResult.IsFailure)
            {
                return new DomainRuleViolation(specificationResult.GetError());
            }

            var snapshot = new ResultDefinitionSnapshot(new ResultDefinitionId(dto.Id), templateId, dto.ResultInstance,
                new UnitId(dto.UnitId.Value), specificationResult.GetValue());

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private static Result<IReadOnlyList<CalculationRuleSnapshot>, ApplicationError> MapCalculationRules(
        StudyTemplateId templateId,
        IReadOnlyList<CalculationRuleDto> dtos)
    {
        var snapshots = new List<CalculationRuleSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            var nameResult = Name.Create(dto.Name);
            if (nameResult.IsFailure)
            {
                return new DomainRuleViolation(nameResult.GetError());
            }

            var descriptionResult = Description.Create(dto.Description);
            if (descriptionResult.IsFailure)
            {
                return new DomainRuleViolation(descriptionResult.GetError());
            }

            var formulaResult = FormulaExpression.Create(dto.FormulaExpression);
            if (formulaResult.IsFailure)
            {
                return new DomainRuleViolation(formulaResult.GetError());
            }

            var snapshot = new CalculationRuleSnapshot(new CalculationRuleId(dto.Id), templateId, nameResult.GetValue(),
                descriptionResult.GetValue(), formulaResult.GetValue(), new ResultDefinitionId(dto.ResultDefinitionId));

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private async Task<Result<StudyTemplateSnapshot, ApplicationError>> SaveAsync(
        StudyTemplateSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        try
        {
            snapshotRepository.Add(snapshot);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return snapshot;
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save StudyTemplateSnapshot: {ex.Message}");
        }
    }
}
