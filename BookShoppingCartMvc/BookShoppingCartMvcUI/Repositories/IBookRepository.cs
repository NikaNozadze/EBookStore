using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Repositories
{
    public interface IBookRepository
    {
        Task Create(Book book, IFormFile? imageFile, IWebHostEnvironment webHostEnvironment);
        Task Edit(Book bookModel, Book bookForEdit, IFormFile? imageFile, IWebHostEnvironment webHostEnvironment);
        void Delete(Book book, IWebHostEnvironment webHostEnvironment);
    }
}