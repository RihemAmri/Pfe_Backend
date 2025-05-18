namespace Credit.API.DTOs
{
    public class CreditMessageDto
    {
        public string IdClient { get; set; }
        public string IdDemande { get; set; }
        public decimal Montant { get; set; }
        public int DureeMois { get; set; }
        public string TypeCredit { get; set; }
        public string Emailclient { get; set; }
       
    }
}
