public class StudentDTO
{
    public int StudentId { get; set; }
    public string? FullName { get; set; }

    public string? Address { get; set; }   
    public string? EmailAddress { get; set; }
    public string? PhoneNo { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Grade { get; set; }
    public string? Gender { get; set; }
   // public string Password { get; set; }

    public string? ImagePath { get; set; }   

    public IFormFile? StudentImage { get; set; }   
    public string? ParentName { get; set; }
    public string? ParentEmail { get; set; }
    public string? ParentPhone { get; set; }
}
