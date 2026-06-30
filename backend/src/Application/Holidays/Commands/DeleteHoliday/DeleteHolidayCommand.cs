using MediatR;

namespace Application.Holidays.Commands.DeleteHoliday;

public record DeleteHolidayCommand(int Id) : IRequest;
