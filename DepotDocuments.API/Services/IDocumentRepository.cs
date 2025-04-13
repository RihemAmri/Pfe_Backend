using DepotDocuments.API.Entities;
public interface IDocumentRepository

{
    Task SaveAsync(Document doc);
}
