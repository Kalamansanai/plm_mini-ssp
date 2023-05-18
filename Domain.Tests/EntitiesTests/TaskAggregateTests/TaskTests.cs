using Domain.Common;
using Domain.Entities;
using Domain.Entities.CompanyHierarchy;
using Domain.Entities.TaskAggregate;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using Object = Domain.Entities.TaskAggregate.Object;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Tests.Entities.TaskAggregate
{
    public class TaskTests
    {
        [Fact]
        public void Task_CreateInstance_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            //Act
            var result = task.CreateInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.Instances.Should().HaveCount(1);
        }
        [Fact]
        public void Task_CreateInstance_ReturnResultFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            //Act
            var result = task.CreateInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_AddObjects_ObjectsAdded()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            Object object2 = new Object("test2");
            List<Object> objects = new List<Object>();
            objects.Add(object1);
            objects.Add(object2);
            //Act
            task.AddObjects(objects);
            //Assert
            task.Objects.Should().HaveCount(2);
        }
        [Fact]
        public void Task_AddSteps_StepsAdded()
        {
            //Arrange
            var task = A.Fake<Task>();
            Step step1 = new Step(0);
            Step step2 = new Step(1);
            List<Step> steps = new List<Step>();
            steps.Add(step1);
            steps.Add(step2);
            //Act
            task.AddSteps(steps);
            //Assert
            task.Steps.Should().HaveCount(2);
        }
        [Fact]
        public void Task_ModifyObject_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            object1.Id = 1;
            task.Objects.Add(object1);

            int objectId = 1;
            string name = "newName";
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = task.ModifyObject(objectId, name, objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.Objects.FirstOrDefault(o => o.Id == objectId).Name.Should().Be(name);
            task.Objects.FirstOrDefault(o => o.Id == objectId).Coordinates.X.Should().Be(objectCoordinates.X);
        }
        [Fact]
        public void Task_ModifyObject_ReturnResultFail_v1_ObjectNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            object1.Id = 1;
            task.Objects.Add(object1);

            int objectId = 0;
            string name = "newName";
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = task.ModifyObject(objectId, name, objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_ModifyObject_ReturnResultFail_v2_RenameFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            object1.Id = 1;
            Object object2 = new Object("test2");
            object1.Id = 2;
            task.Objects.Add(object1);
            task.Objects.Add(object2);

            int objectId = 1;
            string name = "test2";
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 300;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = task.ModifyObject(objectId, name, objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_ModifyObject_ReturnResultFail_v3_CoordinatesFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            object1.Id = 1;
            task.Objects.Add(object1);

            int objectId = 1;
            string name = "newName";
            var objectCoordinates = A.Fake<ObjectCoordinates>();
            objectCoordinates.X = 10000;
            objectCoordinates.Height = 3;
            objectCoordinates.Y = 300;
            objectCoordinates.Width = 3;
            //Act
            var result = task.ModifyObject(objectId, name, objectCoordinates);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_ModifyStep_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            Step step1 = new Step(1);
            task.Steps.Add(step1);

            int stepId = 1;
            int orderNum = 1;
            TemplateState templateStateInit = TemplateState.Present;
            TemplateState templateStateSubs = TemplateState.Present;
            var testObject = A.Fake<Object>();
            //Act
            var result = task.ModifyStep(stepId, orderNum, templateStateInit, templateStateSubs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.Steps.FirstOrDefault(o => o.Id == stepId).OrderNum.Should().Be(orderNum);
            task.Steps.FirstOrDefault(o => o.Id == stepId).ExpectedInitialState.Should().Be(templateStateInit);
            task.Steps.FirstOrDefault(o => o.Id == stepId).ExpectedSubsequentState.Should().Be(templateStateSubs);
            task.Steps.FirstOrDefault(o => o.Id == stepId).Object.Should().Be(testObject);
        }
        [Fact]
        public void Task_ModifyStep_ReturnResultFail_v1_StepNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            Step step1 = new Step(0);
            task.Steps.Add(step1);

            int stepId = 1;
            int orderNum = 1;
            TemplateState templateStateInit = TemplateState.Present;
            TemplateState templateStateSubs = TemplateState.Present;
            var testObject = A.Fake<Object>();
            //Act
            var result = task.ModifyStep(stepId, orderNum, templateStateInit, templateStateSubs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_ModifyStep_ReturnResultFail_v2_UpdateFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            Step step1 = new Step(1);
            task.Steps.Add(step1);

            int stepId = 1;
            int orderNum = 1;
            TemplateState templateStateInit = TemplateState.UnknownObject;
            TemplateState templateStateSubs = TemplateState.Present;
            var testObject = A.Fake<Object>();
            //Act
            var result = task.ModifyStep(stepId, orderNum, templateStateInit, templateStateSubs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_RemoveObjects_ObjectsRemoved()
        {
            //Arrange
            var task = A.Fake<Task>();
            Object object1 = new Object("test1");
            object1.Id = 1;
            Object object2 = new Object("test2");
            object2.Id = 2;
            List<int> objectIds = new List<int>();
            task.Objects.Add(object1);
            task.Objects.Add(object2);
            objectIds.Add(1);
            objectIds.Add(2);
            //Act
            task.RemoveObjects(objectIds);
            //Assert
            task.Objects.Should().HaveCount(0);
        }/*
        [Fact]
        public void Task_RemoveSteps_StepsRemoved()
        {
            //Arrange
            var task = A.Fake<Task>();
            Step step1 = new Step(1);
            Step step2 = new Step(2);
            List<int> stepIds = new List<int>();
            task.Steps.Add(step1);
            task.Steps.Add(step2);
            stepIds.Add(1);
            stepIds.Add(2);
            //Act
            task.RemoveSteps(stepIds);
            //Assert
            task.Steps.Should().HaveCount(0);
        }*/
        [Fact]
        public void Task_Rename_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.Id = 1;
            var taskTest = A.Fake<Task>();
            task.Id = 2;
            taskTest.Name = "test";
            string newName = "newName";
            var job = A.Fake<Job>();
            job.Tasks = A.Fake<List<Task>>();
            job.Tasks.Add(taskTest);
            //Act
            var result = task.Rename(newName, job);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.Name.Should().Be(newName);
        }
        [Fact]
        public void Task_Rename_ReturnResultFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.Id = 1;
            var taskTest = A.Fake<Task>();
            taskTest.Name = "test";
            task.Id = 2;
            string newName = "test";
            var job = A.Fake<Job>();
            job.Tasks = A.Fake<List<Task>>();
            job.Tasks.Add(taskTest);
            //Act
            var result = task.Rename(newName, job);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }/*
        [Fact]
        public void Task_AddEventToCurrentInstance_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.Completed;
            task.OngoingInstance.RemainingStepIds = new int[] { 1 };
            task.Location = A.Fake<Location>();
            var step = A.Fake<Step>();
            step.Id = 1;
            task.Steps.Add(step);
            int stepId = 1;
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = false;
            var detector = A.Fake<Detector>();
            //Act
            var result = task.AddEventToCurrentInstance(stepId, eventResult, detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.OngoingInstance.Should().BeNull();
        }
        [Fact]
        public void Task_AddEventToCurrentInstance_ReturnResultFailV1_OnGoingInstanceNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance = null;
            int stepId = 1;
            var eventResult = A.Fake<EventResult>();
            var detector = A.Fake<Detector>();
            //Act
            var result = task.AddEventToCurrentInstance(stepId, eventResult, detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
            task.OngoingInstance.Should().BeNull();

        }
        [Fact]
        public void Task_AddEventToCurrentInstance_ReturnResultFailV2_StepNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            var step = A.Fake<Step>();
            step.Id = 3;
            task.Steps.Add(step);
            int stepId = 1;
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = false;
            var detector = A.Fake<Detector>();
            //Act
            var result = task.AddEventToCurrentInstance(stepId, eventResult, detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_AddEventToCurrentInstance_ReturnResultFailV3_AddEventFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.RemainingStepIds = new int[] { 3 };
            var step = A.Fake<Step>();
            step.Id = 1;
            task.Steps.Add(step);
            int stepId = 1;
            var eventResult = A.Fake<EventResult>();
            eventResult.Success = false;
            var detector = A.Fake<Detector>();
            //Act
            var result = task.AddEventToCurrentInstance(stepId, eventResult, detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }*/
        [Fact]
        public void Task_StopCurrentInstance_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.InProgress;
            //Act
            var result = task.StopCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            task.OngoingInstance.Should().BeNull();
        }
        [Fact]
        public void Task_StopCurrentInstance_ReturnResultFailV1_OngoingInstanceNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = null;
            //Act
            var result = task.StopCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
            task.OngoingInstance.Should().BeNull();
        }
        [Fact]
        public void Task_StopCurrentInstance_ReturnResultFailV2_AbandonReturnedFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.Completed;
            //Act
            var result = task.StopCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_PauseCurrentInstance_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.InProgress;
            //Act
            var result = task.PauseCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public void Task_PauseCurrentInstance_ReturnResultFailV1_OngoingInstanceNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = null;
            //Act
            var result = task.PauseCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
            task.OngoingInstance.Should().BeNull();
        }
        [Fact]
        public void Task_PauseCurrentInstance_ReturnResultFailV2_PauseReturnedFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.Abandoned;
            //Act
            var result = task.PauseCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Task_ResumeCurrentInstance_ReturnResultOK()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.Paused;
            //Act
            var result = task.ResumeCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public void Task_ResumeCurrentInstance_ReturnResultFailV1_OngoingInstanceNull()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = null;
            //Act
            var result = task.ResumeCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
            task.OngoingInstance.Should().BeNull();
        }
        [Fact]
        public void Task_ResumeCurrentInstance_ReturnResultFailV2_ResumeReturnedFail()
        {
            //Arrange
            var task = A.Fake<Task>();
            task.OngoingInstance = A.Fake<TaskInstance>();
            task.OngoingInstance.State = TaskInstanceState.Completed;
            //Act
            var result = task.ResumeCurrentInstance();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
