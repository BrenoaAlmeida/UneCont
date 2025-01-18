﻿using System;
using System.Threading.Tasks;
using Model;

namespace Infraestrutura
{
    public class UnitOfWork : IDisposable
    {
        private UneContexto _contexto;

        public UnitOfWork(UneContexto contexto)
        {
            _contexto = contexto;
        }

        private LogInfraestrutura _logInfraestrutura;
        private LogMinhaCdnInfraestrutura _logMinhaCdnInfraestrutura;
        private LogAgoraInfraestrutura _logAgoraInfraestrutura;

        public LogInfraestrutura Log => _logInfraestrutura ?? (_logInfraestrutura = new LogInfraestrutura(_contexto));        

        public LogMinhaCdnInfraestrutura LogMinhaCdn => _logMinhaCdnInfraestrutura ?? (_logMinhaCdnInfraestrutura  = new LogMinhaCdnInfraestrutura(_contexto));
        
        public LogAgoraInfraestrutura LogAgora => _logAgoraInfraestrutura ?? (_logAgoraInfraestrutura = new LogAgoraInfraestrutura(_contexto));

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _contexto.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task IniciarTransacao()
        {
            _contexto.Database.BeginTransaction();
        }
    }
}
