using Domain.Common;
using Domain.Entities.TaskAggregate;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Object = Domain.Entities.TaskAggregate.Object;

namespace Domain.Tests.Entities.TaskAggregate
{
    public class StepTests
    {
        [Fact]
        public void Step_Update_ReturnResultOK()
        {
            //Arrange
            var step = A.Fake<Step>();
            int orderNum = 1;
            TemplateState init = TemplateState.Present;
            TemplateState subs = TemplateState.Missing;
            var testObject = A.Fake<Object>();
            //Act
            var result = step.Update(orderNum, init, subs , testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            step.OrderNum.Should().Be(orderNum);
            step.ExpectedInitialState.Should().Be(init);
            step.ExpectedSubsequentState.Should().Be(subs);
            step.Object.Should().Be(testObject);
        }
        [Fact]
        public void Step_Update_ReturnResultFail_v1()
        {
            //Arrange
            var step = A.Fake<Step>();
            int orderNum = 1;
            TemplateState init = TemplateState.Uncertain;
            TemplateState subs = TemplateState.Missing;
            var testObject = A.Fake<Object>();
            //Act
            var result = step.Update(orderNum, init, subs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Step_Update_ReturnResultFail_v2()
        {
            //Arrange
            var step = A.Fake<Step>();
            int orderNum = 1;
            TemplateState init = TemplateState.Present;
            TemplateState subs = TemplateState.UnknownObject;
            var testObject = A.Fake<Object>();
            //Act
            var result = step.Update(orderNum, init, subs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Step_Create_ReturnStep()
        {
            //Arrange
            int orderNum = 1;
            TemplateState init = TemplateState.Present;
            TemplateState subs = TemplateState.Missing;
            var testObject = A.Fake<Object>();
            //Act
            var result = Step.Create(orderNum, init, subs, testObject);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Step>();
            result.OrderNum.Should().Be(orderNum);
            result.ExpectedInitialState.Should().Be(init);
            result.ExpectedSubsequentState.Should().Be(subs);
            result.Object.Should().Be(testObject);
        }
    }
}
