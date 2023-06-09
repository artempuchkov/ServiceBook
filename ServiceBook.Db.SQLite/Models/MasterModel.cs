using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class MasterModel
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public int Interval { get; set; }
        public string PhoneNumber { get; set; }
    }
}
