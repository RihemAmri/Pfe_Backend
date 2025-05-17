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
        body {{
            font-family: Arial, sans-serif;
            font-size: 12pt;
        }}
        h1 {{
            text-align: center;
            font-size: 16pt;
            font-weight: bold;
            margin-bottom: 30px;
        }}
        .section {{
            margin-bottom: 35px;
        }}
        .details-table {{
            width: 100%;
            margin-top: 10px;
            border-collapse: collapse;
        }}
        .details-table .label {{
            width: 180px;
            vertical-align: top;
            padding-bottom: 8px;
            padding-left: 40px;
        }}
        .details-table td {{
            padding-bottom: 8px;
        }}
        .signature-final {{
            margin-top: 60px;
            text-align: right;
            font-size: 11pt;
        }}
        .signature-final img {{
            height: 70px;
            margin-top: 5px;
            object-fit: contain;
        }}
    </style>
</head>
<body>
    <img src='data:image/png;base64,{logoBase64}' style='height:50px;' />
    <h1>NOTIFICATION D'ACCORD DE PRINCIPE</h1>

    <!-- Section 1 : Objet -->
    <div class='section'>
        <strong>Objet :</strong> Accord de principe pour l'octroi d'un {data.TypeCredit}.
    </div>

    <!-- Section 2 : Infos personnelles -->
    <div class='section'>
        <table class='details-table'>
            <tr>
                <td class='label'><strong>Nom & Prénom :</strong></td>
                <td>{data.NomPrenom}</td>
            </tr>
            <tr>
                <td class='label'><strong>Compte N° :</strong></td>
                <td>{data.NumeroCompte}</td>
            </tr>
        </table>
    </div>

    <!-- Section 3 : Suite à votre demande + conditions principales -->
    <div class='section'>
        Faisant suite à votre demande citée en objet, nous avons le plaisir de vous informer que nous avons donné suite favorable à votre demande et ce, dans les conditions suivantes :
    </div>
    <div class='section'>
        <table class='details-table'>
            <tr>
                <td class='label'><strong>Montant Accordé :</strong></td>
                <td>{data.MontantAccorde}</td>
            </tr>
            <tr>
                <td class='label'><strong>Durée :</strong></td>
                <td>{data.Duree}</td>
            </tr>
        </table>
    </div>

    <!-- Section 4 : Conditions -->
    <div class='section'>
        Il demeure entendu que l'accord définitif est subordonné à l'accomplissement de la (ou des) formalité(s) suivante(s) :
        <ul><li>{data.Conditions}</li></ul>
    </div>

    <!-- Section 5 : Validité -->
    <div class='section'>
        Le présent accord de principe est valable six (06) mois à compter de sa notification.
    </div>

    <!-- Signature -->
    <div class='signature-final'>
        Fait à Tunis, le {data.DateNotification:dd/MM/yyyy}<br/><br/>
        Signature<br/>
        <img src='data:image/png;base64,{signature2Base64}' alt='Signature'/>
    </div>
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
