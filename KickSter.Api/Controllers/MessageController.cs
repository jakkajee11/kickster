using KickSter.Api.Helpers;
using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace KickSter.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/messages")]
    public class MessageController : ApiController
    {
        private readonly IMessageService _service;
        private readonly IIdentity _currentUser;
        public MessageController(IMessageService service)
        {
            _service = service;
            _currentUser = UserHelper.CurrentUser;
        }

        [HttpGet]
        [Route("mine")]
        public async Task<IHttpActionResult> GetUserMessage()
        {
            var result = await _service.FindAsync(_currentUser.Name);
            return Ok(result);
        }

        [HttpPost]
        [Route("Send")]
        public async Task<IHttpActionResult> PostMessage([FromBody] UserMessage message)
        {
            var result = await _service.AddMessageAsync(message.Id, message.Messages);
            return Ok(result);
        }
    }
}
