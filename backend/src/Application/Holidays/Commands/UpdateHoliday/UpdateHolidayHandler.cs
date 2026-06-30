using Application.Shared.DTOs.Holiday;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Holidays.Commands.UpdateHoliday;

public class UpdateHolidayHandler : IRequestHandler<UpdateHolidayCommand, HolidayOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateHolidayHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<HolidayOutput> Handle(UpdateHolidayCommand request, CancellationToken ct)
    {
        if (request.Input.EndDate < request.Input.StartDate)
            throw new InvalidOperationException("End date cannot be before start date.");

        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Holiday with ID {request.Id} not found.");

        holiday.Name            = request.Input.Name;
        holiday.StartDate       = request.Input.StartDate;
        holiday.EndDate         = request.Input.EndDate;
        holiday.Year            = request.Input.StartDate.Year;
        holiday.UpdatedAt       = DateTime.UtcNow;
        holiday.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        return new HolidayOutput
        {
            Id        = holiday.Id,
            Name      = holiday.Name,
            StartDate = holiday.StartDate,
            EndDate   = holiday.EndDate,
            Year      = holiday.Year
        };
    }
}
