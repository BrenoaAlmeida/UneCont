using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table(name:"LogMinhaCdn")]
    public class LogMinhaCdn
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string ResponseSize { get; set; }

        [Required]
        [MaxLength(10)]
        public string StatusCode { get; set; }

        [Required]
        [MaxLength(30)]
        public string CacheStatus { get; set; }

        [Required]
        [MaxLength(50)]
        public string Request { get; set; }

        [Required]
        public string TimeTaken { get; set; }             
        
        public int LogId { get; set; }

        [ForeignKey("LogId")]
        public Log Log { get; set; }
    }
}