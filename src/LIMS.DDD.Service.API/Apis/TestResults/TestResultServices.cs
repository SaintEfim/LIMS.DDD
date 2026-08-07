using LIMS.DDD.Service.Application.Studies.TestResults;

namespace LIMS.DDD.Service.API.Apis.TestResults;

public class TestResultServices(TestResultCommandsHandler commands, TestResultQueries queries)
{
    public TestResultCommandsHandler Commands { get; } = commands;
    public TestResultQueries Queries { get; } = queries;
}
