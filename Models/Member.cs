namespace MiniLibraryManager.Models;

public class Member : Person, ISearchable
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime RegistrationDate { get; set; }

    public Member(int id, string fullName, string email, string phoneNumber)
        : base(id, fullName)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        RegistrationDate = DateTime.Now;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Name: {FullName}");
        Console.WriteLine($"Email: {Email}");
        Console.WriteLine($"Phone: {PhoneNumber}");
        Console.WriteLine($"Registered: {RegistrationDate:yyyy-MM-dd}");
    }
}