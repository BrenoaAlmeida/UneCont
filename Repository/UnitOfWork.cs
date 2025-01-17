using System;
using Model;

namespace Repository
{
    public class UnitOfWork : IDisposable
    {
        private UneContexto _contexto;

        public UnitOfWork(UneContexto contexto)
        {
            _contexto = contexto;
        }

        private LogRepository _logRepository;
        private LogMinhaCdnRepository _logMinhaCdnRepository;
        private LogAgoraRepository _logAgoraRepository;

        public LogRepository Log => _logRepository ?? (_logRepository = new LogRepository(_contexto));        

        public LogMinhaCdnRepository LogMinhaCdn => _logMinhaCdnRepository ?? (_logMinhaCdnRepository  = new LogMinhaCdnRepository(_contexto));
        
        public LogAgoraRepository LogAgora => _logAgoraRepository ?? (_logAgoraRepository = new LogAgoraRepository(_contexto));

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
    }
}
