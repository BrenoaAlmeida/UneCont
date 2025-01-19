using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Infraestrutura
{
    public class LogRepository

    {
        private UneContexto _contexto;
        public LogRepository(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public Log SalvarLog(Log log) {
            _contexto.Add(log);
            return log;
        }

        public Log ObterLogPorIdentificador(int id)
        {
            return _contexto.Log.Where(l => l.Id == id)
                .Include(l => l.LogAgora)
                .Include(l => l.LogArquivo)
                .Include(l => l.LogMinhaCdn)
                .FirstOrDefault();
        }

        public IList<Log> ObterLogs(){
           var logs = _contexto.Log
                .Include(l => l.LogAgora)
                .Include(l => l.LogArquivo)
                .Include(l => l.LogMinhaCdn)
                .ToList();

            if (logs.Count == 0)           
                return null;

            return logs;
            
        }
    }
}
