using Authentication.API.Entities;
using Authentication.API.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Authentication.API.Data
{
    public class AuthenticationContext : IAuthContext
    {
        private readonly DatabaseSettings _settings;

        public AuthenticationContext(IOptions<DatabaseSettings> settings)
        {
            _settings = settings.Value;
            Console.WriteLine($"✅ Connexion à MongoDB avec : {_settings.ConnectionString}");
            var client = new MongoClient(_settings.ConnectionString);
            Console.WriteLine($"📌 Base de données utilisée : {_settings.DatabaseName}");
            var database = client.GetDatabase(_settings.DatabaseName);

            Utilisateurs = database.GetCollection<Utilisateur>(_settings.CollectionName);
            Comptes = database.GetCollection<Compte>("Comptes"); // <-- AJOUTÉ ICI
            Console.WriteLine($"📌 Collection utilisée : {_settings.CollectionName}");

            AuthContextSeed.SeedData(Utilisateurs);
        }

        public IMongoCollection<Utilisateur> Utilisateurs { get; }
        public IMongoCollection<Compte> Comptes { get; } // ← Ne plus null !

        public Task DeleteManyAsync(FilterDefinition<Utilisateur> filter)
        {
            return Utilisateurs.DeleteManyAsync(filter); // Optionnel mais mieux que NotImplementedException
        }
    }
}
