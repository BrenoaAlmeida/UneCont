using Microsoft.EntityFrameworkCore;

namespace Model
{    
    public class UneContext : DbContext
    {
        public UneContext(DbContextOptions<UneContext> options) : base(options)
        {
        }

        public UneContext()
        {            
        }

        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<LogAgora> LogAgora { get; set; }
        public virtual DbSet<LogMinhaCdn> LogMinhaCdn { get; set; }
    }    
}
