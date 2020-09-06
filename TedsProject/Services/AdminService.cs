using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TedsProject.Auth;
using TedsProject.Helpers;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Services
{
    public class AdminService : IAdminService
    {
        private readonly IDbService _dbService;
        private readonly IKeysService _keysService;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AdminService(IDbService dbService, IPasswordHasher hasher, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IKeysService keysService)
        {
            _dbService = dbService;
            _hasher = hasher;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _keysService = keysService;
        }

        public async Task<(dynamic, string)> Login(LoginViewModel model)
        {
            try
            {
                var scanCodition = new List<ScanCondition> {
                     new ScanCondition("Email", ScanOperator.Equal, model.Email)
                };

                var result = await _dbService.GetAll<UserModel>(scanCodition);
                if (result != null && result.Count() > 0)
                {
                    var user = result.FirstOrDefault();
                    var isAuthorized = false;

                    if (_hasher.Check(user.Password, model.Password).Verified)
                        isAuthorized = true;

                    if (isAuthorized)
                    {
                        string apiKey = await GetUserAppKey(user.Id);
                        
                        var jwt = await Tokens.GenerateJwt(_jwtFactory, user, apiKey, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
                        return (JsonConvert.DeserializeObject(jwt), string.Empty);
                    }
                }

                return (null, "Invalid email or passsword");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }

        }

        private async Task<string> GetUserAppKey(string userId)
        {
            var scanCodition = new List<ScanCondition> {
                     new ScanCondition("UserId", ScanOperator.Equal, userId),
                     new ScanCondition("ExpiryDate", ScanOperator.GreaterThanOrEqual, DateTime.UtcNow),
                };

            var result = await _dbService.GetAll<KeysDataModel>(scanCodition) ?? new List<KeysDataModel>();

            return result.Select(s => new { s.Id, s.AppKey })?.FirstOrDefault()?.AppKey;
        }



        public async Task<(dynamic, string)> Register(SignupViewModel model)
        {
            try
            {
                var scanCodition = new List<ScanCondition> {
                     new ScanCondition("Email", ScanOperator.Equal, model.Email)
                };

                var result = await _dbService.GetAll<UserModel>(scanCodition);
                if (result != null && result.Any(x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return (null, Task.FromResult("Email is already taken.").Result);
                }

                var user = new UserModel
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = _hasher.Hash(model.Password),
                    CreateAt = DateTime.Now
                };

                await _dbService.Store<UserModel>(user);
                return (new { statusCode = HttpStatusCode.OK }, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(dynamic, string)> ChangePassword(ChangePasswordViewModel model, string userId)
        {
            try
            {
                var users = await _dbService.GetItemsById<UserModel>(userId);
                UserModel ruser = null;
                if (users != null && users.Count() > 0)
                {
                    ruser = users.FirstOrDefault();
                }

                if (ruser is null || ruser.Id != userId || (!ruser.Email.Equals(model.Email)))
                {
                    return (null, "invalid user");
                }

                if (!_hasher.Check(ruser.Password, model.Password).Verified)
                {
                    return (null, "invalid user");
                }

                ruser.Password = _hasher.Hash(model.NewPassword);
                ruser.UpdateDate = DateTime.Now;

                await _dbService.Store<UserModel>(ruser);

                return (HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }


    }
}
