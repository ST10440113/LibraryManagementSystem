using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index(string filter = "all")
        {
            try
            {
               var books = BookDataStore.GetAllBooks();
                ViewBag.Filter = filter;
                books = filter.ToLower() switch
                {
                    "pending" => BookDataStore.GetBooksByStatus(BookStatus.Pending),
                    "approved" => BookDataStore.GetBooksByStatus(BookStatus.Approved),
                    "declined" => BookDataStore.GetBooksByStatus(BookStatus.Declined),
                    _ => books
                };
                ViewBag.PendingCount = BookDataStore.GetBooksByStatus(BookStatus.Pending).Count;
                ViewBag.ApprovedCount = BookDataStore.GetBooksByStatus(BookStatus.Approved).Count;
                ViewBag.DeclinedCount = BookDataStore.GetBooksByStatus(BookStatus.Declined).Count;

                return View(books);
            }
            catch (Exception ex)
            {
               ViewBag.Error = "Unable to load books";
              return View(new List<Book>());
            }


        }


        public IActionResult Review(int id)
        {
            try
            {
                var book = BookDataStore.GetBookById(id);
                if (book == null)
                {
                    TempData["Error"] = "Book not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading book.";
                return RedirectToAction(nameof(Index));
            }
        }
        // POST: /Admin/Approve - CREATES REVIEW RECORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? comments)
        {
            try
            {
                string reviewedBy = "Admin User";
                string reviewComments = string.IsNullOrWhiteSpace(comments)
                    ? "Approved for library collection"
                    : comments;

                var success = BookDataStore.UpdateBookStatus(id, BookStatus.Approved, reviewedBy, reviewComments);

                if (success)
                {
                    TempData["Success"] = "Book approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Book not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error approving book.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Admin/Decline - CREATES REVIEW RECORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decline(int id, string? comments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for declining.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                string reviewedBy = "Admin User";
                var success = BookDataStore.UpdateBookStatus(id, BookStatus.Declined, reviewedBy, comments);

                if (success)
                {
                    TempData["Success"] = "Book declined.";
                }
                else
                {
                    TempData["Error"] = "Book not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error declining book.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
   