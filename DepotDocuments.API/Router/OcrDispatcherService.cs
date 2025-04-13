using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DepotDocuments.API.Services.Ocr.Interfaces;
using DepotDocuments.API.Services.Ocr; // Pour les CinOcrProcessor, etc.
using DepotDocuments.API.Shared; // Pour TypeDocument

namespace DepotDocuments.API.Router
{
    public class OcrDispatcherService
    {
        private readonly IDictionary<TypeDocument, IOcrProcessor> _processors;

        public OcrDispatcherService(IEnumerable<IOcrProcessor> processors)
        {
            _processors = processors.ToDictionary(
                p => p switch
                {
                    CinOcrProcessor => TypeDocument.CIN,
                    FichePaieOcrProcessor => TypeDocument.FICHE_DE_PAIE,
                    AttestationSalaireOcrProcessor => TypeDocument.ATTESTATION_SALAIRE,
                    _ => throw new NotImplementedException("Type de processor non pris en charge.")
                });
        }

        public Task<string> ProcessAsync(IFormFile file, TypeDocument type)
        {
            if (!_processors.ContainsKey(type))
                throw new ArgumentException("Type de document non supporté");

            return _processors[type].ExtractTextAsync(file);
        }
    }
}
