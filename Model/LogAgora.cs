using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table(name: "LogAgora")]
    public class LogAgora
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Provider = "MINHA CDN";

        [Required]
        [MaxLength(20)]
        public string HttpMethod { get; set; }

        [Required]
        [MaxLength(20)]
        public string StatusCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string UriPath { get; set; }

        [Required]
        public int TimeTaken { get; set; }

        [Required]
        public string ResponseSize { get; set; }

        [Required]
        [MaxLength(50)]
        public string CacheStatus { get; set; }        
        
        public int LogId { get; set; }
        
        [ForeignKey("LogId")]
        public Log Log { get; set; }

        [Required]
        public DateTime DataHoraInsercao { get; set; }

    }
}