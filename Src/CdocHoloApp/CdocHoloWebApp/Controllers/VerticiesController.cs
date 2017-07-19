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
    public class VerticiesController : BaseController
    {
        [HttpGet]
        [Route("cases/{id}/vertices")]
        public async Task<HttpResponseMessage> GetVerticies()
        {
            var result = WebApiApplication.cases;

            return await HandleGetResult(result);
        }

        [HttpGet]
        [Route("cases/{caseId}/vertices/{vertexId}")]
        public async Task<HttpResponseMessage> GetVertex(string caseId, string vertexId)
        {
            var result = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId);

            if (result != null)
            {
                var result2 = result.verticies.Verticies.FirstOrDefault(x => x._id == vertexId);

                return await HandleGetResult(result2);
            }

            return await HandleGetResult(result);
        }

        [HttpPost]
        [Route("cases/{caseId}/vertices")]
        public async Task<HttpResponseMessage> AddCase(string caseId, VertexDto theVertex)
        {
            if (theVertex != null && caseId != null)
            {
                WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId).verticies.Verticies.Add(theVertex);
            }

            return await HandlePostResult(theVertex);
        }

        [HttpPut]
        [Route("cases/{caseId}/vertices/{vertexId}")]
        public async Task<HttpResponseMessage> UpdateCase(string caseId, VertexDto theVertex)
        {
            var caseResult = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId);

            if (caseResult != null)
            {
                if (caseResult.verticies != null)
                {
                    var vertexResult = caseResult.verticies.Verticies.FirstOrDefault(x => x._id == theVertex._id);

                    if (vertexResult != null)
                    {
                        vertexResult = theVertex;
                    }
                    else
                    {
                        caseResult.verticies.Verticies.Add(theVertex);
                    }
                }

                return await Respond(theVertex, HttpStatusCode.OK);
            }
            else
            {
                return await Respond(string.Empty, HttpStatusCode.NotFound);
            }
        }
    }
}
