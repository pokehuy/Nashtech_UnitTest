using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using asp_a2.Controllers;
using asp_a2.Repository;
using System.Collections.Generic;
using asp_a2.Models;
using Microsoft.AspNetCore.Mvc;

namespace UTest;

public class PeopleControllerTest
{
    private Mock<ILogger<PeopleController>> _loggerMock;
    private Mock<IPerson> _peopleMock;
    private static List<PersonModel> _people = new List<PersonModel>(){
        new PersonModel{
                Id = 1,
                FirstName = "01",
                LastName = "Test",
                Gender = "Male",
                DateOfBirth = 2001,
                PhoneNumber = "0123456778",
                BirthPlace = "Ha noi",
                IsGraduated = false
            },
            new PersonModel{
                Id = 2,
                FirstName = "02",
                LastName = "Test",
                Gender = "Male",
                DateOfBirth = 1999,
                PhoneNumber = "01234545667",
                BirthPlace = "Nam dinh",
                IsGraduated = false
            },
            new PersonModel{
                Id = 3,
                FirstName = "03",
                LastName = "Test",
                Gender = "Female",
                DateOfBirth = 1999,
                PhoneNumber = "01298332132",
                BirthPlace = "Thanh hoa",
                IsGraduated = true
            }
    };
    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<PeopleController>>();
        _peopleMock = new Mock<IPerson>();
    }

    [Test]
    public void Index_ReturnsViewResult_GetAListOfPeople()
    {
        //Setup
        _peopleMock.Setup(p => p.List()).Returns(_people);

        //Arrange
        var controller = new PeopleController(_loggerMock.Object, _peopleMock.Object);

        //Act
        var result = controller.Index();

        //Assert
        var view = result as ViewResult;
        Assert.IsInstanceOf<ViewResult>(result, "Index action should return a ViewResult");
        Assert.IsNotNull(result, "Index action should return a ViewResult");
        var model = view?.ViewData.Model as List<PersonModel>;
        Assert.IsAssignableFrom<List<PersonModel>>(model, "Index action should return a list model");
        Assert.AreEqual(3, model?.Count, "Index action should return a list of 3 people");
    }

    [Test]
    public void Details_ReturnsViewResult_GetAPerson()
    {
        //Setup
        int index = 1;
        _peopleMock.Setup(p => p.GetOne(index)).Returns(_people[0]);

        //Arrange
        var controller = new PeopleController(_loggerMock.Object, _peopleMock.Object);

        //Act
        var result = controller.Details(1);

        //Assert
        var view = result as ViewResult;
        Assert.IsInstanceOf<ViewResult>(result, "Details action should return a ViewResult");
        Assert.IsNotNull(result, "Details action should return a ViewResult");
        var model = view?.ViewData.Model as PersonModel;
        Assert.IsAssignableFrom<PersonModel>(model, "Details action should return a person model");
        Assert.AreEqual(1, model?.Id, "Details action should return a person with id = 1");
    }

    [Test]
    public void Create_ReturnsViewResult_GetAPerson()
    {
        //Setup
        PersonModel per = new PersonModel{
                Id = 1,
                FirstName = "01",
                LastName = "Test",
                Gender = "Male",
                DateOfBirth = 2000,
                PhoneNumber = "0123456789",
                BirthPlace = "Hanoi",
                IsGraduated = false
        };
        _peopleMock.Setup(p => p.Create(per)).Callback<PersonModel>((PersonModel per) => _people.Add(per));

        //Arrange
        var expected = _people.Count + 1;
        var controller = new PeopleController(_loggerMock.Object, _peopleMock.Object);

        //Act
        var result = controller.Add(per);
        var actual = _people.Count;

        //Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result, "Add action should return a RedirectToActionResult");
        Assert.IsNotNull(result, "Add action should return a RedirectToActionResult");
        var view = result as RedirectToActionResult;
        Assert.AreEqual("Index", view.ActionName, "Add action should return a RedirectToActionResult to Index action");
    }

    [Test]
    public void Edit_ReturnsViewResult_GetAPerson()
    {
        //Setup
        PersonModel per = new PersonModel{
                Id = 1,
                FirstName = "01",
                LastName = "Test Edit",
                Gender = "Male",
                DateOfBirth = 2000,
                PhoneNumber = "0123456789",
                BirthPlace = "Hanoi",
                IsGraduated = false
        };
        _peopleMock.Setup(p => p.Update(per)).Callback<PersonModel>((PersonModel per) => _people[0] = per);

        //Arrange
        var expectedName = per.LastName;
        var expectedCount = _people.Count;
        var controller = new PeopleController(_loggerMock.Object, _peopleMock.Object);

        //Act
        var result = controller.Edit(per);
        var actualName = _people[0].LastName;
        var actualCount = _people.Count;

        //Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result, "Edit action should return a RedirectToActionResult");
        Assert.IsNotNull(result, "Edit action should not null");
        var view = result as RedirectToActionResult;
        Assert.AreEqual("Index", view.ActionName, "Edit action should return a RedirectToActionResult to Index action");
        Assert.AreEqual(expectedName, actualName, "Edit action should update the person's last name");
        Assert.AreEqual(expectedCount, actualCount, "Edit action should not add a new person");
    }

    [TearDown]
    public void TearDown()
    {
        _loggerMock = null;
        _peopleMock = null;
    }
}