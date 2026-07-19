using LIMS.DDD.Service.Domain.StudyTemplate.Parameter;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class StudyTemplateParameterRepository
    : RepositoryBase<StudyTemplateParameter>,
        IStudyTemplateParameterRepository
{
    public StudyTemplateParameterRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }
}
