using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

namespace LIMS.Service.LaboratoryOperations.API.Apis.StudyTemplateShanpshot;

public class StudyTemplateSnapshotServices(StudyTemplateQueries queries)
{
    public StudyTemplateQueries Queries { get; } = queries;
}
