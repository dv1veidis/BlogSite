using BlogSite.Areas.User.Controllers;
using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using BlogSite.Models.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlogSite.Tests.ControllerTest
{
    public class BlogControllerTests
    {
        private IUnitOfWork _unitOfWork;

        public BlogControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
        }

        [Fact]
        public void BlogController_Blogs_ReturnsBlogOptionVM()
        {
            //Arrange
            var blogPosts = new List<BlogPost>();
            var categories = new List<Category>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetAll(It.IsAny<string>()))
                .Returns(blogPosts); // Set up mock behavior for BlogPost.GetAll
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);
            var controller = new BlogController(unitOfWorkMock.Object);

            //Act

            var result = controller.Blogs();

            //Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<BlogOptionVM>(viewResult.Model);

        }
        [Fact]
        public void BlogController_Details_ReturnsBlogVM()
        {
            //Arrange
            var blogPost = new BlogPost();
            var categories = new List<Category>();
            var reactions = new List<Reaction>();
            var id = 1;
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetFirstOrDefault(It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<string>()))
                .Returns(blogPost);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);
            unitOfWorkMock.Setup(uow => uow.Reaction.GetAll(It.IsAny<string>()))
                .Returns(reactions);
            var controller = new BlogController(unitOfWorkMock.Object);

            //Act
            var result = controller.Details(id);

            //Assert

            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<BlogVM>(viewResult.Model);

        }
        [Fact]
        public void BlogController_UserBlogs_ReturnsToIndexWhenClaimIsWrong()
        {
            //Arrange
            var blogPost = new BlogPost();
            var categories = new List<Category>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetFirstOrDefault(It.IsAny<Expression<Func<BlogPost, bool>>>(),
            It.IsAny<string>()))
            .Returns(blogPost);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);
            var controller = new BlogController(unitOfWorkMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "John Doe"),
                        new Claim(ClaimTypes.Role, "Admin")
                    }))
                }
            };

            //Act
            var result = controller.UserBlogs();

            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("Index", redirectResult.Url);
        }
        [Fact]
        public void BlogController_UserBlogs_RedirectsToIndexWhenClaimValueNotNull()
        {
            // Arrange
            var blogPost = new BlogPost();
            var categories = new List<Category>();
            var claimValue = "123";
            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, claimValue)
            });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetFirstOrDefault(It.IsAny<Expression<Func<BlogPost, bool>>>(),
            It.IsAny<string>()))
            .Returns(blogPost);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);

            var controller = new BlogController(unitOfWorkMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = controller.UserBlogs();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<BlogOptionVM>(viewResult.Model);
        }
        [Fact]
        public void BlogController_SearchedBlogs_ReturnsCorrectViewWhenUsingParameters()
        {
            //Arrange
            int CategoryListId = 1;
            string name = "John";
            var blogPosts = new List<BlogPost>();
            var applicationUser = new ApplicationUser();
            var categories = new List<Category>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetAllFromId(It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<string>()))
                .Returns(blogPosts);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);
            unitOfWorkMock.Setup(uow => uow.ApplicationUser.GetFirstOrDefault(It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<string>()))
                .Returns(applicationUser);

            var controller = new BlogController(unitOfWorkMock.Object);

            //Act
            var result = controller.SearchedBlogs(CategoryListId, name);

            //Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<BlogOptionVM>(viewResult.Model);
        }
        [Theory]
        [InlineData(null, null)]
        [InlineData(0, null)]
        public void BlogController_SearchedBlogs_RedirectsCorrectlyWithBadParameters(int a, string b)
        {
            //Arrange
            var blogPosts = new List<BlogPost>();
            var applicationUser = new ApplicationUser();
            var categories = new List<Category>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetAllFromId(It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<string>()))
                .Returns(blogPosts);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);
            unitOfWorkMock.Setup(uow => uow.ApplicationUser.GetFirstOrDefault(It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<string>()))
                .Returns(applicationUser);


            var controller = new BlogController(unitOfWorkMock.Object);

            //Act
            var result = controller.SearchedBlogs(a, b);

            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("Blogs", redirectResult.Url);
        }

        [Fact]
        public void BlogController_UserBlogsByCategory_CorrectViewIfClaimExists()
        {
            //Arrange
            var blogPosts = new List<BlogPost>();
            var categories = new List<Category>();
            var claimValue = "123";
            int categoryListId = 1;
            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, claimValue)
            });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetAllFromId(It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<string>()))
                .Returns(blogPosts);
            unitOfWorkMock.Setup(uow => uow.Category.GetAll(It.IsAny<string>()))
                .Returns(categories);

            var controller = new BlogController(unitOfWorkMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var result = controller.UserBlogsByCategory(categoryListId);

            //Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsType<BlogOptionVM>(viewResult.Model);
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData("Like", 1)]
        public void BlogController_Reaction_CorrectRedirectIfIdAndClaimCorrect(string? react, int id)
        {
            //Arrange
            var reactions = new Reaction();
            var claimValue = "123";
            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, claimValue)
            });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Reaction.GetFirstOrDefault(It.IsAny<Expression<Func<Reaction, bool>>>(),
                It.IsAny<string>()))
                .Returns(reactions);

            var controller = new BlogController(unitOfWorkMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var result = controller.Reaction(react, 1);

            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("Details?blogPostId="+id, redirectResult.Url);
        }

        [Theory]
        [InlineData(1, true, "Delete Successful")]
        public void BlogController_Delete_CorrectJsonWhenDeletionIsCalled(int? id, bool successExpected, string messageExpected)
        {
            //Arrange
            BlogPost blogPost = new BlogPost();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.BlogPost.GetFirstOrDefault(It.IsAny<Expression<Func<BlogPost, bool>>>(),
            It.IsAny<string>()))
            .Returns(blogPost);
            var controller = new BlogController(unitOfWorkMock.Object);

            //Act
            var result = controller.Delete(id);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonValue = new { success = successExpected, message = messageExpected };
            var expectedJson = JsonSerializer.Serialize(jsonValue);
            var actualJson = JsonSerializer.Serialize(jsonResult.Value);
            Assert.Equal(expectedJson, actualJson);

        }

    }
}
