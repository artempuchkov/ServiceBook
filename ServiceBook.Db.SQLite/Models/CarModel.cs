using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class CarModel
    {
        public string vin { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int release_year { get; set; }
        public string state_number { get; set; }
        public int user_id { get; set; }
    }
}
