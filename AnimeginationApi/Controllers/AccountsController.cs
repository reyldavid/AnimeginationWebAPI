using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AnimeginationApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using AnimeginationApi.Services;
using AnimeginationApi.Filters;
using Swashbuckle.Swagger.Annotations;

namespace AnimeginationApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Route("users")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("GetUsers")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public IHttpActionResult GetUsers()
        {            
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.UserModelFactory.Create(u)));
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("GetUser")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.UserModelFactory.Create(user));
            }
            return NotFound();
        }

        [Route("user/{username}")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("GetUserByName")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.UserModelFactory.Create(user));
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("Login")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> Login([FromBody]AccountModel loginModel)
        {
            var result = await this.AppSignInManager.PasswordSignInAsync(
                userName: loginModel.Username, 
                password: loginModel.Password,
                isPersistent: false, shouldLockout: false);
            
            if (result == Microsoft.AspNet.Identity.Owin.SignInStatus.Success)
            {
                var user = await this.AppUserManager.FindByNameAsync(loginModel.Username);

                if (user != null)
                {
                    ClaimModel claim = new ClaimModel()
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        UserId = user.Id,
                        Roles = RolesManager.UserRolesNames(user.Roles.ToList())
                    };

                    TokenModel token = new TokenModel();
                    var encryptedToken = TokenManager.CreateToken(claim);
                    token.token = encryptedToken;

                    return Ok(token);
                }
                return NotFound();
            }
            return NotFound();
        }

        [Route("create")]
        [SwaggerOperation("CreateUser")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> CreateUser(AccountModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            string[] adminUsersArray = "AdminUserNames".GetConfigurationValue().Split(',');
            List<string> adminUsers = adminUsersArray.ToList();

            string initialRole = adminUsers.Contains(user.UserName) ? "Admin" : 
                string.IsNullOrEmpty(createUserModel.RoleName) ? "Customer" : 
                createUserModel.RoleName;
            var roleResult = AppUserManager.AddToRole(user.Id, initialRole);

            string code = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

            await AppUserManager.SendEmailAsync(user.Id, "Confirm your account", 
                "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, UserModelFactory.Create(user));
        }
        
        [HttpGet]
        [Route("ConfirmEmail", Name ="ConfirmEmailRoute")]
        [SwaggerOperation("ConfirmEmail")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }


        [Route("ChangePassword")]
        [JwtTokenFilter]
        [SwaggerOperation("ChangePassword")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(
                User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        [HttpDelete]
        [Route("user/{id:guid}")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteUser")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            // Delete AspnetUsers Record
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                // Now, Delete UserAccounts Record
                AnimeDB db = new AnimeDB();

                UserAccount userAccount = await db.UserAccounts.FindAsync(id);

                if (userAccount == null)
                {
                    return NotFound();
                }
                db.UserAccounts.Remove(userAccount);
                await db.SaveChangesAsync();

                return Ok();
            }
            return NotFound();
        }
    }
}
