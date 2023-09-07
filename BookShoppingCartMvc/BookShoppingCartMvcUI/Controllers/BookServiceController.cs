using BookShoppingCartMvcUI.Data;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{
    public class BookServiceController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBookRepository _bookRepo;
        private readonly ApplicationDbContext _db;

        public BookServiceController(IWebHostEnvironment webHostEnvironment, IBookRepository bookRepo, ApplicationDbContext db)
        {
            _webHostEnvironment = webHostEnvironment;
            _bookRepo = bookRepo;
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(Book book, IFormFile? imageFile, string deleteImageFile)
        {
            if (!string.IsNullOrEmpty(deleteImageFile))
            {
                imageFile = null;
                book.Image = null;
            }

            book.Genre = _db.Genres.Find(book.GenreId);

            if (!(string.IsNullOrEmpty(book.BookName) || string.IsNullOrEmpty(book.AuthorName) || book.Price == 0 || book.GenreId == 0))
            {
                await _bookRepo.Create(book, imageFile, _webHostEnvironment);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBook(Book bookModel, IFormFile? imageFile, string deleteImageFile)
        {
            var bookForEdit = _db.Books.Find(bookModel.Id);
            if (bookForEdit == null) throw new InvalidOperationException("Book not found.");

            if (!string.IsNullOrEmpty(deleteImageFile))
            {
                imageFile = null;
                if (!string.IsNullOrEmpty(bookForEdit.Image))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", bookForEdit.Image);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                bookForEdit.Image = null;
            }

            if (!(string.IsNullOrEmpty(bookModel.BookName) || string.IsNullOrEmpty(bookModel.AuthorName) || bookModel.Price == 0 || bookModel.GenreId == 0))
            {
                await _bookRepo.Edit(bookModel, bookForEdit, imageFile, _webHostEnvironment);

                return RedirectToAction("Index", "Home");
            }

            return View(bookForEdit);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(Book bookModel)
        {
            var book = _db.Books.Find(bookModel.Id);
            if (book != null)
            {
                _bookRepo.Delete(book, _webHostEnvironment);
                return RedirectToAction("Index", "Home");
            }

            return NotFound();
        }
    }
}