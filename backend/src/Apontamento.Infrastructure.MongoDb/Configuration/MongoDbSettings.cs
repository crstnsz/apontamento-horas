namespace Apontamento.Infrastructure.MongoDb.Configuration;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "apontamento_horas";
    public string ProjetosCollectionName { get; set; } = "projetos";
    public string ApontamentosCollectionName { get; set; } = "apontamentos";
}
