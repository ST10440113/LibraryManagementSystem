using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        public readonly IWebHostEnvironment _environment;
        public readonly FileEncryptionService _encryptionService;

        public BooksController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _encryptionService = new FileEncryptionService();
        }
        public IActionResult Index()
        {
            try
            {
                var books = BookDataStore.GetAllBooks();
                return View(books);
            }
            catch (Exception ex)
            {
                //viewbag is like a little bag that you send to the views where they can dig in and get stuff out
                ViewBag.Error = "Unable to load books at this time. Please try again later.";
                return View(new List<Book>());
            }
           
        }


        [HttpGet]public IActionResult Add()
        {

            return View();
        }


        //POST: /Books/Add - Add form data to the datastore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Book book, List<IFormFile> documents)
        {
            try
            {
                if (string.IsNullOrEmpty(book.Title))
                {
                    ViewBag.Error = "Book title is required";
                    return View(book);
                }

                if (string.IsNullOrEmpty(book.Author))
                {
                    ViewBag.Error = "Book author is required";
                    return View(book);
                }

                if (documents != null && documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".xlsx" };
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ViewBag.Error = $"File extension {extension} not allowed";
                                return View(book);
                            }

                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                            var encryptedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = file.OpenReadStream())
                            {
                                await _encryptionService.EncryptFileAsync(fileStream, encryptedFilePath);
                            }

                            book.Documents.Add(new UploadedDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                FileSize = file.Length,
                                IsEncrypted = true
                            });

                        }
                    }
                }

                BookDataStore.AddBook(book);
                TempData["Success"] = "Book added successfully!";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                //viewbag is a dictionary
                ViewBag["Error"] = "Error adding book. " + ex.Message;
                return View(book);
            }

        }

        public IActionResult Details(int id)
        {
            try
            {
                var books = BookDataStore.GetBookById(id);
                if (books == null)
                {
                    TempData["Error"] = "Book not found.";
                    return View();
                }
                  return View(books);

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading book";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> DownloadDocument(int bookId, int docId)
        {
            try
            {
                var book = BookDataStore.GetBookById(bookId);
                if (book == null) { return NotFound("Book not found."); }

                var document = book.Documents.FirstOrDefault(doc => doc.Id == docId);
                if (document == null) { return NotFound("Document not found."); }

                var encryptedFilePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (!System.IO.File.Exists(encryptedFilePath)) return NotFound("File not found;");

                var decryptedStream = await _encryptionService.DecryptFileAsync(encryptedFilePath);

                var contentType = Path.GetExtension(document.FileName).ToLower()
                    switch
                {
                    ".pdf" => "application/pdf",
                    ".txt" => "application/txt",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                return File(decryptedStream, contentType, document.FileName);

            }
            catch (Exception ex)
            {
                return BadRequest("Error downloading file: " + ex.Message);
            }
        }


    }
}
