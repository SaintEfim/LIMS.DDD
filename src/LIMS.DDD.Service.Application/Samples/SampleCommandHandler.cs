using LIMS.DDD.Service.Application.Samples.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.Services;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Application.Samples;

public sealed class SampleCommandHandler(
    ISampleRepository repository,
    IOrderRepository orderRepository,
    IStudyRepository studyRepository,
    SampleCreationDomainService creationDomainService,
    SampleDeletionDomainService deletionDomainService)
{
    public async Task<Result<Sample, Exception>> CreateAsync(
        CreateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(new OrderId(command.OrderId), cancellationToken);
        if (order is null)
        {
            return Result<Sample, Exception>.Failure(
                new KeyNotFoundException($"Order with id {command.OrderId} not found."));
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

        var name = command.Name is not null
            ? Name.Create(command.Name)
                .GetValue()
            : null;
        var gatherDate = command.GatherDateBegin is not null || command.GatherDateEnd is not null
            ? GatherDate.Create(command.GatherDateBegin ?? sample.GatherDate.Begin,
                    command.GatherDateEnd ?? sample.GatherDate.End)
                .GetValue()
            : null;
        var code = command.Code is not null
            ? Code.Create(command.Code)
                .GetValue()
            : null;

        var updateResult = sample.UpdatePartial(name, gatherDate, code, command.VolumeValue, command.VolumeUnit);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<None, Exception>.Success();
    }

    public async Task<Result<Sample, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetSampleForChangeAsync(id, cancellationToken);

        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<Sample>();
        }

        var template = templateResult.GetValue();

        if (!SampleStatus.TryParse(statusCommand, out var newStatus))
        {
            return Result<Sample, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{statusCommand}'."));
        }

        var changeResult = template.ChangeStatus(newStatus!);

        if (changeResult.IsFailure)
        {
            return changeResult.CastFailure<Sample>();
        }

        return await SaveChangesAsync(template, cancellationToken);
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

        var deleteResult = deletionDomainService.Delete(sample, order, studies);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result<None, Exception>.Success();
    }

    private async Task<Result<Sample, Exception>> SaveNewAsync(
        Sample sample,
        CancellationToken cancellationToken)
    {
        try
        {
            repository.Add(sample);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Sample, Exception>.Success(sample);
        }
        catch (Exception ex)
        {
            return Result<Sample, Exception>.Failure(new Exception($"Failed to save Sample: {ex.Message}", ex));
        }
    }

    private async Task<Result<Sample, Exception>> SaveChangesAsync(
        Sample sample,
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Sample, Exception>.Success(sample);
        }
        catch (Exception ex)
        {
            return Result<Sample, Exception>.Failure(new Exception($"Failed to save Sample: {ex.Message}", ex));
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
