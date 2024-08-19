using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public class ApiReponse<T>
    {
        public T Data { get; set; }
        public Pagination Pagination { get; set; }
        public string Message { get; set; }
        public List<string> ErrorList { get; set; } = new List<string>();
    }
}
