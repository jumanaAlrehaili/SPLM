namespace Application.Shared.Dtos.Auth;

public record RegisterOutputDto(bool Succeeded, string Message,
                                string[] Errors, string[]? Roles = null);
