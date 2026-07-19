using LIMS.DDD.Service.Domain.StudyTemplate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class StudyTemplateRepository
    : RepositoryBase<StudyTemplate>,
        IStudyTemplateRepository
{
    public StudyTemplateRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(x => x.Parameters)
            .Include(x => x.Results)
            .ToListAsync(cancellationToken);
    }
}
