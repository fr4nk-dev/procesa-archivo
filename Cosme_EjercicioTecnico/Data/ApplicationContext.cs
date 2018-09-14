using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosme_EjercicioTecnico.Data.Model;

namespace Cosme_EjercicioTecnico.Data
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext()
            : base("ApplicationConnectionString")
        {
            Database.SetInitializer<ApplicationContext>(new CreateDatabaseIfNotExists<ApplicationContext>());
        }

        public DbSet<ExcepcionContable> ExceptionContables { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
