using GLib.Common;
using KickSter.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace KickSter.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/players")]
    public class PlayerController : ApiController
    {
        readonly IAccountService _service;
        public PlayerController(IAccountService service)
        {
            _service = service;
        }

        // GET: api/Player/{id}
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> List([FromUri] Paging paging = null)
        {
            var accounts = await _service.ListAsync(paging);
            if (accounts == null)
                return InternalServerError();
            else
            {
                var players = new List<Player>();
                foreach (var account in accounts)
                {
                    players.Add(account.Player);
                }

                return Ok(players);
            }
        }

        
        // GET: api/Player/{id}
        [HttpGet]
        [Route("api/player/{id}")]
        public async Task<IHttpActionResult> Get(string id)
        {
            var account = await _service.FindOneAsync(id);
            if (account == null)
                return InternalServerError();
            else   
                return Ok(account.Player);
        }

        [HttpPatch]
        [Route("api/player/{player.Id}/pref")]
        public async Task<IHttpActionResult> Preference(Player player)
        {
            try
            {
                var result = await _service.UpdatePreferenceAsync(player.Id, player.Preferences);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }            
        }

        [HttpPatch]
        [Route("api/player/{player.Id}/other")]
        public async Task<IHttpActionResult> OtherInfo(Player player)
        {
            try
            {
                var result = await _service.UpdateOtherInfoAsync(player.Id, player.OtherInfo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("api/player/{player.Id}/avatar")]
        public async Task<IHttpActionResult> Avatar(Player player)
        {
            try
            {
                var result = await _service.UpdateAvatarAsync(player.Id, player.Avatar);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
