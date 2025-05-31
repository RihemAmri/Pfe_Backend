using DepotDocuments.API.Entities;

public interface ISignatureService
{
    Task SaveSignatureAsync(SignatureRecord signature);
}
