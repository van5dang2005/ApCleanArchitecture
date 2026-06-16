using Domain;
using System;
using System.Collections.Generic;
using System.Text;
namespace Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
