using Application.Shared.DTOs.Holiday;
using MediatR;

namespace Application.Holidays.Commands.UpdateHoliday;

public record UpdateHolidayCommand(int Id, UpdateHolidayInput Input) : IRequest<HolidayOutput>;
