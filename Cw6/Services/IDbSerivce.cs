using Cw6.DTOs;

namespace Cw6.Services;

public interface IDbService
{
    Task<IEnumerable<PatientGetDto>> GetPatientsAsync(string? search);
    Task AssignBedAsync(string pesel, CreateBedAssignmentDto dto);
}