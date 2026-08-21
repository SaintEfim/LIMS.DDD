using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

namespace LIMS.Service.LaboratoryOperations.API.Apis.StudyTemplateShanpshot;

public class StudyTemplateServices(StudyTemplateQueries queries)
{
    public StudyTemplateQueries Queries { get; } = queries;
}
