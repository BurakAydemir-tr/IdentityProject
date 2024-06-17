using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class AppUser : IdentityUser<int>
    {
        public Gender? Gender { get; set; }
        public string? Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        
    }
}
