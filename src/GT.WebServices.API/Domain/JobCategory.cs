namespace GT.WebServices.API.Domain;

public record JobCategory(Guid Id, string Name, int Order, DateTimeOffset LastModifiedOn);
