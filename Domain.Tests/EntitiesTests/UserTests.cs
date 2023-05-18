using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_Authenticate_ReturnResultOK()
        {
            //Arrange
            User user = new User("username", "password", UserRole.SuperUser);
            string password = "password";
            //Act
            var result = user.Authenticate(password);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<string>>();
            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public void User_Authenticate_ReturnResultFail()
        {
            //Arrange
            User user = new User("username", "password", UserRole.SuperUser);
            string password = "wrong";
            //Act
            var result = user.Authenticate(password);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<string>>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void User_Update_ReturnResultOK()
        {
            //Arrange
            User user = new User("username", "password", UserRole.SuperUser);
            User newUser = new User("newuser", "newpass", UserRole.SuperUser);
            //Act
            var result = user.Update("newuser", "newpass", UserRole.SuperUser);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<User>>();
            result.IsSuccess.Should().BeTrue();
            result.Value.UserName.Should().Be(newUser.UserName);
        }
    }
}
