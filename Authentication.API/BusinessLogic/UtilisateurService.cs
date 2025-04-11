﻿using Authentication.API.Data;
using Authentication.API.DTO;
using Authentication.API.Entities;
using Authentication.API.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;
using MongoDB.Bson;
namespace Authentication.API.BusinessLogic;
public class UtilisateurService : IUtilisateurService
{
    private readonly IUtilisateurRepository _repository;
    private readonly ICompteRepository _compteRepository;
    private IAuthContext _context;
    

    public UtilisateurService(IUtilisateurRepository repository, IAuthContext context,ICompteRepository compteRepository)
    {
        _repository = repository;
        _context = context;
         _compteRepository = compteRepository;
    }



   public async Task<Utilisateur> CreateUtilisateur(SignUpDTO utilisateurDTO)
{
     if (utilisateurDTO == null)
    {
        throw new ArgumentNullException(nameof(utilisateurDTO), "UtilisateurDTO cannot be null.");
    }
      bool compteExiste = await _compteRepository.CompteExiste(utilisateurDTO.NumeroCompte);
    if (!compteExiste)
    {
        throw new Exception("Le numéro de compte fourni n'existe pas dans la base des comptes.");
    }
    bool utilisateurExiste = await _repository.CheckIfUtilisateurExists(utilisateurDTO.Email, utilisateurDTO.CIN, utilisateurDTO.NumeroCompte);

    if (utilisateurExiste)
    {
        throw new Exception("Un utilisateur avec le même email, CIN ou numéro de compte existe déjà.");
    }

    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(utilisateurDTO.MotDePasse);
    var utilisateur = new Utilisateur
    {
        CIN = utilisateurDTO.CIN,
        Nom = utilisateurDTO.Nom,
        Email = utilisateurDTO.Email,
        Adresse = utilisateurDTO.Adresse,
        Role = utilisateurDTO.Role,
        NumeroCompte = utilisateurDTO.NumeroCompte,
        MotDePasse = hashedPassword,
        ImageUrl = utilisateurDTO.ImageUrl // Enregistrer l'URL de l'image dans l'entité Utilisateur
    };

    await _repository.CreateUtilisateur(utilisateur);
    return utilisateur;
}


    public async Task<Utilisateur> GetUtilisateurById(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _repository.GetUtilisateurById(id);
    }

    public async Task<IEnumerable<Utilisateur>> GetUtilisateurs()
    {
        return await _repository.GetUtilisateurs();
    }
   

  public async Task<Utilisateur> UpdateUtilisateur(string id, SignUpDTO utilisateurDTO)
{
    // Convertir la chaîne 'id' en ObjectId
    if (!ObjectId.TryParse(id, out ObjectId objectId))
    {
        // Gérer l'erreur si l'ID n'est pas valide
        return null;
    }

    // Effectuer la recherche dans la base de données en utilisant l'ObjectId
    var utilisateur = await _context.Utilisateurs.Find(u => u.Id == objectId).FirstOrDefaultAsync();
    if (utilisateur == null)
    {
        return null; // L'utilisateur n'a pas été trouvé
    }

    // Mise à jour des champs
    if (!string.IsNullOrEmpty(utilisateurDTO.Email))
    {
        utilisateur.Email = utilisateurDTO.Email;
    }
    if (!string.IsNullOrEmpty(utilisateurDTO.Nom))
    {
        utilisateur.Nom = utilisateurDTO.Nom;
    }
    if (!string.IsNullOrEmpty(utilisateurDTO.Adresse))
    {
        utilisateur.Adresse = utilisateurDTO.Adresse;
    }
    if (!string.IsNullOrEmpty(utilisateurDTO.NumeroCompte))
    {
        utilisateur.NumeroCompte = utilisateurDTO.NumeroCompte;
    }
    if (!string.IsNullOrEmpty(utilisateurDTO.MotDePasse))
    {
        utilisateur.MotDePasse = utilisateurDTO.MotDePasse;
    }

    // Mise à jour dans la base de données avec la méthode appropriée
    var filter = Builders<Utilisateur>.Filter.Eq(u => u.Id, objectId);
    var update = Builders<Utilisateur>.Update
        .Set(u => u.Email, utilisateur.Email)
        .Set(u => u.Nom, utilisateur.Nom)
        .Set(u => u.Adresse, utilisateur.Adresse)
        .Set(u => u.NumeroCompte, utilisateur.NumeroCompte)
        .Set(u => u.MotDePasse, utilisateur.MotDePasse);

    await _context.Utilisateurs.UpdateOneAsync(filter, update);

    return utilisateur;
}

    public async Task<bool> DeleteUtilisateur(string id)
    {
        return await _repository.DeleteUtilisateur(id);
    }

   
    public async Task<Utilisateur> Authenticate(string email, string motDePasse)
    {
        // Récupérer l'utilisateur par email
        var utilisateur = await _repository.GetUtilisateurByEmail(email);
        if (utilisateur == null)
        {
            return null; // Utilisateur non trouvé
        }

        // Comparer le mot de passe fourni avec celui hashé en base
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(motDePasse, utilisateur.MotDePasse);
        if (!isPasswordValid)
        {
            return null; // Mot de passe incorrect
        }

        return utilisateur; // Authentification réussie
    }
    public async Task<Utilisateur> GetUtilisateurByResetToken(string resetToken)
{
    return await _context.Utilisateurs
        .Find(u => u.ResetToken == resetToken && u.ResetTokenExpiration > DateTime.UtcNow)
        .FirstOrDefaultAsync();
}

public async Task<bool> RequestPasswordReset(string email)
{
    var utilisateur = await _repository.GetUtilisateurByEmail(email);
    if (utilisateur == null)
        return false; // Utilisateur non trouvé

    // Générer un token aléatoire
    utilisateur.ResetToken = Guid.NewGuid().ToString();
    utilisateur.ResetTokenExpiration = DateTime.UtcNow.AddHours(1); // Expiration dans 1h

    // Mettre à jour l'utilisateur en base
    var filter = Builders<Utilisateur>.Filter.Eq(u => u.Id, utilisateur.Id);
    var update = Builders<Utilisateur>.Update
        .Set(u => u.ResetToken, utilisateur.ResetToken)
        .Set(u => u.ResetTokenExpiration, utilisateur.ResetTokenExpiration);

    await _context.Utilisateurs.UpdateOneAsync(filter, update);

      var passwordResetService = new PasswordResetService();
    passwordResetService.SendResetEmail(email, utilisateur.ResetToken);
    return true;
}
public async Task<bool> ResetPassword(string resetToken, string newPassword)
{
    var utilisateur = await _context.Utilisateurs
        .Find(u => u.ResetToken == resetToken && u.ResetTokenExpiration > DateTime.UtcNow)
        .FirstOrDefaultAsync();

    if (utilisateur == null)
        return false; // Token invalide ou expiré

    // Hasher le nouveau mot de passe
    utilisateur.MotDePasse = BCrypt.Net.BCrypt.HashPassword(newPassword);
    utilisateur.ResetToken = null;
    utilisateur.ResetTokenExpiration = null;

    var filter = Builders<Utilisateur>.Filter.Eq(u => u.Id, utilisateur.Id);
    var update = Builders<Utilisateur>.Update
        .Set(u => u.MotDePasse, utilisateur.MotDePasse)
        .Set(u => u.ResetToken, null)
        .Set(u => u.ResetTokenExpiration, null);

    await _context.Utilisateurs.UpdateOneAsync(filter, update);
    return true;
}


}