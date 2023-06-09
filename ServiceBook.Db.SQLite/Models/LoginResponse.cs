using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBook.Db.SQLite.Models
{
    public class LoginResponse
    {
        public string FIO { get; set; } = null;
        public string Email { get; set; } = null;
        public string Token { get; set; } = null;
    }
}
