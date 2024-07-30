using System;

namespace GT.WebServices.API.Domain
{
    public record JobCode(Guid Id, Guid JobCategoryId, string Name, string Code, DateTimeOffset LastModifiedOn);


}
