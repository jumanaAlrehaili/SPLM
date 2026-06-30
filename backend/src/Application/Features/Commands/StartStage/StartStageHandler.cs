using Application.Shared.Interfaces;
using Domain.Entities.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Commands.StartStage
{
    public class StartStageHandler : IRequestHandler<StartStageCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public StartStageHandler(IAppDbContext db, ICurrentUserService currentUser) 
        { 
            _db = db; 
            _currentUser = currentUser; 
        }

        public async Task Handle(StartStageCommand request, CancellationToken ct)
        {
            var assignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId && fa.StageId == request.StageId, ct)
                ?? throw new KeyNotFoundException("Assignment not found.");

            if (assignment.AssignedUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException("Only the assigned user can start this stage.");

            if (assignment.StartedAt.HasValue)
                throw new InvalidOperationException("Stage already started.");

            assignment.StartedAt = DateTime.UtcNow;

            _db.FeatureStageLogs.Add(new FeatureStageLog
            {
                FeatureId = request.FeatureId,
                StageId = request.StageId,
                UserId = _currentUser.UserId,
                Action = StageActions.Started,
                Comment = "Stage started by user.",
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
        }
    }
}
