// Purpose: Model for DateTimeInfo entity in CDR context.
namespace Cdr.Api.Interfaces;

public interface IMongoDbSettings
{
    string DatabaseName { get; set; }
    
    string CollectionName { get; set; }
}