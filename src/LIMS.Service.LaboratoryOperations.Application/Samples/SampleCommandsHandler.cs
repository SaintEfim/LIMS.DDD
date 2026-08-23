using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Application.Samples.Commands;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Application.Samples;

public sealed class SampleCommandsHandler(
    IUnitOfWork unitOfWork,
    IUnitSnapshotRepository unitSnapshotRepository,
    ISampleRepository repository,
    IOrderRepository orderRepository,
    IStudyRepository studyRepository,
    SampleCreationDomainService creationDomainService,
    SampleDeletionDomainService deletionDomainService,
    SampleStatusChangeDomainService statusChangeDomainService) : ICommandsHandler
{
    public async Task<Result<Sample, ApplicationError>> CreateAsync(
        OrderId orderId,
        CreateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return new NotFoundError($"Order with id '{orderId.Value}' not found.");
        }

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var gatherDateResult = GatherDate.Create(command.GatherDateBegin, command.GatherDateEnd);
        if (gatherDateResult.IsFailure)
        {
            return new DomainRuleViolation(gatherDateResult.GetError());
        }

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure)
        {
            return new DomainRuleViolation(codeResult.GetError());
        }

        UnitId? unitId = null;

        if (command.VolumeUnitId is not null)
        {
            unitId = new UnitId(command.VolumeUnitId.Value);

            var unit = await unitSnapshotRepository.GetByIdAsync(unitId.Value, cancellationToken);
            if (unit is null)
            {
                return new NotFoundError($"Unit with id '{command.VolumeUnitId.Value}' not found.");
            }
        }

        var volumeResult = Volume.Create(command.VolumeValue, unitId);
        if (volumeResult.IsFailure)
        {
            return new DomainRuleViolation(volumeResult.GetError());
        }

        var sampleResult = creationDomainService.CreateSample(order, nameResult.GetValue(), gatherDateResult.GetValue(),
            codeResult.GetValue(), volumeResult.GetValue());

        if (sampleResult.IsFailure)
        {
            return new DomainRuleViolation(sampleResult.GetError());
        }

        return await SaveNewAsync(sampleResult.GetValue(), cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
        Guid id,
        UpdateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var sampleResult = await GetSampleForChangeAsync(id, cancellationToken);
        if (sampleResult.IsFailure)
        {
            return sampleResult.CastFailure<None>();
        }

        var sample = sampleResult.GetValue();

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure)
            {
                return new DomainRuleViolation(nameResult.GetError());
            }

            name = nameResult.GetValue();
        }

        GatherDate? gatherDate = null;
        if (command.GatherDateBegin is not null || command.GatherDateEnd is not null)
        {
            var gatherDateResult = GatherDate.Create(command.GatherDateBegin ?? sample.GatherDate.Begin,
                command.GatherDateEnd ?? sample.GatherDate.End);
            if (gatherDateResult.IsFailure)
            {
                return new DomainRuleViolation(gatherDateResult.GetError());
            }

            gatherDate = gatherDateResult.GetValue();
        }

        Code? code = null;
        if (command.Code is not null)
        {
            var codeResult = Code.Create(command.Code);
            if (codeResult.IsFailure)
            {
                return new DomainRuleViolation(codeResult.GetError());
            }

            code = codeResult.GetValue();
        }

        Volume? volume = null;
        if (command.VolumeValue is not null || command.VolumeUnitId is not null)
        {
            var newValue = command.VolumeValue ?? sample.Volume.Value;
            var newUnitId = command.VolumeUnitId.HasValue
                ? new UnitId(command.VolumeUnitId.Value)
                : sample.Volume.UnitId;

            var volumeResult = Volume.Create(newValue, newUnitId);
            if (volumeResult.IsFailure)
            {
                return new DomainRuleViolation(volumeResult.GetError());
            }

            volume = volumeResult.GetValue();
        }

        var updateResult = sample.UpdatePartial(name, gatherDate, code, volume);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var sampleResult = await GetSampleForChangeAsync(id, cancellationToken);
        if (sampleResult.IsFailure)
        {
            return sampleResult.CastFailure<None>();
        }

        var sample = sampleResult.GetValue();

        if (!SampleStatus.TryParse(statusCommand, out var newStatus) || newStatus is null)
        {
            return new ValidationError($"Unknown status '{statusCommand}'.");
        }

        var studies = (await studyRepository.GetBySampleIdAsync(sample.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(sample, newStatus, studies);
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
        var sampleResult = await GetSampleForChangeAsync(id, cancellationToken);
        if (sampleResult.IsFailure)
        {
            return sampleResult.CastFailure<None>();
        }

        var sample = sampleResult.GetValue();

        var order = await orderRepository.GetByIdAsync(sample.OrderId, cancellationToken);
        if (order is null)
        {
            return new NotFoundError($"Parent Order with id '{sample.OrderId.Value}' not found.");
        }

        var studies = (await studyRepository.GetBySampleIdAsync(sample.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var deleteResult = deletionDomainService.DeleteSample(sample, order, studies.Count != 0);
        if (deleteResult.IsFailure)
        {
            return new DomainRuleViolation(deleteResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Sample, ApplicationError>> SaveNewAsync(
        Sample sample,
        CancellationToken cancellationToken = default)
    {
        try
        {
            repository.Add(sample);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return sample;
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save Sample: {ex.Message}");
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
            return new PersistenceError($"Failed to save Sample: {ex.Message}");
        }
    }

    private async Task<Result<Sample, ApplicationError>> GetSampleForChangeAsync(
        Guid id,
        CancellationToken ct)
    {
        var sample = await repository.GetByIdForChangeAsync(new SampleId(id), ct);
        if (sample is null)
        {
            return new NotFoundError($"Sample with id '{id}' not found.");
        }

        return sample;
    }
}
