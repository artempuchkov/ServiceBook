using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class UserIdentityModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }   
        public string Email { get; set; }
        public int Confirm { get; set; }
    }
}
