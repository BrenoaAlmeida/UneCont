using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Model;
using Service.Enumeradores;

namespace Service
{
    public class LogService
    {
        public Log MapearArquivoDeTextoParaMinhaCdn(string url)
        {
            HttpResponseMessage file;
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

            var logArquivo = MapearLogArquivo(UneContEnum.ETipoLog.MinhaCdn, dataAtual: DateTime.Now, url);
            BaixarArquivo(logArquivo, url);
            log.LogArquivo.Add(logArquivo);
            return log;
        }

        public async void BaixarArquivo(LogArquivo logArquivo, string fileUrl)
        {
            // Caminho da pasta uploads
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Garantir que a pasta existe
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Nome do arquivo (pode ser extraído da URL ou definido manualmente)
            var fileName = logArquivo.NomeArquivo;
            var filePath = Path.Combine(uploadsPath, fileName);

            // Fazer o download do arquivo
            using (var httpClient = new HttpClient())
            {
                var fileContent = await httpClient.GetStringAsync(fileUrl);

                // Salvar o arquivo na pasta uploads
                await File.WriteAllTextAsync(filePath, fileContent);
            }
        }

        public LogArquivo MapearLogArquivo(UneContEnum.ETipoLog tipoLog, DateTime dataAtual, string url = "")
        {
            var logArquivo = new LogArquivo();

            if (tipoLog.ToString() == UneContEnum.ETipoLog.MinhaCdn.ToString())
                logArquivo.NomeArquivo = "minha-cdn_" + dataAtual.ToString("dd-MM-HH HH mm ss") + ".txt";
            else
                logArquivo.NomeArquivo = "agora_" + dataAtual.ToString("dd-MM-HH HH mm ss") + ".txt";

            if (!string.IsNullOrEmpty(url))
                logArquivo.Arquivo = GerarArquivoLendoDaWeb(url);

            logArquivo.TipoLog = tipoLog.ToString();
            logArquivo.DataHoraInsercao = dataAtual;

            return logArquivo;

        }

        public Log MapearModeloMinhaCdnParaModeloAgora(Log log)
        {
            var provider = "MINHA CDN";
            foreach (var logMInhaCdn in log.LogMinhaCdn)
            {
                var logAgora = new LogAgora();
                logAgora.Provider = provider;
                logAgora.StatusCode = logMInhaCdn.StatusCode;
                var tempoConvertido = Decimal.Parse(logMInhaCdn.TimeTaken, System.Globalization.CultureInfo.InvariantCulture);
                logAgora.TimeTaken = (int)Math.Round(tempoConvertido, MidpointRounding.AwayFromZero);
                logAgora.ResponseSize = logMInhaCdn.ResponseSize;
                logAgora.CacheStatus = logMInhaCdn.CacheStatus;

                var propriedades = logMInhaCdn.Request.Split(" ");

                logAgora.HttpMethod = propriedades[0].Replace("\"", "");
                logAgora.UriPath = propriedades[1];

                log.LogAgora.Add(logAgora);
            }
            return log;
        }

        private byte[] GerarArquivoLendoDaWeb(string url)
        {
            HttpResponseMessage file;
            var client = new HttpClient();
            file = client.GetAsync(url).Result;
            return file.Content.ReadAsByteArrayAsync().Result;
        }

        public void EscreverArquivo(List<LogAgora> logAgora, string nomeArquivo, DateTime dataAtual)
        {
            StreamWriter logWriter = null;
            byte[] arquivoEmBytes;
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(path);
                var caminhoDoArquivo = Path.Combine(path, nomeArquivo);
                logWriter = new StreamWriter(caminhoDoArquivo);
                logWriter.WriteLine("#Version: 1.0");
                logWriter.WriteLine("#Date " + dataAtual.ToString("dd/MM/yyyy HH:mm:ss"));
                logWriter.WriteLine("#Fields: provider http-method status-code uri-path time-taken response-size cache-status");

                foreach (var log in logAgora)
                {
                    logWriter.WriteLine(log.Provider + " " + log.HttpMethod + " " + log.StatusCode + " " + " " + log.UriPath + " " + log.TimeTaken + " " + log.ResponseSize + " " + log.CacheStatus);
                }
            }
            finally
            {
                if (logWriter != null)
                {
                    logWriter.Close();
                }
            }
        }

        public void DeletarArquivoTemporario(string nomeArquivo)
        {
            if (File.Exists(nomeArquivo))
                File.Delete(nomeArquivo);
        }

        public Log TransformarLogMinhaCdnParaAgora(string url)
        {
            var dataAtual = DateTime.Now;
            var log = MapearArquivoDeTextoParaMinhaCdn(url);
            log = MapearModeloMinhaCdnParaModeloAgora(log);
            var logArquivo = MapearLogArquivo(UneContEnum.ETipoLog.Agora, dataAtual, url);
            EscreverArquivo(log.LogAgora, logArquivo.NomeArquivo, dataAtual);
            log.LogArquivo.Add(logArquivo);
            return log;
        }

        public Log TransformarLogMinhaCdnParaAgora(Log log)
        {
            var Unecontext = new UneContext();
            var dataAtual = DateTime.Now;
            log = MapearModeloMinhaCdnParaModeloAgora(log);
            var logArquivo = MapearLogArquivo(UneContEnum.ETipoLog.Agora, dataAtual);
            EscreverArquivo(log.LogAgora, logArquivo.NomeArquivo, dataAtual);
            log.LogArquivo.Add(logArquivo);
            return log;
        }

        public byte[] TransformarLogSalvoEmArquivoLogAgora(Log log)
        {
            var dataAtual = DateTime.Now;
            string nomeArquivo = "arquivo.txt";

            log = MapearModeloMinhaCdnParaModeloAgora(log);
            EscreverArquivo(log.LogAgora, nomeArquivo, dataAtual);

            var arquivoLogAgora = File.ReadAllBytes(nomeArquivo);
            return arquivoLogAgora;
        }

        public async Task<MemoryStream> BaixarARquivosEZIpar(Log log)
        {
            var caminhos = new List<string>();
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var logArquivo in log.LogArquivo)
                    {
                        var cam = Path.Combine(Directory.GetCurrentDirectory(), "uploads", logArquivo.NomeArquivo);
                        if (File.Exists(cam))
                        {
                            var file = zipArchive.CreateEntry(logArquivo.NomeArquivo, CompressionLevel.Fastest);
                            using (var entryStream = file.Open())
                            using (var fileStream = File.OpenRead(cam))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream;
                }
            }
            //var files = new List<object>();            


            //// Nome do arquivo ZIP que será retornado
            //var zipFileName = "arquivos.zip";

            //// Criar um stream de memória para o ZIP
            //using (var memoryStream = new MemoryStream())
            //{
            //    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            //    {
            //        foreach (var caminho in caminhos)
            //        {

            //        }
            //        // Adicionar o primeiro arquivo ao ZIP
            //        if (System.IO.File.Exists(file1Path))
            //        {
            //            var file1 = zipArchive.CreateEntry("arquivo1.txt", CompressionLevel.Fastest);
            //            using (var entryStream = file1.Open())
            //            using (var fileStream = System.IO.File.OpenRead(file1Path))
            //            {
            //                await fileStream.CopyToAsync(entryStream);
            //            }
            //        }                    
            //    }

            //    // Retornar o ZIP como resposta

        }
    }
}
