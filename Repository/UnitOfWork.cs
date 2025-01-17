using System;
using Model;

namespace Repository
{
    public class UnitOfWork : IDisposable
    {
        private UneContext _context;

        public UnitOfWork(UneContext context)
        {
            _context = context;
        }

        private LogRepository logRepository;
        private LogMinhaCdnRepository logMinhaCdnRepository;
        private LogAgoraRepository logAgoraRepository;
        
        public LogRepository Log => logRepository == null ? new LogRepository(_context) : logRepository;
        
        public LogMinhaCdnRepository LogMinhaCdn => logMinhaCdnRepository == null ? new LogMinhaCdnRepository(_context) : logMinhaCdnRepository;
        
        public LogAgoraRepository LogAgora => logAgoraRepository == null ? new LogAgoraRepository(_context) : logAgoraRepository;

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
