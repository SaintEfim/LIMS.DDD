using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid> CreateAsync(
        CreateStudyTemplateCommand createCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = StudyTemplate.Create(new Name(createCommand.Name),
            new Description(createCommand.Description), new Revision(createCommand.Revision));

        repository.Add(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return studyTemplate.Id.Value;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null) return false;

        Name? name = updateCommand.Name is not null ? new Name(updateCommand.Name) : null;
        Description? desc = updateCommand.Description is not null ? new Description(updateCommand.Description) : null;
        Revision? rev = updateCommand.Revision is not null ? new Revision(updateCommand.Revision) : null;

        studyTemplate.UpdatePartial(name, desc, rev);

        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        if (studyTemplate is null) return false;

        repository.Remove(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ChangeStatusAsync(
        Guid id,
        ChangeStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);
        if (studyTemplate is null) return false;

        if (!Enum.TryParse<Status>(command.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Invalid status value: {command.Status}");

        studyTemplate.ChangeStatus(newStatus);

        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
