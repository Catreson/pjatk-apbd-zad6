using Cw6.Data;
using Cw6.DTOs;
using Cw6.Exceptions;
using Cw6.Models;
using Microsoft.EntityFrameworkCore;

namespace Cw6.Services;

public class DbService : IDbService
{
    private readonly HospitalContext _context;

    public DbService(HospitalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PatientGetDto>> GetPatientsAsync(string? search)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));
        }

        return await query
            .Select(p => new PatientGetDto
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",
                Admissions = p.Admissions.Select(a => new PatientAdmissionDto
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardDto
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }),
                BedAssignments = p.BedAssignments.Select(ba => new PatientBedAssignmentDto
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedDto
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeDto
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new RoomDto
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardDto
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                })
            })
            .ToListAsync();
    }

    public async Task AssignBedAsync(string pesel, CreateBedAssignmentDto dto)
    {
        var patientExists = await _context.Patients.AnyAsync(p => p.Pesel == pesel);
        if (!patientExists)
            throw new NotFoundException($"Pacjent z peselem '{pesel}' nie został znaleziony.");

        var ward = await _context.Wards.FirstOrDefaultAsync(w => w.Name == dto.Ward);
        if (ward is null)
            throw new NotFoundException($"Oddział '{dto.Ward}' nie został znaleziony.");

        var bedType = await _context.BedTypes.FirstOrDefaultAsync(bt => bt.Name == dto.BedType);
        if (bedType is null)
            throw new NotFoundException($"Łóżko typu '{dto.BedType}' nie zostało znalezione.");

        var availableBed = await _context.Beds
            .Where(b => b.Room.WardId == ward.Id && b.BedTypeId == bedType.Id)
            .Where(b => !b.BedAssignments.Any(ba =>
                (ba.To == null || dto.From < ba.To) &&
                (dto.To == null || ba.From < dto.To)))
            .FirstOrDefaultAsync();

        if (availableBed is null)
            throw new NotFoundException(
                $"Brak dostępnego łóżka typu '{dto.BedType}' w oddziale '{dto.Ward}' dla zadanego okna czasowego.");

        var assignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = dto.From,
            To = dto.To
        };

        await _context.BedAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }
}