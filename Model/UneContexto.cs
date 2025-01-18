using Microsoft.EntityFrameworkCore;

namespace Model
{    
    public class UneContexto : DbContext
    {
        public UneContexto(DbContextOptions<UneContexto> options) : base(options)
        {
        }

        public UneContexto()
        {            
        }

        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<LogAgora> LogAgora { get; set; }
        public virtual DbSet<LogMinhaCdn> LogMinhaCdn { get; set; }

        public virtual DbSet<LogArquivo> LogArquivo { get; set; }
    }    
}
