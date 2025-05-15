using Microsoft.AspNetCore.Mvc;
using Credit.API.Services;
using Microsoft.Extensions.Options;
using Credit.API.DTOs;
using MongoDB.Driver;
using Credit.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Credit.API.Services
{
    public class CreditService : ICreditService
    {
        private readonly IMongoCollection<Credits> _credits;
        private readonly IMongoCollection<DemandeAnticipation> _demandeAnticipationCollection;

        private readonly CloudinaryService _cloudinaryService;
        public CreditService(IOptions<CreditDatabaseSettings> settings, CloudinaryService cloudinaryService)
        {
            var config = settings.Value;

            var client = new MongoClient(config.ConnectionString);
            var database = client.GetDatabase(config.DatabaseName);
            _credits = database.GetCollection<Credits>(config.CreditCollectionName);
            _demandeAnticipationCollection = database.GetCollection<DemandeAnticipation>("DemandeAnticipation");
            _cloudinaryService = cloudinaryService;

        }

        public async Task<CreditResponseDto> AjouterCreditAsync(CreateCreditDto dto)
        {
            var credit = new Credits
            {
                IdClient = dto.IdClient,
                IdDemande = dto.IdDemande,
                Montant = dto.Montant,
                DureeMois = dto.DureeMois,
                DateDebut = dto.DateDebut,
                TypeCredit = dto.TypeCredit,  // Récupération du Type de crédit
                InteretFixe = dto.DureeMois >= 180,
                TauxInteret = dto.DureeMois >= 180 ? 0.06f : 0.04f
            };

            credit.TableauAmortissement = GenererTableau(credit);

            await _credits.InsertOneAsync(credit);  // Enregistrement dans MongoDB

            return ToDto(credit);
        }

        public async Task<List<CreditResponseDto>> GetCreditsClientAsync(string idClient)
        {
            var credits = await _credits.Find(c => c.IdClient == idClient).ToListAsync();
            return credits.Select(ToDto).ToList();
        }

        public async Task MettreAJourAmortissementParIdAsync(string idCredit)
        {
            var credit = await _credits.Find(c => c.Id == idCredit).FirstOrDefaultAsync();
            if (credit == null) return;

            var maintenant = DateTime.Now;

            foreach (var ligne in credit.TableauAmortissement)
            {
                if (!ligne.Paye && ligne.DateEcheance <= maintenant)
                {
                    ligne.Paye = true;
                    ligne.CapitalRestant -= ligne.Mensualite - ligne.Interet;
                }
            }

            await _credits.ReplaceOneAsync(c => c.Id == credit.Id, credit);
        }
        public async Task<List<CreditSansAmortissementDto>> GetAllCreditsSansAmortissementAsync()
        {
            var credits = await _credits.Find(_ => true).ToListAsync();
            return credits.Select(c => new CreditSansAmortissementDto
            {
                Id = c.Id,
                IdClient = c.IdClient,
                IdDemande = c.IdDemande,
                Status = c.Status,

                Montant = c.Montant,
                DureeMois = c.DureeMois,
                TauxInteret = c.TauxInteret,
                InteretFixe = c.InteretFixe,
                DateDebut = c.DateDebut,
                TypeCredit = c.TypeCredit
            }).ToList();
        }
        public async Task<bool> CloturerCreditAsync(string idCredit)
        {
            var credit = await _credits.Find(c => c.Id == idCredit).FirstOrDefaultAsync();
            if (credit == null)
                return false;

            foreach (var ligne in credit.TableauAmortissement)
            {
                ligne.Paye = true;
                ligne.CapitalRestant = 0;
            }

            credit.Status = "Cloture";

            await _credits.ReplaceOneAsync(c => c.Id == idCredit, credit);
            return true;
        }

        public async Task MettreAJourAmortissementsAsync()
        {
            var tous = await _credits.Find(_ => true).ToListAsync();
            var maintenant = DateTime.Now;

            foreach (var credit in tous)
            {
                foreach (var ligne in credit.TableauAmortissement)
                {
                    if (!ligne.Paye && ligne.DateEcheance <= maintenant)
                    {
                        ligne.Paye = true;
                        ligne.CapitalRestant -= ligne.Mensualite - ligne.Interet;
                    }
                }

                // Vérifier si toutes les lignes sont payées
                credit.Status = credit.TableauAmortissement.All(a => a.Paye) ? "Cloture" : "EnCours";

                await _credits.ReplaceOneAsync(c => c.Id == credit.Id, credit);
            }
        }
        public async Task<List<CreditResponseDto>> GetCreditsParStatusAsync(string status)
        {
            var credits = await _credits.Find(c => c.Status == status).ToListAsync();
            return credits.Select(ToDto).ToList();
        }


        // Helpers
        private List<Amortissement> GenererTableau(Credits credit)
        {
            var list = new List<Amortissement>();
            var montant = credit.Montant;
            var tauxMensuel = (decimal)credit.TauxInteret / 12;
            var mensualite = montant * tauxMensuel / (1 - (decimal)Math.Pow(1 + (double)tauxMensuel, -credit.DureeMois));
            var capitalRestant = montant;

            for (int i = 1; i <= credit.DureeMois; i++)
            {
                var interet = capitalRestant * tauxMensuel;
                var capital = mensualite - interet;
                capitalRestant -= capital;

                list.Add(new Amortissement
                {
                    Mois = i,
                    Mensualite = Math.Round(mensualite, 2),
                    Interet = Math.Round(interet, 2),
                    CapitalRestant = Math.Round(capitalRestant, 2),
                    DateEcheance = credit.DateDebut.AddMonths(i)
                });
            }

            return list;
        }
        public async Task<CreditResponseDto> GetCreditParIdAsync(string idCredit)
        {
            var credit = await _credits.Find(c => c.Id == idCredit).FirstOrDefaultAsync();
            return credit != null ? ToDto(credit) : null;
        }
        public async Task<bool> PayerIntegralementCreditAsync(string idCredit)
        {
            var credit = await _credits.Find(c => c.Id == idCredit).FirstOrDefaultAsync();
            if (credit == null)
                return false;

            foreach (var ligne in credit.TableauAmortissement)
            {
                if (!ligne.Paye)
                {
                    ligne.Paye = true;
                    ligne.CapitalRestant = 0;
                }
            }



            await _credits.ReplaceOneAsync(c => c.Id == idCredit, credit);
            return true;
        }
        public async Task<bool> AjouterDemandeAnticipationAsync(DemandeAnticipationDto dto)
        {
            var demande = new DemandeAnticipation
            {
                IdCredit = dto.IdCredit,
                IdClient = dto.IdClient,
                NumeroCompte = dto.NumeroCompte,
                Raison = dto.Raison,
                Statut = "En Attente",
                DateDemande = DateTime.UtcNow
            };

            await _demandeAnticipationCollection.InsertOneAsync(demande);
            return true;
        }


        public async Task<List<CreditSansAmortissementDto>> GetCreditsParTypeSansAmortissementAsync(string typeCredit)
        {
            var credits = await _credits.Find(c => c.TypeCredit == typeCredit).ToListAsync();

            return credits.Select(c => new CreditSansAmortissementDto
            {
                Id = c.Id,
                IdClient = c.IdClient,
                IdDemande = c.IdDemande,
                Montant = c.Montant,
                DureeMois = c.DureeMois,
                TauxInteret = c.TauxInteret,
                InteretFixe = c.InteretFixe,
                DateDebut = c.DateDebut,
                TypeCredit = c.TypeCredit,
                Status = c.Status
            }).ToList();
        }

        private CreditResponseDto ToDto(Credits credit) => new()
        {
            Id = credit.Id,
            IdClient = credit.IdClient,
            IdDemande = credit.IdDemande,
            Status = credit.Status,

            Montant = credit.Montant,
            DureeMois = credit.DureeMois,
            TauxInteret = credit.TauxInteret,
            InteretFixe = credit.InteretFixe,
            DateDebut = credit.DateDebut,
            TypeCredit = credit.TypeCredit,  // Ajout du Type de crédit dans la réponse
            TableauAmortissement = credit.TableauAmortissement.Select(a => new AmortissementDto
            {
                Mois = a.Mois,
                Mensualite = a.Mensualite,
                Interet = a.Interet,
                CapitalRestant = a.CapitalRestant,
                Paye = a.Paye,
                DateEcheance = a.DateEcheance
            }).ToList()
        };
        public async Task<List<DemandeAnticipation>> GetAnticipationsSansReponseAsync()
{
    return await _demandeAnticipationCollection
        .Find(d => d.ReponseAdmin == null)
        .ToListAsync();
}

public async Task<List<DemandeAnticipation>> GetAnticipationsAvecReponseAsync()
{
    return await _demandeAnticipationCollection
        .Find(d => d.ReponseAdmin != null)
        .ToListAsync();
}

        public async Task<bool> RepondreAnticipationAsync(ReponseAnticipationDto dto)
        {
            var filter = Builders<DemandeAnticipation>.Filter.Eq(a => a.IdDemande, dto.IdDemande);
            var update = Builders<DemandeAnticipation>.Update
                .Set(a => a.ReponseAdmin, dto.ReponseAdmin)
                .Set(a => a.Commentaire, dto.Commentaire)
                .Set(a => a.DateReponse, dto.DateReponse)
                .Set(a => a.Statut, "Répondu");

            var result = await _demandeAnticipationCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task<List<DemandeAnticipation>> GetDemandesAnticipationParClientAsync(string IdCredit)
        {
            var demandes = await _demandeAnticipationCollection
                .Find(d => d.IdCredit == IdCredit)
                .ToListAsync();

            return demandes;
        }
public async Task<bool> UploadRecupayementAsync(string idDemande, IFormFile fichier)
{
    if (fichier == null || fichier.Length == 0) return false;

    var demande = await _demandeAnticipationCollection.Find(d => d.IdDemande == idDemande).FirstOrDefaultAsync();
    if (demande == null) return false;

    var url = await _cloudinaryService.UploadFileAsync(fichier);

    var update = Builders<DemandeAnticipation>.Update
        .Set(d => d.AttestationPaiementUrl, url);

    var result = await _demandeAnticipationCollection.UpdateOneAsync(d => d.IdDemande == idDemande, update);

    return result.ModifiedCount > 0;
}


    }
}
