using Authentication.API.Entities;
using System.Threading.Tasks;

public interface ICompteRepository
{
    Task<bool> CompteExiste(string numeroCompte);
}
