using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServiceBook.Db.SQLite.Models
{
    public class RequestAdminModel
    {
        public int id { get; set; }
        public DateTime date_request { get; set; }
        public string description { get; set; }
        public string vin { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string fio { get; set; }
        public string number_phone { get; set; }
    }
}