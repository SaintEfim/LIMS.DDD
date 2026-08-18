using LIMS.Service.LaboratoryOperations.Application.Studies.Core.Commands;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed class StudyCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyRepository studyRepository,
    ISampleRepository sampleRepository,
    IOrderRepository orderRepository,
    IStudyTemplateSnapshotRepository templateRepository,
    StudyCreationDomainService domainService,
    StudyStatusChangeDomainService statusChangeDomainService)
{
    public async Task<Result<Study, Exception>> CreateAsync(
        SampleId sampleId,
        CreateStudyCommand command,
        CancellationToken cancellationToken = default)
    {
        var sample = await sampleRepository.GetByIdAsync(sampleId, cancellationToken);
        if (sample is null)
        {
            return Result<Study, Exception>.Failure(
                new KeyNotFoundException($"Sample with id {sampleId.Value} not found."));
        }

        var order = await orderRepository.GetByIdAsync(sample.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<Study, Exception>.Failure(
                new KeyNotFoundException($"Order with id {sample.OrderId} not found."));
        }

        var template =
            await templateRepository.GetByIdAsync(new StudyTemplateId(command.TemplateId), cancellationToken);
        if (template is null)
        {
            return Result<Study, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {command.TemplateId} not found."));
        }

        var createResult = domainService.CreateStudyByTemplate(sample, order, template);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);

        return null!;
    }

    public async Task<Result<None, Exception>> UpdateNotesAsync(
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
                return descResult.CastFailure<None>();
            }

            description = descResult.GetValue();
        }

        var updateResult = studyResult.GetValue()
            .UpdateNotes(description);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> ReassignSampleAsync(
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
            return Result<None, Exception>.Failure(
                new KeyNotFoundException($"New Sample with id {command.NewSampleId} not found."));
        }

        var reassignResult = studyResult.GetValue()
            .ReassignSample(new SampleId(command.NewSampleId));
        if (reassignResult.IsFailure)
        {
            return reassignResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> ChangeStatusAsync(
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
            return Result<None, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{command.Status}'."));
        }

        var sample = await sampleRepository.GetByIdAsync(study.SampleId, cancellationToken);
        if (sample is null)
        {
            return Result<None, Exception>.Failure(
                new KeyNotFoundException($"Parent sample with id {study.SampleId.Value} not found."));
        }

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(study, newStatus, sample);
        if (changeResult.IsFailure)
        {
            return changeResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> DeleteAsync(
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
            return deleteResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    #region Private Helpers

    private async Task<Result<Study, Exception>> GetStudyForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(id), cancellationToken);
        return study is null
            ? Result<Study, Exception>.Failure(new KeyNotFoundException($"Study with id {id} not found."))
            : Result<Study, Exception>.Success(study);
    }

    private async Task<Result<Study, Exception>> SaveNewAsync(
        Study study,
        CancellationToken cancellationToken = default)
    {
        try
        {
            studyRepository.Add(study);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Study, Exception>.Success(study);
        }
        catch (Exception ex)
        {
            return Result<Study, Exception>.Failure(new Exception($"Failed to save Study: {ex.Message}", ex));
        }
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }

    #endregion
}
