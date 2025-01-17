using Microsoft.EntityFrameworkCore;
using Model;

namespace Tests
{
    public class Helper
    {
        public static UneContext ObterContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<UneContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
            return new UneContext(options);
        }
    }
}
