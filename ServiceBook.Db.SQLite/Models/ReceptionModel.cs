using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class ReceptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Master { get; set; }
        public string Interval { get; set; }
    }
}
