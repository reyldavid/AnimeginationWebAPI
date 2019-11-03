using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using AnimeginationApi.Services;
using Swashbuckle.Swagger.Annotations;
using System.Net.Http;
using System.Threading.Tasks;
using AnimeginationApi.Filters;
using Microsoft.AspNet.Identity;

namespace AnimeginationApi.Controllers
{
    public class UserAccountsController : BaseApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/UserAccounts
        [Route("api/useraccounts/all")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("GetUserAccountsAll")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public IEnumerable<object> GetUserAccountsAll()
        {

            // var userFeedbacks = db.UserFeedbacks
            //     .Join(db.UserAccounts,
            //         feed => feed.UserId,
            //         user => user.UserId,
            //         (feed, user) => new {feed, user})
            //     .Join(db.Products,
            //         fu => fu.feed.ProductId,
            //         prod => prod.ProductID,
            //         (fu, prod) => new {fu, prod })

            var userAccounts = db.UserAccounts
                .Join(db.AspnetUsers,
                    a => a.UserId,
                    u => u.ID,
                    (a, u) => new {a, u})
                .Select(ua => new UserAccountModel
                {
                    UserId = ua.a.UserId,
                    UserName = ua.u.UserName,
                    FirstName = ua.a.FirstName,
                    LastName = ua.a.LastName,
                    Address = ua.a.StreetAddress,
                    City = ua.a.City,
                    State = ua.a.State.StateCode,
                    StateId = ua.a.StateId,
                    ZipCode = ua.a.ZipCode,
                    CellPhone = ua.a.CellPhoneNumber,
                    HomePhone = ua.a.HomePhoneNumber,
                    Email = ua.a.EmailAddress,
                    Created = ua.a.Created,
                    CreditCardType = ua.a.CreditCardType,
                    CreditCardNumber = ua.a.CreditCardNumber,
                    CreditCardExpiration = ua.a.CreditCardExpiration
                }).AsEnumerable();

            return userAccounts;
        }

        // GET: api/UserAccounts/Guid
        [HttpGet]
        [Route("api/UserAccounts/id")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("GetUserAccounts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public object GetUserAccounts(string id)
        {
            var userAccount =
                db.UserAccounts.Select(ua => new
                {
                    UserId = ua.UserId,
                    FirstName = ua.FirstName,
                    LastName = ua.LastName,
                    Address = ua.StreetAddress,
                    City = ua.City,
                    State = ua.State.StateName,
                    ZipCode = ua.ZipCode,
                    CellPhone = ua.CellPhoneNumber,
                    HomePhone = ua.HomePhoneNumber,
                    Email = ua.EmailAddress,
                    Created = ua.Created,
                    CreditCardType = ua.CreditCardType,
                    CreditCardNumber = ua.CreditCardNumber,
                    CreditCardExpiration = ua.CreditCardExpiration
                })
                .Where(ua => ua.UserId == id);

            return userAccount;
        }

        // GET: api/UserAccounts
        [HttpGet]
        [Route("api/useraccounts")]
        [JwtTokenFilter]
        [SwaggerOperation("GetUserAccounts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public object GetUserAccounts()
        {
            string userId = Request.UserId();
            string userName = Request.UserName();

            var userAccount =
                db.UserAccounts.Select(ua => new
                {
                    UserId = ua.UserId,
                    UserName = userName,
                    FirstName = ua.FirstName,
                    LastName = ua.LastName,
                    Address = ua.StreetAddress,
                    City = ua.City,
                    State = ua.State.StateName,
                    ZipCode = ua.ZipCode,
                    CellPhone = ua.CellPhoneNumber,
                    HomePhone = ua.HomePhoneNumber,
                    Email = ua.EmailAddress,
                    Created = ua.Created,
                    CreditCardType = ua.CreditCardType,
                    CreditCardNumber = ua.CreditCardNumber,
                    CreditCardExpiration = ua.CreditCardExpiration
                })
                .Where(ua => ua.UserId == userId)
                .FirstOrDefault();

            return userAccount;
        }

        [HttpPut]
        [Route("api/useraccounts")]
        [SwaggerOperation("PutUserAccount")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PutUserAccount([FromBody] ApiUserAccount apiUserAccount)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Unauthorized);

            UserAccount userAccount = new UserAccount
            {
                UserId = apiUserAccount.UserId,
                FirstName = apiUserAccount.FirstName,
                LastName = apiUserAccount.LastName,
                StreetAddress = apiUserAccount.Address,
                City = apiUserAccount.City,
                StateId = apiUserAccount.StateId,
                ZipCode = apiUserAccount.ZipCode,
                CellPhoneNumber = apiUserAccount.CellPhone,
                HomePhoneNumber = apiUserAccount.HomePhone,
                EmailAddress = apiUserAccount.Email,
                Created = DateTime.Now,
                CreditCardType = apiUserAccount.CreditCardType,
                CreditCardNumber = apiUserAccount.CreditCardNumber,
                CreditCardExpiration = apiUserAccount.CreditCardExpiration
            };

            db.UserAccounts.Add(userAccount);
            db.SaveChanges();

            ClaimModel claim = new ClaimModel()
            {
                Email = apiUserAccount.Email,
                UserName = apiUserAccount.UserName,
                UserId = apiUserAccount.UserId,
                Roles = apiUserAccount.Roles
            };

            TokenModel token = new TokenModel();
            var encryptedToken = TokenManager.CreateToken(claim);
            token.token = encryptedToken;

            response = Request.CreateResponse(HttpStatusCode.OK, token);

            return response;
        }

        [HttpPost]
        [Route("api/useraccounts")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("PostUserAccount")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PostUserAccount([FromBody] ApiUserAccount apiUserAccount)
        {

            UserAccount account = db.UserAccounts.SingleOrDefault(ua => ua.UserId == apiUserAccount.UserId);

            if (account == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            account.FirstName = string.IsNullOrEmpty(apiUserAccount.FirstName) ? account.FirstName : apiUserAccount.FirstName;
            account.LastName = string.IsNullOrEmpty(apiUserAccount.LastName) ? account.LastName : apiUserAccount.LastName;
            account.EmailAddress = string.IsNullOrEmpty(apiUserAccount.Email) ? account.EmailAddress : apiUserAccount.Email;
            account.CellPhoneNumber = string.IsNullOrEmpty(apiUserAccount.CellPhone) ? account.CellPhoneNumber : apiUserAccount.CellPhone;
            account.HomePhoneNumber = string.IsNullOrEmpty(apiUserAccount.HomePhone) ? account.HomePhoneNumber : apiUserAccount.HomePhone;
            account.StreetAddress = string.IsNullOrEmpty(apiUserAccount.Address) ? account.StreetAddress : apiUserAccount.Address;
            account.City = string.IsNullOrEmpty(apiUserAccount.City) ? account.City : apiUserAccount.City;
            account.StateId = apiUserAccount.StateId == 0 ? account.StateId : apiUserAccount.StateId;
            account.ZipCode = string.IsNullOrEmpty(apiUserAccount.ZipCode) ? account.ZipCode : apiUserAccount.ZipCode;
            account.CreditCardType = string.IsNullOrEmpty(apiUserAccount.CreditCardType) ? account.CreditCardType : apiUserAccount.CreditCardType;
            account.CreditCardNumber = string.IsNullOrEmpty(apiUserAccount.CreditCardNumber) ? account.CreditCardNumber : apiUserAccount.CreditCardNumber;
            account.CreditCardExpiration = string.IsNullOrEmpty(apiUserAccount.CreditCardExpiration) ? account.CreditCardExpiration : apiUserAccount.CreditCardExpiration;

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, account);
        }

        [HttpPost]
        [Route("api/useraccounts/names")]
        [JwtTokenFilter]
        [SwaggerOperation("PostUserAccountNames")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PostUserAccountNames([FromBody] ApiUserAccount apiUserAccount)
        {
            string userId = Request.UserId();

            if (!userId.Equals(apiUserAccount.UserId))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            UserAccount account = db.UserAccounts.SingleOrDefault(ua => ua.UserId == apiUserAccount.UserId);

            if (account == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            account.FirstName = string.IsNullOrEmpty(apiUserAccount.FirstName) ? account.FirstName : apiUserAccount.FirstName;
            account.LastName = string.IsNullOrEmpty(apiUserAccount.LastName) ? account.LastName : apiUserAccount.LastName;
            account.EmailAddress = string.IsNullOrEmpty(apiUserAccount.Email) ? account.EmailAddress : apiUserAccount.Email;
            account.CellPhoneNumber = string.IsNullOrEmpty(apiUserAccount.CellPhone) ? account.CellPhoneNumber : apiUserAccount.CellPhone;
            account.HomePhoneNumber = string.IsNullOrEmpty(apiUserAccount.HomePhone) ? account.HomePhoneNumber : apiUserAccount.HomePhone;

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, account);
        }

        [HttpPost]
        [Route("api/useraccounts/address")]
        [JwtTokenFilter]
        [SwaggerOperation("PostUserAccountAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PostUserAccountAddress([FromBody] ApiUserAccount apiUserAccount)
        {
            string userId = Request.UserId();

            if (!userId.Equals(apiUserAccount.UserId))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            UserAccount account = db.UserAccounts.SingleOrDefault(ua => ua.UserId == apiUserAccount.UserId);

            if (account == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            account.StreetAddress = string.IsNullOrEmpty(apiUserAccount.Address) ? account.StreetAddress : apiUserAccount.Address;
            account.City = string.IsNullOrEmpty(apiUserAccount.City) ? account.City : apiUserAccount.City;
            account.StateId = apiUserAccount.StateId == 0 ? account.StateId : apiUserAccount.StateId;
            account.ZipCode = string.IsNullOrEmpty(apiUserAccount.ZipCode) ? account.ZipCode : apiUserAccount.ZipCode;
            account.CreditCardType = string.IsNullOrEmpty(apiUserAccount.CreditCardType) ? account.CreditCardType : apiUserAccount.CreditCardType;
            account.CreditCardNumber = string.IsNullOrEmpty(apiUserAccount.CreditCardNumber) ? account.CreditCardNumber : apiUserAccount.CreditCardNumber;
            account.CreditCardExpiration = string.IsNullOrEmpty(apiUserAccount.CreditCardExpiration) ? account.CreditCardExpiration : apiUserAccount.CreditCardExpiration;

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, account);
        }


        // DELETE: api/useraccounts/guid
        [HttpDelete]
        [Route("api/useraccounts")]
        [AdminRoleFilter]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteUserAccounts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteUserAccount(string id)
        {
            // Delete User Account Record
            UserAccount userAccount = await db.UserAccounts.FindAsync(id);
            // UserAccount userAccount2 = db.UserAccounts.Where(ua => ua.UserId == id).FirstOrDefault();

            if (userAccount == null)
            {
                return NotFound();
            }

            db.UserAccounts.Remove(userAccount);
            await db.SaveChangesAsync();

            // Now, Delete AspnetUsers Record
            //var appUser = await this.AppUserManager.FindByIdAsync(id);

            //if (appUser != null)
            //{
            //    IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

            //    if (!result.Succeeded)
            //    {
            //        return GetErrorResult(result);
            //    }
            //}
            return Ok(userAccount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserAccountExists(string id)
        {
            return db.UserAccounts.Count(e => e.UserId == id) > 0;
        }
    }
}
