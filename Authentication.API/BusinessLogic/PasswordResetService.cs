using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Authentication.API.Entities;

namespace Authentication.API.BusinessLogic
{
    public class PasswordResetService
    {
        private static List<PasswordResetToken> _tokens = new List<PasswordResetToken>();

        public string GenerateResetToken(string email)
        {
            var token = Guid.NewGuid().ToString();
            var expiration = DateTime.UtcNow.AddHours(1);

            _tokens.Add(new PasswordResetToken
            {
                Email = email,
                Token = token,
                Expiration = expiration
            });

            return token;
        }

        public bool ValidateToken(string email, string token)
        {
            return _tokens.Any(t => t.Email == email && t.Token == token && t.Expiration > DateTime.UtcNow);
        }

        public void SendResetEmail(string email, string token)
        {
            var resetLink = $"http://localhost:4200/reset-password?token={token}&email={email}";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("credit.platform0@gmail.com", "gbir clnu mvgm itie"), // Utiliser le mot de passe d'application ici
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("credit.platform0@gmail.com"), 
                Subject = "Réinitialisation du mot de passe",
                Body = $"Cliquez ici pour réinitialiser votre mot de passe : {resetLink}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Log ou afficher l'erreur
                Console.WriteLine("Erreur lors de l'envoi de l'email : " + ex.Message);
            }
        }
    }
}
