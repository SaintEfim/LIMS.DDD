namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public enum Status
{
    Draft = 1,    // Черновик (можно свободно редактировать)
    Active = 2,   // Действующая / Утвержденная (редактирование ЗАПРЕЩЕНО, только создание новой ревизии)
    Archived = 3  // Архивная / Выведенная из действия (ГОСТ п. 8.8.1)
}
