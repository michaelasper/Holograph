using System.Net;
using System.Web.Http;
using CdocHoloWebApp.Models;

namespace CdocHoloWebApp.Controllers
{
    [RoutePrefix("api")]
    public class GtbbController : ApiController
    {
        public GtbbController()
        {
        }

        // GET api/Login
        [HttpPost]
        [Route("Login")]
        public SessionDto Login([FromBody] SessionDto session)
        {
            // DO your DB stuff here!!!
            session.sessionId = "12324567890";

            return session;
        }

        [Route("Test")]
        public string GetTest()
        {
            return "test worked!";
        }

        [HttpPost]
        [Route("TestPost")]
        public IHttpActionResult GenerateXml([FromBody] SessionDto ruleSet)
        {
            return Content(HttpStatusCode.OK, "test worked!");
        }
    }
}
