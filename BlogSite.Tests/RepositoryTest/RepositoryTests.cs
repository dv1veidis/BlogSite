using BlogSite.DataAccess;
using BlogSite.DataAccess.Repository;
using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.Tests.RepositoryTest
{
    public class RepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ApplicationDbContext databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (databaseContext.BlogPosts.Count() == 0)
            {
                for (int i = 100; i < 110; i++)
                {
                    databaseContext.BlogPosts.Add(
                    new BlogPost
                    {
                        Id = i,
                        ApplicationUserId = "1",
                        ApplicationUser = new ApplicationUser()
                        {
                            Id = i.ToString(),
                            UserName = "1",
                            NormalizedUserName = "1",
                            Email = "1@email.com",
                            NormalizedEmail = "1@EMAIL.COM",
                            EmailConfirmed = false,
                            PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                            SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                            ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                        },
                        Title = "1",
                        Text = "1",
                        CreatedDateTime = DateTime.Now,
                        CategoryId = i,
                        Category = new Category() { Id = i, Name = "Tomato" }
                    });
                    databaseContext.SaveChanges();
                }

            }
            if(databaseContext.Categories.Count() == 0)
            {
                databaseContext.Categories.Add(
                    new Category
                    {
                        Id = 1,
                        Name = "Potato"
                    });
                databaseContext.SaveChanges();
            }
            if (databaseContext.Reactions.Count() == 0)
            {
                databaseContext.Reactions.Add(
                    new Reaction
                    {
                        Id = 1,
                        ApplicationUserId = "1",
                        Action = "Like",
                        BlogPostId = 101
                    });
                databaseContext.SaveChanges();
            }
            return databaseContext;
        }

        [Fact]
        public async void BlogPostRepository_GetAll_ReturnsCorrectAmountOfItems()
        {
            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);

            //Act
            var result = blogPostRepository.GetAll().Count();

            //Assert
            result.Should().Be(10);
        }

        [Fact]
        public async void BlogPostRepository_Add_ReturnsTheCorrectAmountAfterSave()
        {
            //Arrange
            BlogPost blogPost = new BlogPost()
            {
                Id = 11,
                ApplicationUserId = "2",
                ApplicationUser = new ApplicationUser()
                {
                    Id = "100000",
                    UserName = "2",
                    NormalizedUserName = "2",
                    Email = "2@email.com",
                    NormalizedEmail = "2@EMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                    SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                    ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                },
                Title = "4",
                Text = "4",
                CreatedDateTime = DateTime.Now,
                CategoryId = 2,
                Category = new Category() { Id = 2, Name = "Potato" }
            };

            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);

            //Act
            var resultBefore = blogPostRepository.GetAll().Count();
            blogPostRepository.Add(blogPost);
            await dbContext.SaveChangesAsync();
            var resultAfter = blogPostRepository.GetAll().Count();

            //Assert
            resultBefore.Should().BeLessThan(resultAfter);
        }

        [Theory]
        [InlineData(105, 1)]
        [InlineData(97, 0)]
        public async void BlogPostRepository_GetAllFromId_CorrectAmountOfResultsWhenUsingAnId(int categoryId, int expectedResult)
        {
            //Arrange
            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);

            //Act
            var result = blogPostRepository.GetAllFromId(u => u.CategoryId == categoryId).Count();

            //Assert
            result.Should().Be(expectedResult);

        }
        [Theory]
        [InlineData(105, 105)]
        [InlineData(102, 102)]
        public async void BlogPostRepository_GetFirstOrDefault_CorrectResultWithAnId(int id, int expectedResult)
        {
            //Arrange
            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);

            //Act
            var result = blogPostRepository.GetFirstOrDefault(u => u.Id == id);

            //Assert

            result.CategoryId.Should().Be(expectedResult);
        }

        [Fact]
        public async void BlogRepository_Remove_BlogPostAmmountDecreasesByOne()
        {
            //Arrange
            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);
            BlogPost blogPost = new BlogPost()
            {
                Id = 101,
                ApplicationUserId = "1",
                ApplicationUser = new ApplicationUser()
                {
                    Id = "101",
                    UserName = "1",
                    NormalizedUserName = "1",
                    Email = "1@email.com",
                    NormalizedEmail = "1@EMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                    SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                    ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                },
                Title = "4",
                Text = "4",
                CreatedDateTime = DateTime.Now,
                CategoryId = 101,
                Category = new Category() { Id = 101, Name = "Potato" }
            };

            //Act
            dbContext.ChangeTracker.Clear();
            blogPostRepository.Remove(blogPost);
            await dbContext.SaveChangesAsync();
            var result = blogPostRepository.GetAll().Count();

            //Assert
            result.Should().Be(9);
        }

        [Fact]
        public async void BlogRepository_RemoveRange_BlogPostAmmountDecreasesByTwo()
        {
            //Arrange
            var dbContext = await GetDbContext();

            var blogPostRepository = new BlogPostRepository(dbContext);
            BlogPost blogPost1 = new BlogPost()
            {
                Id = 101,
                ApplicationUserId = "1",
                ApplicationUser = new ApplicationUser()
                {
                    Id = "101",
                    UserName = "1",
                    NormalizedUserName = "1",
                    Email = "1@email.com",
                    NormalizedEmail = "1@EMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                    SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                    ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                },
                Title = "4",
                Text = "4",
                CreatedDateTime = DateTime.Now,
                CategoryId = 101,
                Category = new Category() { Id = 101, Name = "Potato" }
            };
            BlogPost blogPost2 = new BlogPost()
            {
                Id = 102,
                ApplicationUserId = "1",
                ApplicationUser = new ApplicationUser()
                {
                    Id = "102",
                    UserName = "1",
                    NormalizedUserName = "1",
                    Email = "1@email.com",
                    NormalizedEmail = "1@EMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                    SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                    ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                },
                Title = "4",
                Text = "4",
                CreatedDateTime = DateTime.Now,
                CategoryId = 102,
                Category = new Category() { Id = 102, Name = "Potato" }
            };
            IEnumerable<BlogPost> blogPosts = new List<BlogPost>(){blogPost1, blogPost2};

            //Act
            dbContext.ChangeTracker.Clear();
            blogPostRepository.RemoveRange(blogPosts);
            await dbContext.SaveChangesAsync();
            var result = blogPostRepository.GetAll().Count();

            //Assert
            result.Should().Be(8);
        }
        [Fact]
        public async void BlogPostRepository_Update_BlogPostChangesMatchGivenParameters()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var blogPostRepository = new BlogPostRepository(dbContext);
            int id = 101;
            int categoryId = 101;
            string title = "Changed";
            string text = "Changed";
            string email = "Changed@email.com";
            BlogPost blogPost = new BlogPost()
            {
                Id = id,
                ApplicationUserId = "1",
                ApplicationUser = new ApplicationUser()
                {
                    Id = id.ToString(),
                    UserName = "1",
                    NormalizedUserName = "1",
                    Email = email,
                    NormalizedEmail = "1@EMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEHffxrEospJQ/wG1HQKzOhwZH3Twkh76yizQjIkyIydL5OU8UA/O8PvFz+6D4k4L9w==",
                    SecurityStamp = "ALPL7NR3CKKG5LKYEW6T3LT7HX2GR6L3",
                    ConcurrencyStamp = "92c22df0-54d9-41d1-96ab-8b9ec26144ea",
                },
                Title = title,
                Text = text,
                CreatedDateTime = DateTime.Now,
                CategoryId = categoryId,
                Category = new Category() { Id = 102, Name = "Potato" }
            };

            //Act
            blogPostRepository.Update(blogPost);
            var result = blogPostRepository.GetFirstOrDefault(u => u.Id == id);

            //Assert
            result.Title.Should().Be(title);
            result.Text.Should().Be(text);
            result.CategoryId.Should().Be(categoryId);
            result.ApplicationUser.Email.Should().Be("1@email.com");
        }

        [Fact]
        public async void Reaction_Update_ReactionChangesMatchGivenParameters()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var reactionRepository = new ReactionRepository(dbContext);
            int id = 1;
            Reaction reaction = new Reaction()
            {
                Id = id,
                ApplicationUserId = "Changed",
                Action = "Dislike",
                BlogPostId = 105
            };
            dbContext.ChangeTracker.Clear();

            //Act
            reactionRepository.Update(reaction);
            var result = reactionRepository.GetFirstOrDefault(u => u.Id == id);

            //Assert
            result.ApplicationUserId.Should().Be("1");
            result.Action.Should().Be("Dislike");
            result.BlogPostId.Should().Be(101);


        }
    }
}
