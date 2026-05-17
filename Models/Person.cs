namespace MiniLibraryManager.Models;

public abstract class Person
{
    public int Id { get; set; }
    public string FullName { get; set; }

    protected Person(int id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }

    public abstract void DisplayInfo();
}