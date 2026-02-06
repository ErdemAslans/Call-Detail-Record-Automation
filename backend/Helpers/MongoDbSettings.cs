using Cdr.Api.Interfaces;

namespace Cdr.Api.Helpers
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public required string DatabaseName { get; set; }
        
        public required string CollectionName { get; set; }
        
        public required string OperatorCollectionName { get; set; }
        
        public required string DepartmentCollectionName { get; set; }

        public required string BreakCollectionName { get; set; }
    }
}