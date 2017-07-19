using System.Collections.Generic;

namespace CdocHoloWebApp.Models
{
    public class CaseDto
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public VerticiesDto verticies {get;set; }
        public EdgesDto edges { get; set; }
    }

    public class CasesDto
    {
        public List<CaseDto> Cases { get; set; }
    }
}