using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class WorkingModeModel
    {
        public int Id { get; set; }
        public string day { get; set; }
        public string time_start { get; set; }
        public string time_end { get; set; }
    }
}
