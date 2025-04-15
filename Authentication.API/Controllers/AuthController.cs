﻿using Authentication.API.DTO;
using Authentication.API.Entities;
using Authentication.API.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using Authentication.API.Services;
using System.Threading.Tasks;


namespace Authentication.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUtilisateurService _service;
        private readonly TokenService _tokenService;
        private object _utilisateurService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUtilisateurService service, ILogger<AuthController> logger, CloudinaryService cloudinaryService, TokenService tokenService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));  // Vérification de l'injection
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));  // Vérification de l'injection
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));  // Vérification de l'injection
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] SignUpDTO signUpDto, IFormFile imageFile)
        {
            if (signUpDto == null)
            {
                return BadRequest("Les données de l'utilisateur sont manquantes.");
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Le fichier image est requis.");
            }

            try
            {
                // Upload de l'image sur Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile);
                if (uploadResult == null)
                {
                    _logger.LogError("Upload de l'image échoué.");
                    return BadRequest("Erreur lors du téléchargement de l'image.");
                }

                // Ajout de l'URL de l'image dans le DTO
                signUpDto.ImageUrl = uploadResult.SecureUrl.ToString();

                // Appel au service pour créer l'utilisateur
                var utilisateur = await _service.CreateUtilisateur(signUpDto);

                // Création de la réponse utilisateur (DTO)
                var utilisateurResponse = new UtilisateurDTO
                {
                    Id = utilisateur.Id.ToString(),
                    CIN = utilisateur.CIN,
                    Nom = utilisateur.Nom,
                    Prenom=utilisateur.Prenom,
                    Email = utilisateur.Email,
                    Adresse = utilisateur.Adresse,
                    Role = utilisateur.Role,
                    NumeroCompte = utilisateur.NumeroCompte,
                    ImageUrl = signUpDto.ImageUrl  // Ajout de l'URL de l'image dans la réponse
                };

                // Retourne un code HTTP 201 (Created) avec la ressource créée
                return CreatedAtAction(nameof(GetUtilisateurById), new { id = utilisateur.Id.ToString() }, utilisateurResponse);
            }
            catch (Exception ex)
            {
                // Si une exception survient (par exemple : email ou CIN déjà utilisé), retourne un BadRequest avec le message d'erreur
                return BadRequest($"Erreur : {ex.Message}");
            }
        }






        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UtilisateurDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UtilisateurDTO>> GetUtilisateurById(string id)
        {
            var utilisateur = await _service.GetUtilisateurById(id);
            if (utilisateur == null)
                return NotFound();

            var utilisateurResponse = new UtilisateurDTO
            {
                Id = utilisateur.Id.ToString(),
                CIN = utilisateur.CIN,
                Nom = utilisateur.Nom,
                Prenom=utilisateur.Prenom,
                Email = utilisateur.Email,
                Adresse = utilisateur.Adresse,
                Role = utilisateur.Role,
                NumeroCompte = utilisateur.NumeroCompte,
                ImageUrl = utilisateur.ImageUrl
            };

            return Ok(utilisateurResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UtilisateurDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UtilisateurDTO>>> GetUtilisateurs()
        {
            var utilisateurs = await _service.GetUtilisateurs();
            var utilisateursResponse = utilisateurs.Select(u => new UtilisateurDTO
            {
                Id = u.Id.ToString(),
                CIN = u.CIN,
                Nom = u.Nom,
                Prenom=u.Prenom,
                Email = u.Email,
                Adresse = u.Adresse,
                Role = u.Role,
                NumeroCompte = u.NumeroCompte,
                ImageUrl = u.ImageUrl
            }).ToList();

            return Ok(utilisateursResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateUtilisateur(string id, [FromBody] SignUpDTO utilisateurDTO)
        {
            var utilisateur = await _service.UpdateUtilisateur(id, utilisateurDTO);
            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            return Ok(utilisateur);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUtilisateur(string id)
        {
            var deleted = await _service.DeleteUtilisateur(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var utilisateur = await _service.Authenticate(loginDTO.Email, loginDTO.MotDePasse);
            if (utilisateur == null)
            {
                return Unauthorized("Email ou mot de passe incorrect.");
            }

            // Crée un token d'authentification si nécessaire
            var token = _tokenService.GenerateToken(utilisateur);
            return Ok(new { token = token });
        }




        [HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
{
    bool result = await _service.RequestPasswordReset(forgotPasswordDto.Email);
    if (!result)
        return NotFound(new { message = "Utilisateur non trouvé." });

    return Ok(new { message = "Un email a été envoyé avec les instructions de réinitialisation." });
}

       [HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
{
    if (resetPasswordDto == null)
    {
        return BadRequest("Données reçues sont nulles.");
    }

    Console.WriteLine($"Email: {resetPasswordDto.Email}");
    Console.WriteLine($"Token: {resetPasswordDto.Token}");
    Console.WriteLine($"NewPassword: {resetPasswordDto.NewPassword}");

    if (string.IsNullOrEmpty(resetPasswordDto.Email) ||
        string.IsNullOrEmpty(resetPasswordDto.Token) ||
        string.IsNullOrEmpty(resetPasswordDto.NewPassword))
    {
        return BadRequest("Un des champs est vide.");
    }

    bool result = await _service.ResetPassword(resetPasswordDto.Token, resetPasswordDto.NewPassword);
    if (!result)
        return BadRequest("Token invalide ou expiré.");

    return Ok(new { message = "Mot de passe réinitialisé avec succès." });
}




    }
}