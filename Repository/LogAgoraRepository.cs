using System.Collections.Generic;
using System.Linq;
using Model;

namespace Repository
{
    public class LogAgoraRepository
    {
        private UneContexto _contexto;
        public LogAgoraRepository(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public List<LogAgora> ObterLogsAgoraPorIdentificador(int id)
        {
            return _contexto.LogAgora.Where(l => l.LogId == id).ToList();
        }
    }
}
