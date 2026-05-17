using MiniLibraryManager.Models;

namespace MiniLibraryManager.Services;

public interface ILibraryService
{
    void AddBook(Book book);
    List<Book> GetAllBooks();
    List<Book> SearchBooks(string keyword);
    void DeleteBook(int bookId);
    void EditBook(int bookId, string newTitle, string newAuthor, BookCategory newCategory);
    void RegisterMember(Member member);
    List<Member> GetAllMembers();
    void DeleteMember(int memberId);
    void EditMember(int memberId, string newName, string newEmail, string newPhone);
    bool BorrowBook(int bookId, int memberId);
    bool ReturnBook(int bookId);
    List<BorrowRecord> GetBorrowingHistory();
    void DeleteRecord(int recordId);
}