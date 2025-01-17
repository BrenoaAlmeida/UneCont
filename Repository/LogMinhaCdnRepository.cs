using System;
using Model;

namespace Repository
{
    public class LogMinhaCdnRepository
    {
        private UneContext _context;
        public LogMinhaCdnRepository(UneContext context)
        {
            _context = context;
        }
    }
}
