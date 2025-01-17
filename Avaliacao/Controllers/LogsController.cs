using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;
using Service;
using Service.Enumeradores;

namespace Avaliacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Obtem um arquivo no formato "Minha CDN" e o tranforma no formato "Agora", podendo passar como  entrar uma URL contendo o arquivo TXT ou um  identificador
        /// para um arquivo que ja foi salvo no banco de dados, como POST pode salvar o arquivo no servidor e retornar o path dele  
        /// </summary>
        /// <param name="somepara">Required parameter: Example: </param>
        /// <return>Returns comment</return>
        /// <response code="200">Ok</response>

        public LogsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("TransformarArquivo")]
        public ActionResult<IEnumerable<string>> TransformarArquivo(string url, bool retornarPath = false,
            //string url = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt",            
            int id = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(url) && id == 0 || !string.IsNullOrEmpty(url) && id != 0)                
                    return BadRequest("É necessario informar a url ou o Identificador");                
                

                var logService = new LogService();
                var log = new Log();
                if (!retornarPath)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        log = logService.TransformarLogMinhaCdnParaAgora(url);
                        log = _unitOfWork.Log.SalvarLog(log);
                    }
                    else
                    {
                        log = _unitOfWork.Log.ObterLogPorIdentificador(id);
                        log = logService.TransformarLogMinhaCdnParaAgora(log);
                        log = _unitOfWork.Log.AtualizarLog(log);
                    }                    

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var path = $"/uploads/{log.LogArquivo.Where(l => l.TipoLog == UneContEnum.ETipoLog.Agora.ToString()).First().NomeArquivo}";
                    var fullUrl = new Uri(new Uri(baseUrl), path);

                    return Ok(new { path = fullUrl });
                }
                else
                {
                    log = _unitOfWork.Log.ObterLogPorIdentificador(id);                    
                    var arquivo = logService.TransformarLogSalvoEmArquivoLogAgora(log);

                    var arquivoEmTexto = Encoding.UTF8.GetString(arquivo);
                    return Content(arquivoEmTexto, "text/plain");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }

        [HttpGet]
        [Route("BuscarLogsSalvos")]
        public ActionResult<string> BuscarLogsSalvos()
        {
            var logs = _unitOfWork.Log.ObterLogsMinhaCdn();                        
            return Ok(logs);
        }

        //Retorna todos os logs do banco no formato original "Minha CDN" e os logs no formato "Agora"
        [HttpGet]
        [Route("BuscarLogTransformadosNoBackend")]
        public ActionResult<string> BuscarLogTransformadosNoBackend(int id)
        {
            var logService = new LogService();
            var logs = _unitOfWork.Log.ObterLogPorIdentificador(id);
            var memo = logService.BaixarARquivosEZIpar(logs).Result;
            return File(memo.ToArray(), "application/zip", "modelos.zip");
        }

        [HttpGet]
        [Route("BuscaLogSalvosPorIdentificador")]
        public ActionResult<string> BuscaLogSalvosPorIdentificador(int id)
        {
            var logs = _unitOfWork.Log.ObterLogPorIdentificador(id);            
            return Ok(logs);
        }

        [HttpGet]
        [Route("BuscarLogsTransformadosPorIdentificador")]
        public ActionResult<string> BuscarLogsTransformadosPorIdentificador(int id)
        {
            var logs = _unitOfWork.Log.ObterLogsAgoraPorIdentificador(id);
            return Ok(logs);
        }

        //Salvar o arquivo no servidor
        [HttpPost]
        [Route("SalvarLogs")]
        public ActionResult<string> SalvarLogs(string url = "https://s3.amazonaws.com/uux-itaas-static/minha-cdn-logs/input-01.txt")
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("É necessario preencher o campo Url");

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var logService = new LogService();
                var log = logService.MapearArquivoDeTextoParaMinhaCdn(url);
                log = _unitOfWork.Log.SalvarLog(log);
                return Ok(new { mensagem = "Log foi salvo com sucesso!", log.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("ObterLog/{id}")]
        public ActionResult<string> ObterLog(string id = "1")
        {
            var log = _unitOfWork.Log.ObterLogPorIdentificador(Convert.ToInt32(id));
            var arquivo = log.LogArquivo.Where(l => l.TipoLog == UneContEnum.ETipoLog.Agora.ToString()).First().Arquivo;
            //Isso aqui baixo o arquivo
            //return File(arquivo, "text/plain", "arquivo.txt");

            var arquivoEmTexto = Encoding.UTF8.GetString(arquivo);
            return Content(arquivoEmTexto, "text/plain");
        }
    }
}
