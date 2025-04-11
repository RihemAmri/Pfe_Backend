using Authentication.API.Data;
using Authentication.API.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Authentication.API.Repository
{
    public class CompteRepository : ICompteRepository
    {
        private readonly IAuthContext _context;

        public CompteRepository(IAuthContext context)
        {
            _context = context;
        }

        public async Task<bool> CompteExiste(string numeroCompte)
        {
            var compte = await _context.Comptes
                .Find(c => c.NumeroCompte == numeroCompte)
                .FirstOrDefaultAsync();

            return compte != null;
        }
    }
}
