using Domain.SeedWork;
using Domain.SeedWork.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

public sealed class InputParameterSnapshot : SoftDeletableModel
{
    private InputParameterSnapshot()
    {
    }

    public InputParameterSnapshot(
        InputParameterId id,
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        Id = id;
        StudyTemplateId = studyTemplateId;
        Name = name;
        Description = description;
        AliasName = aliasName;
        Specification = specification;
    }

    public InputParameterId Id { get; private set; }
    public StudyTemplateId StudyTemplateId { get; private set; }
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public AliasName AliasName { get; private set; } = null!;
    public Specification Specification { get; private set; } = null!;
}
