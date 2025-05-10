using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using DepotDocuments.API.Services;
namespace DepotDocuments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {   
        // En mémoire pour dev/test (à remplacer par Redis ou autre en prod)
        private static ConcurrentDictionary<string, (string code, DateTime expiration)> _otpStore = new();
        private readonly MailService _mailService;

        public OtpController(MailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("envoi")]
        public async Task<IActionResult> EnvoyerOtp([FromBody] EmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email invalide.");

            var code = new Random().Next(100000, 999999).ToString(); // Code 6 chiffres
            var expiration = DateTime.UtcNow.AddMinutes(5);

            _otpStore[dto.Email] = (code, expiration);

            await _mailService.EnvoyerOtpParMail(dto.Email, code);

            // En dev seulement : retourner le code dans la réponse
            return Ok(new { message = "OTP envoyé", code });
        }

        [HttpPost("verification")]
        public IActionResult VerifierOtp([FromBody] OtpVerificationDto dto)
        {
            if (!_otpStore.TryGetValue(dto.Email, out var otpInfo))
                return BadRequest(new { valide = false, message = "Aucun OTP trouvé pour cet email." });

            if (otpInfo.expiration < DateTime.UtcNow)
            {
                _otpStore.TryRemove(dto.Email, out _);
                return BadRequest(new { valide = false, message = "OTP expiré." });
            }

            if (otpInfo.code == dto.Code)
            {
                _otpStore.TryRemove(dto.Email, out _);
                return Ok(new { valide = true });
            }

            return BadRequest(new { valide = false, message = "OTP invalide." });
        }
    }

    public class EmailDto
    {
        public string Email { get; set; }
    }

    public class OtpVerificationDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
