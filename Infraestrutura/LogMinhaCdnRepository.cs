using System.Collections.Generic;
using System.Linq;
using Model;

namespace Infraestrutura
{
    public class LogMinhaCdnRepository
    {
        private UneContexto _contexto;
        public LogMinhaCdnRepository(UneContexto context)
        {
            _contexto = context;
        }

        public IList<LogMinhaCdn> ObterPorIdentificador(int identificador)
        {
            return _contexto.LogMinhaCdn.Where(l => l.LogId == identificador).ToList();
        }
    }
}
