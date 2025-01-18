using System.Collections.Generic;
using System.Linq;
using Model;

namespace Infraestrutura
{
    public class LogArquivoInfraestrutura
    {
        UneContexto _contexto;
        public LogArquivoInfraestrutura(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public List<LogArquivo> ObterLogsArquivo()
        {
            var logsArquivo = _contexto.LogArquivo.ToList();

            if (logsArquivo.Count == 0)
                return null;

            return logsArquivo;
        }
    }
}