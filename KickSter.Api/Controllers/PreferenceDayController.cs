using GLib.Common;
using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.ViewModels;
using MongoDB.Bson;
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
    [RoutePrefix("api/days")]
    public class PreferenceDayController : ApiController
    {
        readonly IPreferenceDayService _service;

        public PreferenceDayController(IPreferenceDayService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody] PreferenceDay day)
        {
            try
            {
                var result = await _service.CreateAsync(day);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] IEnumerable<PreferenceDay> days)
        {
            try
            {
                await _service.ImportAsync(days);
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
        public async Task<IHttpActionResult> FindId([FromBody] IEnumerable<IdViewModel> days)
        {
            try
            {                
                var result = await _service.FindInAsync("Id", (days.Select(d=> d.Id)));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
