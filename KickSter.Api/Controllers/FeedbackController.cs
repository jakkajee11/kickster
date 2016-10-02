using GLib.Common;
using KickSter.Api.Models;
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
    [Authorize]
    [RoutePrefix("api/feedbacks")]
    public class FeedbackController : ApiController
    {
        readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> List([FromUri] Paging paging = null)
        {
            try
            {
                var result = await _service.ListAsync(paging);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(Feedback feedback)
        {
            try
            {
                var result = await _service.CreateAsync(feedback);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
