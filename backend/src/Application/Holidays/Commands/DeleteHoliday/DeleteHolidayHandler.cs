using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Holidays.Commands.DeleteHoliday;

public class DeleteHolidayHandler : IRequestHandler<DeleteHolidayCommand>
{
    private readonly IAppDbContext _db;

    public DeleteHolidayHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteHolidayCommand request, CancellationToken ct)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Holiday with ID {request.Id} not found.");

        _db.Holidays.Remove(holiday);
        await _db.SaveChangesAsync(ct);
    }
}
