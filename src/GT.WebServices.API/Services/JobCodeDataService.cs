using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Services;

public class JobCodeDataService
{
    private static List<JobCode> DemoData;

    private readonly ILogger _logger;

    static JobCodeDataService()
    {
        LoadDemoData();
    }

    public JobCodeDataService(ILogger<JobCodeDataService> logger)
    {
        _logger = logger;
    }

    public IQueryable<JobCode> Query() =>
                DemoData.OrderBy(x => x.LastModifiedOn).AsQueryable();

    public JobCode GetById(Guid id)
    {
        return DemoData.AsQueryable().SingleOrDefault(x => x.Id == id);
    }

    public List<Guid> GetIds()
    {
        return Query().Select(x => x.Id).ToList();
    }

    public int GetCurrentCount()
    {
        return Query().Count();
    }

    private static void LoadDemoData()
    {
        DemoData = new List<JobCode>
        {
            new (new Guid("e8c2df4c-82b2-41aa-87e2-a6e3ca8ee098"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Sales", "1-1", new DateTimeOffset(2024, 06, 02, 11, 1, 0, TimeSpan.Zero)),
            new (new Guid("442dcb8e-e398-4ae9-b2c1-5a9449e889be"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Marketing", "1-2", new DateTimeOffset(2024, 06, 02, 11, 2, 0, TimeSpan.Zero)),
            new (new Guid("27ac3f63-6d37-4b8b-8363-e1d711e4aa21"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Development", "1-3", new DateTimeOffset(2024, 06, 02, 11, 3, 0, TimeSpan.Zero)),
            new (new Guid("7a4db469-ba07-487f-9f00-70991b187f84"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Kitchen", "1-4", new DateTimeOffset(2024, 06, 02, 11, 4, 0, TimeSpan.Zero)),
            new (new Guid("383e82ed-013f-4b95-bd4e-d53ecd05e172"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Front of House", "1-5", new DateTimeOffset(2024, 06, 01, 11, 5, 0, TimeSpan.Zero)),
            new (new Guid("caab1bee-e571-4ed2-ac69-2b45a8cc0900"), new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Maintenance", "1-6", new DateTimeOffset(2024, 06, 02, 11, 6, 0, TimeSpan.Zero)),
            
            new (new Guid("0e6e1910-e626-4d0b-b1c0-10a13425f453"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Customer Sales", "2-1", new DateTimeOffset(2024, 06, 02, 12, 1, 0, TimeSpan.Zero)),
            new (new Guid("a15030ff-d48a-4ca7-9334-bd7ec3ee4528"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Customer Support", "2-2", new DateTimeOffset(2024, 06, 02, 12, 2, 0, TimeSpan.Zero)),
            new (new Guid("4e80973a-73f8-491e-831b-637030ade7be"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Fish", "2-3", new DateTimeOffset(2024, 06, 02, 12, 3, 0, TimeSpan.Zero)),
            new (new Guid("2bc91a99-3c34-497b-8612-31ffac001b3a"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Preperation", "2-4", new DateTimeOffset(2024, 06, 02, 12, 4, 0, TimeSpan.Zero)),
            new (new Guid("993e88aa-b8e4-438a-86cb-aedb4a288df9"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Deserts", "2-5", new DateTimeOffset(2024, 06, 02, 12, 5, 0, TimeSpan.Zero)),
            new (new Guid("8340586f-c3c0-481b-9b1a-559dffb8c6c6"), new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Electrical", "2-6", new DateTimeOffset(2024, 06, 02, 12, 6, 0, TimeSpan.Zero)),
            
            new (new Guid("24a9ce2e-f092-4c6d-853e-c399cdca7718"), new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Normal Time", "3-1", new DateTimeOffset(2024, 06, 02, 13, 1, 0, TimeSpan.Zero)),
            new (new Guid("399cae72-7189-4376-97cb-c74df4beb58e"), new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Overtime 1x", "3-2", new DateTimeOffset(2024, 06, 02, 13, 2, 0, TimeSpan.Zero)),
            new (new Guid("a334ed84-9207-457e-9631-052ae85d7ab9"), new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Overtime 2x", "3-3", new DateTimeOffset(2024, 06, 02, 13, 3, 0, TimeSpan.Zero)),
            new (new Guid("dba583cd-4a97-456e-937c-bddb2bc9b11a"), new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Out of hours on call", "3-4", new DateTimeOffset(2024, 06, 02, 13, 4, 0, TimeSpan.Zero)),
        };
    }
}
