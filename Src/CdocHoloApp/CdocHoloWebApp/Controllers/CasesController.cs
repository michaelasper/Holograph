using CdocHoloWebApp;
using CdocHoloWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CdocHoloWebApp.Controllers
{
    [RoutePrefix("v1")]
    public class CasesController : BaseController
    {
        [HttpGet]
        [Route("cases")]
        public async Task<HttpResponseMessage> GetCases()
        {
            var result = WebApiApplication.cases;

            return await HandleGetResult(result);
        }

        [HttpGet]
        [Route("cases/full")]
        public async Task<HttpResponseMessage> GetFullCases()
        {
            var result = WebApiApplication.cases;

            return await HandleGetResult(result);
        }

        [HttpGet]
        [Route("cases/{id}")]
        public async Task<HttpResponseMessage> GetCase(string id)
        {
            var result = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == id);

            return await HandleGetResult(result);
        }

        [HttpPost]
        [Route("cases")]
        public async Task<HttpResponseMessage> AddCase([FromBody] CaseDto theCase)
        {
            if (theCase != null)
            {
                WebApiApplication.cases.Cases.Add(theCase);
            }

            return await HandlePostResult(theCase);
        }

        [HttpPut]
        [Route("cases/{id}")]
        public async Task<HttpResponseMessage> UpdateCase([FromBody] CaseDto theCase)
        {
            var result = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == theCase._id);

            if (result != null)
            {
                result = theCase;

                return await Respond(result, HttpStatusCode.OK);
            }
            else
            {
                WebApiApplication.cases.Cases.Add(theCase);

                return await HandlePostResult(theCase);
            }
        }
    }
}
