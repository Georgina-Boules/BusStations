using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public class User
    {
        public int Id { get; set; }
        public string En_Full_Name { get; set; }
        public string Ar_Full_Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
    }
}
