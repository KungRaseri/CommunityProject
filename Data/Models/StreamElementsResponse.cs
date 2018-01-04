using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Data.Models
{
    public class StreamElementsResponse
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }
}
