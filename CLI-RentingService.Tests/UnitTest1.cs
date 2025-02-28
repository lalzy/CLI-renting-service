using Xunit;
using System;
using NuGet.Frameworks;
using System.ComponentModel;
public class UnitTest1
  {
    /// <summary>
    /// Get the count of books from the library.
    /// </summary>
    /// <param name="rs">RentingService instance</param>
    /// <param name="bookTitle">Title of the book</param>
    /// <returns></returns>
    private int GetBookCount(RentingService rs, string bookTitle){
      return (from book in rs.ListAllBooks()
          where book.Key.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase)
          select book.Value).FirstOrDefault();
    }

    /// <summary>
    /// Testing adding a new book to the library.
    /// </summary>
    [Fact]
    public void AddingNewBook(){
      string bookTitle = "-new-book-added";
      string author = "-somedude-";
      int amount = 2;

      RentingService rs = new RentingService();
      rs.AddBook(bookTitle, author, amount);

      KeyValuePair<Book, int>? newBook = rs.CheckAvailability(bookTitle);
      Assert.NotNull(newBook);
      Assert.Equal(newBook.Value.Key.Title, bookTitle);
      Assert.Equal(newBook.Value.Key.Author, author);
      Assert.Equal(newBook.Value.Value, amount);
    }

    /// <summary>
    /// Testing to check when a book we're asking for is avilable and valid.
    /// </summary>
    /// <param name="bookTitle">Title of the book</param>
    [InlineData("Harry Potter")]
    [InlineData("FounDation")]
    [InlineData("harry potter")]
    [Theory]
    public void WhenBookAvailable(string? bookTitle)
    {
      RentingService rs = new RentingService();
      var ret = rs.CheckAvailability(bookTitle);
      Assert.IsType<KeyValuePair<Book, int>>(ret);
      Assert.True(ret.Value.Value > 0);
    }

    /// <summary>
    /// Test for CheckAvailability with a book that doesn't actually exist.
    /// </summary>
    /// <param name="bookTitle">Title of the book</param>
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void IfInvalidOrEmptyQuery(string? bookTitle)
    {
      RentingService rs = new RentingService();
      var ret = rs.CheckAvailability(bookTitle);
      Assert.Null(ret);
    }


    /// <summary>
    /// Test for if we attempt to rent an unavailable book (that is an valid existing book in the library).
    /// </summary>
    [Fact]
    public void IfUnavailableBook(){
      string bookTitle = "foundation";
      RentingService rs = new RentingService();
      int bookCount = GetBookCount(rs, bookTitle);

      for(int i = 0; i < bookCount ; i++){
        rs.RentBook(bookTitle);
      }

      var ret = rs.RentBook(bookTitle);
      Assert.Null(ret);
    } 

    /// <summary>
    /// Ensuring renting of a book is handled correctly.
    /// </summary>
    [Fact]
    public void BookRented(){
      string bookTitle = "foundation";
      RentingService rs = new RentingService();
      int preBorrowCount = GetBookCount(rs, bookTitle);
      var ret = rs.RentBook(bookTitle);
      Assert.NotNull(ret);

      int postBorrowCount = GetBookCount(rs, bookTitle);
      Assert.NotEqual(preBorrowCount, postBorrowCount);
      Assert.Equal(preBorrowCount - 1, postBorrowCount);
    } 

    /// <summary>
    /// Test if we are renting an non-existant book.
    /// </summary>
    [Fact]
    public void RentNonExistentBook(){
      RentingService rs = new RentingService();
      var ret = rs.RentBook(null);
      Assert.Null(ret);

      string bookTitle = "--doesn'tactuallyexisthere--";
      ret = rs.RentBook(bookTitle);
      Assert.Null(ret);
    }

    /// <summary>
    /// Testing return of a book
    /// </summary>
    /// <param name="bookTitle">title of the book</param>
    [InlineData("harry PottEr")]
    [InlineData("foundation")]
    [Theory]
    public void ReturningBook(string bookTitle){
      RentingService rs = new RentingService();
      int preBorrow = GetBookCount(rs, bookTitle);

      BorrowReceipt? br = rs.RentBook(bookTitle);
      Assert.NotNull(br);

      var ret = rs.ReturnBook(bookTitle, br.BorrowDate);
      Assert.NotNull(ret);
      Assert.IsType<ReturnReceipt>(ret);
      int postGivenBack = GetBookCount(rs, bookTitle);
      Assert.Equal(preBorrow, postGivenBack);
    }

    /// <summary>
    /// Test if we attempt to return a book that doesn't exist.
    /// </summary>
    [Fact]
    public void ReturningInvalidBook(){
      string bookTitle = "doesn't exist";
      BorrowReceipt br = new BorrowReceipt(bookTitle);
      RentingService rs = new RentingService();

      var ret = rs.ReturnBook(bookTitle, br.BorrowDate);
      Assert.Null(ret);
    }
  }
