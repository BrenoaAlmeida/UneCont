namespace Api.DTOs
{
    public class LogMinhaCdnDTO
    {
        public int Id { get; set; }

        public int LogId { get; set; }

        public string ResponseSize { get; set; }

        public string StatusCode { get; set; }

        public string CacheStatus { get; set; }

        public string Request { get; set; }

        public string TimeTaken { get; set; }
    }
}