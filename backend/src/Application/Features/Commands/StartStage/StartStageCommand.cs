using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Commands.StartStage
{
    public record StartStageCommand(int FeatureId, int StageId) : IRequest;
}
