using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

namespace LIMS.DDD.Service.Application.Studies.MeasuredValues;

public sealed class MeasuredValueQueries(IStudyRepository repository)
{
    public async Task<MeasuredValueDto?> GetByIdAsync(
        Guid studyId,
        Guid measuredValueId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        var measuredValue = study?.MeasuredValues.SingleOrDefault(mv => mv.Id == new MeasuredValueId(measuredValueId));

        return measuredValue is not null ? MeasuredValueDto.FromDomain(measuredValue) : null;
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

        return study.MeasuredValues
            .Select(MeasuredValueDto.FromDomain)
            .ToList();
    }
}
