using Authentication.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Reflection;

namespace Authentication.API.Shared
{
    public static class AuthContextSeed
    {
        public static void SeedData(IMongoCollection<Utilisateur> utilisateursCollection)
        {
            bool existUser = utilisateursCollection.Find(u => true).Any();
            if (!existUser)
            {
                utilisateursCollection.InsertManyAsync(GetPreconfiguredUtilisateurs());
            }
        }
        public static async Task SeedCompteData(IMongoCollection<Compte> comptesCollection)
        {
            var comptes = await comptesCollection.Find(c => true).ToListAsync();
            if (comptes.Count == 0)
            {
                await comptesCollection.InsertOneAsync(new Compte
                {
                    NumeroCompte = "1234567897854",  // Exemple de numéro de compte
                    DateActivation = DateTime.Now,
                    Status = "Actif",
                    Salaire = 3000,
                    HistoriqueCredit = "Aucun crédit"
                });
            }
        }

        private static IEnumerable<Utilisateur> GetPreconfiguredUtilisateurs()
        {
            return new List<Utilisateur>
            {
                new Utilisateur
                {
                    CIN = 12345678,
                    Nom = "foulen ben follen",
                    Prenom="Ben follen",
                    Email = "foulen.foulen@example.com",
                    Adresse = "Tunis",
                    MotDePasse = "hashed_password_1",
                    Role = "Utilisateur",

                },
                new Utilisateur
                {
                    CIN = 87654321,
                    Nom = "Mohamed Elsaeed",
                    Prenom="Ben follen",
                    Email = "Mohamed.Elsaeed@example.com",
                    Adresse = "Sfax",
                    MotDePasse = "hashed_password_2",
                    Role = "Utilisateur",

                }
            };
        }
    }
}