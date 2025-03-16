using Simulation.API.Services;

namespace Simulation.API.Shared
{
    public class SimulationRequest
    {
        public double TotalPrixAcquisition { get; set; }
        public double ApportPropre { get; set; }
        public double RevenuBrut { get; set; }
        public double? MensualiteAutresFinancements { get; set; } = 0;

        public int DureeRemboursement { get; set; }

        public string TypeCredit { get; set; } // Ajout du type de crédit ("immobilier", "auto", "conso")

        public string TypeFinancement { get; set; }



    }

    public class SimulationResponse
    {
        public double Mensualite { get; set; }
        public string Message { get; set; }
        public double TauxEndettement { get; set; }
        public List<Echeance> ListeSM { get; set; }
    }
}
