using Simulation.API.Shared;
namespace Simulation.API.Services
{
    public class SimulationService
    {
        public SimulationResponse CalculateSimulation(SimulationRequest request)
        {
            // Déterminer le taux selon le type de crédit
            double tauxAnnuel = GetTauxParTypeCredit(request.TypeCredit);
            double tauxMensuel = Math.Pow(1 + tauxAnnuel, 1.0 / 12) - 1;
            double montantFinancement = request.TotalPrixAcquisition - request.ApportPropre;

            // Vérification de l'apport propre
            if ((request.TypeCredit == "immobilier" || request.TypeCredit == "auto") &&
                (request.TotalPrixAcquisition * 0.2) > request.ApportPropre)
            {
                return new SimulationResponse
                {
                    Mensualite = 0,
                    Message = $"L'autofinancement doit être au minimum de {(request.TotalPrixAcquisition * 0.2):N2} DT (20% du montant).",
                    TauxEndettement = 0
                };
            }

            // Vérifier si le montant financé est positif
            if (montantFinancement <= 0)
            {
                return new SimulationResponse
                {
                    Mensualite = 0,
                    Message = "Le montant du financement est trop faible.",
                    TauxEndettement = 0
                };
            }

            // Durée en mois
            int dureeEnMois = request.DureeRemboursement * 12;

            // Vérification pour éviter une division par zéro
            if (request.RevenuBrut <= 0)
            {
                return new SimulationResponse
                {
                    Mensualite = 0,
                    Message = "Le revenu brut doit être supérieur à 0.",
                    TauxEndettement = 0
                };
            }

            // Calcul de la mensualité
            double mensualite = (montantFinancement * tauxMensuel) / (1 - Math.Pow(1 + tauxMensuel, -dureeEnMois));

            // Calcul du taux d'endettement
            double mensualiteTotale = mensualite + (request.MensualiteAutresFinancements ?? 0);
            double tauxEndettement = (mensualiteTotale / request.RevenuBrut) * 100;

            // Déterminer le message en fonction du taux d'endettement
            string message;
            if (tauxEndettement > 40)
            {
                message = "Votre capacité ne vous permet pas d'obtenir ce crédit.";
            }
            else if (tauxEndettement > 33)
            {
                message = "Votre taux d'endettement est élevé.";
            }
            else
            {
                message = "Vous êtes éligible pour ce crédit.";
            }

            // Calcul de l'échéancier
            var echeancier = new List<Echeance>();
            double capitalRestant = montantFinancement;

            for (int mois = 1; mois <= dureeEnMois; mois++)
            {
                double interet = capitalRestant * tauxMensuel;
                double capitalRembourse = mensualite - interet;

                // Vérification pour éviter que le capital restant devienne négatif
                if (capitalRembourse > capitalRestant)
                {
                    capitalRembourse = capitalRestant;
                    mensualite = capitalRembourse + interet; // Ajuster la dernière mensualité
                }

                capitalRestant -= capitalRembourse;

                // Ajouter l'échéance
                echeancier.Add(new Echeance
                {
                    NumeroEcheance = mois.ToString(),
                    EnCoursCredit = Math.Max(0, capitalRestant).ToString("F4"),
                    Capital = capitalRembourse.ToString("F4"),
                    Interet = interet.ToString("F4"),
                    Mensualite = mensualite.ToString("F4")
                });

                if (capitalRestant <= 0)
                    break;
            }

            return new SimulationResponse
            {
                Mensualite = Math.Round(mensualite, 2),
                Message = message,
                TauxEndettement = Math.Round(tauxEndettement, 2),
                ListeSM = echeancier
            };
        }

        private double GetTauxParTypeCredit(string typeCredit)
        {
            return typeCredit switch
            {
                "immobilier" => 0.09,  // 5%
                "auto" => 0.935,  // 4%
                "conso" => 1.1,  // 6%
                _ => 0.07  // Par défaut, 3%
            };
        }
    }

    public class Echeance
    {
        public string NumeroEcheance { get; set; }
        public string EnCoursCredit { get; set; }
        public string Capital { get; set; }
        public string Interet { get; set; }
        public string Mensualite { get; set; }
    }
}
