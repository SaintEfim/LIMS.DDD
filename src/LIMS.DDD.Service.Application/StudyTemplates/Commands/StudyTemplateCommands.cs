using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid?> CreateAsync(
        CreateStudyTemplateCommand createCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = StudyTemplate.Create(new Name(createCommand.Name),
            new Description(createCommand.Description), new Revision(createCommand.Revision));

        repository.Add(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return studyTemplate.Id.Value;
    }

    public async Task UpdateAsync(
        StudyTemplate studyTemplate,
        CancellationToken cancellationToken = default)
    {
        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(id, cancellationToken);

        if (studyTemplate is null) return false;

        repository.Remove(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
