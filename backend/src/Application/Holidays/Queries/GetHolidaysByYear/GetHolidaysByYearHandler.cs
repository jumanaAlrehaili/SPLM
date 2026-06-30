using Application.Shared.DTOs.Holiday;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Holidays.Queries.GetHolidaysByYear;

public class GetHolidaysByYearHandler : IRequestHandler<GetHolidaysByYearQuery, IEnumerable<HolidayOutput>>
{
    private readonly IAppDbContext _db;

    public GetHolidaysByYearHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<HolidayOutput>> Handle(GetHolidaysByYearQuery request, CancellationToken ct)
    {
        return await _db.Holidays
            .AsNoTracking()
            .Where(h => h.Year == request.Year)
            .OrderBy(h => h.StartDate)
            .Select(h => new HolidayOutput
            {
                Id        = h.Id,
                Name      = h.Name,
                StartDate = h.StartDate,
                EndDate   = h.EndDate,
                Year      = h.Year
            })
            .ToListAsync(ct);
    }
}
