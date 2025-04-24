using DepotDocuments.API.Entities;

public interface ICreditDocumentService
{
    Task AddDocumentsAsync(List<CreditDocument> documents);
    Task<List<CreditDocument>> GetDocumentsByIdsAsync(List<string> ids);
}
