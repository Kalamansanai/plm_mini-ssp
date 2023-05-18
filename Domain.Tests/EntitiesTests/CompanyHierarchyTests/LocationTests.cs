using Domain.Common;
using Domain.Entities;
using Domain.Entities.CompanyHierarchy;
using Domain.Interfaces;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Entities.CompanyHierarchy
{
    public class LocationTests
    {
        [Fact]
        public void Location_Rename_ReturnResultOK()
        {
            //Arrange
            var location = A.Fake<Location>();
            string newName = "test";
            var nameUniquenessChecker = A.Fake<ICHNameUniquenessChecker<Station, Location>>();
            A.CallTo(() => nameUniquenessChecker.IsDuplicate(location.Parent, newName, location)).Returns(false);
            //Act
            var result = location.Rename(newName, nameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            location.Name.Should().Be(newName);
        }
        [Fact]
        public void Location_Rename_ReturnResultFail()
        {
            //Arrange
            var location = A.Fake<Location>();
            string newName = "test";
            var nameUniquenessChecker = A.Fake<ICHNameUniquenessChecker<Station, Location>>();
            A.CallTo(() => nameUniquenessChecker.IsDuplicate(location.Parent, newName, location)).Returns(true);
            //Act
            var result = location.Rename(newName, nameUniquenessChecker);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Location_AttachDetector_ReturnResultOK_v1()
        {
            //Arrange
            var location = A.Fake<Location>();
            Detector detector = new Detector("test");
            //Act
            var result = location.AttachDetector(detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            location.Detector.Should().Be(detector);

        }
        [Fact]
        public void Location_AttachDetector_ReturnResultOK_v2()
        {
            //Arrange
            var location = A.Fake<Location>();
            location.Detector = A.Fake<Detector>();
            location.Detector.Id = 1;
            var detector = A.Fake<Detector>();
            detector.Id = 1;
            //Act
            var result = location.AttachDetector(detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public void Location_AttachDetector_ReturnResultOK_v3()
        {
            //Arrange
            var location = A.Fake<Location>();
            location.Detector = A.Fake<Detector>();
            location.Detector.Id = 1;
            location.Detector.State = DetectorState.Off;
            var detector = A.Fake<Detector>();
            detector.Id = 2;
            //Act
            var result = location.AttachDetector(detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            location.Detector.Should().Be(detector);
        }
        [Fact]
        public void Location_AttachDetector_ReturnResultFail()
        {
            //Arrange
            var location = A.Fake<Location>();
            location.Detector = A.Fake<Detector>();
            location.Detector.Id = 1;
            location.Detector.State = DetectorState.Streaming;
            var detector = A.Fake<Detector>();
            detector.Id = 2;
            //Act
            var result = location.AttachDetector(detector);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Location_DetachDetector_ReturnResultOK()
        {
            //Arrange
            var location = A.Fake<Location>();
            location.Detector = A.Fake<Detector>();
            location.Detector.State = DetectorState.Off;
            //Act
            var result = location.DetachDetector();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            location.Detector.Should().BeNull();

        }
        [Fact]
        public void Location_DetachDetector_ReturnResultFail_v1()
        {
            //Arrange
            var location = A.Fake<Location>(); //Detector is null here
            //Act
            var result = location.DetachDetector();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public void Location_DetachDetector_ReturnResultFail_v2()
        {
            //Arrange
            var location = A.Fake<Location>();
            location.Detector = A.Fake<Detector>();
            location.Detector.State = DetectorState.Streaming;
            //Act
            var result = location.DetachDetector();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
