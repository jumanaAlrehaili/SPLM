using Application.Shared.DTOs.Holiday;
using MediatR;

namespace Application.Holidays.Commands.CreateHoliday;

public record CreateHolidayCommand(CreateHolidayInput Input) : IRequest<HolidayOutput>;
