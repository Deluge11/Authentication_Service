using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
