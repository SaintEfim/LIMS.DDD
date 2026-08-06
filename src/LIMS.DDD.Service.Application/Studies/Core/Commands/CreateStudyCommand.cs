namespace LIMS.DDD.Service.Application.Studies.Core.Commands;

public sealed record CreateStudyCommand(Guid SampleId, Guid TemplateId);
