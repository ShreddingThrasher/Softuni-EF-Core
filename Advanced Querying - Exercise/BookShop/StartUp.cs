namespace BookShop
{
    using System;
    using Data;
    using Initializer;
    using System.Text;
    using System.Linq;
    using BookShop.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();

            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());

            //string result = GetMostRecentBooks(db);

            //Console.WriteLine(result);

            Console.WriteLine(RemoveBooks(db));
        }

        //2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            AgeRestriction ageRestriction;

            bool hasParsed = Enum.TryParse<AgeRestriction>(command, true, out ageRestriction);

            if (!hasParsed)
            {
                return string.Empty;
            }

            var books = context.Books
                            .Where(b => b.AgeRestriction == ageRestriction)
                            .Select(b => new
                            {
                                Title = b.Title,
                            })
                            .OrderBy(b => b.Title)
                            .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            EditionType goldEdition = EditionType.Gold;

            var goldenBooks = context.Books
                                    .Where(b => b.EditionType == goldEdition && b.Copies < 5000)
                                    .OrderBy(b => b.BookId)
                                    .Select(b => b.Title)
                                    .ToList();

            return string.Join(Environment.NewLine, goldenBooks);
        }

        //4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksByPrice = context.Books
                                    .Where(b => b.Price > 40)
                                    .OrderByDescending(b => b.Price)
                                    .Select(b => $"{b.Title} - ${b.Price:F2}")
                                    .ToList();

            return string.Join(Environment.NewLine, booksByPrice);
        }


        //5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var booksNotReleasedIn = context.Books
                                        .Where(b => b.ReleaseDate.Value.Year != year)
                                        .OrderBy(b => b.BookId)
                                        .Select(b => b.Title)
                                        .ToList();

            return string.Join(Environment.NewLine, booksNotReleasedIn);
        }

        //6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => x.ToLower())
                                       .ToArray();

            var booksByCategory = context.Books
                                    .Where(b => categories.Contains(b.BookCategories
                                                                .FirstOrDefault().Category.Name.ToLower()))
                                    .Select(b => b.Title)
                                    .OrderBy(b => b)
                                    .ToList();

            return string.Join(Environment.NewLine, booksByCategory);
        }

        //7. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books
                            .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                            .OrderByDescending(b => b.ReleaseDate)
                            .Select(b => $"{b.Title} - {b.EditionType.ToString()} - ${b.Price:F2}")
                            .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                            .Where(a => a.FirstName.EndsWith(input))
                            .Select(a => $"{a.FirstName} {a.LastName}")
                            .ToList()
                            .OrderBy(a => a);

            return string.Join(Environment.NewLine, authors);
        }

        //9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                            .Select(b => b.Title)
                            .ToList()
                            .Where(t => t.Contains(input, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(t => t);

            return string.Join(Environment.NewLine, books);
        }

        //10. Book Search by Author 
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksAndAuthors = context.Books
                                    .ToList()
                                    .Where(b => b.Author.LastName.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                                    .OrderBy(b => b.BookId)
                                    .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})");

            return string.Join(Environment.NewLine, booksAndAuthors);
        }

        //11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var count = context.Books
                            .Where(b => b.Title.Length > lengthCheck)
                            .Count();

            return count;
        }

        //12. Total Book Copies 
        public static string CountCopiesByAuthor(BookShopContext context)
        {

            StringBuilder sb = new StringBuilder();

            var authorsAndCopies = context.Authors
                                        .Select(a => new
                                        {
                                            Name = $"{a.FirstName} {a.LastName}",
                                            TotalCopies = a.Books.Sum(b => b.Copies)
                                        })
                                        .OrderByDescending(a => a.TotalCopies)
                                        .ToList();

            foreach (var author in authorsAndCopies)
            {
                sb.AppendLine($"{author.Name} - {author.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesProfit = context.Categories
                                        .Select(c => new
                                        {
                                            c.Name,
                                            Profit = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                                        })
                                        .OrderByDescending(c => c.Profit)
                                        .ThenBy(c => c.Name)
                                        .ToList();

            foreach (var category in categoriesProfit)
            {
                sb.AppendLine($"{category.Name} ${category.Profit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //14. Most Recent Books 
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var mostRecent = context.Categories
                                .Select(c => new
                                {
                                    c.Name,
                                    RecentBooks = c.CategoryBooks
                                                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                                                        .Take(3)
                                                        .Select(cb => new
                                                        {
                                                            cb.Book.Title,
                                                            cb.Book.ReleaseDate
                                                        })
                                                        .Take(3)
                                })
                                .OrderBy(c => c.Name);

            foreach (var category in mostRecent)
            {
                sb.AppendLine($"--{category.Name}");

                foreach (var book in category.RecentBooks)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                            .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                                    .Where(b => b.Copies < 4200);

            int deletedBooks = booksToRemove.Count();

            context.Books.RemoveRange(booksToRemove);

            context.SaveChanges();

            return deletedBooks;
        }
    }
}
