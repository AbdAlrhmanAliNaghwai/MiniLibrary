namespace MiniLibraryManager.Models;

public enum BookCategory
{
    Programming,
    Science,
    History,
    Novel,
    Other
}

public class Book : ISearchable
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublishedYear { get; set; }
    public BookCategory Category { get; set; }
    public bool IsAvailable { get; set; }

    public Book(int id, string title, string author, int publishedYear, BookCategory category)
    {
        Id = id;
        Title = title;
        Author = author;
        PublishedYear = publishedYear;
        Category = category;
        IsAvailable = true;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Title: {Title}");
        Console.WriteLine($"Author: {Author}");
        Console.WriteLine($"Year: {PublishedYear}");
        Console.WriteLine($"Category: {Category}");
        Console.WriteLine($"Status: {(IsAvailable ? "Available" : "Borrowed")}");
    }
}