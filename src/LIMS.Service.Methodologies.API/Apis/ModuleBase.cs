using Domain.SeedWork.Errors;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Errors;

namespace LIMS.Service.Methodologies.API.Apis;

public class ModuleBase
{
    internal static IResult HandleFailure(
        Exception error)
    {
        return error switch
        {
            EntityNotFoundException or EntityAlreadyDeletedException => Results.NotFound(new
            {
                code = ((DomainException) error).Code,
                message = error.Message
            }),

            DuplicateEntityException or EntityInUseException or InvalidStatusTransitionException
                or TemplateNotEditableException => Results.Conflict(new
                {
                    code = ((DomainException) error).Code,
                    message = error.Message
                }),

            ValidationException exception => Results.BadRequest(new
            {
                code = exception.Code,
                message = exception.Message
            }),

            PersistenceException => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "Persistence Error"),

            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
    }
}
