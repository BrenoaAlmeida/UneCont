using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Repository
{
    public class LogRepository
    {
        private UneContext _context;
        public LogRepository(UneContext context)
        {
            _context = context;
        }

        public Log SalvarLog(Log log) { 
            _context.Add(log);
            _context.SaveChanges();
            return log;
        }

        public Log AtualizarLog(Log log)
        {
            _context.Update(log);
            _context.SaveChanges();
            return log;
        }

        public List<Log> ObterLog()
        {
            return _context.Log.ToList();
        }
        public List<LogMinhaCdn> ObterLogsMinhaCdn()
        {            
            return _context.LogMinhaCdn.ToList();
        }

        public Log ObterLogPorIdentificador(int id)
        {
            return _context.Log.Where(l => l.Id == id)
                .Include(l => l.LogAgora)
                .Include(l => l.LogArquivo)
                .Include(l => l.LogMinhaCdn)
                .First();
        }

        public List<LogAgora> ObterLogsAgora()
        {
            return _context.LogAgora.ToList();
        }

        public List<LogAgora> ObterLogsAgoraPorIdentificador(int id)
        {
            return _context.LogAgora.Where(l => l.LogId == id).ToList();
        }
    }
}
