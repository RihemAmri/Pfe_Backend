﻿using Authentication.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUtilisateurService
{
    Task<Utilisateur> CreateUtilisateur(Authentication.API.DTO.SignUpDTO utilisateurDTO);
    Task<Utilisateur> GetUtilisateurById(string id);
    Task<IEnumerable<Utilisateur>> GetUtilisateurs();
    Task<Utilisateur> UpdateUtilisateur(string id, Authentication.API.DTO.SignUpDTO utilisateurDTO);
    Task<bool> DeleteUtilisateur(string id);
    Task<Utilisateur> Authenticate(string email, string motDePasse);

    Task<bool> RequestPasswordReset(string email);
    
    Task<Utilisateur> GetUtilisateurByResetToken(string resetToken);
     Task<bool> ResetPassword(string resetToken, string newPassword);
    
    
}