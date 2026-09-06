using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;
using Library.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class TestResultDomainService
{
    public Result<None, DomainError> SetValue(
        TestResultId testResultId,
        Study study,
        double value,
        Specification specification)
    {
        if (value < 0)
        {
            return new ValidationError("Test result value cannot be negative.");
        }

        var isWithinSpec = specification.IsWithinSpec(value);

        var updateTestResult = study.UpdateTestResult(testResultId, value, !isWithinSpec);
        return updateTestResult.IsFailure ? updateTestResult : new None();
    }
}
