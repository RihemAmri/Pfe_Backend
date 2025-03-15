using Authentication.API.Entities;
using System.Data;

namespace Authentication.API.Repository
{
    public interface IUtilisateurRepository
    {
        // CRUD + Recherche par nom + rôle
        Task CreateUtilisateur(Utilisateur utilisateur);
        Task<Utilisateur> GetUtilisateurById(string id);
        Task<IEnumerable<Utilisateur>> GetUtilisateurs();
        Task<bool> UpdateUtilisateur(Utilisateur utilisateur);
        Task<bool> DeleteUtilisateur(string id);
        Task<bool> CheckIfUtilisateurExists(string email, int cin, string numeroCompte);
        Task<Utilisateur> GetUtilisateurByEmail(string email);

    }
}
