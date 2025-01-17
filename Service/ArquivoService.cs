using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Model;

namespace Service
{
    public class ArquivoService
    {
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
                        var caminhoDoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "uploads", logArquivo.NomeArquivo);
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

        public byte[] LerArquivoLogMinhaCdnDaWeb(string url)
        {
            HttpResponseMessage file;
            var client = new HttpClient();
            file = client.GetAsync(url).Result;
            return file.Content.ReadAsByteArrayAsync().Result;
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
    }
}
