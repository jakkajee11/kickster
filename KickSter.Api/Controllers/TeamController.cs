using AutoMapper;
using GLib.Common;
using GLib.Extensions;
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
    [RoutePrefix("api/teams")]
    public class TeamController : ApiController
    {
        IUserService _userService;
        IPostService _postService;
        IZoneService _zoneService;
        IArenaService _arenaService;
        ITeamService _teamService;
        IMapper _mapper;

        public IEnumerable<MatchedPost> MachingHelpers { get; private set; }

        private readonly IIdentity _currentUser;

        public TeamController(IUserService userService, 
            IPostService postService, 
            IZoneService zoneService,
            IArenaService arenaService,
            ITeamService teamService,
            IMapper mapper)
        {
            _userService = userService;
            _postService = postService;
            _zoneService = zoneService;
            _arenaService = arenaService;
            _teamService = teamService;
            _mapper = mapper;
            _currentUser = UserHelper.CurrentUser;
        }
        

        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> List([FromUri] Paging paging = null)
        {
            try
            {
                var result = await _teamService.ListAsync(paging);
                return Ok(result);
            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("Team/{id}")]
        public async Task<IHttpActionResult> GetTeam(string id)
        {
            try
            {
                var result = await _teamService.FindOneAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("MyTeam")]
        public async Task<IHttpActionResult> GetTeamByUser(string id)
        {
            try
            {
                var result = await _teamService.FindTeamByUser(_currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(Team team)
        {
            try
            {
                var result = await _teamService.CreateAsync(team);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("Update")]
        public async Task<IHttpActionResult> Update(Team team)
        {
            try
            {
                var result = await _teamService.UpdateAsync(team);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("{team.Id}/member/add")]
        public async Task<IHttpActionResult> AddMember([FromBody] Team team)
        {
            try
            {
                if (team.Members == null || team.Members.Count == 0)
                    return BadRequest("Player must not be empty.");

                var result = await _teamService.AddMemberAsync(team.Id, team.Members);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("{team.Id}/member/remove")]
        public async Task<IHttpActionResult> RemoveMember([FromBody] Team team)
        {
            try
            {
                var result = await _teamService.RemoveMemberAsync(team.Id, team.Members);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("{team.Id}/member/block")]
        public async Task<IHttpActionResult> BlockMember([FromBody] Team team)
        {
            try
            {
                var result = await _teamService.BlockMemberAsync(team.Id, team.Members);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("{team.Id}/member/request")]
        public async Task<IHttpActionResult> RequestMember([FromBody] Team team)
        {
            try
            {
                var result = await _teamService.RequestMemberAsync(team.Id, team.RequestPending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
