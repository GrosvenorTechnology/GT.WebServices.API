using GT.WebServices.API.Domain;
using GT.WebServices.API.Application.Mapping;

namespace GT.WebServices.API.Services;

public class JobCategoryDataService
{
    private static List<JobCategory> DemoData;

    private readonly ILogger _logger;

    static JobCategoryDataService()
    {
        LoadDemoData();
    }

    public JobCategoryDataService(ILogger<JobCategoryDataService> logger)
    {
        _logger = logger;
    }

    public IQueryable<JobCategory> Query() =>
                DemoData.OrderBy(x => x.LastModifiedOn).AsQueryable();

    public JobCategory GetById(Guid id)
    {
        var mapper = new AdsMapper();
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
        DemoData = new List<JobCategory>
        {
            new (new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Department", 1, new DateTimeOffset(2024, 06, 02, 1, 0, 0, TimeSpan.Zero)),
            new (new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Activity", 2, new DateTimeOffset(2024, 06, 02, 2, 0, 0, TimeSpan.Zero)),
            new (new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Pay Type", 3, new DateTimeOffset(2024, 06, 02, 3, 0, 0, TimeSpan.Zero)),
        };
    }
}
