using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace KickSter.Api.Controllers
{
    [RoutePrefix("api/sysmessages")]
    public class SystemMessageController : ApiController
    {
        private readonly ISystemMessageService _service;
        public SystemMessageController(ISystemMessageService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("welcome")]
        public async Task<IHttpActionResult> Welcome()
        {
            var result = await _service.FindOneAsync("category", "welcome");
            if (result == null) return Ok("");
            else return Ok(result);
        }
    }
}
