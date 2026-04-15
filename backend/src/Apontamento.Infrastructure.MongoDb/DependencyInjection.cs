using Apontamento.Domain.Repositories;
using Apontamento.Infrastructure.MongoDb.Configuration;
using Apontamento.Infrastructure.MongoDb.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Apontamento.Infrastructure.MongoDb;

public static class DependencyInjection
{
    public static IServiceCollection AddMongoRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
            ?? new MongoDbSettings();

        services.AddSingleton(settings);
        services.AddSingleton<IMongoClient>(_ => new MongoClient(settings.ConnectionString));
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<IProjetoRepository>(sp => new ProjetoMongoRepository(
            sp.GetRequiredService<IMongoDatabase>(),
            settings.ProjetosCollectionName));

        services.AddScoped<IApontamentoRepository>(sp => new ApontamentoMongoRepository(
            sp.GetRequiredService<IMongoDatabase>(),
            settings.ApontamentosCollectionName));

        return services;
    }
}
