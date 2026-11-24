using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayLink.Api.Controllers;

namespace PayLink.Api.Tests;

public class HealthControllerTests
{
    [Fact]
    public void GetHealth_ReturnsOkWithExpectedPayload()
    {
        // Arrange
        var beforeUtc = DateTime.UtcNow.AddSeconds(-1);
        var controller = new HealthController();

        // Act
        IActionResult result = controller.GetHealth();

        // Assert
        var afterUtc = DateTime.UtcNow.AddSeconds(1);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(okResult.Value);

        var json = JsonSerializer.Serialize(okResult.Value);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(200, root.GetProperty("status").GetInt32());
        Assert.Equal("PayLink APP", root.GetProperty("service").GetString());

        var timestamp = root.GetProperty("timestamp").GetDateTime();
        Assert.InRange(timestamp, beforeUtc, afterUtc);
    }
}