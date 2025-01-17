using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Repository
{
    public class LogRepository
    {
        private UneContexto _contexto;
        public LogRepository(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public Log SalvarLog(Log log) {
            log.DataDeInsercao = DateTime.Now;
            _contexto.Add(log);
            _contexto.SaveChanges();
            return log;
        }

        public Log ObterLogPorIdentificador(int id)
        {
            return _contexto.Log.Where(l => l.Id == id)
                .Include(l => l.LogAgora)
                .Include(l => l.LogArquivo)
                .Include(l => l.LogMinhaCdn)
                .First();
        }
    }
}
