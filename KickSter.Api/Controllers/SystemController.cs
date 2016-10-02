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

    public class SystemController : ApiController
    {
        private readonly IClientService _clientService;
        private readonly IZoneService _zoneService;
        private readonly IArenaService _arenaService;

        public SystemController(IClientService clientService, IZoneService zoneService, IArenaService arenaService)
        {
            _clientService = clientService;
            _zoneService = zoneService;
            _arenaService = arenaService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> Initialize()
        {
            await _clientService.CreateAsync(new Models.Client
            {
                Id = "57d2e575b1b43e86f456d321",
                Domain = "client.kickster.club"
            });
            return Ok();
        }
    }
}
