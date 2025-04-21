using DepotDocuments.API.Entities;

public interface ICreditDocumentService
{
    Task AddDocumentsAsync(List<CreditDocument> documents);
}
