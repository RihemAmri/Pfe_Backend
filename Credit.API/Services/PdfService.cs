using DinkToPdf;
using DinkToPdf.Contracts;
using Credit.API.Models;
using Credit.API.DTOs;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Credit.API.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;
        private readonly string _signaturePath;
        private readonly string _logoPath;

        public PdfService(IConverter converter)
        {
            _converter = converter;

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            _signaturePath = $"file://{Path.Combine(rootPath, "signature.png").Replace("\\", "/")}";
            _logoPath = $"file://{Path.Combine(rootPath, "logo_STB.png").Replace("\\", "/")}";
        }

        public async Task<byte[]> GenerateMainleveePdfAsync(CreditResponseDto credit)
        {
            var user = await GetUserFromAuthApi(credit.IdClient);
            if (user == null)
                throw new Exception("Impossible de récupérer les informations du client.");

            var cinFormatted = user.CIN.ToString("D8");

            var htmlContent = $@"
<html>
<head>
    <meta charset='utf-8'/>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 12pt;
            margin: 40px;
            color: #333;
        }}
        h1 {{
            text-align: center;
            color: #0d47a1;
            margin-bottom: 30px;
            text-transform: uppercase;
            letter-spacing: 2px;
        }}
        .section {{
            margin-bottom: 20px;
        }}
        .label {{
            font-weight: bold;
            width: 180px;
            display: inline-block;
        }}
        .field {{
            margin-bottom: 8px;
        }}
        .declaration {{
            margin-top: 30px;
            font-size: 12pt;
            line-height: 1.5;
            text-align: justify;
            color: #000;
        }}
        .signature {{
            margin-top: 50px;
            text-align: right;
        }}
        .signature img {{
            width: 200px;
            height: auto;
        }}
        .footer {{
            margin-top: 60px;
            text-align: center;
            font-style: italic;
            font-size: 10pt;
            color: #555;
        }}
    </style>
</head>
<body>
    <img src='{_logoPath}' style='width: 150px; margin-bottom: 30px;' />

    <h1>Attestation de Mainlevée</h1>
    
    <div class='section'>
        <h3>Informations Client</h3>
        <div class='field'><span class='label'>Nom :</span> {user.Nom} {user.Prenom}</div>
        <div class='field'><span class='label'>CIN :</span> {cinFormatted}</div>
        <div class='field'><span class='label'>Email :</span> {user.Email}</div>
        <div class='field'><span class='label'>N° Compte :</span> {user.NumeroCompte}</div>
    </div>

    <div class='section'>
        <h3>Détails du Crédit</h3>
        <div class='field'><span class='label'>Crédit ID :</span> {credit.Id}</div>
        <div class='field'><span class='label'>Type de crédit :</span> {credit.TypeCredit}</div>
        <div class='field'><span class='label'>Montant :</span> {credit.Montant} DT</div>
        <div class='field'><span class='label'>Date de début :</span> {credit.DateDebut:dd/MM/yyyy}</div>
        <div class='field'><span class='label'>Clôturé le :</span> {DateTime.Now:dd/MM/yyyy}</div>
    </div>

    <div class='declaration'>
        En conséquence, nous déclarons que la STB donne mainlevée de toute inscription hypothécaire ou autre sûreté rattachée à ce crédit.<br/><br/>
        La présente attestation est délivrée pour servir et valoir ce que de droit.
    </div>

    <div class='signature'>
        <img src='{_signaturePath}' alt='Signature STB' />
    </div>

    <div class='footer'>
         
    </div>
</body>
</html>";

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 40, Bottom = 40, Left = 40, Right = 40 },
                    DPI = 120,
                    DocumentTitle = "Attestation de Mainlevée"
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8", LoadImages = true }
                    }
                }
            };

            return _converter.Convert(doc);
        }

        private async Task<UserDto> GetUserFromAuthApi(string idClient)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://authentication-api:4000/api/v1/Auth/{idClient}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur lors de l'appel à l'API Auth: {response.StatusCode} - {errorContent}");
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
