using Simulation.API.Shared;

namespace Simulation.API.Services
{
    public class SimulationService
    {
        public SimulationResponse CalculateSimulation(SimulationRequest request)
        {
            // Déterminer le taux selon le type de crédit
            double tauxAnnuel = GetTauxParTypeCredit(request.TypeCredit);
            double tauxMensuel = tauxAnnuel / 12;
            double montantFinancement = request.TotalPrixAcquisition - request.ApportPropre;

            // Vérification de l'apport propre (immobilier et auto nécessitent 20%)
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

            // Vérifier si le montant financé est valide
            if (montantFinancement <= 0)
            {
                return new SimulationResponse
                {
                    Mensualite = 0,
                    Message = "Le montant du financement est trop faible.",
                    TauxEndettement = 0
                };
            }

            // Vérification du revenu brut
            if (request.RevenuBrut <= 0)
            {
                return new SimulationResponse
                {
                    Mensualite = 0,
                    Message = "Le revenu brut doit être supérieur à 0.",
                    TauxEndettement = 0
                };
            }

            // Durée en mois
            int dureeEnMois = request.DureeRemboursement * 12;

            // Calcul de la mensualité selon la formule de l'annuité
            double mensualite = (montantFinancement * tauxMensuel) / (1 - Math.Pow(1 + tauxMensuel, -dureeEnMois));
            mensualite = Math.Round(mensualite, 2);

            // Calcul du taux d'endettement
            double mensualiteTotale = mensualite + (request.MensualiteAutresFinancements ?? 0);
            double tauxEndettement = (mensualiteTotale / request.RevenuBrut) * 100;

            // Déterminer l'éligibilité en fonction du taux d'endettement
            string message = tauxEndettement switch
            {
                > 40 => "Votre capacité ne vous permet pas d'obtenir ce crédit.",
                > 33 => "Votre taux d'endettement est élevé.",
                _ => "Vous êtes éligible pour ce crédit."
            };

            // Calcul de l'échéancier
            var echeancier = CalculerEcheancier(montantFinancement, tauxMensuel, mensualite, dureeEnMois);

            return new SimulationResponse
            {
                Mensualite = mensualite,
                Message = message,
                TauxEndettement = Math.Round(tauxEndettement, 2),
                ListeSM = echeancier
            };
        }

        private double GetTauxParTypeCredit(string typeCredit)
        {
            return typeCredit switch
            {
                "immobilier" => 0.09,  // 9%
                "auto" => 0.11,  // 9.35%
                "conso" => 0.2,  // 11%
                _ => 0.07  // 7%
            };
        }

        private List<Echeance> CalculerEcheancier(double capitalInitial, double tauxMensuel, double mensualite, int dureeEnMois)
        {
            var echeancier = new List<Echeance>();
            double capitalRestant = capitalInitial;

            for (int mois = 1; mois <= dureeEnMois; mois++)
            {
                double interet = Math.Round(capitalRestant * tauxMensuel, 2);
                double capitalRembourse = Math.Round(mensualite - interet, 2);
                capitalRestant = Math.Round(capitalRestant - capitalRembourse, 2);

                echeancier.Add(new Echeance
                {
                    NumeroEcheance = mois.ToString(),
                    EnCoursCredit = Math.Max(0, capitalRestant).ToString("F2"),
                    Capital = capitalRembourse.ToString("F2"),
                    Interet = interet.ToString("F2"),
                    Mensualite = mensualite.ToString("F2")
                });

                if (capitalRestant <= 0) break;
            }
            return echeancier;
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
