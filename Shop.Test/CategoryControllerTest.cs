using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Shop.Api.Controllers;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Category.GetCategoryById;

namespace Shop.Test;

public class CategoryControllerTest
{
    [Fact]
    public async Task GetCategoryById_ReturnsOk_WhenCategoryExists()
    {
        //A - Arrange
        var mediatorMock = new Mock<IMediator>();
        var category = new CategoryReadDTO
        {
            Id = 1,
            Name = "Test category",
            Slug = "test-category"
        };

        mediatorMock
            .Setup(mediator => mediator.Send(
                It.Is<GetCategoryByIdQuery>(query => query.id == 1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var controller = new CategoryController(
            Mock.Of<ICategoryService>(),
            Mock.Of<IImageService>(),
            Mock.Of<IConfiguration>(),
            mediatorMock.Object,
            Mock.Of<IValidator<CategoryCreateDTO>>());

        //A - Act
        var result = await controller.GetCategoryById(1, CancellationToken.None);

        //A - Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCategory = Assert.IsType<CategoryReadDTO>(okResult.Value);
        Assert.Equal("Test category", returnedCategory.Name);
    }
}
