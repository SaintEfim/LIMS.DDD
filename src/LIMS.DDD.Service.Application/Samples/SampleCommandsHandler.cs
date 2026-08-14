using LIMS.DDD.Service.Application.Samples.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Services;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Application.Samples;

public sealed class SampleCommandsHandler(
    IUnitOfWork unitOfWork,
    ISampleRepository repository,
    IOrderRepository orderRepository,
    IStudyRepository studyRepository,
    SampleCreationDomainService creationDomainService,
    SampleDeletionDomainService deletionDomainService,
    SampleStatusChangeDomainService statusChangeDomainService)
{
    public async Task<Result<Sample, Exception>> CreateAsync(
        OrderId orderId,
        CreateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return Result<Sample, Exception>.Failure(
                new KeyNotFoundException($"Order with id {orderId.Value} not found."));
        }

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<Sample>();
        }

        var gatherDateResult = GatherDate.Create(command.GatherDateBegin, command.GatherDateEnd);
        if (gatherDateResult.IsFailure)
        {
            return gatherDateResult.CastFailure<Sample>();
        }

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure)
        {
            return codeResult.CastFailure<Sample>();
        }

        var volumeResult = Volume.Create(command.VolumeValue, command.VolumeUnit);
        if (volumeResult.IsFailure)
        {
            return volumeResult.CastFailure<Sample>();
        }

        var sampleResult = creationDomainService.CreateSample(order, nameResult.GetValue(), gatherDateResult.GetValue(),
            codeResult.GetValue(), volumeResult.GetValue());

        if (sampleResult.IsFailure)
        {
            return sampleResult.CastFailure<Sample>();
        }

        return await SaveNewAsync(sampleResult.GetValue(), cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
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
                return nameResult.CastFailure<None>();
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
                return gatherDateResult.CastFailure<None>();
            }

            gatherDate = gatherDateResult.GetValue();
        }

        Code? code = null;
        if (command.Code is not null)
        {
            var codeResult = Code.Create(command.Code);
            if (codeResult.IsFailure)
            {
                return codeResult.CastFailure<None>();
            }

            code = codeResult.GetValue();
        }

        var updateResult = sample.UpdatePartial(name, gatherDate, code, command.VolumeValue, command.VolumeUnit);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> ChangeStatusAsync(
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
            return Result<None, Exception>.Failure(new InvalidOperationException($"Unknown status '{statusCommand}'."));
        }

        var studies = (await studyRepository.GetBySampleIdAsync(sample.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(sample, newStatus, studies);
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
        var sampleResult = await GetSampleForChangeAsync(id, cancellationToken);
        if (sampleResult.IsFailure)
        {
            return sampleResult.CastFailure<None>();
        }

        var sample = sampleResult.GetValue();

        var order = await orderRepository.GetByIdAsync(sample.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<None, Exception>.Failure(
                new KeyNotFoundException($"Parent Order with id {sample.OrderId} not found."));
        }

        var studies = (await studyRepository.GetBySampleIdAsync(sample.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var deleteResult = deletionDomainService.DeleteSample(sample, order, studies.Count != 0);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Sample, Exception>> SaveNewAsync(
        Sample sample,
        CancellationToken cancellationToken = default)
    {
        try
        {
            repository.Add(sample);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Sample, Exception>.Success(sample);
        }
        catch (Exception ex)
        {
            return Result<Sample, Exception>.Failure(new Exception($"Failed to save Sample: {ex.Message}", ex));
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
            return Result<None, Exception>.Failure(new Exception($"Failed to save Sample: {ex.Message}", ex));
        }
    }

    private async Task<Result<Sample, Exception>> GetSampleForChangeAsync(
        Guid id,
        CancellationToken ct)
    {
        var sample = await repository.GetByIdForChangeAsync(new SampleId(id), ct);
        return sample is null
            ? Result<Sample, Exception>.Failure(new KeyNotFoundException($"Sample with id {id} not found."))
            : Result<Sample, Exception>.Success(sample);
    }
}
