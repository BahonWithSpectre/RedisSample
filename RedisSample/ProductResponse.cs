using System;
using System.Net;

namespace RedisSample
{
    public class ProductResponse
    {
        public HttpStatusCode StatusCode
        { get; set; }
        public string? Message
        { get; set; }
        public object? Data
        { get; set; }
        public bool IsDataFromCache
        { get; set; }
        public DateTime Timestamp
        { get; set; }
    }
}
