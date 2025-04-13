using MongoDB.Driver;
using DepotDocuments.API.Entities;
using DepotDocuments.API.Data; 

namespace DepotDocuments.API.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IMongoCollection<Document> _collection;

        public DocumentRepository(IDocumentContext context)
        {
            _collection = context.Documents;
        }

        public async Task SaveAsync(Document doc)
        {
            await _collection.InsertOneAsync(doc);
        }
    }
}
