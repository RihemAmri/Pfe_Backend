
using DepotDocuments.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
namespace DepotDocuments.API.Services
{
    public class SignatureService : ISignatureService
{
    private readonly IMongoCollection<SignatureRecord> _signatureCollection;

    public SignatureService(IConfiguration config)
    {
        var connectionString = config["SignatureSettings:ConnectionString"];
        var dbName = config["SignatureSettings:DatabaseName"];
        var collectionName = config["SignatureSettings:CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _signatureCollection = database.GetCollection<SignatureRecord>(collectionName);
    }

    public async Task SaveSignatureAsync(SignatureRecord signature)
    {
        await _signatureCollection.InsertOneAsync(signature);
    }
}

}