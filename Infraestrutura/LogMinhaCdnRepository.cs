using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace Infraestrutura
{
    public class LogMinhaCdnInfraestrutura
    {
        private UneContexto _contexto;
        public LogMinhaCdnInfraestrutura(UneContexto context)
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
