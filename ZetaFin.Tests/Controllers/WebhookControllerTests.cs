using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZetaFin.API.Controllers;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.Tests.Controllers;
public class WebhookControllerTests
{
    [Fact]
    public async Task HandlePixNotification_ShouldReturnOk_WhenGoalExists()
    {
        // Arrange
        var mockGoalService = new Mock<IGoalService>();

        var goalId = Guid.NewGuid();
        var notification = new MercadoPagoPixNotification
        {
            GoalId = goalId,
            Amount = 150.00m,
            PixKey = "teste@pix.com",
            TransactionId = "123456789"
        };

        // Simula que a meta existe
        mockGoalService.Setup(s => s.GetGoalByIdAsync(goalId))
            .ReturnsAsync(new GoalDto { Id = goalId, Description = "Viagem", TargetAmount = 1000 });

        // Simula que o depósito será adicionado sem erro
        mockGoalService.Setup(s => s.AddDepositToGoalAsync(goalId, notification.Amount))
            .Returns(Task.CompletedTask);

        var controller = new WebhookController(mockGoalService.Object);

        // Act
        var result = await controller.HandlePixNotification(notification);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Deposit registered successfully.", okResult.Value);
    }

    [Fact]
    public async Task HandlePixNotification_ShouldReturnNotFound_WhenGoalDoesNotExist()
    {
        // Arrange
        var mockGoalService = new Mock<IGoalService>();

        var goalId = Guid.NewGuid();
        var notification = new MercadoPagoPixNotification
        {
            GoalId = goalId,
            Amount = 150.00m,
            PixKey = "teste@pix.com",
            TransactionId = "123456789"
        };

        // Simula que a meta NÃO existe
        mockGoalService.Setup(s => s.GetGoalByIdAsync(goalId))
            .ReturnsAsync((GoalDto?)null);

        var controller = new WebhookController(mockGoalService.Object);

        // Act
        var result = await controller.HandlePixNotification(notification);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Goal not found.", notFoundResult.Value);
    }
}
