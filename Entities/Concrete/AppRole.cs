﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class AppRole : IdentityRole<int>
    {
        public DateTime CreatedOn { get; set; }
    }
}
