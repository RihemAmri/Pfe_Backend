namespace DepotDocuments.API.DTO
{
public class CreditMessageDto
{
    public string IdDemande { get; set; }
    public string IdClient { get; set; }
    public decimal Montant { get; set; }
    public int DureeMois { get; set; }
    public string TypeCredit { get; set; }
}}
