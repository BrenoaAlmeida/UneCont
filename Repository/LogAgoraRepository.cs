using System;
using Model;

namespace Repository
{
    public class LogAgoraRepository
    {
        private UneContext _context;
        public LogAgoraRepository(UneContext context)
        {
            _context = context;
        }
    }
}
