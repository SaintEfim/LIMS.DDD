using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

namespace LIMS.Service.LaboratoryOperations.API.Apis.StudyTemplateSnapshot;

public class StudyTemplateSnapshotServices(StudyTemplateQueries queries)
{
    public StudyTemplateQueries Queries { get; } = queries;
}
