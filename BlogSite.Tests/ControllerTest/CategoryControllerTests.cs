using BlogSite.Areas.Admin.Controllers;
using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using BlogSite.Models.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.Tests.ControllerTest
{
    public class CategoryControllerTests
    {
        private IUnitOfWork _unitOfWork;

        public CategoryControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
        }
        [Fact]
        public void CategoryController_Edit_ReturnsCorrectViewIfIDCorrect()
        {
            //Arrange
            var id = 1;
            var category = new Category();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string>()))
                .Returns(category);
            var controller = new CategoryController(unitOfWorkMock.Object);

            //Act
            var result = controller.Edit(id);

            //Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<Category>(viewResult.Model);
        }

        [Fact]
        public void CategoryController_Delete_CorrectViewOnSuccess()
        {
            //Arrange
            var id = 1;
            var category = new Category();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string>()))
                .Returns(category);
            var controller = new CategoryController(unitOfWorkMock.Object);

            //Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<Category>(viewResult.Model);
        }
        public void CategoryController_Edit_CorrectRedirectWhenModelStateIsValid()
        {
            //Arrange
            var category = new Category();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string>()))
                .Returns(category);
            var controller = new CategoryController(unitOfWorkMock.Object);

            //Act
            var result = controller.Edit(category);

            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("Index", redirectResult.Url);
        }
    }
}
