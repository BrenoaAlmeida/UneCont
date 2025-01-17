using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;
using Service;

namespace Api.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        ArquivoService _arquivoService;
        LogService _logService;

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
            _arquivoService = new ArquivoService();
            _logService = new LogService(unitOfWork);
        }

        [HttpGet]
        [Route("transformar-formato")]
        public ActionResult<IEnumerable<string>> TransformarFormato(string url, bool retornarPath)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("É necessario informar a url");

                /*
                    Transformação de um formato de Log para outro;
                    ○ Formato de saída pode variar (O usuário vai selecionar na requisição):
                    resultado: 
                        path do arquivo salvo no servidor OU log transformado
                 */

                // acoes:
                // - transformar o log
                // - retornar o path ou o proprio log transformado

                var caminhoDoArquivo = _logService.TransformarLogMinhaCdnParaAgora(url);

                if (retornarPath)
                {
                    var nomeDoArquivo = Path.GetFileName(caminhoDoArquivo);
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var path = $"/uploads/{nomeDoArquivo}";
                    var fullUrl = new Uri(new Uri(baseUrl), path);
                    return Ok(new { path = fullUrl });
                }
                else
                {
                    var arquivoEmTexto = System.IO.File.ReadAllTextAsync(caminhoDoArquivo).Result;
                    return Content(arquivoEmTexto, "text/plain");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }

        [HttpPost]
        [Route("transformar-formato/{identificador}")]
        public ActionResult<IEnumerable<string>> TransformarArquivo(int identificador, bool retornarCaminho)
        {
            try
            {
                if (identificador == 0)
                    return BadRequest("É necessario informar o Identificador");

                var log = new Log();
                var caminhoDoArquivo = _logService.TransformarLogMinhaCdnParaAgora(identificador);
                if (retornarCaminho)
                {

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var nomeDoArquivo = Path.GetFileName(caminhoDoArquivo);
                    var path = $"/uploads/{nomeDoArquivo}";
                    var fullUrl = new Uri(new Uri(baseUrl), path);

                    return Ok(new { path = fullUrl });
                }
                else
                {
                    var arquivoEmTexto = System.IO.File.ReadAllTextAsync(caminhoDoArquivo).Result;
                    return Content(arquivoEmTexto, "text/plain");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }

        [HttpGet]
        [Route("buscar-logs-salvos")]
        public ActionResult<string> BuscarLogsSalvos()
        {
            var logs = _unitOfWork.LogMinhaCdn.ObterLogsMinhaCdn();
            return Ok(logs);
        }

        //Retorna todos os logs do banco no formato original "Minha CDN" e os logs no formato "Agora"
        [HttpGet]
        [Route("buscar-logs-transformados-no-backend/{identificador}")]
        public ActionResult<string> BuscarLogTransformadosNoBackend(int identificador)
        {
            var logs = _unitOfWork.Log.ObterLogPorIdentificador(identificador);
            var arquivoZip = _arquivoService.BaixarARquivosEZipar(logs).Result;

            if (arquivoZip == null)
                return NotFound("Nenhum arquivo foi encontrado para o identificador fornecido!!");

            return File(arquivoZip.ToArray(), "application/zip", "modelos.zip");
        }

        [HttpGet]
        [Route("buscar-logs-salvos-por-identificador/{identificador}")]
        public ActionResult<string> BuscaLogSalvosPorIdentificador(int identificador)
        {
            var logs = _unitOfWork.Log.ObterLogPorIdentificador(identificador);
            return Ok(logs);
        }

        [HttpGet]
        [Route("buscar-logs-transformados-por-identificador/{identificador}")]
        public ActionResult<string> BuscarLogsTransformadosPorIdentificador(int identificador)
        {
            var logs = _unitOfWork.LogAgora.ObterLogsAgoraPorIdentificador(identificador);
            return Ok(logs);
        }

        //Salvar o arquivo no servidor
        [HttpPost]
        [Route("salvar-logs")]
        public ActionResult<string> SalvarLogs(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("É necessario preencher o campo Url");
                
                var log = _logService.SalvarLog(url);                
                return Ok(new { mensagem = "Log foi salvo com sucesso!", log.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}
