using MiniLibraryManager.Models;
using MiniLibraryManager.Services;

var service = new LibraryService();
bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("===== Mini Library Manager =====");
    Console.WriteLine("1. Add Book");
    Console.WriteLine("2. View All Books");
    Console.WriteLine("3. Search Book");
    Console.WriteLine("4. Register Member");
    Console.WriteLine("5. View All Members");
    Console.WriteLine("6. Borrow Book");
    Console.WriteLine("7. Return Book");
    Console.WriteLine("8. View Borrowing History");
    Console.WriteLine("9. Remove Book");
    Console.WriteLine("10. Remove Member");
    Console.WriteLine("11. Remove Record");
    Console.WriteLine("12. Edit Member");
    Console.WriteLine("13. Edit Book");
    Console.WriteLine("14. Exit");
    Console.Write("Choose an option 1-14: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            AddBook();
            break;
        case "2":
            ViewAllBooks();
            break;
        case "3":
            SearchBooks();
            break;
        case "4":
            RegisterMember();
            break;
        case "5":
            ViewAllMembers();
            break;
        case "6":
            BorrowBook();
            break;
        case "7":
            ReturnBook();
            break;
        case "8":
            ViewBorrowingHistory();
            break;
        case "9":
            DeleteBook();
            break;
        case "10":
            DeleteMember();
            break;
        case "11":
            DeleteRecord();
            break;
        case "12":
            EditMember();
            break;
        case "13":
            EditBook();
            break;
        case "14":
            running = false;
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option. Please choose 1-14.");
            break;
    }
}

void DeleteRecord()
{
    Console.WriteLine("\n--- Remove Record ---");

    var records = service.GetBorrowingHistory();
    if (records.Count == 0)
    {
        Console.WriteLine("No Borrow Record Found");
        return;
    }
    var books = service.GetAllBooks();
    var members = service.GetAllMembers();

    foreach (var r in records)
    {
        Console.WriteLine(new string('-', 30));
        var book = books.FirstOrDefault(b => b.Id == r.BookId);
        var member = members.FirstOrDefault(m => m.Id == r.MemberId);
        Console.WriteLine($"Record ID: {r.RecordId}");
        Console.WriteLine($"Book: {book?.Title ?? "Unknown"}");
        Console.WriteLine($"Member: {member?.FullName ?? "Unknown"}");
        Console.WriteLine($"Borrowed: {r.BorrowDate:yyyy-MM-dd}");
        Console.WriteLine(r.IsReturned ? $"Returned: {r.ReturnDate:yyyy-MM-dd}" : "Status: Not yet returned");
    }
    Console.WriteLine(new string('-', 30));
    Console.Write("Enter Record ID to delete: ");

    if (!int.TryParse(Console.ReadLine(), out int recordId))
    {
        Console.WriteLine("Invalid ID. Must be a number.");
        return;
    }
    try
    {
        service.DeleteRecord(recordId);
        Console.WriteLine("Record Removed Successfully");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void DeleteBook()
{
    Console.WriteLine("\n--- Remove Book ---");

    Console.Write("Delete Book With ID: ");

    if (!int.TryParse(Console.ReadLine(), out int bookid))
    {
        Console.WriteLine("Must Be a Valid Book ID");
        return;
    }
    try
    {
        service.DeleteBook(bookid);
        Console.WriteLine("Book Removed Successfully");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void DeleteMember()
{
    Console.WriteLine("\n--- Remove Member ---");

    Console.Write("Delete Member With ID: ");

    if (!int.TryParse(Console.ReadLine(), out int memberId))
    {
        Console.WriteLine("Must Be a Valid Member ID");
        return;
    }
    try
    {
        service.DeleteMember(memberId);
        Console.WriteLine("Member Removed Successfully");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void AddBook()
{
    Console.WriteLine("\n--- Add New Book ---");

    Console.Write("Book ID: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Invalid ID. Must be a number.");
        return;
    }

    Console.Write("Title: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    Console.Write("Author: ");
    var author = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(author))
    {
        Console.WriteLine("Author cannot be empty.");
        return;
    }

    Console.Write("Published Year: ");
    if (!int.TryParse(Console.ReadLine(), out int year))
    {
        Console.WriteLine("Invalid year.");
        return;
    }

    Console.WriteLine("Categories: " + string.Join(", ", Enum.GetNames<BookCategory>()));
    Console.Write("Category: ");
    if (!Enum.TryParse<BookCategory>(Console.ReadLine(), true, out var category))
    {
        Console.WriteLine("Invalid category. Defaulting to Other.");
        category = BookCategory.Other;
    }

    try
    {
        var book = new Book(id, title, author, year, category);
        service.AddBook(book);
        Console.WriteLine("Book added successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ViewAllBooks()
{
    Console.WriteLine("\n--- All Books ---");
    var books = service.GetAllBooks();

    if (books.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    foreach (var book in books)
    {
        Console.WriteLine(new string('-', 30));
        book.DisplayInfo();
    }
}

void SearchBooks()
{
    Console.WriteLine("\n--- Search Books ---");
    Console.Write("Enter keyword (title, author, or category): ");
    var keyword = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(keyword))
    {
        Console.WriteLine("Keyword cannot be empty.");
        return;
    }

    var results = service.SearchBooks(keyword);

    if (results.Count == 0)
    {
        Console.WriteLine("No books found matching your search.");
        return;
    }

    Console.WriteLine($"\nFound {results.Count} result(s):");
    foreach (var book in results)
    {
        Console.WriteLine(new string('-', 30));
        book.DisplayInfo();
    }
}

void RegisterMember()
{
    Console.WriteLine("\n--- Register New Member ---");

    Console.Write("Member ID: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Invalid ID. Must be a number.");
        return;
    }

    Console.Write("Full Name: ");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Email: ");
    var email = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(email))
    {
        Console.WriteLine("Email cannot be empty.");
        return;
    }

    Console.Write("Phone Number: ");
    var phone = Console.ReadLine() ?? "";

    try
    {
        var member = new Member(id, name, email, phone);
        service.RegisterMember(member);
        Console.WriteLine("Member registered successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ViewAllMembers()
{
    Console.WriteLine("\n--- All Members ---");
    var members = service.GetAllMembers();

    if (members.Count == 0)
    {
        Console.WriteLine("No members registered.");
        return;
    }

    foreach (var member in members)
    {
        Console.WriteLine(new string('-', 30));
        member.DisplayInfo();
    }
}

void BorrowBook()
{
    Console.WriteLine("\n--- Borrow Book ---");

    Console.Write("Book ID: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID.");
        return;
    }

    Console.Write("Member ID: ");
    if (!int.TryParse(Console.ReadLine(), out int memberId))
    {
        Console.WriteLine("Invalid Member ID.");
        return;
    }

    try
    {
        service.BorrowBook(bookId, memberId);
        Console.WriteLine("Book borrowed successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ReturnBook()
{
    Console.WriteLine("\n--- Return Book ---");

    Console.Write("Book ID: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID.");
        return;
    }

    try
    {
        service.ReturnBook(bookId);
        Console.WriteLine("Book returned successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ViewBorrowingHistory()
{
    Console.WriteLine("\n--- Borrowing History ---");
    var records = service.GetBorrowingHistory();

    if (records.Count == 0)
    {
        Console.WriteLine("No borrowing history found.");
        return;
    }

    var books = service.GetAllBooks();
    var members = service.GetAllMembers();

    foreach (var record in records)
    {
        Console.WriteLine(new string('-', 30));
        var book = books.FirstOrDefault(b => b.Id == record.BookId);
        var member = members.FirstOrDefault(m => m.Id == record.MemberId);

        Console.WriteLine($"Record ID: {record.RecordId}");
        Console.WriteLine($"Book: {book?.Title ?? "Unknown"} (ID: {record.BookId})");
        Console.WriteLine($"Member: {member?.FullName ?? "Unknown"} (ID: {record.MemberId})");
        Console.WriteLine($"Borrowed: {record.BorrowDate:yyyy-MM-dd}");
        Console.WriteLine(record.IsReturned
            ? $"Returned: {record.ReturnDate:yyyy-MM-dd}"
            : "Status: Not yet returned");
    }
}

void EditBook()
{
    Console.WriteLine("\n--- Edit Book ---");

    Console.Write("Enter Book ID to edit: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid ID. Must be a number.");
        return;
    }

    var book = service.GetAllBooks().FirstOrDefault(b => b.Id == bookId);
    if (book == null)
    {
        Console.WriteLine("Book not found.");
        return;
    }

    if (!book.IsAvailable)
    {
        Console.WriteLine($"Cannot edit '{book.Title}' because it is currently borrowed.");
        return;
    }

    Console.WriteLine($"Current Title: {book.Title}");
    Console.Write("New Title (leave blank to keep current): ");
    var newTitle = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newTitle))
        newTitle = book.Title;

    Console.WriteLine($"Current Author: {book.Author}");
    Console.Write("New Author (leave blank to keep current): ");
    var newAuthor = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newAuthor))
        newAuthor = book.Author;

    Console.WriteLine($"Current Category: {book.Category}");
    Console.WriteLine("Categories: " + string.Join(", ", Enum.GetNames<BookCategory>()));
    Console.Write("New Category (leave blank to keep current): ");
    var categoryInput = Console.ReadLine();
    var newCategory = string.IsNullOrWhiteSpace(categoryInput)
        ? book.Category
        : Enum.TryParse<BookCategory>(categoryInput, true, out var parsed) ? parsed : book.Category;

    try
    {
        service.EditBook(bookId, newTitle, newAuthor, newCategory);
        Console.WriteLine("Book updated successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void EditMember()
{
    Console.WriteLine("\n--- Edit Member ---");

    Console.Write("Enter Member ID to edit: ");
    if (!int.TryParse(Console.ReadLine(), out int memberId))
    {
        Console.WriteLine("Invalid ID. Must be a number.");
        return;
    }

    var member = service.GetAllMembers().FirstOrDefault(m => m.Id == memberId);
    if (member == null)
    {
        Console.WriteLine("Member not found.");
        return;
    }

    Console.WriteLine($"Current Name: {member.FullName}");
    Console.Write("New Name (leave blank to keep current): ");
    var newName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newName))
        newName = member.FullName;

    Console.WriteLine($"Current Email: {member.Email}");
    Console.Write("New Email (leave blank to keep current): ");
    var newEmail = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newEmail))
        newEmail = member.Email;

    Console.WriteLine($"Current Phone: {member.PhoneNumber}");
    Console.Write("New Phone (leave blank to keep current): ");
    var newPhone = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newPhone))
        newPhone = member.PhoneNumber;

    try
    {
        service.EditMember(memberId, newName, newEmail, newPhone);
        Console.WriteLine("Member updated successfully.");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}