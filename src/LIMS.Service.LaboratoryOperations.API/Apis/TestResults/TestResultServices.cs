using LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;

namespace LIMS.Service.LaboratoryOperations.API.Apis.TestResults;

public class TestResultServices(TestResultCommandsHandler commands, TestResultQueries queries)
{
    public TestResultCommandsHandler Commands { get; } = commands;
    public TestResultQueries Queries { get; } = queries;
}
