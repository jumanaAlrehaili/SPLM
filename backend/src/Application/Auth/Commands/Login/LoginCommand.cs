using Application.Shared.Dtos.Auth;
using MediatR;

namespace Application.Auth.Commands.Login;

public record LoginCommand(LoginInputDto Input) : IRequest<LoginOutputDto>;