using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

public sealed class CalculationRuleQueries(IStudyTemplateRepository repository)
{
    public async Task<CalculationRuleDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid ruleId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var rule = studyTemplate?.CalculationRules.SingleOrDefault(r => r.Id == new CalculationRuleId(ruleId));

        return rule != null ? CalculationRuleDto.FromDomain(rule) : null;
    }

    public async Task<ICollection<CalculationRuleDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null) return [];

        return studyTemplate.CalculationRules
            .Select(CalculationRuleDto.FromDomain)
            .ToList();
    }
}
