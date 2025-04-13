using MongoDB.Driver;
using DepotDocuments.API.Entities;  // Assure-toi d'importer les entités correctes

namespace DepotDocuments.API.Data
{
    public interface IDocumentContext
    {
        IMongoCollection<Document> Documents { get; }
    }

    public class DocumentContext : IDocumentContext
    {
        private readonly IMongoDatabase _database;

        public DocumentContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            _database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
        }

        public IMongoCollection<Document> Documents => _database.GetCollection<Document>("document");  // Nom de la collection
    }
}
