using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;

public sealed record MeasuredValueDto(Guid Id, Guid StudyId, InputParameterDto InputParameter, double? Value)
{
    public static MeasuredValueDto FromDomain(
        MeasuredValue mv,
        InputParameterSnapshot templateParameter)
    {
        return new MeasuredValueDto(mv.Id.Value, mv.StudyId.Value, InputParameterDto.FromSnapshot(templateParameter),
            mv.Value);
    }
}
