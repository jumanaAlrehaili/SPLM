using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Auth
{
   public record ResetPasswordDto
   (
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
   ); 
}
