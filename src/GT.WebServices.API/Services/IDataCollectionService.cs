using GT.WebServices.API.Domain;
using System;

namespace GT.WebServices.API.Services
{
    public interface IDataCollectionService
    {
        DataCollection GetData();
        DateTime GetRevision();
    }
}