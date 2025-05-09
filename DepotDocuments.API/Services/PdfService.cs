using DinkToPdf;
using DinkToPdf.Contracts;
using DepotDocuments.API.DTO;
using System.IO;

namespace DepotDocuments.API.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerateDemandePdf(DepotDemandeDto demande)
        {
            var htmlContent = $@"
                <html>
                <head><style>body {{ font-family: Arial; }}</style></head>
                <body>
                    <h1>Demande de crédit</h1>
                    <p><strong>Nom :</strong> {demande.Nom}</p>
                    <p><strong>Prénom :</strong> {demande.Prenom}</p>
                    <p><strong>CIN :</strong> {demande.Cin}</p>
                    <p><strong>Montant demandé :</strong> {demande.MontantDemande} DT</p>
                    <p><strong>Durée :</strong> {demande.DureeEnAnnees} années</p>
                    <p><strong>Email :</strong> {demande.Email}</p>
                    <p><strong>Type de crédit :</strong> {demande.TypeCredit}</p>
                </body>
                </html>";

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return _converter.Convert(doc);
        }
    
public byte[] GenerateDemandePdfWithSignature(DepotDemandeDto demande, byte[] signatureImage)
{
    // Enregistrer l’image temporairement
    string imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
    File.WriteAllBytes(imagePath, signatureImage);

    string imageSrc = $"file:///{imagePath.Replace("\\", "/")}";

    var htmlContent = $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            font-size: 12pt;
            margin: 40px;
            color: #333;
        }}
        h1 {{
            text-align: center;
            color: #0056b3;
            margin-bottom: 50px;
        }}
        .section {{
            margin-bottom: 40px;
        }}
        .section-title {{
            font-size: 16pt;
            color: #0056b3;
            margin-bottom: 10px;
            border-bottom: 1px solid #ccc;
        }}
        .field {{
            margin-bottom: 5px;
        }}
        .label {{
            font-weight: bold;
            width: 200px;
            display: inline-block;
        }}
        .signature-container {{
            margin-top: 40px;
            text-align: right;
        }}
        .signature-container img {{
            width: 200px;
            
        }}
        .signature-label {{
            font-style: italic;
            margin-bottom: 5px;
        }}
    </style>
</head>
<body>
    <h1>Demande de Crédit</h1>

    <div class='section'>
        <div class='section-title'>Informations personnelles</div>
        <div class='field'><span class='label'>Nom :</span> {demande.Nom}</div>
        <div class='field'><span class='label'>Prénom :</span> {demande.Prenom}</div>
        <div class='field'><span class='label'>CIN :</span> {demande.Cin}</div>
        <div class='field'><span class='label'>Date de naissance :</span> {demande.DateNaissance}</div>
        <div class='field'><span class='label'>Civilité :</span> {demande.Civilite}</div>
        <div class='field'><span class='label'>Téléphone :</span> {demande.Telephone}</div>
        <div class='field'><span class='label'>Email :</span> {demande.Email}</div>
        <div class='field'><span class='label'>Adresse :</span> {demande.Adresse}</div>
    </div>

    <div class='section'>
        <div class='section-title'>Compte et revenu</div>
        <div class='field'><span class='label'>Numéro de compte :</span> {demande.NumeroCompte}</div>
        <div class='field'><span class='label'>Revenu mensuel :</span> {demande.RevenuMensuelOcr}</div>
        <div class='field'><span class='label'>Attestation salaire :</span> {demande.AttestationSalaireOcr}</div>
    </div>

    <div class='section'>
        <div class='section-title'>Détails du crédit</div>
        <div class='field'><span class='label'>Type de crédit :</span> {demande.TypeCredit}</div>
        <div class='field'><span class='label'>Type de financement :</span> {demande.TypeFinancement}</div>
        <div class='field'><span class='label'>Montant demandé :</span> {demande.MontantDemande} DT</div>
        <div class='field'><span class='label'>Mensualité estimée :</span> {demande.MensualiteEstimee} DT</div>
        <div class='field'><span class='label'>Durée :</span> {demande.DureeEnAnnees} année(s)</div>
        <div class='field'><span class='label'>Date de soumission :</span> {DateTime.Now:dd/MM/yyyy}</div>
    </div>

    <div class='signature-container'>
        <div class='signature-label'>Signature du demandeur :</div>
        <img src='{imageSrc}' alt='Signature'/>
    </div>
</body>
</html>";

    var doc = new HtmlToPdfDocument
    {
        GlobalSettings = new GlobalSettings
        {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait,
            Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
        },
        Objects = {
            new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
    };

    return _converter.Convert(doc);
}


}
}
