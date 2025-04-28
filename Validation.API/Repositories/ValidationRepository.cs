using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Validation.API.Models;
using Microsoft.Extensions.Configuration;

namespace Validation.API.Repositories
{
    public class ValidationRepository : IValidationRepository
    {
        private readonly IMongoCollection<BsonDocument> _demandeCollection;

        public ValidationRepository(IConfiguration config)
        {
            var connectionString = config["DepotDemandeSettings:ConnectionString"];
            var databaseName = config["DepotDemandeSettings:DatabaseName"];
            var collectionName = config["DepotDemandeSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _demandeCollection = database.GetCollection<BsonDocument>(collectionName);
        }

        public Task<ValidationItem> GetByIdAsync(string id)
        {
            // Plus nécessaire dans la nouvelle logique.
            return Task.FromResult<ValidationItem>(null);
        }

        public Task UpdateAsync(ValidationItem item)
        {
            // Plus nécessaire non plus.
            return Task.CompletedTask;
        }
        public async Task<ValidationItem> GetDemandeByIdAsync(string id)
{
    // Recherche de la demande dans la base MongoDB par son ID.
    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
    var projection = Builders<BsonDocument>.Projection.Include("Nom")
                                                       .Include("Prenom")
                                                       .Include("NumeroCompte")
                                                       .Include("MontantDemande")
                                                       .Include("DureeEnAnnees")
                                                       .Include("TypeCredit")
                                                       .Include("Email");

    var demandeDocument = await _demandeCollection.Find(filter).Project<BsonDocument>(projection).FirstOrDefaultAsync();

    if (demandeDocument == null)
        return null;

    // Transformation du document BSON en ValidationItem.
    var demande = new ValidationItem
    {
        //Reference = demandeDocument["Reference"].ToString(),
        Nom = demandeDocument["Nom"].ToString(),
        Prenom = demandeDocument["Prenom"].ToString(),
        NumeroCompte = demandeDocument["NumeroCompte"].ToString(),
        MontantDemande = demandeDocument["MontantDemande"].ToDecimal(),
        DureeEnAnnees = demandeDocument["DureeEnAnnees"].ToInt32(),
        TypeCredit = demandeDocument["TypeCredit"].ToString(),
        Email = demandeDocument["Email"].ToString()
    };

    return demande;
}

        public async Task<bool> UpdateDemandeStatusAsync(string id, string simplifiedStatus)
        {
            var update = Builders<BsonDocument>.Update
                .Set("Statut", simplifiedStatus)
                .Set("DateDerniereModification", DateTime.UtcNow);

            try
            {
                var result = await _demandeCollection.UpdateOneAsync(
                    Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id)), update);

                return result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public Task CreateAsync(ValidationItem item)
        {
            // Plus de création.
            return Task.CompletedTask;
        }
    }
}
