using Domain.SeedWork.Errors;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Errors;

public sealed class TemplateNotEditableError(string statusName, string operation)
    : DomainError($"Cannot {operation} while template is in '{statusName}' status.")
{
    public override string Code => "TEMPLATE_NOT_EDITABLE";
}

public sealed class DuplicateEntityError(string entityName, string fieldName, object fieldValue)
    : DomainError($"{entityName} with {fieldName} '{fieldValue}' already exists.")
{
    public override string Code => "DUPLICATE_ENTITY";
}

public sealed class EntityInUseError(string entityName, string usedBy)
    : DomainError($"{entityName} cannot be removed because it is used by {usedBy}.")
{
    public override string Code => "ENTITY_IN_USE";
}
