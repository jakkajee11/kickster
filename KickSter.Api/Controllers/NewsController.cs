using GLib.Common;
using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KickSter.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody] News news)
        {
            try
            {
                var result = await _service.CreateAsync(news);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        

        [AllowAnonymous]
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> List([FromUri] Paging paging = null)
        {
            try
            {
                if (paging == null) paging = new Paging { Sort = "DateChanged", Desc = true };
                var result = await _service.ListAsync(paging);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
    }
}
