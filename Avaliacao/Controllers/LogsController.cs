using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace Api.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        ArquivoHelper _arquivoService;
        LogService _logService;

        /// <summary>
        /// Obtem um arquivo no formato "Minha CDN" e o tranforma no formato "Agora", podendo passar como  entrar uma URL contendo o arquivo TXT ou um  identificador
        /// para um arquivo que ja foi salvo no banco de dados, como POST pode salvar o arquivo no servidor e retornar o path dele  
        /// </summary>
        /// <param name="somepara">Required parameter: Example: </param>
        /// <return>Returns comment</return>
        /// <response code="200">Ok</response>

        public LogsController(LogService logService)
        {
            _arquivoService = new ArquivoHelper();
            _logService = logService;
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
                    ○ Formato de saída pode variar (O usuário vai selecionar na requisição)
                    resultado: 
                        path do arquivo salvo no servidor OU log transformado
                 */

                // acoes:
                // - transformar o log
                // - retornar o path ou o proprio log transformado

                var result = _logService.TransformarLogMinhaCdnParaAgora(url, retornarPath);

                if (!retornarPath)
                    // foi retornado o log transformado no formato Agora
                    return Content(result, "text/plain");

                // o log no formato Agora foi salvo em pasta do servidor e retornado seu caminho                
                var urlBase = $"{Request.Scheme}://{Request.Host}";
                var urlCompleta = _arquivoService.RetornarCaminhoDoArquivoNoServidor(urlBase, result);

                return Ok(new { path = urlCompleta });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }

        [HttpGet]
        [Route("transformar-formato/{identificador}")]
        public ActionResult<IEnumerable<string>> TransformarArquivo(int identificador, bool retornarCaminho)
        {
            try
            {
                if (identificador == 0)
                    return BadRequest("É necessario informar o Identificador");

                var result = _logService.TransformarLogMinhaCdnParaAgora(identificador, retornarCaminho);

                if (string.IsNullOrEmpty(result))
                    return StatusCode((int)HttpStatusCode.NoContent, "Nenhum registro encontrado para o identificador informado!");

                if (retornarCaminho)
                {

                    var urlBase = $"{Request.Scheme}://{Request.Host}";
                    var urlCompleta = _arquivoService.RetornarCaminhoDoArquivoNoServidor(urlBase, result);
                    return Ok(new { path = urlCompleta });
                }
                else
                    return Content(result, "text/plain");
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
            var logs = _logService.ObterLogs();

            if (logs == null)
                return StatusCode((int)HttpStatusCode.NoContent, "Nenhum registro encontrado");

            return Ok(logs);
        }

        //Retorna todos os logs do banco no formato original "Minha CDN" e os logs no formato "Agora"
        [HttpGet]
        [Route("buscar-logs-transformados-no-backend")]
        public ActionResult<string> BuscarLogTransformadosNoBackend()
        {

            //TODO MELHORAR ESSE METODO
            var logsArquvio = _logService.ObterLogsArquivo();

            if (logsArquvio == null)
                return StatusCode((int)HttpStatusCode.NoContent, "Nenhum log foi encontrado para o identificador fornecido");

            //TODO: Mostrar os 2 arquivos em texto na resposta do Swagger
            var result = _arquivoService.RetornarLogsEmTexto(logsArquvio);

            return Content(result, "text/plain");
        }

        [HttpGet]
        [Route("buscar-log-salvo/{identificador}")]
        public ActionResult<string> BuscaLogSalvos(int identificador)
        {
            var log = _logService.ObterLogPorIdentificador(identificador);
            if (log == null)
                return StatusCode((int)HttpStatusCode.NoContent, "Nenhum log foi encontrado para o identificador fornecido");

            return Ok(log);
        }

        [HttpGet]
        [Route("buscar-logs-transformados-por-identificador/{identificador}")]
        public ActionResult<string> BuscarLogsTransformadosPorIdentificador(int identificador)
        {
            var logs = _logService.ObterLogsAgoraPorIdentificador(identificador);

            if (logs == null)
                return StatusCode((int)HttpStatusCode.NoContent, "Nenhum log foi encontrado para o identificador fornecido");

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

                var urlBase = $"{Request.Scheme}://{Request.Host}";
                var log = _logService.SalvarLog(url, urlBase).Result;
                return Ok(new { mensagem = "Log foi salvo com sucesso!", log.Id });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }
    }
}
