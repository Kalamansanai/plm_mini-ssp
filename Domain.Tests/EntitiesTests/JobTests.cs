using Domain.Common;
using Domain.Entities;
using Domain.Entities.TaskAggregate;
using Domain.Interfaces;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Tests.Entities
{
    public class JobTests
    {
        [Fact]
        public void Job_Create_ReturnResultOK()
        {
            //Arrange
            var iJobNameUniquenessChecker = A.Fake<IJobNameUniquenessChecker>();
            string name = "newName";
            A.CallTo(() => iJobNameUniquenessChecker.IsDuplicate(name,null)).Returns(false);
            //Act
            var result = Job.Create(name, iJobNameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<Job>>();
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(name);
        }
        [Fact]
        public void Job_Create_ReturnResultFail_DuplicateName()
        {
            //Arrange
            var iJobNameUniquenessChecker = A.Fake<IJobNameUniquenessChecker>();
            string name = "newName";
            A.CallTo(() => iJobNameUniquenessChecker.IsDuplicate(name, null)).Returns(true);
            //Act
            var result = Job.Create(name, iJobNameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<Job>>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Job_Rename_ReturnResultOK()
        {
            //Arrange
            var job = A.Fake<Job>();
            var iJobNameUniquenessChecker = A.Fake<IJobNameUniquenessChecker>();
            string name = "newName";
            A.CallTo(() => iJobNameUniquenessChecker.IsDuplicate(name, job)).Returns(false);
            //Act
            var result = job.Rename(name, iJobNameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            job.Name.Should().Be(name);
        }
        [Fact]
        public void Job_Rename_ReturnResultFail_DuplicateName()
        {
            //Arrange
            var job = A.Fake<Job>();
            var iJobNameUniquenessChecker = A.Fake<IJobNameUniquenessChecker>();
            string name = "newName";
            A.CallTo(() => iJobNameUniquenessChecker.IsDuplicate(name, job)).Returns(true);
            //Act
            var result = job.Rename(name, iJobNameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Job_DeleteTask_TaskDeleted()
        {
            //Arrange
            var job = A.Fake<Job>();
            job.Tasks = A.Fake<List<Task>>();
            Task taskInList = new Task("test", 1, TaskType.ToolKit);
            job.Tasks.Add(taskInList);
            Task taskToDelete = taskInList;
            //Act
            job.DeleteTask(taskToDelete);
            //Assert
            job.Tasks.Should().BeEmpty();
        }
    }
}
