using FluentAssertions;
using NSubstitute;
using ZeroBudget.Application.Features.Auth.Login;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly IAuthSettings _authSettings = Substitute.For<IAuthSettings>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _authSettings.Email.Returns("admin@example.com");
        _authSettings.Password.Returns("secret");
        _sut = new LoginCommandHandler(_authSettings, _jwt);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var expiry = DateTime.UtcNow.AddDays(7);
        _jwt.Generate("admin@example.com").Returns(("jwt-token", expiry));

        var result = await _sut.Handle(new LoginCommandInput("admin@example.com", "secret"), default);

        result.Token.Should().Be("jwt-token");
        result.ExpiresAt.Should().Be(expiry);
        result.Email.Should().Be("admin@example.com");
    }

    [Fact]
    public async Task Handle_WrongEmail_ThrowsUnauthorizedException()
    {
        var act = () => _sut.Handle(new LoginCommandInput("other@example.com", "secret"), default);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedException()
    {
        var act = () => _sut.Handle(new LoginCommandInput("admin@example.com", "wrong"), default);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
