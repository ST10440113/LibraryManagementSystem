namespace LibraryManagementSystem.Models
{
    public class BookReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewerRole { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public BookStatus Decision { get; set; }
        public string Comments { get; set; } = string.Empty;
        //public string ReviewType { get; set; } = string.Empty; // "Initial", "Re-review"
    }
}
