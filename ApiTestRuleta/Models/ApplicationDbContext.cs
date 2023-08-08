using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApiTestRuleta.Models
{
    public interface IApplicationDbContext
    {
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("pruebaEntities")
        {
        }

        public DbSet<User> Users { get; set; }
    }
}