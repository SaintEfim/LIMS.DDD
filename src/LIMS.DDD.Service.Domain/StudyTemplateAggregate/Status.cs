namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public enum Status
{
    Draft,    // Черновик (можно свободно редактировать)
    Active,   // Действующая / Утвержденная (редактирование ЗАПРЕЩЕНО, только создание новой ревизии)
    Archived  // Архивная / Выведенная из действия (ГОСТ п. 8.8.1)
}
