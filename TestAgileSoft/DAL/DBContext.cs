using Microsoft.EntityFrameworkCore;
using TestAgileSoft.Models;

namespace ApiCertiCorp.DAL
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        public DbSet<Usuario> TSTAS_Usuarios { get; set; }
        public DbSet<Tarea> TSTAG_Tareas { get; set; }
    }
}
