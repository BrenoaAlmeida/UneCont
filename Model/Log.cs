using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table(name:"Log")]
    public class Log
    {
        public Log()
        {
            LogMinhaCdn = new List<LogMinhaCdn>();
            LogAgora = new List<LogAgora>();
            LogArquivo = new List<LogArquivo>();
        }

        [Key]
        public int Id { get; set; }        

        public string Url { get; set; }

        public DateTime DataDeInsercao { get; set; }

        public List<LogMinhaCdn> LogMinhaCdn { get; set; }

        public List<LogAgora> LogAgora { get; set; }

        public List<LogArquivo> LogArquivo { get; set; }
    }
}
