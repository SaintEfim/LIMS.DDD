using Application.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;

public sealed class MeasuredValueQueries(
    IStudyRepository repository,
    IStudyTemplateSnapshotRepository snapshotRepository) : IQueries
{
    public async Task<MeasuredValueDto?> GetByIdAsync(
        Guid studyId,
        Guid measuredValueId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        if (study is null)
        {
            throw new KeyNotFoundException("study not found");
        }

        var measuredValue = study.MeasuredValues.SingleOrDefault(mv => mv.Id == new MeasuredValueId(measuredValueId));

        if (measuredValue is null)
        {
            return null;
        }

        var inputParameter = await snapshotRepository.GetInputParameterAsync(study.StudyTemplateId,
            measuredValue.InputParameterId, cancellationToken);
        return inputParameter is null
            ? throw new KeyNotFoundException("input parameter not found")
            : MeasuredValueDto.FromDomain(measuredValue, inputParameter);
    }

    public async Task<ICollection<MeasuredValueDto>> GetAllByStudyIdAsync(
        Guid studyId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        if (study is null)
        {
            return [];
        }

        var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (snapshot is null)
        {
            throw new KeyNotFoundException("template not found");
        }

        var inputParameters =
            await snapshotRepository.GetInputParameterSnapshotsAsync(study.StudyTemplateId, cancellationToken);

        var inputParametersDict = inputParameters.ToDictionary(ip => ip.Id);

        return study.MeasuredValues
            .Select(mv =>
            {
                var inputParameter = inputParametersDict.GetValueOrDefault(mv.InputParameterId);
                return MeasuredValueDto.FromDomain(mv,
                    inputParameter ?? throw new KeyNotFoundException("input parameter not found"));
            })
            .ToList();
    }
}
