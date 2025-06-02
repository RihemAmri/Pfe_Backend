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
                    "Pour simuler un crédit, vous devez renseigner : \n - le type de crédit, le prix total, votre apport propre, vos revenus, la durée de remboursement, etc.",

                var q when IsSimilar(q, "total prix acquisition", "prix d’acquisition", "prix total", "Total Prix Acquisition") =>
                    "Le champ 'Total Prix Acquisition' représente le coût total du bien que vous souhaitez financer, par exemple : \n - le prix d’une maison ou d’une voiture.",

                var q when IsSimilar(q, "apport propre", "apport personnel", "Apport Propre") =>
                    "Le champ 'Apport Propre' est le montant que vous êtes prêt à payer de votre poche. Il réduit le montant à financer par le crédit.",

                var q when IsSimilar(q, "revenu", "revenu brut", "Revenu Brut") =>
                    "Le champ 'Revenu Brut' correspond à vos revenus mensuels avant déductions. Il permet d’évaluer votre capacité de remboursement.",

                var q when IsSimilar(q, "mensualite autres financements", "autres crédits", "Mensualité Autres Financements") =>
                    "Le champ 'Mensualite Autres Financements' correspond à la somme que vous remboursez chaque mois pour d’autres crédits en cours.",

                var q when IsSimilar(q, "duree", "durée remboursement", "Durée de Remboursement") =>
                "La durée de remboursement dépend du type de crédit :\n" +
                "- Crédit immobilier : jusqu’à 20 ans maximum.\n" +
                "- Crédit auto : selon le type de véhicule, jusqu’à 7 ans maximum.\n" +
                "- Crédit consommation/personnel : jusqu’à 3 ans maximum.\n" +
                "En général, plus la durée est longue, plus la mensualité est faible, mais le coût total du crédit augmente.",

                var q when IsSimilar(q, "type credit") =>
                    "Le champ 'TypeCredit' permet de spécifier le type de crédit souhaité : \n - immobilier, auto, ou consommation.",

                var q when IsSimilar(q, "type financement", "Type de Financement", "type de financement") =>
                    "Le champ 'Type Financement' précise la nature du financement : \n - partiel ou total. Il peut aider à mieux structurer le dossier.",

                var q when IsSimilar(q, "bonjour", "salut", "bonsoir", "coucou") =>
                    $"Bonjour {userName} 👋 ! Je suis votre assistant crédit. \n  Posez-moi vos questions sur les crédits, la simulation ou les documents nécessaires.",

                var q when IsSimilar(q, "anticipation", "rembourser en avance", "payer le crédit avant", "remboursement anticipé") =>
                    "Pour faire une demande d’anticipation de crédit :\n1. Connectez-vous à votre compte.\n2. Allez dans vos crédits en cours.\n3. Cliquez sur 'Demande d’anticipation'.\n4. Expliquez la cause de votre demande.\n5. Joignez une attestation de paiement du reste du crédit.\n6. Après validation, votre crédit sera considéré comme anticipé avec un ajustement des taux si nécessaire.",

                var q when IsSimilar(q, "comment prendre un crédit", "obtenir un crédit", "obtenir un credit", "étapes", "etapes") =>
                    "Voici les étapes pour obtenir un crédit :\n1. Simuler votre crédit.\n2. Déposer les documents nécessaires.\n3. Attendre l’étude de votre dossier.\n4. Signer le contrat si le crédit est approuvé.",

                var q when IsSimilar(q, "documents nécessaires", "pièces à fournir", "documents","Quels sont les documents") =>
                    "Les documents à fournir varient selon votre activité, le type de crédit et le type de financement. Par exemple :\n" +
                    "- Pour un emploi libéral : \n Engagement de domiciliation, relevés bancaires des 6 derniers mois, déclaration de revenus.\n" +
                    "- Pour un crédit immobilier Masken Awal : \n Extrait de naissance, engagement sur l'honneur, attestation de salaire, etc.\n" +
                    "- Si le vendeur est un particulier : \n des documents supplémentaires comme le titre de propriété et l’attestation de conformité sont requis.\n" +
                    "Pour plus de détails, accédez à l’espace justificatifs après avoir simulé un crédit.",

                var q when IsSimilar(q, "délai", "traitement du dossier") =>
                    "Le traitement de votre dossier prend généralement entre 3 à 5 jours ouvrables après réception de tous les documents.",

                var q when IsSimilar(q, "refus") =>
                    "Un crédit peut être refusé si votre taux d’endettement est trop élevé ou si vos revenus sont insuffisants.",

                var q when IsSimilar(q, "taux") =>
                    "Les taux dépendent de la durée, du type de crédit et de votre profil. Utilisez notre simulateur pour une estimation précise.",

                var q when IsSimilar(q, "conditions") =>
                    "Les conditions d’octroi dépendent du type de crédit, de vos revenus, de votre apport et de vos charges mensuelles.",
                var q when IsSimilar(q, "crédit direct", "credit direct") =>
                "Le Crédit Direct est destiné à financer vos dépenses imprévues ou des achats pressants.",
                var q when IsSimilar(q, "prêt epargne confort", "prêt confort", "epargne confort") =>
                    "Le Prêt Épargne Confort est un crédit adossé à un Plan Épargne Confort. Il est destiné à financer vos dépenses relatives aux voyages, loisirs, mariages, etc.",
                var q when IsSimilar(q, "masken awal", "crédit masken awal") =>
                    "Le Crédit MASKEN AWAL est une offre globale de financement liée au projet « Premier Logement » initié par l’État. Il comprend :\n- Une dotation MASKEN AWAL jusqu’à 40 000 DT pour couvrir l’autofinancement.\n- Un crédit standing MASKEN AWAL avec remboursement souple jusqu’à 160 000 DT.",
                var q when IsSimilar(q, "crédit habitat", "credit habitat") =>
                    "Le Crédit Habitat vous permet d’accéder à la propriété de votre logement. Il peut servir pour :\n- L’acquisition d’un logement neuf ou ancien\n- L’achat d’un terrain pour construire un logement\n- La construction ou l’extension d’un logement.",
                var q when IsSimilar(q, "prêt epargne logement", "epargne logement") =>
                    "Le Prêt Épargne Logement est accessible après deux années d’épargne. Il permet de financer :\n- L’achat d’un logement neuf ou ancien à usage d’habitation ou commercial\n- L’achat d’un terrain pour construire un logement ou un terrain à usage agricole\n- La construction ou l’extension d’un logement d’habitation\n- La construction d’un immeuble à usage mixte (habitation et commerce).",
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
    question = question.ToLower();

    foreach (var keyword in keywords)
    {
        var kw = keyword.ToLower();

        // Si la question contient le mot-clé exact
        if (question.Contains(kw))
            return true;

        // Si une similarité partielle (par ex fautes ou tournures proches)
        var score = Fuzz.PartialRatio(question, kw);
        if (score >= 80)
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
