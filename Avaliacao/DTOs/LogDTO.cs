using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Service.Enumeradores;

namespace Api.DTOs
{
    public class LogDTO
    {
        public LogDTO()
        {
            LogMinhaCdn = new List<LogMinhaCdnDTO>();
            LogAgora = new List<LogAgoraDTO>();
        }

        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime DataHoraDeInsercao { get; set; }

        public IList<LogMinhaCdnDTO> LogMinhaCdn { get; set; }

        public IList<LogAgoraDTO> LogAgora { get; set; }        

        public string CaminhoDoArquivoLogMinhaCdn { get; set; }

        public string CaminhoDoArquivoLogAgora { get; set; }

        public static LogDTO Criar(Log log)
        {
            if (log == null)
                return null;

            var logDto = new LogDTO();
            logDto.Id = log.Id;
            logDto.Url = log.Url;
            logDto.DataHoraDeInsercao = log.DataDeInsercao;

            foreach (var logMinhaCdnModel in log.LogMinhaCdn)
            {
                var logMinhaCdnDTO = new LogMinhaCdnDTO();
                logMinhaCdnDTO.Id = logMinhaCdnModel.Id;
                logMinhaCdnDTO.LogId = logMinhaCdnModel.LogId;
                logMinhaCdnDTO.Request = logMinhaCdnModel.Request;
                logMinhaCdnDTO.ResponseSize = logMinhaCdnModel.ResponseSize;
                logMinhaCdnDTO.StatusCode = logMinhaCdnModel.StatusCode;
                logMinhaCdnDTO.CacheStatus = logMinhaCdnModel.CacheStatus;
                logMinhaCdnDTO.TimeTaken = logMinhaCdnModel.TimeTaken;
                logDto.LogMinhaCdn.Add(logMinhaCdnDTO);
            }

            foreach (var logAgoraModel in log.LogAgora)
            {
                var logAgoraDto = new LogAgoraDTO();
                logAgoraDto.Id = logAgoraModel.Id;
                logAgoraDto.LogId = logAgoraModel.LogId;
                logAgoraDto.Provider = logAgoraModel.Provider;
                logAgoraDto.HttpMethod = logAgoraModel.HttpMethod;
                logAgoraDto.StatusCode = logAgoraModel.StatusCode;
                logAgoraDto.UriPath = logAgoraModel.UriPath;
                logAgoraDto.TimeTaken = logAgoraModel.TimeTaken;
                logAgoraDto.ResponseSize = logAgoraModel.ResponseSize;
                logAgoraDto.CacheStatus = logAgoraModel.CacheStatus;
                logDto.LogAgora.Add(logAgoraDto);
            }

            logDto.CaminhoDoArquivoLogAgora = log.LogArquivo.Where(l => l.TipoLog == UneContEnum.ETipoLog.Agora.ToString()).FirstOrDefault().CaminhoDoArquivo;
            logDto.CaminhoDoArquivoLogMinhaCdn = log.LogArquivo.Where(l => l.TipoLog == UneContEnum.ETipoLog.MinhaCdn.ToString()).FirstOrDefault().CaminhoDoArquivo;

            return logDto;
        }

        public static IList<LogDTO> Criar(IList<Log> log)
        {
            var logsDto = new List<LogDTO>();
            foreach (var logItem in log)
            {
                var logDto = Criar(logItem);
                logsDto.Add(logDto);
            }
            return logsDto;
        }
    }
}
