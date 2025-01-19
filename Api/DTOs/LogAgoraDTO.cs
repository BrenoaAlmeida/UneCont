using Model;
using System.Collections.Generic;

namespace Api.DTOs
{
    public class LogAgoraDTO
    {
        public string Provider = "MINHA CDN";

        public string HttpMethod { get; set; }

        public string StatusCode { get; set; }

        public string UriPath { get; set; }

        public int TimeTaken { get; set; }

        public string ResponseSize { get; set; }

        public string CacheStatus { get; set; }


        public static LogAgoraDTO Criar(LogAgora logAgora)
        {
            if (logAgora == null)
                return null;

            var logAgoraDTO = new LogAgoraDTO();
            logAgoraDTO.Provider = logAgora.Provider;
            logAgoraDTO.HttpMethod = logAgora.HttpMethod;
            logAgoraDTO.StatusCode = logAgora.StatusCode;
            logAgoraDTO.UriPath = logAgora.UriPath;
            logAgoraDTO.TimeTaken = logAgora.TimeTaken;
            logAgoraDTO.ResponseSize = logAgora.ResponseSize;
            logAgoraDTO.CacheStatus = logAgora.CacheStatus;
            return logAgoraDTO;
        }

        public static IList<LogAgoraDTO> Criar(IList<LogAgora> logAgora)
        {
            if (logAgora == null || logAgora.Count == 0)
                return null;

            var logsAgoraDto = new List<LogAgoraDTO>();
            foreach (var logAgoraItem in logAgora)
            {
                var logAgoraDto = Criar(logAgoraItem);
                logsAgoraDto.Add(logAgoraDto);
            }
            return logsAgoraDto;
        }
    }
}