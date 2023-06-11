using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class RequestUserModel
    {
        public DateTime date_request { get; set;}
        public string description { get; set;}
        public string car_id { get; set; }
        public int user_id { get; set; }
    }
}
