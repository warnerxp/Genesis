using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApiCore.Models;

namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {

        #region Variables

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public AccountController(
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        #endregion

        #region HttpMethods
        /// <summary>
        /// Allow search an User by token value
        /// </summary>
        /// <returns>Return complete information about the user</returns>
        [HttpGet]
        public IActionResult SearchUser()
        {
            try
            {
                string token = string.Empty;
                if (Request.Headers.ContainsKey("Authorization"))
                {

                    if (Request.Headers.TryGetValue("Authorization", out var tokenHeader))
                    {
                        token = tokenHeader.ToString().Replace("Bearer", "").Trim();
                        var resultTokenExists = _context.Tokens.FirstOrDefault(x => x.ValueToken == token);
                        if (resultTokenExists == null)
                        {
                            return Unauthorized();
                        }

                        var resultAccount = _context.Account
                            .Where(c => c.TokenId == resultTokenExists.GuidToken)
                            .OrderByDescending(t => t.LastLoginOn)
                            .FirstOrDefault();

                        //not the same token
                        if (resultAccount == null)
                        {
                            return Unauthorized();
                        }

                        if ((DateTime.Now - resultTokenExists.CreatedDate).Minutes > Convert.ToInt32(_configuration.GetSection("TokenExpiration").Value))
                        {
                            return BadRequest(new
                            {
                                Message = "Invalid Session"
                            });
                        }

                        AccountSecurity resultAccountSecurity = GetAccountSecurity(resultAccount.IdUser);

                        StringBuilder listPhones = new StringBuilder();
                        foreach (var item in resultAccountSecurity.Phones)
                        {
                            listPhones.AppendFormat("number:{0}, ", item.Value);
                        }
                        return Ok(new
                        {
                            idUser = resultAccount.IdUser,
                            name = resultAccountSecurity.Name,
                            email =  resultAccountSecurity.Email,
                            phones = listPhones.ToString(),
                            createdOn = resultAccount.CreatedOn,
                            lastUpdateOn = resultAccount.LastUpdateOn,
                            lastLoginOn = resultAccount.LastLoginOn,
                            token = resultTokenExists.ValueToken
                        });

                    }

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return BadRequest();
        }

        #endregion

        #region PrivateMethods

        private AccountSecurity GetAccountSecurity(Guid userId)
        {
            return _context.AccountSecurity.Include(y=>y.Phones).FirstOrDefault(x => x.UserId == userId);
        }

        #endregion
    }
}