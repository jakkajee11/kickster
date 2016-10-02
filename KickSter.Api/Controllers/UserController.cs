using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Models;
using AutoMapper;
using KickSter.Api.Helpers;
using KickSter.Api.Services;
using System.Web;
using System.Security.Principal;
using GLib.Common;

namespace KickOff.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly IIdentity _currentUser;
        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
            _currentUser = UserHelper.CurrentUser;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> List([FromUri] Paging paging = null)
        {
            var result = await _service.ListAsync(paging);

            if (result == null)
            {
                return InternalServerError();
            }
            else
            {
                return Ok(result);
            }
        }

        //[HttpGet]
        //[Route("{id}/Friend/Matched")]
        //public async Task<IHttpActionResult> Friends(string id)
        //{
        //    var friends = await MatchingHelpers.MatchedFriend(id);

        //    if (friends == null)
        //    {
        //        return InternalServerError();
        //    }
        //    else
        //    {
        //        return Ok(friends);
        //    }
        //}

        [HttpGet]
        [Route("FriendMatchedMe")]
        public async Task<IHttpActionResult> MatchFriend()
        {
            var friends = await MatchingHelpers.MatchedFriend(_currentUser.Name);

            if (friends == null)
            {
                return InternalServerError();
            }
            else
            {
                return Ok(friends);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IHttpActionResult> Login([FromBody] Login login)
        {            
            var result = await _service.Login(login.Username, login.Password);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Invalid Credentials.");
            } else
            {
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody] UserProfile user)
        {
            try
            {                
                var result = await _service.CreateAsync(user);
                return Ok(result);
            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }                       
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] UserProfile user)
        {
            var result = await _service.UpdateAsync(user);
            if (result == null)
                return InternalServerError();
            else
                return Ok(result);
        }

        //[HttpPost]
        //[Route("{user.Id}/Preference/Update")]
        //public async Task<IHttpActionResult> Preference([FromBody] UserProfile user)
        //{            
        //    try
        //    {
        //        var result = await _service.UpdatePreferenceAsync(user.Id, user.Preferences);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        [HttpPost]
        [Route("Preference/Update")]
        public async Task<IHttpActionResult> UpdatePreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.UpdatePreferenceAsync(user.Id, user.Preferences);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[HttpPatch]
        //[Route("{user.Id}/Preference/Zones")]
        //public async Task<IHttpActionResult> AddZonePreference([FromBody] UserProfile user)
        //{
        //    try
        //    {
        //        var result = await _service.AddZonePreferenceAsync(user.Id, user.Preferences.Zones);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        [HttpPatch]
        [Route("Preference/Zones")]
        public async Task<IHttpActionResult> AddZonePreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.AddZonePreferenceAsync(user.Id, user.Preferences.Zones);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("Preference/Arenas")]
        public async Task<IHttpActionResult> AddArenaPreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.AddArenaPreferenceAsync(user.Id, user.Preferences.Arenas);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("Preference/Days")]
        public async Task<IHttpActionResult> AddDayPreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.AddDayPreferenceAsync(user.Id, user.Preferences.DayTimes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("Preference/Zones/Remove")]
        public async Task<IHttpActionResult> RemoveZonePreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.RemoveZonePreferenceAsync(user.Id, user.Preferences.Zones.FirstOrDefault().Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("Preference/Arenas/Remove")]
        public async Task<IHttpActionResult> RemoveArenaPreference([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.RemoveArenaPreferenceAsync(user.Id, user.Preferences.Arenas.FirstOrDefault().Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[HttpPost]
        //[Route("{user.Id}/Preference/Days/Remove")]
        //public async Task<IHttpActionResult> RemoveDayPreference([FromBody] UserProfile user)
        //{
        //    try
        //    {
        //        var result = await _service.RemoveDayPreferenceAsync(user.Id, user.Preferences.DayTimes.FirstOrDefault().Day, user.Preferences.DayTimes.FirstOrDefault().Time);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] List<UserProfile> users)
        {
            try
            {
                //users.Where(a=> a.Player == null)
                //foreach(var a in users.Where(ac=> ac.Player == null))
                //{
                //    a.Player = new Player { Name = a.UserName };
                //}
                await _service.ImportAsync(users);
                return Ok("Successfully.");
            } catch
            {
                return InternalServerError();
            }
        }

        //[HttpGet]
        //[Route("{id}/Info")]
        //public async Task<IHttpActionResult> Info([FromUri] string id)
        //{
        //    try
        //    {
        //        var result = await _service.GetUserById(id);
                
        //        return Ok(_mapper.Map<UserInfo>(result));
        //    }
        //    catch
        //    {
        //        return InternalServerError();
        //    }
        //}

        [HttpGet]
        [Route("Info")]
        public async Task<IHttpActionResult> UserInfo()
        {
            try
            {
                var result = await _service.GetUserById(_currentUser.Name);

                return Ok(_mapper.Map<UserInfo>(result));
            }
            catch
            {
                return InternalServerError();
            }
        }        

        //[HttpGet]
        //[Route("{id}/InUsed/{nickname}")]
        //public async Task<IHttpActionResult> IsNameInUsed([FromUri] string id, [FromUri] string nickname)
        //{
        //    try
        //    {
        //        var result = await _service.IsNameInUsed(id, nickname);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        [HttpGet]
        [Route("NameInUsed/{nickname}")]
        public async Task<IHttpActionResult> IsNameInUsed([FromUri] string nickname)
        {
            try
            {
                var result = await _service.IsNameInUsed(_currentUser.Name, nickname);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[HttpGet]
        //[Route("MatchedFriend/{id}")]
        //public async Task<IHttpActionResult> MatchedFriend([FromUri] string id)
        //{            
        //    try
        //    {
        //        var result = await MatchingHelpers.MatchedFriend(id);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpGet]
        //[Route("v2/MatchedFriend")]
        //public async Task<IHttpActionResult> MatchedFriend2()
        //{
        //    try
        //    {
        //        var result = await MatchingHelpers.MatchedFriend(_currentUser.Name);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        [AllowAnonymous]
        [HttpPost]
        [Route("auth0/sync")]
        public async Task<IHttpActionResult> Auth0Sync([FromBody] UserProfile user)
        {
            try
            {
                var result = await _service.Auth0SyncUserAsync(user);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //[HttpPatch]
        //[Route("auth0/usp/{user.UserId}/update1")]
        //public IRestResponse UpdatePreferences([FromBody] UserProfile user)
        //{
        //    //var userMetaData = "{ \"user_metadata\": " + JsonConvert.SerializeObject(user.Preferences) + "}";
        //    var userMetaData = "{ \"user_metadata\": {\"preferences\": {\"color\":\"red\"}}}";
        //    var client = new RestClient("https://kickster.au.auth0.com/api/v2/users/auth0%7C57b1e0f28bcba7a536cef1e4");
        //    var request = new RestRequest(Method.PATCH);
        //    request.AddHeader("authorization", "Bearer " + user.Token);
        //    request.AddParameter("undefined", userMetaData, ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);

        //    return response;
        //}

        //[HttpPatch]
        //[Route("auth0/usp/{user.UserId}/update")]
        //public async Task<IHttpActionResult> Auth0UpdatePreferences([FromBody] UserProfile user)
        //{
        //    try
        //    {
        //        var helper = new HttpClientHelpers(AppConfig.AUTH0_DOMAIN);
        //        var userMetaData = "{ \"user_metadata\": " + JsonConvert.SerializeObject(user.Preferences) + "}";
        //        //var userMetaData = new Auth0UserMetaData { user_metadata = JsonConvert.(user.Preferences) };
        //        var response = await helper.PatchAsync<Auth0UserMetaData>(AppConfig.AUTH0_USER_API, userMetaData, "authorization", "Bearer " + user.Token);
        //        return Ok("{ \"user_metadata\": " + JsonConvert.SerializeObject(user.Preferences) + "}");
        //    } catch(Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }                        
        //}
    }
}
