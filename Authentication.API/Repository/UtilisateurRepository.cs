using Authentication.API.Data;
using Authentication.API.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.API.Repository
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly IAuthContext _context;

        public UtilisateurRepository(IAuthContext context)
        {
            _context = context;
        }

        public async Task CreateUtilisateur(Utilisateur utilisateur)
        {
            await _context.Utilisateurs.InsertOneAsync(utilisateur);
        }

        public async Task<bool> UpdateUtilisateur(Utilisateur utilisateur)
        {
            var updateResult = await _context.Utilisateurs.ReplaceOneAsync(
                u => u.Id == utilisateur.Id, utilisateur);

            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUtilisateur(string id)
        {
            var filter = Builders<Utilisateur>.Filter.Eq(u => u.Id, id);
            var deleteResult = await _context.Utilisateurs.DeleteOneAsync(filter);

            return deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Utilisateur>> GetUtilisateurs()
        {
            var utilisateurs = await _context.Utilisateurs.Find(_ => true).ToListAsync();
            Console.WriteLine($"🔎 Nombre d'utilisateurs trouvés : {utilisateurs.Count}");
            return utilisateurs;
        }

        public async Task<Utilisateur> GetUtilisateurById(string id)
        {
            return await _context.Utilisateurs.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Utilisateur>> GetUtilisateurByNom(string nom)
        {
            var filter = Builders<Utilisateur>.Filter.Eq(u => u.Nom, nom);
            return await _context.Utilisateurs.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Utilisateur>> GetUtilisateurByRole(string role)
        {
            var filter = Builders<Utilisateur>.Filter.Eq(u => u.Role, role);
            return await _context.Utilisateurs.Find(filter).ToListAsync();
        }
    }
}
