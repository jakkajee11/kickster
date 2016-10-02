using AutoMapper;
using GLib.Extensions;
using KickSter.Api.Helpers;
using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace KickSter.Api.Controllers
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        IUserService _userService;
        IPostService _postService;
        IZoneService _zoneService;
        IArenaService _arenaService;
        IMapper _mapper;

        public IEnumerable<MatchedPost> MachingHelpers { get; private set; }

        public TestController(IUserService userService, 
            IPostService postService, 
            IZoneService zoneService,
            IArenaService arenaService,
            IMapper mapper)
        {
            _userService = userService;
            _postService = postService;
            _zoneService = zoneService;
            _arenaService = arenaService;
            _mapper = mapper;
        }

        [HttpGet]
        public IHttpActionResult Test()
        {
            HttpRequestContext httpRequestContext = Request.GetRequestContext();
            Debug.WriteLine(string.Format("", httpRequestContext.Principal.Identity.IsAuthenticated));
            Debug.WriteLine(string.Format("Principal authenticated from extension method: {0}", httpRequestContext.Principal.Identity.IsAuthenticated));
            Debug.WriteLine(string.Format("Principal authenticated from shorthand property: {0}", RequestContext.Principal.Identity.IsAuthenticated));

            return Ok();
        }

        [HttpGet]
        [Route("Hello")]
        public IHttpActionResult Hello(string name)
        {
            return Ok("Hello " + name);
        }

        [HttpGet]
        [Route("Secure")]
        [Authorize]
        public IHttpActionResult SecureHello(string name)
        {
            return Ok("Hello " + name);
        }

        [HttpGet]
        [Route("MatchUser/{userId}")]
        public async Task<IEnumerable<MatchedPost>> MatchUser(string userId)
        {
            
            return await MatchingHelpers.MatchedByUserPreference(userId);
        }

        [HttpGet]
        [Route("MatchPost/{postId}")]
        public async Task<IEnumerable<MatchedPost>> MatchPost(string postId)
        {
            return await MatchingHelpers.MatchedPost(postId);
        }

        [HttpGet]
        [Route("Hash")]
        public IHttpActionResult Hash()
        {
            var obj = new ZoneDetail
            {
                City = "A",
                Country = "B",
                Language = "C",
                Province = "D"
            };

            var obj2 = new ZoneDetail
            {
                City = "a",
                Country = "B",
                Language = "C",
                Province = "D"
            };
            //_zoneService.FindAnyInAsync("Details")
            var result = new List<string>();
            result.Add(obj.JsonHash());
            result.Add(obj2.JsonHash());
            return Ok(result);
        }

        [HttpPost]
        [Route("GenPost/{num}")]
        public async Task<IHttpActionResult> GenPost(int num)
        {
            try
            {
                var paging = new GLib.Common.Paging { Page = 0, Size = num };
                var users = await _userService.ListAsync(paging);
                var zones = await _zoneService.ListAsync(paging);
                var arenas = await _arenaService.ListAsync(paging);
                Random rnd = new Random();

                //var user = users.ToArray()[1];
                var posts = new List<Post>();
                for (var i = 1; i <= num; i++)
                {
                    var n = rnd.Next(1, num);
                    var user = n > users.Count ? users.ToArray()[users.Count - 1] : users.ToArray()[n-1];
                    var postedBy = _mapper.Map<User>(user); //new Models.User { Id = user.Id, Name = user.Name, Picture = user.Picture };
                    var _arenas = arenas.Take(n).ToList();
                    var _zones = (IEnumerable<ZoneBase>)zones.Where(zn => _arenas.Exists(an => an.Zone.Id == zn.Id)).Take(n).ToList();
                    //var _zones = zones.Take(n).ToList();
                    //var _arenas = arenas.Where(an => _zones.Exists(zn => zn.Id.Equals(an.Zone.Id))).Take(n).ToList();//_mapper.Map<List<Arena>>(arenas).Where(an => _zones.Exists(zn => zn.Id.Equals(an.Zone.Id))).Take(n).ToList();
                    var post = new Post
                    {
                        Message = "Auto generated post #" + i,
                        Type = "หาเพื่อน",//KickSter.Models.Enums.PostType.FindPlayer,
                        Status = KickSter.Models.Enums.PostStatus.Open,
                        PostedBy = postedBy,
                        Arenas = _arenas,//_mapper.Map<List<Arena>>(arenas.Take(n)),
                        Zones = _zones.ToList()
                    };
                    posts.Add(post);
                }

                await _postService.ImportAsync(posts);

                return Ok(posts);
            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
