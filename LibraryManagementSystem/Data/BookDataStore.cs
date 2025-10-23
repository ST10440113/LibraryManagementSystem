using AspNetCoreGeneratedDocument;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class BookDataStore
    {
        public static  List<Book> _books = new List<Book>
        {
            new Book
            {
                Id = 1,
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Category = "Fiction",
                ISBN = "978-074-3273",
                Description = "A novel set in the Jazz Age that tells the story of Jay Gatsby's unrequited love for Daisy Buchanan.",
                SubmittedDate = DateTime.Now.AddDays(-5), //fake that it was added 5 days ago
                Status = BookStatus.Pending,
                Documents = new List<UploadedDocument>()

            },
            new Book
            {
                Id = 2,
                Title = "1984",
                Author = "George Orwell",
                Category = "Dystopian",
                ISBN = "978-041-5249",
                Description = "A dystopian novel that explores the dangers of totalitarianism and extreme political ideology.",
                SubmittedDate = DateTime.Now.AddDays(-3),
                Status = BookStatus.Approved,
                Documents = new List<UploadedDocument>()

            },
            new Book
            {
                Id = 3,
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                Category = "Fiction",
                ISBN = "978-006-1120",
                Description = "A novel that addresses serious issues such as racial inequality and moral growth through the eyes of a young boy.",
                SubmittedDate = DateTime.Now.AddDays(-4),
                Status = BookStatus.Declined,
                Documents = new List<UploadedDocument>()


            }
        };
        private static int _nextId = 4;
        private static int _nextReviewId = 1;
        public static List<Book>GetAllBooks() => _books.ToList();

        public static Book?GetBookById(int id) => _books.FirstOrDefault(b => b.Id == id);

        public static List<Book>GetBooksByStatus(BookStatus status) => _books.Where(b => b.Status == status).ToList();

        public static void AddBook(Book book)
        {
            book.Id = _nextId;   // you wont need this for the database
            _nextId++;
            book.SubmittedDate = DateTime.Now;
            book.Status = BookStatus.Pending;
            _books.Add(book);
        }

        public static bool UpdateBookStatus(int id, BookStatus newStatus, string reviewedBy, string comments)
        {
            var book = GetBookById(id);
            if (book == null) return false;

            // CREATE REVIEW RECORD
            var review = new BookReview
            {
                Id = _nextReviewId,
                BookId = id,
                ReviewerName = reviewedBy,
                ReviewerRole = "Administrator",
                ReviewDate = DateTime.Now,
                Decision = newStatus,
                Comments = comments
            };
            _nextReviewId++;

            book.Reviews.Add(review);

            // UPDATE BOOK STATUS
            book.Status = newStatus;
            book.ReviewedBy = reviewedBy;
            book.ReviewedDate = DateTime.Now;

            return true;
        }

        public static int GetPendingCount() => _books.Count(b => b.Status == BookStatus.Pending);

        public static int GetApprovedCount() => _books.Count(b => b.Status == BookStatus.Approved);

        public static int GetDeclinedCount() => _books.Count(b => b.Status == BookStatus.Declined);
    }
}
