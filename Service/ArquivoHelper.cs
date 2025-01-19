using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Model;

namespace Service
{
    public class ArquivoHelper
    {

        private const string PastaParaLogs = "logs";        

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

        public string TransformarLogMinhaCdnParaAgora(string url, bool retornarPath, string nomeDoArquivo = "")
        {
            // transformar no formato Agora
            var logNoFormatoMinhaCdn = new HttpClient().GetStringAsync(url).Result; // obtendo arquivo a partir da url fornecida
            var linhasDeLog = logNoFormatoMinhaCdn.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return ExtrairLinhasDoLogMinhaCdnParaFormatoLogAgora(retornarPath, ref nomeDoArquivo, linhasDeLog);
        }

        public string ExtrairLinhasDoLogMinhaCdnParaFormatoLogAgora(bool retornarPath, ref string nomeDoArquivo, string[] linhasDeLog)
        {
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
            if (string.IsNullOrEmpty(nomeDoArquivo))
                nomeDoArquivo = $"agora_{DateTime.Now.ToString("dd-MM-HH_HH-mm-ss")}.txt";

            return CriarArquivo(nomeDoArquivo, textoDoLogNoFormatoMinhaCdn);
        }

        public string TransformarLogMinhaCdnParaAgora(List<LogMinhaCdn> logsMinhaCdn, bool retornarPath)
        {
            var dataAtual = DateTime.Now;

            if (logsMinhaCdn.Count == 0)
                return null;

            // transformar no formato Agora

            var textoDoLogNoFormatoMinhaCdn = "#Version: 1.0 \r\n"
                + $"#Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} \r\n"
                + "#Fields: provider http-method status-code uri-path time-taken response-size cache-status \r\n";

            foreach (var logMinhaCdn in logsMinhaCdn)
            {
                var subPropriedades = logMinhaCdn.Request.Replace("\"", "").Split(" ");
                var httpMethod = subPropriedades[0];
                var uriPath = subPropriedades[1];
                var responseSize = logMinhaCdn.ResponseSize;
                var statusCode = logMinhaCdn.StatusCode;
                var cacheStatus = logMinhaCdn.CacheStatus;
                var tempoConvertido = Decimal.Parse(logMinhaCdn.TimeTaken, System.Globalization.CultureInfo.InvariantCulture);
                var timeTaken = (int)Math.Round(tempoConvertido, MidpointRounding.AwayFromZero);

                textoDoLogNoFormatoMinhaCdn += $"MINHA CDN {httpMethod} {statusCode} {uriPath} {timeTaken} {responseSize} {cacheStatus} \r\n";
            }

            if (!retornarPath)
                return textoDoLogNoFormatoMinhaCdn;

            // gravar log em pasta no servidor e retornar o path do arquivo
            var nomeDoArquivo = $"agora_{DateTime.Now.ToString("dd-MM-HH_HH-mm-ss")}.txt";
            return CriarArquivo(nomeDoArquivo, textoDoLogNoFormatoMinhaCdn);
        }

        public void DeletarArquivo(string caminhoDoArquivo)
        {
            if (File.Exists(caminhoDoArquivo))
                File.Delete(caminhoDoArquivo);
        }
    }
}
