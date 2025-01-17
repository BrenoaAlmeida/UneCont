﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table(name:"LogArquivo")]
    public class LogArquivo
    {
        [Key]
        public int Id { get; set; }
        public int LogId { get; set; }

        public string TipoLog { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeArquivo { get; set; }

        public byte[] Arquivo { get; set; }

        public DateTime DataHoraInsercao { get; set; }

        [ForeignKey("LogId")]
        Log Log { get; set; }
    }
}
