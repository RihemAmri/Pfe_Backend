using DepotDocuments.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
public class CreditDocumentService : ICreditDocumentService
{
    private readonly IMongoCollection<CreditDocument> _documents;

    public CreditDocumentService(IConfiguration config)
    {
        var connectionString = config["CreditDocumentSettings:ConnectionString"];
        var databaseName = config["CreditDocumentSettings:DatabaseName"];
        var collectionName = config["CreditDocumentSettings:CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _documents = database.GetCollection<CreditDocument>(collectionName);
    }

    public async Task AddDocumentsAsync(List<CreditDocument> documents)
    {
        await _documents.InsertManyAsync(documents);
    }
  public async Task<List<CreditDocument>> GetDocumentsByIdsAsync(List<string> ids)
{
    return await _documents.Find(doc => ids.Contains(doc.Id)).ToListAsync();
}
}
