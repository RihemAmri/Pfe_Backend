using Authentication.API.Data;
using Authentication.API.Entities;
using Authentication.API.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public async Task<Utilisateur> GetUtilisateurById(string id)
    {
        var objectId = new ObjectId(id);
        return await _context.Utilisateurs.Find(u => u.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Utilisateur>> GetUtilisateurs()
    {
        return await _context.Utilisateurs.Find(_ => true).ToListAsync();
    }

    public async Task<bool> UpdateUtilisateur(Utilisateur utilisateur)
    {
        var update = Builders<Utilisateur>.Update
            .Set(u => u.Nom, utilisateur.Nom)
            .Set(u => u.Email, utilisateur.Email)
            .Set(u => u.Adresse, utilisateur.Adresse)
            .Set(u => u.Role, utilisateur.Role)
            .Set(u => u.NumeroCompte, utilisateur.NumeroCompte);

        var result = await _context.Utilisateurs.UpdateOneAsync(u => u.Id == utilisateur.Id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteUtilisateur(string id)
    {
        var objectId = new ObjectId(id);
        var result = await _context.Utilisateurs.DeleteOneAsync(u => u.Id == objectId);
        return result.DeletedCount > 0;
    }
    public async Task<bool> CheckIfUtilisateurExists(string email, int cin, string numeroCompte)
    {
        var utilisateur = await _context.Utilisateurs
            .Find(u => u.Email == email || u.CIN == cin || u.NumeroCompte == numeroCompte)
            .FirstOrDefaultAsync();

        return utilisateur != null;
    }

    public async Task<Utilisateur> GetUtilisateurByEmail(string email)
    {
        return await _context.Utilisateurs.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

}
