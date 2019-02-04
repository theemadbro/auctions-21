using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace beltfix.Models
{
    
   
    public class CurrentUser
    {
        public int id { get; set; }
        public string name { get; set; }
        public int wallet { get; set; }

    }
}
