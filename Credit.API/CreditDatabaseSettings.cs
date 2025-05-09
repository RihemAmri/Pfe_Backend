namespace Credit.API.Models
{
    public class CreditDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CreditCollectionName { get; set; } = null!;
        public string AmortissementCollectionName { get; set; } = null!;
    }
}
