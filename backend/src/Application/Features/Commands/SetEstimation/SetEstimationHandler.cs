using Application.Shared.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.SetEstimation
{
    public class SetEstimationHandler : IRequestHandler<SetEstimationCommand>
    {
        private readonly IAppDbContext _db;

        public SetEstimationHandler(IAppDbContext db) 
        { 
            _db = db; 
        }

        public async Task Handle(SetEstimationCommand request, CancellationToken ct)
        {
            var assignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId && fa.StageId == request.StageId, ct)
                ?? throw new KeyNotFoundException("Assignment not found.");

            if (assignment.StartedAt.HasValue)
                throw new InvalidOperationException("Cannot update estimation after starting the stage.");

            assignment.Estimation = request.Input.Estimation;
            assignment.EstimationUnitId = request.Input.EstimationUnitId;

            await _db.SaveChangesAsync(ct);
        }
    }
}
