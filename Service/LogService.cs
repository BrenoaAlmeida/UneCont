using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infraestrutura;
using Model;
using Service.Enumeradores;

namespace Service
{
    public class LogService
    {
        private UnitOfWork _unitOfWork;
        UneContexto _contexto;
        ArquivoHelper _arquivoHelper = new ArquivoHelper();

        public LogService(UnitOfWork unitOfWork, UneContexto contexto)
        {
            _contexto = contexto;
            _unitOfWork = unitOfWork;
        }

        public Log ObterLogPorIdentificador(int identificador)
        {
            return _unitOfWork.Log.ObterLogPorIdentificador(identificador);
        }

        public IList<Log> ObterLogs()
        {
            return _unitOfWork.Log.ObterLogs();                        
        }

        public IList<LogArquivo> ObterLogsArquivo()
        {
            return _unitOfWork.LogArquivo.ObterLogsArquivo();
        }

        public IList<LogAgora> ObterLogsAgoraPorIdentificador(int identificador)
        {
            var logsAgora = _unitOfWork.LogAgora.ObterLogsAgoraPorIdentificador(identificador);

            if (logsAgora.Count == 0)
                return null;

            return logsAgora;
        }

        public async Task<Log> SalvarLog(string url, string urlBase)
        {
            HttpResponseMessage file;
            var dataAtual = DateTime.Now;
            var logNoFormatoMinhaCdn = new HttpClient().GetStringAsync(url).Result;
            var linhasLog = logNoFormatoMinhaCdn.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var log = new Log();
            log.DataDeInsercao = dataAtual;
            log.Url = url;
            
            foreach (var linha in linhasLog)
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
            logArquivoMinhaCdn.CaminhoDoArquivo = $"{urlBase}/logs/{logArquivoMinhaCdn.NomeArquivo}";
            log.LogArquivo.Add(logArquivoMinhaCdn);

            var logArquivoAgora = new LogArquivo();
            logArquivoAgora.NomeArquivo = $"agora_{dataAtual.ToString("dd-MM-HH_HH-mm-ss")}.txt";
            logArquivoAgora.TipoLog = UneContEnum.ETipoLog.Agora.ToString();
            logArquivoAgora.CaminhoDoArquivo = $"{urlBase}/logs/{logArquivoAgora.NomeArquivo}";
            log.LogArquivo.Add(logArquivoAgora);

            _arquivoHelper.BaixarArquivo(logArquivoMinhaCdn, url);
            TransformarLogMinhaCdnParaAgora(url, retornarPath: true, logArquivoAgora.NomeArquivo);

            var transaction = await _contexto.Database.BeginTransactionAsync();
            
            try
            {
                log = _unitOfWork.Log.SalvarLog(log);
                _unitOfWork.Salvar();
                transaction.Commit();
            }
            
            catch (Exception ex)
            {
                transaction.Rollback();
                var caminhoDaPastaDeLogs = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                
                var caminhoLogMinhaCdn = Path.Combine(caminhoDaPastaDeLogs, logArquivoMinhaCdn.NomeArquivo);
                _arquivoHelper.DeletarArquivo(caminhoLogMinhaCdn);                

                var caminhoLogAgora = Path.Combine(caminhoDaPastaDeLogs, logArquivoAgora.NomeArquivo);
                _arquivoHelper.DeletarArquivo(caminhoLogAgora);                
                throw;
            }

            return log;
        }

        public string TransformarLogMinhaCdnParaAgora(string url, bool retornarPath, string nomeDoArquivo = "")
        {            
            return _arquivoHelper.TransformarLogMinhaCdnParaAgora(url, retornarPath, nomeDoArquivo);
        }

        public string TransformarLogMinhaCdnParaAgora(int identificador, bool retornarPath)
        {
            var dataAtual = DateTime.Now;
            var logsMinhaCdn = _unitOfWork.LogMinhaCdn.ObterPorIdentificador(identificador);

            return _arquivoHelper.TransformarLogMinhaCdnParaAgora(logsMinhaCdn, retornarPath);
        }
    }
}
