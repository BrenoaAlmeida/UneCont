namespace Api.DTOs
{
    public class LogMinhaCdnDTO
    {    
        public string ResponseSize { get; set; }

        public string StatusCode { get; set; }

        public string CacheStatus { get; set; }

        public string Request { get; set; }

        public string TimeTaken { get; set; }
    }
}