using Xunit;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using System.Text;
namespace LibraryManagementSystem.Tests
{
    public class LmsTest
    {
        [Fact]
        public void Test1_AddBook_Successful()   //expect boos status to be pending, expect the id to be one moe, expect the book id to be found in the data store
        {
            //Create a new book
            var initialCount = BookDataStore.GetAllBooks().Count;
            var newBook = new Book
            {
                Title = "Test Book for Add",
                Author = "Test Author",
                Category = "Fantasy",
                ISBN = "1234567890",
                Description = "This is a test book.",
                SubmittedDate = DateTime.Now,
                SubmittedBy = "Test User",

            };
            //perform the add operation
            BookDataStore.AddBook(newBook);

            //get the count and verify if the book was added
            var finalCount = BookDataStore.GetAllBooks().Count;
            Assert.Equal(initialCount + 1, finalCount);
            Assert.True(newBook.Id > 0, "Book should have an ID assigned");

            Assert.Equal(BookStatus.Pending, newBook.Status);

            //verify if we can retrive the book by its id
            var retrievedBook = BookDataStore.GetBookById(newBook.Id);
            Assert.NotNull(retrievedBook);
            Assert.Equal("Test Book for Add", retrievedBook.Title);
        }

        [Fact]
        public async Task Test2_EncryptionFile_Successful()
        {
            //encrypting file path
            var originalContent = "This is a secret file content that should be encrypted.";
            var originalBytes = System.Text.Encoding.UTF8.GetBytes(originalContent);
            var inputStream = new MemoryStream(originalBytes);
            var tempFile = Path.GetTempFileName();
            //create a variable of the encryption service 
            var encryptionService = new FileEncryptionService();

            // try to encrypt file

            try
            {
                //await from the file service and then call encryption method
                await encryptionService.EncryptFileAsync(inputStream, tempFile);// tempFile is where itll get saved
                Assert.True(File.Exists(tempFile), "Encrypted file should exist.");  //checking if the file exists

                //Read encrypted file
                var encryptedBytes = await File.ReadAllBytesAsync(tempFile);
                Assert.NotEqual(originalBytes, encryptedBytes); //ensuring that the original bytes are not equal to the encrypted bytes

                //Verify the encryption have content
                Assert.True(encryptedBytes.Length > 0, "Encrypted file should have content.");

                //Verify we cannot read the original text from the encrypted file
                //Make sure that without the key, the text wont decrypt
                var encryptedContent = Encoding.UTF8.GetString(encryptedBytes);
                Assert.DoesNotContain("This is a secret file content that should be encrypted.", encryptedContent);

            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
            {

            }
        }

        //testing the decryption
        [Fact]
        public async Task Test3_DecryptionFile_Successful()
        {
            //Create and encrypt the file first
            var originalContent = "This is a secret document.";
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();
            try
            {
                //Encrypt the file
                await encryptionService.EncryptFileAsync(inputStream, tempFile);
                //Now decrypt the file
                var decryptedStream = await encryptionService.DecryptFileAsync(tempFile);
                //send the decrypted content to secret keys
                var decryptedContent = Encoding.UTF8.GetString(decryptedStream.ToArray());
                //Verify that the decrypted content matches the original content
                Assert.Equal(originalContent, decryptedContent);
                Assert.Contains(originalContent, decryptedContent);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        //test if booked can be approved
        [Fact]
        public void Test4_ApproveBook_Successful()
        {
            //Create a new book to approve
            var newBook = new Book
            {
                Title = "Book to Approve",
                Author = "Author Name",
                SubmittedBy = "Approver User",
                Status = BookStatus.Pending,
            };
            //Add the book to the data store
            BookDataStore.AddBook(newBook);

            //Approve the book , thus update book
            var success = BookDataStore.UpdateBookStatus(newBook.Id, BookStatus.Approved, "Admin User", "Looks good.");
            Assert.True(success, "Book approval should be successful.");

            //Retrieve the book and verify status and other info from reviewer
            var approvedBook = BookDataStore.GetBookById(newBook.Id);
            Assert.Equal(BookStatus.Approved, approvedBook.Status);
            Assert.Equal("Admin User", approvedBook.ReviewedBy);

        }

        //test if booked can be approved
        [Fact]
        public void Test5_DeclineBook()
        {
            //Create a new book to decline
            var newBook = new Book
            {
                Title = "Book to Decline",
                Author = "Author",
                SubmittedBy = "Declining User",
                Status = BookStatus.Pending,
            };
            //Add the book to the data store
            BookDataStore.AddBook(newBook);

            //Decline the book, thus update book
            var success = BookDataStore.UpdateBookStatus(newBook.Id, BookStatus.Declined, "Admin User", "tf is this?");
            Assert.True(success, "Book Declination should be successful.");

            //Retrieve the book and verify status and other info from reviewer
            var declinedBook = BookDataStore.GetBookById(newBook.Id);
            Assert.Equal(BookStatus.Declined, declinedBook.Status);
            Assert.Equal("Admin User", declinedBook.ReviewedBy);
            Assert.NotNull(declinedBook.ReviewedDate);

        }
    }
}
//you must build before going to test explorer to run all tests