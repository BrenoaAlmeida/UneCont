using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Infraestrutura;
using Service;

namespace Tests
{
    [TestClass]
    public class LogTest
    {
        //[TestMethod]
        //public void DeveMapearMinhaCdnParaAgora()
        //{
        //    //ARRANGE            
        //    var mockUneContext = new Mock<UneContexto>();
        //    var unitOfWork = new UnitOfWork(mockUneContext.Object);
        //    var service = new LogService(unitOfWork, mockUneContext);
        //    mockUneContext.Setup(x => x.Log.Find(It.IsAny<int>())).Returns(new Log()
        //    {
        //        Id = 1,
        //        LogMinhaCdn = new List<LogMinhaCdn>()
        //        {
        //            new LogMinhaCdn()
        //            {
        //                Id = 1,
        //                LogId =1,
        //                ResponseSize = "312",
        //                StatusCode = "200",
        //                CacheStatus = "HIT",
        //                Request = "\"GET /robots.txt HTTP/1.1\"",
        //                TimeTaken = "100.2",
        //            }
        //        }
        //    });
        //    //ACT            
        //}

        [TestMethod]
        public void DeveBuscarLogsSalvosNoBancoPorIdentificador()
        {
            //ARRANGE
            var context = Helper.ObterContextoEmMemoria();
            var logId = 1;
            context.Log.Add(new Log()
            {
                Id = logId,
                Url = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt",
                LogArquivo = new List<LogArquivo>()
                {
                    new LogArquivo(){
                        NomeArquivo = "teste1.txt",
                    }
                },
                LogMinhaCdn = new List<LogMinhaCdn>(){
                    new LogMinhaCdn (){
                        LogId = logId,
                        Id = 1,
                        ResponseSize = "312",
                        StatusCode = "200",
                        Request = "\"GET /robots.txt HTTP/1.1\"",
                        TimeTaken = "100.2",
                        CacheStatus = "HIT"
                    },
                    new LogMinhaCdn (){
                        LogId = logId,
                        Id = 2,
                        ResponseSize = "101",
                        StatusCode = "200",
                        Request = "\"POST /myImages HTTP/1.1\"",
                        TimeTaken = "319.4",
                        CacheStatus = "MISS"
                    }
                }
            });

            //ACT
            context.SaveChanges();
            var logs = context.Log.Where(r => r.Id == logId);

            //ASSERT
            Assert.IsTrue(logs.Any());
        }

        [TestMethod]
        public void DeveTransformarArquivoNoFormatoMinhaCdnParaAgora()
        {
            //ARRANGE            
            //var uneContext = Helper.ObterContextoEmMemoria();
            //var unitOfWork = new UnitOfWork(uneContext);
            //var url = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt";
            //var logService = new LogService();
            //var log = logService.MapearArquivoDeTextoParaMinhaCdn(url);
            //log = logService.MapearModeloMinhaCdnParaModeloAgora(log);

        }
    }
}
