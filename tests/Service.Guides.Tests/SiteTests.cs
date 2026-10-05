using Library.Application.SeedWork.Errors;
using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Guides.Application.Sites;
using Service.Guides.Domain.Sites;
using Service.Guides.Persistence;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Service.Guides.API.Apis;
using ValidationError = Library.Domain.SeedWork.Errors.ValidationError;
namespace Service.Guides.Tests;

public sealed class SiteTests
{
    private static Site NewSite(string name = "Site", SiteId? parent = null) => new(Name.Create(name).GetValue(), parent);

    [Fact]
    public void SelfParentIsRejected()
    {
        var site = NewSite();
        Assert.IsType<ValidationError>(SiteHierarchy.ValidateParent(site.Id, site.Id, [site]));
        Assert.Throws<ArgumentException>(() => site.SetParent(site.Id));
    }
    [Fact]
    public void DescendantAtAnyDepthIsRejected()
    {
        var root = NewSite();
        var sites = new List<Site> { root };
        for (var i = 0; i < 1000; i++) sites.Add(NewSite(parent: sites[^1].Id));
        Assert.IsType<ValidationError>(SiteHierarchy.ValidateParent(root.Id, sites[^1].Id, sites));
        Assert.Null(SiteHierarchy.ValidateParent(sites[^1].Id, root.Id, sites));
        Assert.Null(SiteHierarchy.ValidateParent(root.Id, null, sites));
    }
    [Fact]
    public void MissingParentIsRejected() =>
        Assert.IsType<EntityNotFoundError>(SiteHierarchy.ValidateParent(new SiteId(Guid.NewGuid()), new SiteId(Guid.NewGuid()), []));

    [Fact]
    public async Task InvalidMoveDoesNotRenameOrSave()
    {
        var root = NewSite("Root");
        var child = NewSite("Child", root.Id);
        var repository = new FakeRepository([root, child]);
        var work = new FakeUnitOfWork();
        var result = await new SiteCommandsHandler(work, repository).UpdateAsync(root.Id.Value, new("Changed", child.Id.Value));
        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(Assert.IsType<DomainRuleViolation>(result.GetError()).Error);
        Assert.Equal("Root", root.Name.Value);
        Assert.Null(root.ParentId);
        Assert.Equal(0, work.Saves);
        Assert.True(work.RolledBack);
    }
    [Fact]
    public async Task MovingToRootAndRenamingSucceeds()
    {
        var root = NewSite("Root");
        var child = NewSite("Child", root.Id);
        var repository = new FakeRepository([root, child]);
        var work = new FakeUnitOfWork();
        var result = await new SiteCommandsHandler(work, repository).UpdateAsync(child.Id.Value, new("Renamed", null));
        Assert.True(result.IsSuccess);
        Assert.Null(child.ParentId);
        Assert.Equal("Renamed", child.Name.Value);
        Assert.True(work.Committed);
    }
    [Fact]
    public async Task ParentCannotBeDeletedUntilChildIsDeleted()
    {
        var root = NewSite();
        var child = NewSite(parent: root.Id);
        var repository = new FakeRepository([root, child]);
        var commands = new SiteCommandsHandler(new FakeUnitOfWork(), repository);
        Assert.True((await commands.DeleteAsync(root.Id.Value)).IsFailure);
        Assert.False(root.IsDeleted);
        Assert.True((await commands.DeleteAsync(child.Id.Value)).IsSuccess);
        Assert.True((await commands.DeleteAsync(root.Id.Value)).IsSuccess);
    }
    [Fact]
    public async Task CreateValidatesNameAndParent()
    {
        var repository = new FakeRepository([]);
        var commands = new SiteCommandsHandler(new FakeUnitOfWork(), repository);
        Assert.True((await commands.CreateAsync(new(" "))).IsFailure);
        Assert.True((await commands.CreateAsync(new("Site", Guid.NewGuid()))).IsFailure);
        var root = (await commands.CreateAsync(new("Root"))).GetValue();
        var child = (await commands.CreateAsync(new("Child", root.Id.Value))).GetValue();
        Assert.Equal(root.Id, child.ParentId);
        Assert.Equal(2, repository.Sites.Count);
    }
    [Fact]
    public async Task TreeContainsRootsAndChildrenAndOmitsDeletedNodes()
    {
        var root = NewSite("Root");
        var child = NewSite("Child", root.Id);
        var grandchild = NewSite("Grandchild", child.Id);
        var deleted = NewSite("Deleted");
        deleted.MarkAsDeleted();
        var tree = await new SiteQueries(new FakeRepository([root, child, grandchild, deleted])).GetTreeAsync();
        Assert.Equal(root.Id.Value, Assert.Single(tree).Id);
        Assert.Equal(child.Id.Value, Assert.Single(tree[0].Children).Id);
        Assert.Equal(grandchild.Id.Value, Assert.Single(tree[0].Children[0].Children).Id);
    }
    [Fact]
    public void PostgreSqlModelHasNullableParentAndRestrictDelete()
    {
        using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test").Options);
        var entity = context.Model.FindEntityType(typeof(Site))!;
        Assert.True(entity.FindProperty(nameof(Site.ParentId))!.IsNullable);
        Assert.Equal(DeleteBehavior.Restrict, Assert.Single(entity.GetForeignKeys()).DeleteBehavior);
        Assert.NotEmpty(entity.GetDeclaredQueryFilters());
        Assert.Contains("ORDER BY", context.Sites.OrderBy(site => site.Name).ThenBy(site => site.Id).ToQueryString());
    }

    [Fact]
    public async Task HttpApiCreatesTreeAndRejectsCycleAndParentDeletion()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSingleton<ISiteRepository>(new FakeRepository([]));
        builder.Services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork());
        builder.Services.AddScoped<SiteQueries>();
        builder.Services.AddScoped<SiteCommandsHandler>();
        await using var app = builder.Build();
        new SiteModule().AddRoutes(app);
        await app.StartAsync();
        using var client = new HttpClient { BaseAddress = new Uri(app.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!.Addresses.Single()) };
        using var rootResponse = await client.PostAsJsonAsync("/api/sites", new CreateSiteCommand("Lab"));
        Assert.Equal(HttpStatusCode.Created, rootResponse.StatusCode);
        var root = (await rootResponse.Content.ReadFromJsonAsync<SiteDto>())!;
        using var childResponse = await client.PostAsJsonAsync("/api/sites", new CreateSiteCommand("Room", root.Id));
        var child = (await childResponse.Content.ReadFromJsonAsync<SiteDto>())!;
        using var cycleResponse = await client.PutAsJsonAsync($"/api/sites/{root.Id}", new UpdateSiteCommand("Lab", child.Id));
        Assert.Equal(HttpStatusCode.BadRequest, cycleResponse.StatusCode);
        using var deleteResponse = await client.DeleteAsync($"/api/sites/{root.Id}");
        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
        var tree = (await client.GetFromJsonAsync<List<SiteTreeDto>>("/api/sites/tree"))!;
        Assert.Equal(child.Id, Assert.Single(Assert.Single(tree).Children).Id);
        using var missingResponse = await client.GetAsync($"/api/sites/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
        await app.StopAsync();
    }
    private sealed class FakeRepository(List<Site> sites) : ISiteRepository
    {
        public List<Site> Sites { get; } = sites;
        private bool locked;
        public Task LockHierarchyAsync(CancellationToken cancellationToken = default) { locked = true; return Task.CompletedTask; }
        public Task<List<Site>> GetAllAsync(bool forChange = false, CancellationToken cancellationToken = default)
        {
            if (forChange) Assert.True(locked);
            return Task.FromResult(Sites.Where(site => !site.IsDeleted).ToList());
        }
        public Task<Site?> GetByIdAsync(SiteId id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Sites.SingleOrDefault(site => site.Id == id && !site.IsDeleted));
        public void Add(Site site) => Sites.Add(site);
    }
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int Saves;
        public bool Committed;
        public bool RolledBack;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(++Saves);
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) { Committed = true; return Task.CompletedTask; }
        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) { RolledBack = true; return Task.CompletedTask; }
    }
}