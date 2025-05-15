using DinkToPdf;
using DinkToPdf.Contracts;

namespace Validation.API.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerateNotificationPdf(NotificationData data, byte[] signatureAdminBytes = null)
        {
            string logoBase64 = GetImageAsBase64(data.LogoPath);
            string signature1Base64 = GetImageAsBase64(data.Signature1Path);
            //string signature2Base64 = GetImageAsBase64(data.Signature2Path);
            string signature2Base64 = signatureAdminBytes != null
                ? Convert.ToBase64String(signatureAdminBytes)
                : GetImageAsBase64(data.Signature2Path);
            var html = $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
    body {{ font-family: Arial, sans-serif; font-size: 12pt; }}
    h1 {{ text-align: center; font-size: 16pt; font-weight: bold; }}
    .section {{ margin-bottom: 10px; }}
    .details-table {{ width: 100%; margin-top: 10px; }}
    .details-table td {{ padding: 5px; vertical-align: top; }}
    .signature-wrapper {{
        margin-top: 40px;
        display: flex;
        justify-content: space-between;
        align-items: flex-end; /* 💥 Ajouté pour aligner le bas */
    }}
    .signature-block {{
        width: 45%;
        text-align: center;
    }}
    .signature-block img {{
        height: 60px;
        margin-top: 10px;
        object-fit: contain; /* 💥 Ajouté pour que toutes les images gardent leurs proportions sans casser */
    }}
    .footer {{ text-align: right; font-size: 10pt; margin-top: 20px; }}
</style>

            </head>
            <body>
                <img src='data:image/png;base64,{logoBase64}' style='height:50px;' />
                <h1>NOTIFICATION D'ACCORD DE PRINCIPE</h1>

                <div class='section'>
                    <strong>Objet :</strong> Accord de principe pour l'octroi d'un {data.TypeCredit}.<br/>
                   
                </div>

                <table class='details-table'>
                    <tr><td><strong>Nom & Prénom :</strong></td><td>{data.NomPrenom}</td></tr>
                    <tr><td><strong>Compte N° :</strong></td><td>{data.NumeroCompte}</td></tr>
                </table>

                <div class='section'>
                    <strong>Montant Accordé :</strong> {data.MontantAccorde}<br/>
                    <strong>Durée :</strong> {data.Duree}
                </div>

                <div class='section'>
                    Il demeure entendu que l'accord définitif est subordonné à l'accomplissement des formalités suivantes :
                    <ul><li>{data.Conditions}</li></ul>
                    Condition particulière : {data.ConditionParticuliere}
                </div>

                <div class='section'>
                    Le présent accord de principe est valable six (06) mois à compter de sa notification.
                </div>

                <div class='section'>
                    Fait à Tunis, le {data.DateNotification:dd/MM/yyyy}
                </div>

                <div class='signature-wrapper'>
                    <div class='signature-block left'>
                        <div>Direction Bien-Être Social</div>
                        <img src='data:image/png;base64,{signature1Base64}' alt='Signature 1'/>
                    </div>
                    <div class='signature-block right'>
                        <div>Direction Centrale Capital Humain</div>
                        <img src='data:image/png;base64,{signature2Base64}' alt='Signature 2'/>
                    </div>
                </div>

                <div class='footer'>1/2</div>
            </body>
            </html>";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = html
                    }
                }
            };

            return _converter.Convert(doc);
        }

        private string GetImageAsBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }
    }

    public class NotificationData
    {
        public string LogoPath { get; set; }
        public string Signature1Path { get; set; }
        public string Signature2Path { get; set; }
        //public string Reference { get; set; }
        public string NomPrenom { get; set; }
        public string NumeroCompte { get; set; }
        public string MontantAccorde { get; set; }
        public string Duree { get; set; }
        public string TypeCredit { get; set; }
        public string Conditions { get; set; }
        public string ConditionParticuliere { get; set; }
        public DateTime DateNotification { get; set; }
    }
}
