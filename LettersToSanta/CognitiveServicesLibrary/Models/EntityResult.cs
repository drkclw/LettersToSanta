using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesLibrary.Models
{
    public class EntityResult
    {
        public string? Text { get; set; }
        public string? Category { get; set; }
        public double Score { get; set; }
    }
}
