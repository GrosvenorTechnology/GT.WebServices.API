using System.Xml;
using System.Xml.Serialization;

namespace GT.WebServices.API.Services;

public class ScheduleDataService
{
    private static List<Schedule> DemoData;

    private readonly ILogger _logger;

    static ScheduleDataService()
    {
        LoadDemoData();
    }

    public ScheduleDataService(ILogger<ScheduleDataService> logger)
    {
        _logger = logger;
    }

    public IQueryable<Schedule> Query() =>
                DemoData.OrderBy(x => x.LastModifiedOn).AsQueryable();

    public Schedule GetById(Guid id)
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
        DemoData =
        [
            new ()
            {
                Id = new Guid("6b8dc3cc-da96-494a-9c7b-3d5f9cb84d2b"),
                ExternalId = "123456",
                EmpId = new Guid("CCBB34B8-90DE-4976-93FA-C1B34F12A95C"),
                Name = "Yesterday",
                StartDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(9),
                EndDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(17),
                SubSchedules = new SubScheduleWrapper
                {
                    Items =
                    [
                        new SubSchedule
                        {
                            Id = "SS1-1",  //Ids do not have to be guids
                            Name = "SubSchedule 1",
                            StartDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(9),
                            EndDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(12),
                        },
                        new SubSchedule
                        {
                            Id = "SS1-2",
                            Name = "SubSchedule 2",
                            StartDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(13),
                            EndDateTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(17),
                        }
                    ]
                }
            },
            new ()
            {
                Id = new Guid("ca3fdbaf-7d16-4548-a60c-7589957d21cd"),
                ExternalId = "587135",
                EmpId = new Guid("CCBB34B8-90DE-4976-93FA-C1B34F12A95C"),
                Name = "Today",
                StartDateTime = DateTime.UtcNow.Date.AddHours(9),
                EndDateTime = DateTime.UtcNow.Date.AddHours(17),
                SubSchedules = new SubScheduleWrapper
                {
                    Items =
                    [
                        new SubSchedule
                        {
                            Id = "SS2-1",  //Ids do not have to be guids
                            Name = "SubSchedule 1",
                            StartDateTime = DateTime.UtcNow.Date.AddHours(9),
                            EndDateTime = DateTime.UtcNow.Date.AddHours(12),
                        },
                        new SubSchedule
                        {
                            Id = "SS2-2",
                            Name = "SubSchedule 2",
                            StartDateTime = DateTime.UtcNow.Date.AddHours(13),
                            EndDateTime = DateTime.UtcNow.Date.AddHours(17),
                        }
                    ]
                }
            },
            new ()
            {
                Id = new Guid("a7359374-4d42-4026-b9c8-d2e130bcf839"),
                ExternalId = "987364",
                EmpId = new Guid("CCBB34B8-90DE-4976-93FA-C1B34F12A95C"),
                Name = "Tomorrow",
                StartDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
                EndDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(17),
                SubSchedules = new SubScheduleWrapper
                {
                    Items =
                    [
                        new SubSchedule
                        {
                            Id = "SS3-1",  //Ids do not have to be guids
                            Name = "SubSchedule 1",
                            StartDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
                            EndDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(12),
                        },
                        new SubSchedule
                        {
                            Id = "SS3-2",
                            Name = "SubSchedule 2",
                            StartDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(13),
                            EndDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(17),
                        }
                    ]
                }
            },
        ];
    }
}

[XmlType(TypeName = "schedules")]
public class Schedules : List<Schedule>
{
}

public class Schedule
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; }  //Refernce to schedule in external system
    public Guid EmpId { get; set; }
    public TranslatableString Name { get; set; } = new();
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int? GraceStart { get; set; }
    public int? GraceEnd { get; set; }
    public int? BreakMins { get; set; }
    public SubScheduleWrapper SubSchedules { get; set; } = new SubScheduleWrapper();
    public DateTimeOffset LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
}


public class SubScheduleWrapper
{
    [XmlElement("subSchedule")] 
    public List<SubSchedule> Items { get; set; } = new List<SubSchedule>();
}


public class SubSchedule
{
    public string Id { get; set; }
    public TranslatableString Name { get; set; } = new();
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}


public class TranslatableString
{
    public class TranslatableStringItem
    {
        [XmlAttribute("language")]
        public string Language { get; set; }

        [XmlText]
        public string Value { get; set; }

        public TranslatableStringItem(string value)
        {
            Language = "en";
            Value = value;
        }

        public TranslatableStringItem(string language, string value)
        {
            Language = language;
            Value = value;
        }

        public TranslatableStringItem()
        {

        }
    }


    [XmlElement("text")]
    public List<TranslatableStringItem> Items { get; set; } = new();


    public TranslatableString()
    {
        Items.Add(new(""));
    }

    public TranslatableString(string value)
    {
        Items.Add(new(value));
    }

    public static implicit operator TranslatableString(string input)
    {
        return new TranslatableString(input);
    }
}