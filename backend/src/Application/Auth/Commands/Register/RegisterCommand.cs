using Application.Shared.Dtos.Auth;
using MediatR;

namespace Application.Auth.Commands.Register;

public record RegisterCommand(RegisterInputDto Input) : IRequest<RegisterOutputDto>;