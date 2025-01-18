using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Model;

namespace Service
{
    public class ArquivoHelper
    {

        private const string PastaParaLogs = "logs";

        public async Task<MemoryStream> BaixarARquivosEZipar(Log log)
        {
            bool arquivosExistem = false;

            var caminhos = new List<string>();
            using (var memoryStream = new MemoryStream())
            {
                using (var arquivoZip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var logArquivo in log.LogArquivo)
                    {
                        var caminhoDoArquivo = Path.Combine(Directory.GetCurrentDirectory(), PastaParaLogs, logArquivo.NomeArquivo);
                        if (File.Exists(caminhoDoArquivo))
                        {
                            arquivosExistem = true;
                            var arquivo = arquivoZip.CreateEntry(logArquivo.NomeArquivo, CompressionLevel.Fastest);
                            using (var entryStream = arquivo.Open())
                            using (var fileStream = File.OpenRead(caminhoDoArquivo))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }

                    if (!arquivosExistem)
                        return null;

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream;
                }
            }
        }

        public async void BaixarArquivo(LogArquivo logArquivo, string urlDoArquiivo)
        {
            // Caminho da pasta logs
            var pastaNoServidor = Path.Combine(Directory.GetCurrentDirectory(), PastaParaLogs);

            // Garantir que a pasta existe
            if (!Directory.Exists(pastaNoServidor))
            {
                Directory.CreateDirectory(pastaNoServidor);
            }

            // Nome do arquivo (pode ser extraído da URL ou definido manualmente)
            var nomeDoArquivo = logArquivo.NomeArquivo;
            var caminhoDoArquivo = Path.Combine(pastaNoServidor, nomeDoArquivo);

            // Fazer o download do arquivo
            using (var httpClient = new HttpClient())
            {
                var conteudoDoArquivo = await httpClient.GetStringAsync(urlDoArquiivo);

                // Salvar o arquivo na pasta uploads
                await File.WriteAllTextAsync(caminhoDoArquivo, conteudoDoArquivo);
            }
        }

        public string CriarArquivo(string nomeDoArquivo, string conteudoDoArquivo)
        {
            var pastaNoServidor = CriarPastaDeLogsSeNecessario();

            var caminhoDoArquivo = Path.Combine(pastaNoServidor, nomeDoArquivo);

            using (var writer = new StreamWriter(caminhoDoArquivo))
            {
                try
                {
                    writer.WriteLine(conteudoDoArquivo);
                    return caminhoDoArquivo;
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        public static string CriarPastaDeLogsSeNecessario()
        {
            var pastaNoServidor = Path.Combine(Directory.GetCurrentDirectory(), PastaParaLogs);

            if (!Directory.Exists(pastaNoServidor))
                Directory.CreateDirectory(pastaNoServidor);

            return pastaNoServidor;
        }

        public string CriarArquivoAgoraAPartirdoArquivoMinhaCdn(List<LogMinhaCdn> logsMinhaCdn)
        {
            var dataAtualFormatada = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            var pastaNoServidor = ArquivoHelper.CriarPastaDeLogsSeNecessario();
            var nomeDoArquivo = $"agora_{dataAtualFormatada}.txt";
            pastaNoServidor = Path.Combine(pastaNoServidor, nomeDoArquivo);

            var writer = new StreamWriter(pastaNoServidor);
            try
            {
                writer.WriteLine("Version: 1.0");
                writer.WriteLine($"Date: {dataAtualFormatada}");
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

            return pastaNoServidor;

        }

        public Uri RetornarCaminhoDoArquivoNoServidor(string urlBase, string caminhoDoArquivo)
        {
            var nomeDoArquivo = Path.GetFileName(caminhoDoArquivo);
            var path = $"/{PastaParaLogs}/{nomeDoArquivo}";
            var urlCompleta = new Uri(new Uri(urlBase), path);
            return urlCompleta;
        }

        public string RetornarLogsEmTexto(List<LogArquivo> logsArquivo)
        {
            string arquivosDeLogEmTexto = string.Empty;

            foreach (var log in logsArquivo)
            {
                foreach (var logArquivo in logsArquivo)
                {
                    HttpClient httpClient = new HttpClient();
                    arquivosDeLogEmTexto += httpClient.GetStringAsync(logArquivo.CaminhoDoArquivo).Result;
                    arquivosDeLogEmTexto += "\r\n";
                }
            }

            return arquivosDeLogEmTexto;
        }
    }
}
