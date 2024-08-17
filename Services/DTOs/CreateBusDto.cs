using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;

namespace Services.DTOs
{
    public class CreateBusDto
    {
        public string DriverName { get; set; }
        public string DriverPhoneNumber { get; set; }
        public string BusStopStation { get; set; }
        public int CarNumber { get; set; }
        public int BusCapacity { get; set; }
        public string CarModel { get; set; }
        public string BusLineStops { get; set; }
        public string BusType { get; set; }
    }
}
