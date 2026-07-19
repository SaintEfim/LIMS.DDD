using LIMS.DDD.Service.Domain.StudyTemplate.Result;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class StudyTemplateResultRepository
    : RepositoryBase<StudyTemplateResult>,
        IStudyTemplateResultRepository
{
    public StudyTemplateResultRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }
}
