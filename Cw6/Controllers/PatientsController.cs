using Cw6.DTOs;
using Cw6.Exceptions;
using Cw6.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IDbService _dbService;

    public PatientsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search)
    {
        var patients = await _dbService.GetPatientsAsync(search);
        return Ok(patients);
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBed(string pesel, CreateBedAssignmentDto dto)
    {
        try
        {
            await _dbService.AssignBedAsync(pesel, dto);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}