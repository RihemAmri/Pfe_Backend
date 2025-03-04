using Authentication.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;

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

        private static IEnumerable<Utilisateur> GetPreconfiguredUtilisateurs()
        {
            return new List<Utilisateur>
            {
                new Utilisateur
                {
                    CIN = 12345678,
                    Nom = "foulen ben follen",
                    Email = "foulen.foulen@example.com",
                    Adresse = "Tunis",
                    MotDePasse = "hashed_password_1",
                    Role = "Utilisateur"
                },
                new Utilisateur
                {
                    CIN = 87654321,
                    Nom = "Mohamed Elsaeed",
                    Email = "Mohamed.Elsaeed@example.com",
                    Adresse = "Sfax",
                    MotDePasse = "hashed_password_2",
                    Role = "Utilisateur"
                }
            };
        }
    }
}