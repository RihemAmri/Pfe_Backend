using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FuzzySharp;


namespace ChatbotBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] ChatRequest request)
        {
            string question = request.Text.ToLower();
            string userName = request.UserName;
            string response;

            response = question switch
            {
                var q when IsSimilar(q, "crédit personnel") =>
                    "Le crédit personnel permet de financer tous types de projets.",

                var q when IsSimilar(q, "crédit auto") =>
                    "Le crédit auto est destiné à l’achat d’un véhicule neuf ou d’occasion.",

                var q when IsSimilar(q, "prêt immobilier") =>
                    "Le prêt immobilier est utilisé pour financer l'achat d'un bien immobilier.",

                var q when IsSimilar(q, "simulation", "simuler") =>
                    "Pour simuler un crédit, vous devez renseigner le type de crédit, le prix total, votre apport propre, vos revenus, la durée de remboursement, etc.",

                var q when IsSimilar(q, "total prix acquisition", "prix d’acquisition", "prix total", "Total Prix Acquisition") =>
                    "Le champ 'TotalPrixAcquisition' représente le coût total du bien que vous souhaitez financer, par exemple le prix d’une maison ou d’une voiture.",

                var q when IsSimilar(q, "apport propre", "apport personnel", "Apport Propre") =>
                    "Le champ 'ApportPropre' est le montant que vous êtes prêt à payer de votre poche. Il réduit le montant à financer par le crédit.",

                var q when IsSimilar(q, "revenu", "revenu brut", "Revenu Brut") =>
                    "Le champ 'RevenuBrut' correspond à vos revenus mensuels avant déductions. Il permet d’évaluer votre capacité de remboursement.",

                var q when IsSimilar(q, "mensualite autres financements", "autres crédits", "Mensualité Autres Financements") =>
                    "Le champ 'MensualiteAutresFinancements' correspond à la somme que vous remboursez chaque mois pour d’autres crédits en cours.",

                var q when IsSimilar(q, "duree", "durée remboursement", "Durée de Remboursement") =>
                    "Le champ 'DureeRemboursement' est la durée souhaitée du crédit en années. Plus elle est longue, plus la mensualité est basse, mais vous paierez plus d’intérêts.",

                var q when IsSimilar(q, "type credit") =>
                    "Le champ 'TypeCredit' permet de spécifier le type de crédit souhaité : immobilier, auto, ou consommation.",

                var q when IsSimilar(q, "type financement", "Type de Financement", "type de financement") =>
                    "Le champ 'TypeFinancement' précise la nature du financement : partiel ou total. Il peut aider à mieux structurer le dossier.",

                var q when IsSimilar(q, "bonjour", "salut", "bonsoir", "coucou") =>
                    $"Bonjour {userName} 👋 ! Je suis votre assistant crédit. Posez-moi vos questions sur les crédits, la simulation ou les documents nécessaires.",

                var q when IsSimilar(q, "anticipation", "rembourser en avance", "payer le crédit avant", "remboursement anticipé") =>
                    "Pour faire une demande d’anticipation de crédit :\n1. Connectez-vous à votre compte.\n2. Allez dans vos crédits en cours.\n3. Cliquez sur 'Demande d’anticipation'.\n4. Expliquez la cause de votre demande.\n5. Joignez une attestation de paiement du reste du crédit.\n6. Après validation, votre crédit sera considéré comme anticipé avec un ajustement des taux si nécessaire.",

                var q when IsSimilar(q, "comment prendre un crédit", "obtenir un crédit", "obtenir un credit", "étapes", "etapes") =>
                    "Voici les étapes pour obtenir un crédit :\n1. Simuler votre crédit.\n2. Déposer les documents nécessaires.\n3. Attendre l’étude de votre dossier.\n4. Signer le contrat si le crédit est approuvé.",

                var q when IsSimilar(q, "documents nécessaires", "pièces à fournir", "documents") =>
                    "Les documents à fournir varient selon votre activité, le type de crédit et le type de financement. Par exemple :\n" +
                    "- Pour un emploi libéral : Engagement de domiciliation, relevés bancaires des 6 derniers mois, déclaration de revenus.\n" +
                    "- Pour un crédit immobilier Masken Awal : Extrait de naissance, engagement sur l'honneur, attestation de salaire, etc.\n" +
                    "- Si le vendeur est un particulier : des documents supplémentaires comme le titre de propriété et l’attestation de conformité sont requis.\n" +
                    "Pour plus de détails, accédez à l’espace justificatifs après avoir simulé un crédit.",

                var q when IsSimilar(q, "délai", "traitement du dossier") =>
                    "Le traitement de votre dossier prend généralement entre 3 à 5 jours ouvrables après réception de tous les documents.",

                var q when IsSimilar(q, "refus") =>
                    "Un crédit peut être refusé si votre taux d’endettement est trop élevé ou si vos revenus sont insuffisants.",

                var q when IsSimilar(q, "taux") =>
                    "Les taux dépendent de la durée, du type de crédit et de votre profil. Utilisez notre simulateur pour une estimation précise.",

                var q when IsSimilar(q, "conditions") =>
                    "Les conditions d’octroi dépendent du type de crédit, de vos revenus, de votre apport et de vos charges mensuelles.",

                var q when IsSimilar(q, "salaire minimum") =>
                    "Il n’y a pas de salaire fixe, mais votre revenu doit suffire à couvrir la mensualité sans dépasser un taux d’endettement de 33% à 40%.",

                var q when IsSimilar(q, "client", "est-ce que je dois être client", "client stb") =>
                    "Oui, vous devez être client STB pour demander un crédit en ligne.\nSi vous n’êtes pas encore client, vous pouvez créer un compte via l'application STB Everywhere ou vous rendre à l'agence la plus proche.",

                var q when IsSimilar(q, "types de crédit", "type de crédit", "types crédits", "types credits", "types de  credits", "types") =>
                    "Voici les principaux types de crédit proposés :\n- Crédit immobilier\n- Crédit auto\n- Crédit consommation (personnel)",

                var q when IsSimilar(q, "c’est quoi un crédit", "qu’est-ce qu’un crédit", "credit") =>
                    "Un crédit est une somme d’argent que vous empruntez à la banque pour financer un projet (maison, voiture, études…). En retour, vous vous engagez à rembourser cette somme chaque mois, avec des intérêts, sur une durée déterminée.",

                var q when IsSimilar(q, "merci", "thanks") =>
                    "Avec plaisir 😊 ! Si vous avez d'autres questions, je suis là pour vous aider.",

                _ => "Désolé, je n’ai pas compris votre question. Pouvez-vous la reformuler ?"
            };

            return Ok(new { reply = response });
        }

        // Fonction utilitaire pour vérifier si la question contient un des mots-clés
        private static bool IsSimilar(string question, params string[] keywords)
{
    foreach (var keyword in keywords)
    {
        var score = Fuzz.Ratio(question.ToLower(), keyword.ToLower());
        if (score >= 80) // 80 est un seuil ajustable
            return true;
    }
    return false;
}

    }

    public class ChatRequest
    {
        public string Text { get; set; }
        public string UserName { get; set; }
    }
}
