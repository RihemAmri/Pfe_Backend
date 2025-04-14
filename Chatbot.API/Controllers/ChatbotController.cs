using Microsoft.AspNetCore.Mvc;

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
                // Questions sur les types de crédits
                var q when q.Contains("crédit personnel") => "Le crédit personnel permet de financer tous types de projets.",
                var q when q.Contains("crédit auto") => "Le crédit auto est destiné à l’achat d’un véhicule neuf ou d’occasion.",
                var q when q.Contains("prêt immobilier") => "Le prêt immobilier est utilisé pour financer l'achat d'un bien immobilier.",

                // Questions sur la simulation
                var q when q.Contains("simulation") || q.Contains("simuler") =>
                    "Pour simuler un crédit, vous devez renseigner le type de crédit, le prix total, votre apport propre, vos revenus, la durée de remboursement, etc.",

                // Explication des champs
                var q when q.Contains("total prix acquisition") || q.Contains("prix d’acquisition") || q.Contains("prix total") || q.Contains("Total Prix Acquisition") =>
                    "Le champ 'TotalPrixAcquisition' représente le coût total du bien que vous souhaitez financer, par exemple le prix d’une maison ou d’une voiture.",
                var q when q.Contains("apport propre") || q.Contains("apport personnel") || q.Contains("Apport Propre") =>
                    "Le champ 'ApportPropre' est le montant que vous êtes prêt à payer de votre poche. Il réduit le montant à financer par le crédit.",
                var q when q.Contains("revenu") || q.Contains("revenu brut") || q.Contains("Revenu Brut ") =>
                    "Le champ 'RevenuBrut' correspond à vos revenus mensuels avant déductions. Il permet d’évaluer votre capacité de remboursement.",
                var q when q.Contains("mensualite autres financements") || q.Contains("autres crédits") || q.Contains("Mensualité Autres Financements") =>
                    "Le champ 'MensualiteAutresFinancements' correspond à la somme que vous remboursez chaque mois pour d’autres crédits en cours.",
                var q when q.Contains("duree") || q.Contains("durée remboursement") || q.Contains("Durée de Remboursement") =>
                    "Le champ 'DureeRemboursement' est la durée souhaitée du crédit en années. Plus elle est longue, plus la mensualité est basse, mais vous paierez plus d’intérêts.",
                var q when q.Contains("type credit") =>
                    "Le champ 'TypeCredit' permet de spécifier le type de crédit souhaité : immobilier, auto, ou consommation.",
                var q when q.Contains("type financement") || q.Contains("Type de Financement") || q.Contains("type de financement") =>
                    "Le champ 'TypeFinancement' précise la nature du financement : partiel ou total. Il peut aider à mieux structurer le dossier.",
                var q when q.Contains("bonjour") || q.Contains("salut") || q.Contains("bonsoir") || q.Contains("coucou") =>
                    $"Bonjour {userName} 👋 ! Je suis votre assistant crédit. Posez-moi vos questions sur les crédits, la simulation ou les documents nécessaires.",

                // Autres questions générales
                var q when q.Contains("comment prendre un crédit") || q.Contains("obtenir un crédit") || q.Contains("obtenir un credit") || q.Contains("étapes") || q.Contains("etapes") =>
                    "Voici les étapes pour obtenir un crédit :\n1. Simuler votre crédit.\n2. Déposer les documents nécessaires.\n3. Attendre l’étude de votre dossier.\n4. Signer le contrat si le crédit est approuvé.",
                var q when q.Contains("documents nécessaires") || q.Contains("pièces à fournir") || q.Contains("documents") =>
                    "Les documents nécessaires sont :\n- Copie de la CIN\n- Fiche de paie\n- Attestation de salaire\n- Relevé bancaire, si disponible.",
                var q when q.Contains("délai") || q.Contains("traitement du dossier") =>
                    "Le traitement de votre dossier prend généralement entre 3 à 5 jours ouvrables après réception de tous les documents.",
                var q when q.Contains("refus") => "Un crédit peut être refusé si votre taux d’endettement est trop élevé ou si vos revenus sont insuffisants.",
                var q when q.Contains("taux") => "Les taux dépendent de la durée, du type de crédit et de votre profil. Utilisez notre simulateur pour une estimation précise.",
                var q when q.Contains("conditions") => "Les conditions d’octroi dépendent du type de crédit, de vos revenus, de votre apport et de vos charges mensuelles.",
                var q when q.Contains("salaire minimum") => "Il n’y a pas de salaire fixe, mais votre revenu doit suffire à couvrir la mensualité sans dépasser un taux d’endettement de 33% à 40%.",
                var q when q.Contains("client") || q.Contains("est-ce que je dois être client") || q.Contains("client stb") =>
                    "Oui, vous devez être client STB pour demander un crédit en ligne.\nSi vous n’êtes pas encore client, vous pouvez créer un compte via l'application STB Everywhere ou vous rendre à l'agence la plus proche.",
                var q when q.Contains("types de crédit") || q.Contains("type de crédit") || q.Contains("types crédits") || q.Contains("types credits") || q.Contains("types de  credits") || q.Contains("types") =>
                    "Voici les principaux types de crédit proposés :\n- Crédit immobilier\n- Crédit auto\n- Crédit consommation (personnel)",
                var q when q.Contains("c’est quoi un crédit") || q.Contains("qu’est-ce qu’un crédit") || q.Contains("credit") =>
                    "Un crédit est une somme d’argent que vous empruntez à la banque pour financer un projet (maison, voiture, études…). En retour, vous vous engagez à rembourser cette somme chaque mois, avec des intérêts, sur une durée déterminée.",
                 
                var q when q.Contains("merci") || q.Contains("thanks") =>
                  "Avec plaisir 😊 ! Si vous avez d'autres questions, je suis là pour vous aider.",
                _ => "Désolé, je n’ai pas compris votre question. Pouvez-vous la reformuler ?"
            };

            return Ok(new { reply = response });
        }
    }

    public class ChatRequest
    {
        public string Text { get; set; }
        public string UserName { get; set; }
    }
}
