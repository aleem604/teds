﻿
using TedsProject.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TedsProject.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(UserModel userName, string apiKey);
        ClaimsIdentity GenerateClaimsIdentity(UserModel ususerInfo, string apiKey);
    }
}
