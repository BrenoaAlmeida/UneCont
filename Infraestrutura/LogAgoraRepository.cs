using System.Collections.Generic;
using System.Linq;
using Model;

namespace Infraestrutura
{
    public class LogAgoraInfraestrutura
    {
        private UneContexto _contexto;
        public LogAgoraInfraestrutura(UneContexto contexto)
        {
            _contexto = contexto;
        }

        public List<LogAgora> ObterLogsAgoraPorIdentificador(int id)
        {
            return _contexto.LogAgora.Where(l => l.LogId == id).ToList();
        }
    }
}
