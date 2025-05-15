using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Credit.API.Models{

public class Credits
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string IdClient { get; set; }
     public string IdDemande { get; set; }
    public decimal Montant { get; set; }
    public int DureeMois { get; set; }
    public float TauxInteret { get; set; }
    public bool InteretFixe { get; set; }
    public string TypeCredit { get; set; }
    public string Status { get; set; } = "EnCours"; 

    public DateTime DateDebut { get; set; } = DateTime.Now;

    public List<Amortissement> TableauAmortissement { get; set; } = new();
}}
