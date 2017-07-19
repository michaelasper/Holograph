using System.Collections.Generic;

namespace CdocHoloWebApp.Models
{
    public class EdgeDto
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class EdgesDto
    {
        public List<EdgeDto> Edges { get; set; }
    }
}