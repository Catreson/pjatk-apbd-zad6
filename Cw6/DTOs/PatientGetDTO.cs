namespace Cw6.DTOs;

public class PatientGetDto
{
    public string Pesel { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Sex { get; set; } = string.Empty;
    public IEnumerable<PatientAdmissionDto> Admissions { get; set; } = [];
    public IEnumerable<PatientBedAssignmentDto> BedAssignments { get; set; } = [];
}

public class PatientAdmissionDto
{
    public int Id { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public WardDto Ward { get; set; } = new();
}

public class PatientBedAssignmentDto
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public BedDto Bed { get; set; } = new();
}

public class BedDto
{
    public int Id { get; set; }
    public BedTypeDto BedType { get; set; } = new();
    public RoomDto Room { get; set; } = new();
}

public class BedTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RoomDto
{
    public string Id { get; set; } = string.Empty;
    public bool HasTv { get; set; }
    public WardDto Ward { get; set; } = new();
}

public class WardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}