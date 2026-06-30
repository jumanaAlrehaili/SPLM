using Application.Shared.DTOs.Holiday;
using MediatR;

namespace Application.Holidays.Queries.GetHolidaysByYear;

public record GetHolidaysByYearQuery(int Year) : IRequest<IEnumerable<HolidayOutput>>;
