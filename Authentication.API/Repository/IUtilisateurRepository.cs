using Authentication.API.Entities;
using System.Data;

namespace Authentication.API.Repository
{
    public interface IUtilisateurRepository
    {
        // CRUD + Recherche par nom + rôle
        Task CreateUtilisateur(Utilisateur utilisateur);
        Task<bool> UpdateUtilisateur(Utilisateur utilisateur);
        Task<bool> DeleteUtilisateur(string id);
        Task<IEnumerable<Utilisateur>> GetUtilisateurs();
        Task<Utilisateur> GetUtilisateurById(string id);
        Task<IEnumerable<Utilisateur>> GetUtilisateurByNom(string nom);
        Task<IEnumerable<Utilisateur>> GetUtilisateurByRole(string role);
    }
}
