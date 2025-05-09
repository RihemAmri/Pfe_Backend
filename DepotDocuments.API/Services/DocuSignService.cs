using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
namespace DepotDocuments.API.Services
{
    public class DocuSignService
    {
        private readonly string integrationKey = "6ea4db3c-68f5-4b59-a260-8e5bacc8a61e"; // aussi appelé client_id
        private readonly string userId = "02f6acb0-e692-4e9f-af45-5f75beb47cb1"; // GUID de ton utilisateur DocuSign
        private readonly string authServer = "account-d.docusign.com";
        private readonly string privateKey = @"
        -----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAlrxesPOxX4GFwyg/+W5Plpp5SXBjGin1h7C8+cWt8unJZVWQ
0Ev5+rh1vhMEQ5yg31xHfigHAtJIMdtEIKwqrGEsuxygEmNUdz5s5DAVAgKRLn4o
kFjrmAnARH5DEpOGC4B97P4Z47LVxdmRiUegBHB+3YGvWpZYbEei2cq3Be/rJq0o
FudfmoNhZqrgrGivuDoBm55PeLGbthJso/7RHHUexVoVA2A1jV/aBsCe+eaFspZ6
lA+NJDBd8kDXQR5Ur4kmz2TxD3T87fJmptjPxdhPlIanUSNqkM9NC901UFynrth8
+qVR9G6LXlzEj+nZ27zJJMP2U+m+5LMgSxfpXQIDAQABAoIBAA7C77sAgnRJ6OGN
xYYVyXGEOzB38xyS01qwcjE7rKdHQdPMp1vetOJawh0VG6vLYO2+ybmPDfC2yx8m
y+svMRakhY7lZL+oPpNW568JDmWd/r3A2RQZwSioe825V7QwAkaKFMrqr3pe/xRb
FCePfhoxMhtbIeMLKimgclBFy9qm15BsB9hDucZZBc6fba5r25faUOJ/Pk+dry1N
vdQbvz3w1Mvjpj6ZD38jhJzgWpWmgvoZFtBJKVWUAWPjtUzTvW/cXWrU5Mo/zXM+
fXI8BVaO6JSwbdl+a4ymsZiJw8w2p4rRFK0wBEvDFAl+uiiHnajzT8+9jmumrP6i
n3p7bkECgYEA1hWkkOdpZocHAlK3nICWUSxBUfPqpG5NWff3NL0iRjxSexW6tMz1
9uJkjfe9O2IGq0gURhzEUXTGkbGLgu56sTCZPzmKit7CdHDL7eOCvFJptd48to4N
tdNt8X+lq7PtM1RBtorm1g35dEM2ZO6BgvCNlJC7y8uaAgh9DeR/NtUCgYEAtD+P
FOZvSuxIwBlnzm2nsFQNZf1gMMpAkMLGxUUsWBIm1o3cFGp02edAZIi2WOc1G7pc
g7VH0WUCSk1tuSNKcGAm0a3aag27ZnZzllS1bMbJqKVqd6psYWlJfq38zy0hXSkG
AKl5VtA5le0rLIYiISCYZige5VGfFrcNEAsOvGkCgYARdchrLQRyoTaYIOjqsa4Q
xrw5E9yiNoDGgqu9uGMwOUE82qPUlRbGYOZ0kaz0R+VlWMaWhUgZoJ2FSmancg29
n8oP1wCOnOZdGsn3B+Qkuc/3Tj3DYciKeMjxtkhrvYvt8MQ/QArdkFw0+Dpadv1H
EZlqcXdvgpZ37ftNA8LAVQKBgHgEHgwnhxEXFTW7dNARWWdh/+3Z8NNrx+PbnSg6
79TQeUXA/TomzBlPDiQil2/Iuyb0Rqd37BLzs7uvpJByfB0pGI/it5yH+jC2TC5b
xtVf42REiAiX1ERkK4iK5ts3zJBQQWvtbBqu2LkISwgBY/Y00uSqcC/20fv49Fyr
UCuJAoGBAJQWEcn+T03DhGFYxsCmpyv+X/uTp57CKlhqfT7GzpthlHRJjjaZegrg
Wyj2bYaFk2TK+VdvOmQhTaFoFRG0SIMk1q9XDYJ7LsB5JwMn+RfoKESWH/LvRuyj
qHDY535YCPU1VkMWCZ6e22TfRhA0TWYmI6oQhAN5xThJReT04BAI
-----END RSA PRIVATE KEY-----
";
        private readonly string basePath = "https://demo.docusign.net/restapi"; // URL pour sandbox

        public async Task<string> CreateEmbeddedSigning(string signerEmail, string signerName, byte[] pdfBytes)
        {
            var scopes = new List<string> { "signature", "impersonation" };

            var apiClient = new ApiClient(basePath);
            apiClient.SetOAuthBasePath(authServer);

            OAuth.OAuthToken tokenInfo = apiClient.RequestJWTUserToken(
                integrationKey,
                userId,
                authServer,
                System.Text.Encoding.UTF8.GetBytes(privateKey),
                1,
                scopes
            );

            var userInfo = apiClient.GetUserInfo(tokenInfo.access_token);
            string accountId = userInfo.Accounts[0].AccountId;
            //apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + tokenInfo.access_token);
            apiClient.Configuration.DefaultHeader["Authorization"] = "Bearer " + tokenInfo.access_token;

            EnvelopeDefinition envelope = new EnvelopeDefinition
            {
                EmailSubject = "Merci de signer votre demande de crédit",
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(pdfBytes),
                        Name = "Demande.pdf",
                        FileExtension = "pdf",
                        DocumentId = "1"
                    }
                },
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Email = signerEmail,
                            Name = signerName,
                            RecipientId = "1",
                            ClientUserId = "1234", // important pour embedded
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "200",
                                        YPosition = "400"
                                    }
                                }
                            }
                        }
                    }
                },
                Status = "sent"
            };

            //EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
            EnvelopesApi  envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(accountId, envelope);

            string envelopeId = result.EnvelopeId;

            // Création de l'URL pour signature intégrée
            RecipientViewRequest viewRequest = new RecipientViewRequest
            {
                ReturnUrl = "http://localhost:4200/demande/demande-crédit", // ou une autre page Angular après signature
                AuthenticationMethod = "none",
                Email = signerEmail,
                UserName = signerName,
                ClientUserId = "1234"
            };

            ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);
            return viewUrl.Url;
        }
    }
}
