using MiniLibraryManager.Models;

namespace MiniLibraryManager.Services;

public class LibraryService : ILibraryService
{
    private List<Book> _books = new();
    private List<Member> _members = new();
    private List<BorrowRecord> _borrowRecords = new();

    private const string BooksFile = "Data/books.txt";
    private const string MembersFile = "Data/members.txt";
    private const string RecordsFile = "Data/borrow_records.txt";

    public LibraryService()
    {
        Directory.CreateDirectory("Data");
        LoadBooks();
        LoadMembers();
        LoadBorrowRecords();
    }

    public void AddBook(Book book)
    {
        if (_books.Any(b => b.Id == book.Id))
            throw new InvalidOperationException($"A book with ID {book.Id} already exists.");

        _books.Add(book);
        SaveBooks();
    }

    public List<Book> GetAllBooks() => _books;

    public List<Book> SearchBooks(string keyword)
    {
        return _books
            .Where(b =>
                b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Category.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void DeleteBook(int bookId)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId) ?? throw new InvalidOperationException($"Book with ID {bookId} does not exist");
        if (!book.IsAvailable)
            throw new InvalidOperationException($"Cannot Delete book with '{book.Title}' because it is currently borrowed");
        _books.Remove(book);
        SaveBooks();
    }

    public void RegisterMember(Member member)
    {
        if (_members.Any(m => m.Id == member.Id))
            throw new InvalidOperationException($"A member with ID {member.Id} already exists.");

        _members.Add(member);
        SaveMembers();
    }

    public void EditMember(int memberId, string newName, string newEmail, string newPhone)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId) ?? throw new InvalidOperationException($"Member with ID {memberId} does not exist");

        member.FullName = newName;
        member.Email = newEmail;
        member.PhoneNumber = newPhone;

        SaveMembers();
    }

    public void EditBook(int bookId, string newTitle, string newAuthor, BookCategory newCategory)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId) ?? throw new InvalidOperationException($"Book with ID {bookId} does not exist.");

        if (!book.IsAvailable)
            throw new InvalidOperationException($"Cannot edit '{book.Title}' because it is currently borrowed");

        book.Title = newTitle;
        book.Author = newAuthor;
        book.Category = newCategory;

        SaveBooks();
    }


    public void DeleteRecord(int recordId)
    {
        var record = _borrowRecords.FirstOrDefault(r => r.RecordId == recordId) ?? throw new InvalidOperationException($"Record with ID {recordId} does not exist");

        if (!record.IsReturned)
        { 
            var book = _books.FirstOrDefault(b => b.Id == record.BookId);
            if (book != null)
                book.IsAvailable = true;
            SaveBooks();
        }
        _borrowRecords.Remove(record);
        SaveBorrowRecords();
    }

    public List<Member> GetAllMembers() => _members;

    public bool BorrowBook(int bookId, int memberId)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException($"Book with ID {bookId} does not exist.");

        if (_members.FirstOrDefault(m => m.Id == memberId) == null)
            throw new InvalidOperationException($"Member with ID {memberId} does not exist.");

        if (!book.IsAvailable)
            throw new InvalidOperationException($"Book '{book.Title}' is already borrowed.");

        int nextId = _borrowRecords.Count > 0 ? _borrowRecords.Max(r => r.RecordId) + 1 : 1;
        var record = new BorrowRecord(nextId, bookId, memberId);
        _borrowRecords.Add(record);

        book.IsAvailable = false;

        SaveBooks();
        SaveBorrowRecords();
        return true;
    }

    public void DeleteMember(int memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId) ?? throw new InvalidOperationException($"Member with ID {memberId} does not exist");
        bool hasActiveBorrow = _borrowRecords.Any(r => r.MemberId == memberId && !r.IsReturned);
        if (hasActiveBorrow) throw new InvalidOperationException($"Cannot delete '{member.FullName}' because they have active borrowed books");
        _members.Remove(member);
        SaveMembers();
    }

    public bool ReturnBook(int bookId)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException($"Book with ID {bookId} does not exist.");

        if (book.IsAvailable)
            throw new InvalidOperationException($"Book '{book.Title}' was not borrowed.");

        var record = _borrowRecords
            .LastOrDefault(r => r.BookId == bookId && !r.IsReturned)
            ?? throw new InvalidOperationException("No active borrow record found for this book.");

        record.ReturnDate = DateTime.Now;
        record.IsReturned = true;
        book.IsAvailable = true;

        SaveBooks();
        SaveBorrowRecords();
        return true;
    }

    public List<BorrowRecord> GetBorrowingHistory() => _borrowRecords;

    private void SaveBooks()
    {
        var lines = _books.Select(b =>
            $"{b.Id}|{b.Title}|{b.Author}|{b.PublishedYear}|{b.Category}|{b.IsAvailable}");
        File.WriteAllLines(BooksFile, lines);
    }

    private void LoadBooks()
    {
        if (!File.Exists(BooksFile)) return;

        foreach (var line in File.ReadAllLines(BooksFile))
        {
            var parts = line.Split('|');
            if (parts.Length != 6) continue;

            if (!int.TryParse(parts[0], out int id)) continue;
            if (!int.TryParse(parts[3], out int year)) continue;
            if (!Enum.TryParse<BookCategory>(parts[4], out var category)) continue;
            if (!bool.TryParse(parts[5], out bool isAvailable)) continue;

            var book = new Book(id, parts[1], parts[2], year, category)
            {
                IsAvailable = isAvailable
            };
            _books.Add(book);
        }
    }

    private void SaveMembers()
    {
        var lines = _members.Select(m =>
            $"{m.Id}|{m.FullName}|{m.Email}|{m.PhoneNumber}|{m.RegistrationDate:yyyy-MM-dd}");
        File.WriteAllLines(MembersFile, lines);
    }

    private void LoadMembers()
    {
        if (!File.Exists(MembersFile)) return;

        foreach (var line in File.ReadAllLines(MembersFile))
        {
            var parts = line.Split('|');
            if (parts.Length != 5) continue;

            if (!int.TryParse(parts[0], out int id)) continue;
            if (!DateTime.TryParse(parts[4], out var regDate)) continue;

            var member = new Member(id, parts[1], parts[2], parts[3])
            {
                RegistrationDate = regDate
            };
            _members.Add(member);
        }
    }
    private void SaveBorrowRecords()
    {
        var lines = _borrowRecords.Select(r =>
            $"{r.RecordId}|{r.BookId}|{r.MemberId}|{r.BorrowDate:yyyy-MM-dd}|{r.ReturnDate?.ToString("yyyy-MM-dd") ?? ""}|{r.IsReturned}");
        File.WriteAllLines(RecordsFile, lines);
    }

    private void LoadBorrowRecords()
    {
        if (!File.Exists(RecordsFile)) return;

        foreach (var line in File.ReadAllLines(RecordsFile))
        {
            var parts = line.Split('|');
            if (parts.Length != 6) continue;

            if (!int.TryParse(parts[0], out int recordId)) continue;
            if (!int.TryParse(parts[1], out int bookId)) continue;
            if (!int.TryParse(parts[2], out int memberId)) continue;
            if (!DateTime.TryParse(parts[3], out var borrowDate)) continue;
            if (!bool.TryParse(parts[5], out bool isReturned)) continue;

            var record = new BorrowRecord(recordId, bookId, memberId)
            {
                BorrowDate = borrowDate,
                IsReturned = isReturned,
                ReturnDate = DateTime.TryParse(parts[4], out var returnDate) ? returnDate : null
            };
            _borrowRecords.Add(record);
        }
    }
}