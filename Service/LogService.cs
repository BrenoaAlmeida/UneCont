using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Model;
using Infraestrutura;
using Service.Enumeradores;
using System.Threading.Tasks;

namespace Service
{
    public class LogService
    {
        UnitOfWork _unitOfWork;
        UneContexto _contexto;
        ArquivoService _arquivoService = new ArquivoService();
        public LogService(UnitOfWork unitOfWork, UneContexto contexto)
        {
            _contexto = contexto;
            _unitOfWork = unitOfWork;
        }

        public string MapearModeloMinhaCdnParaModeloAgora(List<LogMinhaCdn> logsMinhaCdn)
        {
            HttpResponseMessage file;
            var provider = "MINHA CDN";
            var dataAtual = DateTime.Now;
            var caminhoDoDiretorio = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(caminhoDoDiretorio))
                Directory.CreateDirectory(caminhoDoDiretorio);

            var nomeDoArquivo = "agora_" + dataAtual.ToString("dd-MM-HH HH mm ss") + ".txt";
            caminhoDoDiretorio = Path.Combine(caminhoDoDiretorio, nomeDoArquivo);

            var writer = new StreamWriter(caminhoDoDiretorio);
            try
            {
                writer.WriteLine("Version: 1.0");
                writer.WriteLine($"Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                writer.WriteLine("#Fields: provider http-method status-code uri-path time-taken response-size cache-status");

                foreach (var logMinhaCdn in logsMinhaCdn)
                {
                    var propriedades = logMinhaCdn.Request.Split(" ");

                    var httpMethod = propriedades[0].Replace("\"", "");
                    var statusCode = logMinhaCdn.StatusCode;
                    var uriPath = propriedades[1];
                    var tempoConvertido = Decimal.Parse(logMinhaCdn.TimeTaken, System.Globalization.CultureInfo.InvariantCulture);
                    var timeTaken = (int)Math.Round(tempoConvertido, MidpointRounding.AwayFromZero);
                    var responseSize = logMinhaCdn.ResponseSize;
                    var cacheStatus = logMinhaCdn.CacheStatus;
                    writer.WriteLine($"MINHA CDN {httpMethod} {statusCode} {uriPath} {timeTaken} {responseSize} {cacheStatus}");
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return caminhoDoDiretorio;
        }

        public async Task<Log> SalvarLog(string url, string urlBase)
        {
            HttpResponseMessage file;
            var dataAtual = DateTime.Now;
            var text = new HttpClient().GetStringAsync(url).Result;
            var linhas = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var log = new Log();
            log.DataDeInsercao = dataAtual;
            log.Url = url;
            foreach (var linha in linhas)
            {
                var logMinhaCdn = new LogMinhaCdn();
                var propriedades = linha.Split("|");
                logMinhaCdn.ResponseSize = propriedades[0].ToString();
                logMinhaCdn.StatusCode = propriedades[1].ToString();
                logMinhaCdn.CacheStatus = propriedades[2].ToString();
                logMinhaCdn.Request = propriedades[3].ToString();
                logMinhaCdn.TimeTaken = propriedades[4].ToString();

                var logAgora = new LogAgora();
                logAgora.ResponseSize = logMinhaCdn.ResponseSize;
                logAgora.StatusCode = logMinhaCdn.StatusCode;
                logAgora.CacheStatus = logMinhaCdn.CacheStatus;

                var subPropriedades = logMinhaCdn.Request.Replace("\"", "").Split(" ");
                logAgora.HttpMethod = subPropriedades[0];
                logAgora.UriPath = subPropriedades[1];

                var tempoConvertido = Decimal.Parse(logMinhaCdn.TimeTaken, System.Globalization.CultureInfo.InvariantCulture);
                logAgora.TimeTaken = (int)Math.Round(tempoConvertido, MidpointRounding.AwayFromZero);
                log.LogMinhaCdn.Add(logMinhaCdn);
                log.LogAgora.Add(logAgora);
            }

            var logArquivoMinhaCdn = new LogArquivo();
            logArquivoMinhaCdn.NomeArquivo = $"minha-cdn_{dataAtual.ToString("dd-MM-HH_HH-mm-ss")}.txt";
            logArquivoMinhaCdn.TipoLog = UneContEnum.ETipoLog.MinhaCdn.ToString();
            logArquivoMinhaCdn.CaminhoDoArquivo = $"{urlBase}/uploads/{logArquivoMinhaCdn.NomeArquivo}";
            log.LogArquivo.Add(logArquivoMinhaCdn);

            var logArquivoAgora = new LogArquivo();
            logArquivoAgora.NomeArquivo = $"agora_{dataAtual.ToString("dd-MM-HH_HH-mm-ss")}.txt";
            logArquivoAgora.TipoLog = UneContEnum.ETipoLog.Agora.ToString();
            logArquivoAgora.CaminhoDoArquivo = $"{urlBase}/uploads/{logArquivoAgora.NomeArquivo}";
            log.LogArquivo.Add(logArquivoAgora);

            _arquivoService.BaixarArquivo(logArquivoMinhaCdn, url);
            TransformarLogMinhaCdnParaAgora(url, retornarPath: true, logArquivoAgora.NomeArquivo);

            var transaction = await _contexto.Database.BeginTransactionAsync();
            try
            {
                log = _unitOfWork.Log.SalvarLog(log);                
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                var caminhoDaPastaDeLogs = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                var caminhoLogMinhaCdn = Path.Combine(caminhoDaPastaDeLogs, logArquivoMinhaCdn.NomeArquivo);
                
                if (File.Exists(caminhoLogMinhaCdn))
                    File.Delete(caminhoLogMinhaCdn);

                var caminhoLogAgora = Path.Combine(caminhoDaPastaDeLogs, logArquivoAgora.NomeArquivo);

                if (File.Exists(caminhoLogAgora))
                    File.Delete(caminhoLogAgora);

                throw;
            }

            return log;
        }

        public string TransformarLogMinhaCdnParaAgora(string url, bool retornarPath, string nomeDoArquivo = "")
        {

            // transformar no formato Agora
            var logNoFormatoMinhaCdn = new HttpClient().GetStringAsync(url).Result; // obtendo arquivo a partir da url fornecida
            var linhasDeLog = logNoFormatoMinhaCdn.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var textoDoLogNoFormatoMinhaCdn = "#Version: 1.0 \r\n"
                + $"#Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} \r\n"
                + "#Fields: provider http-method status-code uri-path time-taken response-size cache-status \r\n";

            foreach (var linha in linhasDeLog)
            {
                var propriedades = linha.Split("|");
                var subPropriedades = propriedades[3];
                subPropriedades = subPropriedades.ToString();
                subPropriedades = subPropriedades.Replace("\"", "");
                var request = subPropriedades.Split(" ");
                var httpMethod = request[0];
                var uriPath = request[1];
                var responseSize = propriedades[0].ToString();
                var statusCode = propriedades[1].ToString();
                var cacheStatus = propriedades[2].ToString();
                var tempoConvertido = Decimal.Parse(propriedades[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                var timeTaken = (int)Math.Round(tempoConvertido, MidpointRounding.AwayFromZero);
                textoDoLogNoFormatoMinhaCdn += $"MINHA CDN {httpMethod} {statusCode} {uriPath} {timeTaken} {responseSize} {cacheStatus} \r\n";
            }

            if (!retornarPath)
                return textoDoLogNoFormatoMinhaCdn;

            // gravar log em pasta no servidor e retornar o path do arquivo

            HttpResponseMessage response;
            var dataAtual = DateTime.Now;
            var caminhoDoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(caminhoDoArquivo))
                Directory.CreateDirectory(caminhoDoArquivo);

            if (string.IsNullOrEmpty(nomeDoArquivo))
                nomeDoArquivo = $"agora_{dataAtual.ToString("dd-MM-HH_HH-mm-ss")}.txt";

            caminhoDoArquivo = Path.Combine(caminhoDoArquivo, nomeDoArquivo);

            var writer = new StreamWriter(caminhoDoArquivo);
            try
            {
                writer.WriteLine(textoDoLogNoFormatoMinhaCdn);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return caminhoDoArquivo;
        }

        public string TransformarLogMinhaCdnParaAgora(int identificador)
        {
            var dataAtual = DateTime.Now;
            var logsMinhaCdn = _unitOfWork.LogMinhaCdn.ObterPorIdentificador(identificador);
            var caminhoDoArquivo = MapearModeloMinhaCdnParaModeloAgora(logsMinhaCdn);
            return caminhoDoArquivo;
        }
    }
}
