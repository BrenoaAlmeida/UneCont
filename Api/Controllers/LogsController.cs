using System;
using System.Collections.Generic;
using System.Net;
using Api.DTOs;
using Microsoft.AspNetCore.Mvc;
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

        public LogsController(LogService logService)
        {
            _arquivoService = new ArquivoHelper();
            _logService = logService;
        }

        [HttpGet]
        [Route("transformar-formato")]
        public ActionResult<IEnumerable<string>> TransformarFormato(string url, bool? retornarCaminho)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return StatusCode((int)HttpStatusCode.BadRequest, "É necessario informar a url");

                if (retornarCaminho == null)
                    return StatusCode((int)HttpStatusCode.BadRequest, "É necessario informar se deseja retornar o caminho do arquivo");

                var result = _logService.TransformarLogMinhaCdnParaAgora(url, retornarCaminho.Value);

                // foi retornado o log transformado no formato Agora
                if (!retornarCaminho.Value)
                    return Content(result, "text/plain");

                // o log no formato Agora foi salvo em pasta do servidor e retornado seu caminho                
                var urlBase = $"{Request.Scheme}://{Request.Host}";
                var urlCompleta = _arquivoService.RetornarCaminhoDoArquivoNoServidor(urlBase, result);

                return StatusCode((int)HttpStatusCode.OK, new { url = urlCompleta });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message.ToString());
            }

        }

        [HttpGet]
        [Route("transformar-formato/{identificador}")]
        public ActionResult<IEnumerable<string>> TransformarArquivo(int identificador, bool? retornarCaminho)
        {
            try
            {
                if (identificador == 0)
                    return StatusCode((int)HttpStatusCode.BadRequest, "É necessario informar o Identificador");

                if (retornarCaminho == null)
                    return StatusCode((int)HttpStatusCode.BadRequest, "É necessario informar se deseja retornar o caminho do arquivo");

                var result = _logService.TransformarLogMinhaCdnParaAgora(identificador, retornarCaminho.Value);

                if (string.IsNullOrEmpty(result))
                    return StatusCode((int)HttpStatusCode.NotFound, "Nenhum log encontrado para o identificador informado!");

                if (retornarCaminho.Value)
                {
                    var urlBase = $"{Request.Scheme}://{Request.Host}";
                    var urlCompleta = _arquivoService.RetornarCaminhoDoArquivoNoServidor(urlBase, result);
                    return StatusCode((int)HttpStatusCode.OK, new { url = urlCompleta });
                }

                return Content(result, "text/plain");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message.ToString());
            }

        }

        [HttpGet]
        [Route("buscar-logs-salvos")]
        public ActionResult<IList<LogDTO>> BuscarLogsSalvos()
        {
            var logs = LogDTO.Criar(_logService.ObterLogs());

            if (logs == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhum log encontrado");

            return StatusCode((int)HttpStatusCode.OK, logs);
        }

        //Retorna todos os logs do banco no formato original "Minha CDN" e os logs no formato "Agora"
        [HttpGet]
        [Route("buscar-logs-transformados")]
        public ActionResult<string> BuscarLogTransformadosNoBackend()
        {
            var logsArquivo = _logService.ObterLogsArquivo();

            if (logsArquivo == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhum log foi encontrado para o identificador fornecido");

            var result = _arquivoService.RetornarLogsEmTexto(logsArquivo);

            return Content(result, "text/plain");
        }

        [HttpGet]
        [Route("buscar-log-salvo/{identificador}")]
        public ActionResult<LogDTO> BuscaLogSalvo(int identificador)
        {
            var log = LogDTO.Criar(_logService.ObterLogPorIdentificador(identificador));

            if (log == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhum log foi encontrado para o identificador fornecido");

            return StatusCode((int)HttpStatusCode.OK, log);
        }

        [HttpGet]
        [Route("buscar-logs-transformados/{identificador}")]
        public ActionResult<List<LogAgoraDTO>> BuscarLogsTransformadosPorIdentificador(int identificador)
        {
            var logs = LogAgoraDTO.Criar(_logService.ObterLogsAgoraPorIdentificador(identificador));

            if (logs == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhum log foi encontrado para o identificador fornecido");

            return StatusCode((int)HttpStatusCode.OK, logs);
        }

        //Salvar o arquivo no servidor
        [HttpPost]
        [Route("salvar-logs")]
        public ActionResult<string> SalvarLogs(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return StatusCode((int)HttpStatusCode.BadRequest, "É necessario preencher o campo Url");

                var urlBase = $"{Request.Scheme}://{Request.Host}";
                var log = _logService.SalvarLog(url, urlBase).Result;
                return StatusCode((int)HttpStatusCode.OK, new { mensagem = "Log foi salvo com sucesso!", identificador = log.Id });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }
    }
}
