using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class BusFilterDto
    {
        public string? DriverName { get; set; }

        public string? DriverPhoneNumber { get; set; }

        public string? BusStopStation { get; set; }

        public int? CarNumber { get; set; }

        public int? BusCapacity { get; set; }

        public string? CarModel { get; set; }
    }
}
