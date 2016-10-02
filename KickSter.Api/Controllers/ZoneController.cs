using GLib.Common;
using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace KickSter.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/zones")]
    public class ZoneController : ApiController
    {
        readonly IZoneService _service;

        public ZoneController(IZoneService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody] Zone zone)
        {
            try
            {
                var result = await _service.CreateAsync(zone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] Zone zone)
        {
            var result = await _service.UpdateAsync(zone);
            if (result == null)
                return InternalServerError();
            else
                return Ok(result);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] IEnumerable<Zone> zones)
        {
            try
            {
                await _service.ImportAsync(zones);
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
                var zones = await _service.ListAsync(paging);
                return Ok(zones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("FindId")]
        public async Task<IHttpActionResult> FindId([FromBody] IEnumerable<IdViewModel> zones)
        {
            try
            {                
                var result = await _service.FindObjectIdInAsync(zones.Select(z=> ObjectId.Parse(z.Id)));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
