using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using KickSter.Api.Models;
using KickSter.Api.ViewModels;
using System.Linq;
using MongoDB.Bson;
using System.Web;
using KickSter.Api.Helpers;
using GLib.Common;

namespace KickSter.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/arenas")]
    public class ArenaController : ApiController
    {
        readonly IArenaService _service;
        public ArenaController(IArenaService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] IEnumerable<Arena> arenas)
        {
            try
            { 
                await _service.ImportAsync(arenas);
                return Ok();
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
                var arenas = await _service.ListAsync(paging);
                return Ok(arenas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("FindId")]
        public async Task<IHttpActionResult> FindId([FromBody] IEnumerable<IdViewModel> arenas)
        {
            try
            {
                var result = await _service.FindObjectIdInAsync(arenas.Select(a => ObjectId.Parse(a.Id)));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
