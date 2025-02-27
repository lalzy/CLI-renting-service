using Xunit;
using System;
using NuGet.Frameworks;
public class UnitTest1
  {
    [InlineData("Harry Potter")]
    [Theory]
    public void BookAvailable(string? bookTitle)
    {
      RentingService rs = new RentingService();

      // Hvis jeg setter var = ret,  så failer den??
      KeyValuePair<Book, int>? ret = rs.CheckAvailability(bookTitle);
      Assert.IsType<KeyValuePair<Book, int>>(ret);
    }

    [InlineData("abc")]
    [Theory]
    public void BookNotAvailable(string? bookTitle)
    {
      RentingService rs = new RentingService();
      var ret = rs.CheckAvailability(bookTitle);
      Assert.Null(ret);
    }

    
    [InlineData("FouNdAtioN")]
    [Theory]
    public void caseInsensitive(string? bookTitle)
    {
      RentingService rs = new RentingService();
      var ret = rs.CheckAvailability(bookTitle);
      Assert.NotNull(ret);
    }
  }
