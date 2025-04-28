using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Validation.API.Models
{
    public class ValidationItem
    {
        public string Id { get; set; }
        //public string Reference { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NumeroCompte { get; set; }
        public decimal MontantDemande { get; set; }
        public int DureeEnAnnees { get; set; }
        public string TypeCredit { get; set; }
        public string Email { get; set; }
    }

}
