using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Model;
using Repository;
using Service.Enumeradores;

namespace Service
{
    public class LogService
    {
        UnitOfWork _unitOfWork;
        ArquivoService _arquivoService = new ArquivoService();
        public LogService(UnitOfWork unitOfWork)
        {
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

        public Log SalvarLog(string url)
        {
            HttpResponseMessage file;
            var dataAtual = DateTime.Now;
            var text = new HttpClient().GetStringAsync(url).Result;
            var linhas = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var log = new Log();
            foreach (var linha in linhas)
            {
                var logMinhaCdn = new LogMinhaCdn();
                var propriedades = linha.Split("|");
                logMinhaCdn.ResponseSize = propriedades[0].ToString();
                logMinhaCdn.StatusCode = propriedades[1].ToString();
                logMinhaCdn.CacheStatus = propriedades[2].ToString();
                logMinhaCdn.Request = propriedades[3].ToString();
                logMinhaCdn.TimeTaken = propriedades[4].ToString();
                log.LogMinhaCdn.Add(logMinhaCdn);
            }

            var logArquivo = new LogArquivo();
            logArquivo.NomeArquivo = "minha-cdn_" + dataAtual.ToString("dd-MM-HH HH mm ss") + ".txt";
            logArquivo.Arquivo =  _arquivoService.LerArquivoLogMinhaCdnDaWeb(url);

            logArquivo.TipoLog = UneContEnum.ETipoLog.MinhaCdn.ToString();
            logArquivo.DataHoraInsercao = dataAtual;

            _arquivoService.BaixarArquivo(logArquivo, url);
            log.LogArquivo.Add(logArquivo);
            log = _unitOfWork.Log.SalvarLog(log);
            return log;
        }

        public string TransformarLogMinhaCdnParaAgora(string url)
        {
            HttpResponseMessage file;
            var dataAtual = DateTime.Now;
            var caminhoDoDiretorio = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(caminhoDoDiretorio))
                Directory.CreateDirectory(caminhoDoDiretorio);

            var nomeDoArquivo = "agora_" + dataAtual.ToString("dd-MM-HH HH mm ss") + ".txt";

            var texto = new HttpClient().GetStringAsync(url).Result;
            caminhoDoDiretorio = Path.Combine(caminhoDoDiretorio, nomeDoArquivo);

            var linhas = texto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var writer = new StreamWriter(caminhoDoDiretorio);
            try
            {
                writer.WriteLine("Version: 1.0");
                writer.WriteLine($"Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                writer.WriteLine("#Fields: provider http-method status-code uri-path time-taken response-size cache-status");

                foreach (var linha in linhas)
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

        public string TransformarLogMinhaCdnParaAgora(int identificador)
        {
            var dataAtual = DateTime.Now;
            var logsMinhaCdn = _unitOfWork.LogMinhaCdn.ObterPorIdentificador(identificador);
            var caminhoDoArquivo = MapearModeloMinhaCdnParaModeloAgora(logsMinhaCdn);
            return caminhoDoArquivo;
        }
    }
}
