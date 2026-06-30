using Application.Shared.DTOs.Holiday;
using Application.Shared.Interfaces;
using Domain.Entities.Holidays;
using MediatR;

namespace Application.Holidays.Commands.CreateHoliday;

public class CreateHolidayHandler : IRequestHandler<CreateHolidayCommand, HolidayOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateHolidayHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<HolidayOutput> Handle(CreateHolidayCommand request, CancellationToken ct)
    {
        if (request.Input.EndDate < request.Input.StartDate)
            throw new InvalidOperationException("End date cannot be before start date.");

        var holiday = new Holiday
        {
            Name            = request.Input.Name,
            StartDate       = request.Input.StartDate,
            EndDate         = request.Input.EndDate,
            Year            = request.Input.StartDate.Year,
            CreatedAt       = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        _db.Holidays.Add(holiday);
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
