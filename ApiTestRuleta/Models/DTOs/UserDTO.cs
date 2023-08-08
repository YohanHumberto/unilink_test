using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTestRuleta.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Monto { get; set; }
    }
}