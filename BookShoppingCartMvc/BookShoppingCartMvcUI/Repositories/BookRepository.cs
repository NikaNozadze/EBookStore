using BookShoppingCartMvcUI.Data;
using BookShoppingCartMvcUI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task Create(Book book, IFormFile? imageFile, IWebHostEnvironment webHostEnvironment)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                book.Image = fileName;
            }

            if (book == null)
            {
                throw new Exception("The book could not be added");
            }
            _db.Books.Add(book);
            _db.SaveChanges();
        }

        public async Task Edit(Book bookModel, Book bookForEdit, IFormFile? imageFile, IWebHostEnvironment webHostEnvironment)
        {
            if (bookModel == null) throw new InvalidOperationException("Book not found.");

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(bookForEdit.Image))
                {
                    var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", bookForEdit.Image);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(bookForEdit.Image))
                {
                    var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", bookForEdit.Image);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                bookForEdit.Image = fileName;
            }

            bookForEdit.BookName = bookModel.BookName;
            bookForEdit.AuthorName = bookModel.AuthorName;
            bookForEdit.Price = bookModel.Price;
            bookForEdit.GenreId = bookModel.GenreId;

            _db.SaveChanges();
        }

        public void Delete(Book book, IWebHostEnvironment webHostEnvironment)
        {
            if (book == null)
            {
                throw new DbUpdateException($"Book does not exist ");
            }

            if (!string.IsNullOrEmpty(book.Image))
            {
                var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", book.Image);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            _db.Books.Remove(book);
            _db.SaveChanges();
        }
    }
}
