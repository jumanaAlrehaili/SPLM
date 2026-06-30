using Application.Shared.DTOs.Feature;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Commands.SetEstimation
{
    public record SetEstimationCommand(int FeatureId, int StageId, SetEstimationInput Input) : IRequest;
}
