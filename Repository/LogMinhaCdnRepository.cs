using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace Repository
{
    public class LogMinhaCdnRepository
    {
        private UneContexto _contexto;
        public LogMinhaCdnRepository(UneContexto context)
        {
            _contexto = context;
        }

        public List<LogMinhaCdn> ObterPorIdentificador(int identificador)
        {
            return _contexto.LogMinhaCdn.Where(l => l.LogId == identificador).ToList();
        }

        public List<LogMinhaCdn> ObterLogsMinhaCdn()
        {
            return _contexto.LogMinhaCdn.ToList();
        }

    }
}
