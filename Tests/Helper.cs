using Microsoft.EntityFrameworkCore;
using Model;

namespace Tests
{
    public class Helper
    {
        public static UneContexto ObterContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<UneContexto>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
            return new UneContexto(options);
        }
    }
}
