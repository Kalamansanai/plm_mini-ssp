using Domain.Common;
using Domain.Entities.CompanyHierarchy;
using Domain.Entities.TaskAggregate;
using Domain.Entities;
using FakeItEasy;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Domain.Tests.Entities.TaskAggregate
{
    public class TaskInstanceTests
    {
        [Fact]
        public void TaskInstance_AddEvent_ReturnResultOKV1_WithNoMoreRemainingIds()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.RemainingStepIds = new int[] { 1 };
            var step = A.Fake<Step>();
            step.Id = 1;
            var steps = A.Fake<List<Step>>();
            steps.Add(step);
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = true;
            //Act
            var result = taskInstance.AddEvent(steps, step, eventResult);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            taskInstance.State.Should().Be(TaskInstanceState.Completed);
        }
        [Fact]
        public void TaskInstance_AddEvent_ReturnResultOKV2_WithRemainingIds()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.RemainingStepIds = new int[] { 1, 2, 3, 4, 5 };
            var step = A.Fake<Step>();
            step.Id = 1;
            var steps = A.Fake<List<Step>>();
            steps.Add(step);
            steps.Add(new Step(2));
            steps.Add(new Step(3));
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = true;
            //Act
            var result = taskInstance.AddEvent(steps, step, eventResult);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            taskInstance.RemainingStepIds.ToArray().Count().Should().BeGreaterThan(0);
        }
        [Fact]
        public void TaskInstance_AddEvent_ReturnResultFail_DoesNotContainId()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.RemainingStepIds = new int[] { 2, 3, 4 };
            var step = A.Fake<Step>();
            step.Id = 1;
            var steps = A.Fake<List<Step>>();
            steps.Add(step);
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = true;
            //Act
            var result = taskInstance.AddEvent(steps, step, eventResult);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void TaskInstance_Abandon_ReturnResultOK()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.InProgress;
            //Act
            var result = taskInstance.Abandon();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            taskInstance.State.Should().Be(TaskInstanceState.Abandoned);
        }
        [Fact]
        public void TaskInstance_Abandon_ReturnResultFail()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.Completed;
            //Act
            var result = taskInstance.Abandon();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void TaskInstance_Pause_ReturnResultOK()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.InProgress;
            //Act
            var result = taskInstance.Pause();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            taskInstance.State.Should().Be(TaskInstanceState.Paused);
        }
        [Fact]
        public void TaskInstance_Pause_ReturnResultFail()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.Abandoned;
            //Act
            var result = taskInstance.Pause();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void TaskInstance_Resume_ReturnResultOK()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.Paused;
            //Act
            var result = taskInstance.Resume();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            taskInstance.State.Should().Be(TaskInstanceState.InProgress);
        }
        [Fact]
        public void TaskInstance_Resume_ReturnResultFail()
        {
            //Arrange
            var taskInstance = A.Fake<TaskInstance>();
            taskInstance.State = TaskInstanceState.InProgress;
            //Act
            var result = taskInstance.Resume();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
