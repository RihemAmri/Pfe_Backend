using Authentication.API.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Driver;

namespace Authentication.API.Data
{
    public interface IAuthContext
    {
        IMongoCollection<Utilisateur> Utilisateurs { get; }
    }
}
