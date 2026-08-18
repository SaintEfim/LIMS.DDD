using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
using LIMS.Service.LaboratoryOperations.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Service.LaboratoryOperations.Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISampleRepository, SampleRepository>();
        services.AddScoped<IStudyRepository, StudyRepository>();

        services.AddScoped<IUnitSnapshotRepository, UnitSnapshotRepository>();

        services.AddScoped<IStudyTemplateSnapshotRepository, StudyTemplateSnapshotRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ServiceDB")));
    }
}
