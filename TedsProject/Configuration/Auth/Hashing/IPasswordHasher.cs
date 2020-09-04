using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Services
{
    public interface IPasswordHasher
    {
        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
        string Hash(string password);
    }
}
