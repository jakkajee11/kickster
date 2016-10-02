using KickSter.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using KickSter.Api.Services.Interfaces;
using KickSter.Models;

namespace KickSter.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        readonly IAccountService _service;
        public AccountController(IAccountService service)
        {
            _service = service;
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
        [Route("Register")]
        public async Task<IHttpActionResult> Register([FromBody] Account account)
        {
            try
            {
                account.DateCreated = DateTime.MinValue == account.DateCreated ? DateTime.Now : account.DateCreated;
                var result = await _service.CreateAsync(account);
                return Ok(result);
            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }                       
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] Account account)
        {
            var result = await _service.UpdateAsync(account);
            if (result == null)
                return InternalServerError();
            else
                return Ok(result);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IHttpActionResult> Import([FromBody] List<Account> accounts)
        {
            try
            {
                //accounts.Where(a=> a.Player == null)
                foreach(var a in accounts.Where(ac=> ac.Player == null))
                {
                    a.Player = new Player { Name = a.UserName };
                }
                await _service.ImportAsync(accounts);
                return Ok("Successfully.");
            } catch
            {
                return InternalServerError();
            }
        }
    }
}
