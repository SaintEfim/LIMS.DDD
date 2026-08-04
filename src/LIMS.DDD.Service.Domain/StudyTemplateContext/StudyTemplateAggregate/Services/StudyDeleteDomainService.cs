using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate; // Ссылка на сущность Study из другого контекста
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;

public sealed class StudyTemplateDeletionDomainService
{
    public Result<Exception> Delete(
        StudyTemplate template,
        IReadOnlyCollection<Study> associatedStudies)
    {
        if (template.Status != Status.Draft)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete template '{template.Name.Value}' in '{template.Status.Name}' status. " +
                "Only 'Draft' templates can be deleted. Use 'Archive' for Active/Archived templates."));
        }

        if (associatedStudies.Count > 0)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete template '{template.Name.Value}' because it has {associatedStudies.Count} associated study(ies). " +
                "Please archive the template instead of deleting it to preserve historical data integrity."));
        }

        return template.Delete();
    }
}
