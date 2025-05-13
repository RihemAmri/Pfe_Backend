namespace Credit.API.DTOs
{
    public class CreateCreditDto
    {
        public string IdClient { get; set; }
         public string IdDemande { get; set; }
        public decimal Montant { get; set; }
        public int DureeMois { get; set; }
        

        public DateTime DateDebut { get; set; } = DateTime.Now;
        public string TypeCredit { get; set; }  // Nouveau champ Type de crédit
          public float TauxInteret { get; set; }
          public bool InteretFixe { get; set; }
    }

    public class AmortissementDto
    {
        public int Mois { get; set; }
        public decimal CapitalRestant { get; set; }
        public decimal Mensualite { get; set; }
        public decimal Interet { get; set; }
        public bool Paye { get; set; }
        public DateTime DateEcheance { get; set; }
    }

    public class CreditResponseDto
    {
        public string Id { get; set; }
        public string IdClient { get; set; }
        public string IdDemande { get; set; }
        public decimal Montant { get; set; }
        public int DureeMois { get; set; }
        public float TauxInteret { get; set; }
        public bool InteretFixe { get; set; }
        public DateTime DateDebut { get; set; }
        public string Status { get; set; }

        public string TypeCredit { get; set; }  // Nouveau champ Type de crédit
        public List<AmortissementDto> TableauAmortissement { get; set; }
    }
    public class CreditSansAmortissementDto
    {
        public string Id { get; set; }
        public string IdClient { get; set; }
        public string IdDemande { get; set; }
        public decimal Montant { get; set; }
        public int DureeMois { get; set; }
        public float TauxInteret { get; set; }
        public bool InteretFixe { get; set; }
        public DateTime DateDebut { get; set; }
        public string Status { get; set; }

        public string TypeCredit { get; set; }
    }
}
