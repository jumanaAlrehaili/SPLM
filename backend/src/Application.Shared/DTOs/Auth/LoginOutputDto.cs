using Application.Shared.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Dtos.Auth
{
    public class LoginOutputDto
    {
        public bool Success { get; set; }
        public bool IsLockedOut { get; set; }
        public UserDto User { get; set; }
    }
}
