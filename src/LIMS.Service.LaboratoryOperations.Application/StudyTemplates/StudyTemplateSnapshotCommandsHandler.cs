using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed class StudyTemplateSnapshotCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyTemplateSnapshotRepository snapshotRepository)
{
    public async Task<Result<StudyTemplateSnapshot, Exception>> CreateAsync(
        CreateStudyTemplateSnapshotCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateId = new StudyTemplateId(command.Id);

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<StudyTemplateSnapshot>();
        }

        var descriptionResult = Description.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult.CastFailure<StudyTemplateSnapshot>();
        }

        var revisionResult = Revision.Create(command.Revision);
        if (revisionResult.IsFailure)
        {
            return revisionResult.CastFailure<StudyTemplateSnapshot>();
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

        var snapshot = new StudyTemplateSnapshot(templateId, Name: nameResult.GetValue(),
            Description: descriptionResult.GetValue(), Revision: revisionResult.GetValue(),
            Parameters: inputParametersResult.GetValue(), Results: resultDefinitionsResult.GetValue(),
            CalculationRules: calculationRulesResult.GetValue());

        return await SaveAsync(snapshot, cancellationToken);
    }

    private Result<IReadOnlyList<InputParameterSnapshot>, Exception> MapInputParameters(
        StudyTemplateId templateId,
        IReadOnlyList<InputParameterDto> dtos)
    {
        var snapshots = new List<InputParameterSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            var nameResult = Name.Create(dto.Name);
            if (nameResult.IsFailure)
            {
                return nameResult.CastFailure<IReadOnlyList<InputParameterSnapshot>>();
            }

            var descriptionResult = Description.Create(dto.Description);
            if (descriptionResult.IsFailure)
            {
                return descriptionResult.CastFailure<IReadOnlyList<InputParameterSnapshot>>();
            }

            var aliasResult = AliasName.Create(dto.AliasName);
            if (aliasResult.IsFailure)
            {
                return aliasResult.CastFailure<IReadOnlyList<InputParameterSnapshot>>();
            }

            var specificationResult = Specification.Create(dto.SpecMin, dto.SpecMax);
            if (specificationResult.IsFailure)
            {
                return specificationResult.CastFailure<IReadOnlyList<InputParameterSnapshot>>();
            }

            var snapshot = new InputParameterSnapshot(new InputParameterId(dto.Id), templateId, nameResult.GetValue(),
                descriptionResult.GetValue(), aliasResult.GetValue(), specificationResult.GetValue());

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private Result<IReadOnlyList<ResultDefinitionSnapshot>, Exception> MapResultDefinitions(
        StudyTemplateId templateId,
        IReadOnlyList<ResultDefinitionDto> dtos)
    {
        var snapshots = new List<ResultDefinitionSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            var specificationResult = Specification.Create(dto.SpecMin, dto.SpecMax);
            if (specificationResult.IsFailure)
            {
                return specificationResult.CastFailure<IReadOnlyList<ResultDefinitionSnapshot>>();
            }

            var snapshot = new ResultDefinitionSnapshot(new ResultDefinitionId(dto.Id), templateId, dto.ResultInstance,
                new UnitId(dto.UnitId), specificationResult.GetValue());

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private static Result<IReadOnlyList<CalculationRuleSnapshot>, Exception> MapCalculationRules(
        StudyTemplateId templateId,
        IReadOnlyList<CalculationRuleDto> dtos)
    {
        var snapshots = new List<CalculationRuleSnapshot>(dtos.Count);

        foreach (var dto in dtos)
        {
            var nameResult = Name.Create(dto.Name);
            if (nameResult.IsFailure)
            {
                return nameResult.CastFailure<IReadOnlyList<CalculationRuleSnapshot>>();
            }

            var descriptionResult = Description.Create(dto.Description);
            if (descriptionResult.IsFailure)
            {
                return descriptionResult.CastFailure<IReadOnlyList<CalculationRuleSnapshot>>();
            }

            var formulaResult = FormulaExpression.Create(dto.FormulaExpression);
            if (formulaResult.IsFailure)
            {
                return formulaResult.CastFailure<IReadOnlyList<CalculationRuleSnapshot>>();
            }

            var snapshot = new CalculationRuleSnapshot(new CalculationRuleId(dto.Id), templateId, nameResult.GetValue(),
                descriptionResult.GetValue(), formulaResult.GetValue(), new ResultDefinitionId(dto.ResultDefinitionId));

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private async Task<Result<StudyTemplateSnapshot, Exception>> SaveAsync(
        StudyTemplateSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        try
        {
            snapshotRepository.Add(snapshot);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<StudyTemplateSnapshot, Exception>.Success(snapshot);
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save StudyTemplateSnapshot: {ex.Message}", ex);
        }
    }
}
