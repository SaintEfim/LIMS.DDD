using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class TestResultDomainService
{
    public Result<None, Exception> SetValue(
        TestResultId testResultId,
        Study study,
        double value,
        Specification specification)
    {
        if (value < 0)
        {
            return new InvalidOperationException("Test result value cannot be negative.");
        }

        var isWithinSpec = specification.IsWithinSpec(value);
        var updateTestResult = study.UpdateTestResult(testResultId, value, !isWithinSpec);
        if (updateTestResult.IsFailure)
        {
            updateTestResult.CastFailure<Exception>();
        }

        return new None();
    }
}
