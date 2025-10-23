namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string SubmittedBy { get; set; }

        public BookStatus Status { get; set; } 

        public string ReviewedBy { get; set; }
        public DateTime ReviewedDate { get; set; }

        public List<UploadedDocument> Documents { get; set; }= new List<UploadedDocument>();

        public List<BookReview> Reviews { get; set; } = new List<BookReview>();
    }
}
