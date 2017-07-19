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
    public class EdgesController : BaseController
    {
        [HttpGet]
        [Route("cases/{id}/vertices")]
        public async Task<HttpResponseMessage> GetEdges()
        {
            var result = WebApiApplication.cases;

            return await HandleGetResult(result);
        }

        [HttpGet]
        [Route("cases/{caseId}/edges/{edgeId}")]
        public async Task<HttpResponseMessage> GetVertex(string caseId, string edgeId)
        {
            var result = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId);

            if (result != null)
            {
                var result2 = result.edges.Edges.FirstOrDefault(x => x._id == edgeId);

                return await HandleGetResult(result2);
            }

            return await HandleGetResult(result);
        }

        [HttpPost]
        [Route("cases/{caseId}/edges")]
        public async Task<HttpResponseMessage> AddCase(string caseId, EdgeDto theEdge)
        {
            if (theEdge != null && caseId != null)
            {
                WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId).edges.Edges.Add(theEdge);
            }

            return await HandlePostResult(theEdge);
        }

        [HttpPut]
        [Route("cases/{caseId}/edges/{edgeId}")]
        public async Task<HttpResponseMessage> UpdateEdge(string caseId, EdgeDto theEdge)
        {
            var caseResult = WebApiApplication.cases.Cases.FirstOrDefault(x => x._id == caseId);

            if (caseResult != null)
            {
                if (caseResult.edges != null)
                {
                    var vertexResult = caseResult.edges.Edges.FirstOrDefault(x => x._id == theEdge._id);

                    if (vertexResult != null)
                    {
                        vertexResult = theEdge;
                    }
                    else
                    {
                        caseResult.edges.Edges.Add(theEdge);
                    }
                }

                return await Respond(theEdge, HttpStatusCode.OK);
            }
            else
            {
                return await Respond(string.Empty, HttpStatusCode.NotFound);
            }
        }
    }
}