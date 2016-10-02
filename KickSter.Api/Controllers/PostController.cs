using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using KickSter.Api.Services.Interfaces;
using KickSter.Models;
using KickSter.Api.Models;
using KickSter.Models.Enums;
using GLib.Common;
using KickSter.Api.Helpers;
using System.Security.Principal;

namespace KickOff.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/posts")]
    public class PostController : ApiController
    {
        private readonly IPostService _service;
        private readonly IUserService _userService;
        private readonly IIdentity _currentUser;

        public PostController(IPostService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
            _currentUser = UserHelper.CurrentUser;
        }

        [HttpPost]
        [Route("AddTag")]
        public async Task<IHttpActionResult> AddTag([FromBody] Post post)
        {
            try
            {
                var result = await _service.AddTagAsync(post.Id, post.Tags.FirstOrDefault());
                return Ok(result);
            } catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("Comment")]
        public async Task<IHttpActionResult> AddComment([FromBody] Post post)
        {
            try
            {
                var result = await _service.AddCommentAsync(post.Id, post.Comments.FirstOrDefault());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("Comment/Remove")]
        public async Task<IHttpActionResult> RemoveComment([FromBody] Post post)
        {
            try
            {
                var result = await _service.RemoveCommentAsync(post.Id, post.Comments.FirstOrDefault());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("AddLike")]
        public async Task<IHttpActionResult> AddLike([FromBody] Post post)
        {
            try
            {
                var result = await _service.AddFollowerAsync(post.Id, post.Followers.FirstOrDefault());
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        [Route("Place")]
        public async Task<IHttpActionResult> Place([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdatePlaceAsync(post.Id, post.Place);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Picture")]
        public async Task<IHttpActionResult> Picture([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdatePictureAsync(post.Id, post.Pictures);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Zone")]
        public async Task<IHttpActionResult> Zone([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdateZoneAsync(post.Id, post.Zones);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Arena")]
        public async Task<IHttpActionResult> Arena([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdateArenaAsync(post.Id, post.Arenas);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Tag")]
        public async Task<IHttpActionResult> Tag([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdateTagAsync(post.Id, post.Tags);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Contact")]
        public async Task<IHttpActionResult> Contact([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdateContactInfoAsync(post.Id, post.ContactInfo);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("Days")]
        public async Task<IHttpActionResult> Days([FromBody] Post post)
        {
            try
            {
                var result = await _service.UpdatePreferenceDayAsync(post.Id, post.DayTimes);
                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody] Post post)
        {
            post.Id = null;
            post.Status = PostStatus.Open;
            var result = await _service.CreateAsync(post);
            if (result == null)
                return InternalServerError();
            else
            {
                await _userService.UpdateTotalPost(post.PostedBy.Id);
                return Ok(result);
            }
                
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] Post post)
        {
            var result = await _service.UpdateAsync(post);
            if (result == null)
                return InternalServerError();
            else
                return Ok(result);
        }

        [HttpPost]
        [Route("Replace")]
        public async Task<IHttpActionResult> Replace([FromBody] Post post)
        {
            var result = await _service.ReplaceAsync(post);
            if (result == null)
                return InternalServerError();
            else
                return Ok(result);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] List<Post> posts)
        {
            try
            {    
                foreach(var post in posts)
                {
                    post.DateCreated = DateTime.Now;
                }
                await _service.ImportAsync(posts);
                return Ok("Successfully.");
            } catch
            {
                return InternalServerError();
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
                var posts = await _service.ListAsync(paging);
                var pinnedPosts = posts.Where(p => p.Pinned).OrderByDescending(p => p.DateChanged);
                var normalPosts = posts.Where(p => !p.Pinned).OrderByDescending(p => p.DateChanged);
                //pinnedPosts.Union(normalPosts);
                if (pinnedPosts.Count() > 0 && normalPosts.Count() > 0)
                    return Ok(pinnedPosts.Union(normalPosts));
                else
                    return Ok(posts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("MyPost")]
        public async Task<IHttpActionResult> UserPost([FromUri] Paging paging = null)
        {
            try
            {                
                var posts = await _service.GetUserPosts(_currentUser.Name, paging);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("PostMatchMe")]
        public async Task<IHttpActionResult> MatchUserPreference()
        {
            try
            {
                var matchedPosts = await MatchingHelpers.MatchedByUserPreference(_currentUser.Name);
                return Ok(matchedPosts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
