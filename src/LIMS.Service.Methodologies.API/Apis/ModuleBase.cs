using Application.SeedWork.Errors;
using Domain.SeedWork.Errors;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Errors;
using AppValidationError = Application.SeedWork.Errors.ValidationError;
using DomainValidationError = Domain.SeedWork.Errors.ValidationError;

namespace LIMS.Service.Methodologies.API.Apis;

public class ModuleBase
{
    protected static IResult HandleFailure(
        ApplicationError error)
    {
        return error switch
        {
            NotFoundError notFound => Results.NotFound(new
            {
                code = "NOT_FOUND",
                message = notFound.Message
            }),

            AppValidationError validation => Results.BadRequest(new
            {
                code = "VALIDATION_ERROR",
                message = validation.Message
            }),

            PersistenceError persistence => Results.Problem(persistence.Message,
                statusCode: StatusCodes.Status500InternalServerError, title: "Persistence Error"),

            DomainRuleViolation violation => HandleDomainError(violation.Error),

            _ => Results.Problem("An unexpected application error occurred",
                statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected Error")
        };
    }

    private static IResult HandleDomainError(
        DomainError error)
    {
        return error switch
        {
            EntityNotFoundError or EntityAlreadyDeletedError => Results.NotFound(new
            {
                code = error.Code,
                message = error.Message
            }),

            DuplicateEntityError or EntityInUseError or InvalidStatusTransitionError or TemplateNotEditableError =>
                Results.Conflict(new
                {
                    code = error.Code,
                    message = error.Message
                }),

            DomainValidationError => Results.BadRequest(new
            {
                code = error.Code,
                message = error.Message
            }),

            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "Domain Error")
        };
    }
}
