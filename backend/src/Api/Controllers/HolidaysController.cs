using Application.Shared.DTOs.Holiday;
using Application.Holidays.Commands.CreateHoliday;
using Application.Holidays.Commands.UpdateHoliday;
using Application.Holidays.Commands.DeleteHoliday;
using Application.Holidays.Queries.GetHolidaysByYear;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/holidays")]
    public class HolidaysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HolidaysController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetHolidaysByYear/{year}", Name = "GetHolidaysByYear")]
        public async Task<IActionResult> GetHolidaysByYear([FromRoute] int year, CancellationToken ct)
        {
            try
            {
                var holidays = await _mediator.Send(new GetHolidaysByYearQuery(year), ct);
                return Ok(holidays);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("CreateHoliday", Name = "CreateHoliday")]
        public async Task<IActionResult> CreateHoliday(CreateHolidayInput input, CancellationToken ct)
        {
            try
            {
                var holiday = await _mediator.Send(new CreateHolidayCommand(input), ct);
                return CreatedAtAction(nameof(GetHolidaysByYear), new { year = holiday.Year }, holiday);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("UpdateHoliday/{id}", Name = "UpdateHoliday")]
        public async Task<IActionResult> UpdateHoliday([FromRoute] int id, UpdateHolidayInput input, CancellationToken ct)
        {
            try
            {
                var holiday = await _mediator.Send(new UpdateHolidayCommand(id, input), ct);
                return Ok(holiday);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("DeleteHoliday/{id}", Name = "DeleteHoliday")]
        public async Task<IActionResult> DeleteHoliday([FromRoute] int id, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteHolidayCommand(id), ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
