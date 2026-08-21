using Domain.SeedWork.Errors;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Errors;

public sealed class TemplateNotEditableException(string statusName, string operation)
    : DomainException($"Cannot {operation} while template is in '{statusName}' status.")
{
    public override string Code => "TEMPLATE_NOT_EDITABLE";
}

public sealed class InvalidStatusTransitionException(string fromStatus, string toStatus)
    : DomainException($"Invalid status transition from '{fromStatus}' to '{toStatus}'.")
{
    public override string Code => "INVALID_STATUS_TRANSITION";
}

public sealed class DuplicateEntityException(string entityName, string fieldName, object fieldValue)
    : DomainException($"{entityName} with {fieldName} '{fieldValue}' already exists.")
{
    public override string Code => "DUPLICATE_ENTITY";
}

public sealed class EntityInUseException(string entityName, string usedBy)
    : DomainException($"{entityName} cannot be removed because it is used by {usedBy}.")
{
    public override string Code => "ENTITY_IN_USE";
}
