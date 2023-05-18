using Domain.Common;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Common
{
    public class EventResultTests
    {
        [Fact]
        public void EventResult_Create_ReturnResultOK()
        {
            //Act
            var result1 = EventResult.Create(true, null);
            var result2 = EventResult.Create(false, "Test");
            //Assert
            result1.Should().NotBeNull();
            result1.Should().BeOfType<Result<EventResult>>();
            result1.IsSuccess.Should().BeTrue();
            result1.Value.Success.Should().BeTrue();

            result2.Should().NotBeNull();
            result2.Should().BeOfType<Result<EventResult>>();
            result2.IsSuccess.Should().BeTrue();
            result2.Value.Success.Should().BeFalse();
        }
        [Fact]
        public void EventResult_Create_ReturnResultFailV1_SuccessWithFailureReason()
        {
            //Act
            var result1 = EventResult.Create(true, "Reason");
            //Assert
            result1.Should().NotBeNull();
            result1.Should().BeOfType<Result<EventResult>>();
            result1.IsSuccess.Should().BeFalse();

        }
        [Fact]
        public void EventResult_Create_ReturnResultFailV2_FailWithoutReason()
        {
            //Act
            var result1 = EventResult.Create(false, null);
            //Assert
            result1.Should().NotBeNull();
            result1.Should().BeOfType<Result<EventResult>>();
            result1.IsSuccess.Should().BeFalse();
        }
    }
}
