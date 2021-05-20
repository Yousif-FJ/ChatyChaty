using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.EntityTests
{
    public class UserIdTest
    {
        [Fact]
        public void CreateNewId_NotZero()
        {
            //Act
            var Id = new UserId();
            //Assert
            Assert.NotNull(Id.Value);
            Assert.NotEqual(Id.Value, new Guid().ToString());
        }

        [Fact]
        public void CreateNewId_Unique()
        {
            //Act
            var Id1 = new UserId();
            var Id2 = new UserId();
            //Assert
            Assert.NotEqual(Id1, Id2);
        }

        [Fact]
        public void CreateIdFromString_Equal()
        {
            //Arrange
            var NewId = new UserId();
            //Act
            var Id = new UserId(NewId.Value);
            //Assert
            Assert.NotNull(Id.Value);
            Assert.NotEqual(Id.Value, new Guid().ToString());
            Assert.Equal(NewId, Id);
        }

        [Fact]
        public void CreateIdFromToString_Equal()
        {
            //Arrange
            var NewId = new UserId();
            //Act
            var Id = new UserId(NewId.ToString());
            //Assert
            Assert.NotNull(Id.Value);
            Assert.NotEqual(Id.Value, new Guid().ToString());
            Assert.Equal(NewId, Id);
        }
    }
}
