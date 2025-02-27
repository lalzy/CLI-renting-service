using Xunit;
using System;
  public class UnitTest1
  {
    [Fact]
    public void Test1()
    {
      RentingService rs = new RentingService();
      var ret = rs.rentBook("");
    }
  }
