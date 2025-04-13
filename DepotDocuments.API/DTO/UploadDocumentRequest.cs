using DepotDocuments.API.Shared;
namespace DepotDocuments.API.DTO{
public class UploadDocumentRequest

{
    public IFormFile File { get; set; }
    public TypeDocument TypeDocument { get; set; }
}}
