using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GT.WebServices.API.Application.Dtos;
using Microsoft.Extensions.Logging;
using GT.WebServices.API.Domain;
using GT.WebServices.API.Application.Mapping;

namespace GT.WebServices.API.Services
{
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

        public async Task<JobCategory> GetById(Guid id)
        {
            var mapper = new AdsMapper();
            return DemoData.AsQueryable().SingleOrDefault(x => x.Id == id);
        }

        public async Task<List<Guid>> GetIds()
        {
            return Query().Select(x => x.Id).ToList();
        }

        public async Task<int> GetCurrentCount()
        {
            return Query().Count();
        }

        private static void LoadDemoData()
        {
            DemoData = new List<JobCategory>
            {
                new (new Guid("22faabf9-7011-4f89-88d3-12634c57799e"), "Category 1", 1, new DateTimeOffset(2024, 06, 01, 1, 0, 0, TimeSpan.Zero)),
                new (new Guid("b9635975-25e8-4954-a308-80955a8bb561"), "Category 2", 2, new DateTimeOffset(2024, 06, 01, 2, 0, 0, TimeSpan.Zero)),
                new (new Guid("153f6c49-a927-4098-a31b-b863c888e819"), "Category 3", 3, new DateTimeOffset(2024, 06, 01, 3, 0, 0, TimeSpan.Zero)),
            };
        }
    }
}
