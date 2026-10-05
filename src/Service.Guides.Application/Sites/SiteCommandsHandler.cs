using Library.Application.SeedWork;
using Library.Application.SeedWork.Errors;
using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;
using Library.Domain.SeedWork.ValueObjects;
using Service.Guides.Domain.Sites;
namespace Service.Guides.Application.Sites;

public sealed class SiteCommandsHandler(IUnitOfWork unitOfWork, ISiteRepository repository) : ICommandsHandler
{
    public Task<Result<Site, ApplicationError>> CreateAsync(CreateSiteCommand command, CancellationToken cancellationToken = default) =>
        ChangeAsync(null, command.Name, command.ParentId, false, cancellationToken);
    public Task<Result<Site, ApplicationError>> UpdateAsync(Guid id, UpdateSiteCommand command, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, command.Name, command.ParentId, false, cancellationToken);
    public Task<Result<Site, ApplicationError>> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, null, null, true, cancellationToken);

    private async Task<Result<Site, ApplicationError>> ChangeAsync(Guid? id, string? name, Guid? parentId, bool delete, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await repository.LockHierarchyAsync(cancellationToken);
            var sites = await repository.GetAllAsync(forChange: true, cancellationToken: cancellationToken);
            var result = Apply(sites, id, name, parentId, delete);
            if (result.IsFailure)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return result;
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    private Result<Site, ApplicationError> Apply(List<Site> sites, Guid? id, string? name, Guid? parentId, bool delete)
    {
        var site = id.HasValue ? sites.SingleOrDefault(site => site.Id.Value == id.Value) : null;
        if (id.HasValue && site is null)
            return new DomainRuleViolation(new EntityNotFoundError(nameof(Site), id.Value));
        if (delete)
        {
            if (sites.Any(child => child.ParentId == site!.Id))
                return new DomainRuleViolation(new EntityInUseError(nameof(Site), "child sites"));
            site!.MarkAsDeleted();
            return site;
        }
        var nameResult = Name.Create(name!);
        if (nameResult.IsFailure) return new DomainRuleViolation(nameResult.GetError());
        var parent = parentId.HasValue ? new SiteId(parentId.Value) : (SiteId?)null;
        var newSite = site is null;
        site ??= new Site(nameResult.GetValue());
        var error = SiteHierarchy.ValidateParent(site.Id, parent, sites);
        if (error is not null) return new DomainRuleViolation(error);
        site.Rename(nameResult.GetValue());
        site.SetParent(parent);
        if (newSite) repository.Add(site);
        return site;
    }
}