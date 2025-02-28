using Xunit;
using System;
using NuGet.Frameworks;
using System.ComponentModel;
public class UnitTest1
  {
    [Fact]
    public void AddingNewBook(){
      string title = "-new-book-added";
      string author = "-somedude-";
      int amount = 2;

      RentingService rs = new RentingService();
      rs.addBook(title, author, amount);

      KeyValuePair<Book, int>? newBook = rs.CheckAvailability(title);
      Assert.NotNull(newBook);
      Assert.Equal(newBook.Value.Key.Title, title);
      Assert.Equal(newBook.Value.Key.Author, author);
      Assert.Equal(newBook.Value.Value, amount);
    }

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

    private int GetBookCount(RentingService rs, String bookTitle){
      return (from book in rs.ListAllBooks()
          where book.Key.Title.ToLower() == bookTitle.ToLower()
          select book.Value).FirstOrDefault();
    }

    //[InlineData ("Foundation")]
    [Fact]
    public void BookRented(){
      string title = "foundation";
      RentingService rs = new RentingService();
      int preBorrowCount = GetBookCount(rs, title);
      var ret = rs.rentBook(title);
      Assert.NotNull(ret);

      int postBorrowCount = GetBookCount(rs, title);
      Assert.NotEqual(preBorrowCount, postBorrowCount);
      Assert.Equal(preBorrowCount - 1, postBorrowCount);
    } 

    [Fact]
    public void RentNonExistentBook(){
      RentingService rs = new RentingService();
      var ret = rs.rentBook(null);
      Assert.Null(ret);

      string title = "--doesn'tactuallyexisthere--";
      ret = rs.rentBook(title);
      Assert.Null(ret);
    }

    [InlineData("harry PottEr")]
    [InlineData("foundation")]
    [Theory]
    public void returningBook(String title){
      RentingService rs = new RentingService();
      int preBorrow = GetBookCount(rs, title);

      BorrowReceipt? br = rs.rentBook(title);
      Assert.NotNull(br);

      var ret = rs.ReturnBook(title, br.BorrowDate);
      Assert.NotNull(ret);
      Assert.IsType<ReturnReceipt>(ret);
      int postGivenBack = GetBookCount(rs, title);
      Assert.Equal(preBorrow, postGivenBack);
    }

    [Fact]
    public void ReturningInvalidBook(){
      string title = "doesn't exist";
      BorrowReceipt br = new BorrowReceipt(title);
      RentingService rs = new RentingService();

      var ret = rs.ReturnBook(title, br.BorrowDate);
      Assert.Null(ret);
    }
  }
