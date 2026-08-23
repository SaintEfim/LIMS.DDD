using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Application.Studies.Core.Commands;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed class StudyCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyRepository studyRepository,
    ISampleRepository sampleRepository,
    IOrderRepository orderRepository,
    IStudyTemplateSnapshotRepository templateRepository,
    StudyCreationDomainService domainService,
    StudyStatusChangeDomainService statusChangeDomainService) : ICommandsHandler
{
    public async Task<Result<Study, ApplicationError>> CreateAsync(
        SampleId sampleId,
        CreateStudyCommand command,
        CancellationToken cancellationToken = default)
    {
        var sample = await sampleRepository.GetByIdAsync(sampleId, cancellationToken);
        if (sample is null)
        {
            return new NotFoundError($"Sample with id '{sampleId.Value}' not found.");
        }

        var order = await orderRepository.GetByIdAsync(sample.OrderId, cancellationToken);
        if (order is null)
        {
            return new NotFoundError($"Order with id '{sample.OrderId.Value}' not found.");
        }

        var template =
            await templateRepository.GetByIdAsync(new StudyTemplateId(command.StudyTemplateId), cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"StudyTemplate with id '{command.StudyTemplateId}' not found.");
        }

        var createResult = domainService.CreateStudyByTemplate(sample, order, template);
        if (createResult.IsFailure)
        {
            return new DomainRuleViolation(createResult.GetError());
        }

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateNotesAsync(
        Guid id,
        UpdateStudyNotesCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(id, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return new DomainRuleViolation(descResult.GetError());
            }

            description = descResult.GetValue();
        }

        var updateResult = studyResult.GetValue()
            .UpdateNotes(description);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> ReassignSampleAsync(
        Guid id,
        ReassignStudySampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(id, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var newSample = await sampleRepository.GetByIdAsync(new SampleId(command.NewSampleId), cancellationToken);
        if (newSample is null)
        {
            return new NotFoundError($"New Sample with id '{command.NewSampleId}' not found.");
        }

        var reassignResult = studyResult.GetValue()
            .ReassignSample(new SampleId(command.NewSampleId));
        if (reassignResult.IsFailure)
        {
            return new DomainRuleViolation(reassignResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> ChangeStatusAsync(
        Guid id,
        ChangeStudyStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(id, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var study = studyResult.GetValue();

        if (!StudyStatus.TryParse(command.Status, out var newStatus) || newStatus is null)
        {
            return new ValidationError($"Unknown status '{command.Status}'.");
        }

        var sample = await sampleRepository.GetByIdAsync(study.SampleId, cancellationToken);
        if (sample is null)
        {
            return new NotFoundError($"Parent sample with id '{study.SampleId.Value}' not found.");
        }

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(study, newStatus, sample);
        if (changeResult.IsFailure)
        {
            return new DomainRuleViolation(changeResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(id, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var deleteResult = studyResult.GetValue()
            .Delete();
        if (deleteResult.IsFailure)
        {
            return new DomainRuleViolation(deleteResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    #region Private Helpers

    private async Task<Result<Study, ApplicationError>> GetStudyForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(id), cancellationToken);
        if (study is null)
        {
            return new NotFoundError($"Study with id '{id}' not found.");
        }

        return study;
    }

    private async Task<Result<Study, ApplicationError>> SaveNewAsync(
        Study study,
        CancellationToken cancellationToken = default)
    {
        try
        {
            studyRepository.Add(study);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return study;
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save Study: {ex.Message}");
        }
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }

    #endregion
}
