using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Models
{
    public class SantaLetter
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }
        public string? FileName { get; set; }
        public string[]? Content { get; set; }
        public bool IsRead { get; set; }
        public bool IsLetter { get; set; }
        public bool NeedsAttention { get; set; }
        public string? Language { get; set; }
        public string[]? Requesteditems { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
