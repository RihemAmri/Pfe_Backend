namespace Credit.API.Models{

public class Amortissement
{
    public int Mois { get; set; }
    public decimal CapitalRestant { get; set; }
    public decimal Mensualite { get; set; }
    public decimal Interet { get; set; }
    public bool Paye { get; set; } = false;
    public DateTime DateEcheance { get; set; }
}}
