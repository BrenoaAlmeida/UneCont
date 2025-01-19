using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Infraestrutura;
using Service;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class LogTest
    {

        [TestMethod]
        public void DeveTransformarLogMinhaCdnEmLogAgora()
        {
            //ARRANGE
            var logMInhaCdn = new string[4];
            logMInhaCdn[0] = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            logMInhaCdn[1] = "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4";
            logMInhaCdn[2] = "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9";
            logMInhaCdn[3] = "312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1";

            //ACT
            var arquivoHelper = new ArquivoHelper();
            string nomeDoArquivo = "agora";
            var result = arquivoHelper.ExtrairLinhasDoLogMinhaCdnParaFormatoLogAgora(retornarPath: false, ref nomeDoArquivo, logMInhaCdn);
            var linhasDeLogAgora = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var primeraLinhaLogAgora = linhasDeLogAgora[3].ToString().Split(" ");
            //ASSERT            
            if (primeraLinhaLogAgora[2] != "GET") //http-method
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[4] != "/robots.txt") //uri-path            
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[6] != "312") //response-size
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[3] != "200") //status-code
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[5] != "100") //time-taken
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[7] != "HIT") //cache-status
                Assert.IsTrue(false);
        }

        //deve ler um  arquivo minha cdn e o transformar em um arquivo e retornar a url do  arquivo transformado com o conteudo certo
        [TestMethod]
        public void DeveTransformarECriarUmArquivoERetornarNo()
        {
            //ARRANGE
            var logMInhaCdn = new string[4];
            logMInhaCdn[0] = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
            logMInhaCdn[1] = "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4";
            logMInhaCdn[2] = "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9";
            logMInhaCdn[3] = "312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1";

            //ACT
            var arquivoHelper = new ArquivoHelper();
            string nomeDoArquivo = "agora";
            var result = arquivoHelper.ExtrairLinhasDoLogMinhaCdnParaFormatoLogAgora(retornarPath: true, ref nomeDoArquivo, logMInhaCdn);

            if (!File.Exists(result))
                Assert.IsTrue(false);

            var logAgora = File.ReadAllText(result);
            var linhasDeLogAgora = logAgora.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var primeraLinhaLogAgora = linhasDeLogAgora[3].ToString().Split(" ");
            //ASSERT            
            if (primeraLinhaLogAgora[2] != "GET") //http-method
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[4] != "/robots.txt") //uri-path            
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[6] != "312") //response-size
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[3] != "200") //status-code
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[5] != "100") //time-taken
                Assert.IsTrue(false);
            if (primeraLinhaLogAgora[7] != "HIT") //cache-status
                Assert.IsTrue(false);

            arquivoHelper.DeletarArquivo(result);
        }
    }
}
