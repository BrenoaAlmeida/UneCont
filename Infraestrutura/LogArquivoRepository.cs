﻿using System.Collections.Generic;
using System.Linq;
using Model;

namespace Infraestrutura
{
    public class LogArquivoRepository
    {
        UneContexto _contexto;
        public LogArquivoRepository(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public IList<LogArquivo> ObterLogsArquivo()
        {
            var logsArquivo = _contexto.LogArquivo.ToList();

            if (logsArquivo.Count == 0)
                return null;

            return logsArquivo;
        }
    }
}