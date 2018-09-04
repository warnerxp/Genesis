using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiCore.Miscellaneous;
using WebApiCore.Models;
using System.Net.Http;

namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Security")]
    public class SecurityController : Controller
    {

        #region Variables

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private Account account;
        private JwtSecurityToken token;

        #endregion

        #region Constructor

        public SecurityController(
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        #endregion

        #region HttpMethods

        /// <summary>
        /// Register a new user on the system.
        /// </summary>
        /// <param name="model">string name,string email,string password,list phones. </param>
        /// <returns>Return information about the user recently created.  </returns>
        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] AccountSecurity model)
        {
            if (ModelState.IsValid)
            {
                if (VerifyUniqueEmail(model.Email))
                {
                    return BadRequest(new
                    {
                        Message = "Email already exists"
                    });

                }
                model.UserId = Guid.NewGuid();
                model.Password = PasswordUseFull.GetHash(model.Password);

                for (int j = 0; j < model.Phones.Count; j++)
                {
                    model.Phones[j].UserId = model.UserId;
                }

                var createduser = _context.AccountSecurity.Add(model);

                if (createduser == null)
                    return BadRequest(new
                    {
                        Message = "Username or password invalid"
                    });

                _context.SaveChanges();
                account = new Account()
                {
                    CreatedOn = DateTime.Now,
                    IdUser = model.UserId,
                    LastLoginOn = DateTime.Now,
                    LastUpdateOn = DateTime.Now

                };

                return BuildSignUpResponse(model);

            }

            return BadRequest(ModelState);

        }

        /// <summary>
        /// Receive an object with email and password.
        /// </summary>
        /// <param name="model">email and password.</param>
        /// <returns>Return the following values id, createdOn, lastUpdatedOn, lastLoginOn and token.</returns>
        [Route("SignIn")]
        [HttpPost]
        public IActionResult SignIn([FromBody] AccountSecurity model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultAccountSecurity = _context.AccountSecurity.FirstOrDefault(x => x.Email == model.Email);

                    if (resultAccountSecurity?.Email == null)
                        return BadRequest(new
                        {
                            Message = "Invalid user and / or password"
                        });

                    bool isValid = PasswordUseFull.ValidatePass(model.Password, resultAccountSecurity.Password);

                    if (!isValid)
                    {
                        return Unauthorized();
                    }
                    var resultAccount = _context.Account
                        .Where(c => c.IdUser == resultAccountSecurity.UserId)
                        .OrderByDescending(t => t.LastLoginOn)
                        .FirstOrDefault();

                    var token = BuildToken(model);

                    Token tokenInfo = new Token()
                    {
                        CreatedDate = DateTime.Now,
                        GuidToken = new Guid(token.Id),
                        ValueToken = new JwtSecurityTokenHandler().WriteToken(token)
                    };
                    if (resultAccount != null)
                    {
                        Account account = new Account()
                        {
                            IdUser = resultAccountSecurity.UserId,
                            CreatedOn = resultAccount.CreatedOn,
                            LastLoginOn = DateTime.Now,
                            LastUpdateOn = DateTime.Now,
                            TokenId = new Guid(token.Id)

                        };

                        _context.Account.Add(account);
                        _context.Tokens.Add(tokenInfo);
                        _context.SaveChanges();

                    }

                    if (resultAccount != null)
                        return Ok(new
                        {
                            idUser = resultAccountSecurity.UserId,
                            createdOn = resultAccount.CreatedOn,
                            lastUpdateOn = resultAccount.LastUpdateOn,
                            lastloginOn = resultAccount.LastLoginOn,
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        });

                    return BadRequest(new
                    {
                        Message = "Invalid user and / or password"
                    });
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        #endregion

        #region PrivateMethods

        private bool VerifyUniqueEmail(string email)
        {
            return _context.AccountSecurity.Any(x => x.Email == email);
        }

        private IActionResult BuildSignUpResponse(AccountSecurity userInfo)
        {
            try
            {
                var token = BuildToken(userInfo);
                account.TokenId = new Guid(token.Id);
                _context.Account.Add(account);
                _context.SaveChanges();
                TokenInfo(token);

                return Ok(new
                {
                    userId = account.IdUser,
                    creationOn = account.CreatedOn,
                    lastUpdateOn = account.LastUpdateOn,
                    lastLoginOn = account.LastLoginOn,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private JwtSecurityToken BuildToken(AccountSecurity userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("TokenExpiration").Value));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            return token;
        }

        private void TokenInfo(JwtSecurityToken token)
        {
            Token tokenInfo = new Token()
            {
                GuidToken = new Guid(token.Id),
                ValueToken = new JwtSecurityTokenHandler().WriteToken(token),
                CreatedDate = DateTime.Now
            };
            _context.Tokens.Add(tokenInfo);
            _context.SaveChanges();
        }

        #endregion

    }
}