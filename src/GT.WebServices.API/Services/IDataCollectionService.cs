using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Services;

public interface IDataCollectionService
{
    DataCollection GetData();
    DateTime GetRevision();
}