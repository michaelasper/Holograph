using System.Collections.Generic;

namespace CdocHoloWebApp.Models
{
    public class VertexDto
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class VerticiesDto
    {
        public List<VertexDto> Verticies { get; set; }
    }
}