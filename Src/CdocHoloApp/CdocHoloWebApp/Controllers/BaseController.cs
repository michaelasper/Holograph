using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CdocHoloWebApp.Controllers
{
    public abstract class BaseController: ApiController
    {
        private HttpContent Jsonify(object yourObject)
        {
            return new StringContent(JsonConvert.SerializeObject(yourObject));
        }

        public virtual Task<HttpResponseMessage> Respond(object theObject, HttpStatusCode code)
        {
            return Task.FromResult(
                new HttpResponseMessage()
                {
                    StatusCode = code,
                    Content = Jsonify(theObject)
                });
        }

        public virtual async Task<HttpResponseMessage> HandleGetResult(object result)
        {
            if (result != null)
            {
                return await Respond(result, HttpStatusCode.OK);
            }
            else
            {
                return await Respond(string.Empty, HttpStatusCode.NotFound);
            }
        }

        public virtual async Task<HttpResponseMessage> HandlePostResult(object result)
        {
            if (result != null)
            {
                return await Respond(result, HttpStatusCode.Created);
            }
            else
            {
                return await Respond(string.Empty, HttpStatusCode.BadRequest);
            }
        }
    }
}