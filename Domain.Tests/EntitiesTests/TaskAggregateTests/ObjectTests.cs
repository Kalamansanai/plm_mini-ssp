using Domain.Common;
using Domain.Entities.TaskAggregate;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = Domain.Entities.TaskAggregate.Object;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Tests.Entities.TaskAggregate
{
    public class ObjectTests
    {
        [Fact]
        public void Object_Create_ReturnResultOK()
        {
            //Arrange
            var testObject = A.Fake<Object>();
            testObject.Coordinates = A.Fake<ObjectCoordinates>();
            testObject.Name = "test";
            testObject.Coordinates.X = 300;
            testObject.TaskId = 1;

            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Width = 1;
            objectCoordinates.Y = 300;
            objectCoordinates.Height = 1;
            var task = A.Fake<Task>();
            task.Id = 1;
            var dummyObject = new Object("dummy");
            task.Objects.Add(dummyObject);
            string newName = "test";
            //Act
            var result = Object.Create(newName, objectCoordinates, task);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<Object>>();
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(newName);
            result.Value.Coordinates.X.Should().Be(testObject.Coordinates.X);
            result.Value.TaskId.Should().Be(testObject.TaskId);
        }
        [Fact]
        public void Object_Create_ReturnResultFail_v1()
        {
            //Arrange
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Width = 1;
            objectCoordinates.Y = 300;
            objectCoordinates.Height = 1;
            var task = A.Fake<Task>();
            var dummyObject = new Object("test");  //not unique name
            task.Objects.Add(dummyObject);
            string newName = "test";
            //Act
            var result = Object.Create(newName, objectCoordinates, task);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<Object>>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Object_Create_ReturnResultFail_v2()
        {
            //Arrange
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 800;  //bad coordinate
            objectCoordinates.Width = 1;
            objectCoordinates.Y = 300;
            objectCoordinates.Height = 1;
            var task = A.Fake<Task>();
            var dummyObject = new Object("dummy");
            task.Objects.Add(dummyObject);
            string newName = "test";
            //Act
            var result = Object.Create(newName, objectCoordinates, task);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<Object>>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Object_Rename_ReturnResultOK()
        {
            //Arrange
            var testObject = A.Fake<Object>();
            var task = A.Fake<Task>();
            var dummyObject = new Object("dummy");
            task.Objects.Add(dummyObject);
            string newName = "test";
            //Act
            var result = testObject.Rename(newName, task);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            testObject.Name.Should().Be(newName);
        }
        [Fact]
        public void Object_Rename_ReturnResultFail()
        {
            //Arrange
            var testObject = A.Fake<Object>();
            testObject.Id = 1000;
            var task = A.Fake<Task>();
            var dummyObject = new Object("test");
            task.Objects.Add(dummyObject);
            string newName = "test";
            //Act
            var result = testObject.Rename(newName, task);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Object_UpdateCoordinates_ReturnResultOK()
        {
            //Arrange
            var testObject = A.Fake<Object>();
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = testObject.UpdateCoordinates(objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            testObject.Coordinates.X.Should().Be(300);
            testObject.Coordinates.Height.Should().Be(3);
            testObject.Coordinates.Y.Should().Be(300);
            testObject.Coordinates.Width.Should().Be(3);
        }
        [Fact]
        public void Object_UpdateCoordinates_ReturnResultFail()
        {
            //Arrange
            var testObject = A.Fake<Object>();
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 800;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = testObject.UpdateCoordinates(objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
