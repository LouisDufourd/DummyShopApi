using DummyShopApi.BLL.Implementation;
using DummyShopApi.DAL;
using DummyShopApi.DAL.DAO.Interfaces;
using DummyShopApi.DAL.Entities;
using DummyShopApi.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TestUnitaire.BLL;

public class SecurityServiceTests
{
    private readonly Mock<IUOW> _uow;
    private readonly Mock<IUserDAO> _userDao;
    private readonly IConfiguration _configuration;

    private readonly SecurityService _service;

    public SecurityServiceTests()
    {
        _userDao = new Mock<IUserDAO>();
        _uow = new Mock<IUOW>();

        _uow.Setup(x => x.User)
            .Returns(_userDao.Object);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "6kMdarlfRQRWA3Sj9/gQEIef6JsYXjctdeGCVVW/nX0=",
                ["Jwt:Issuer"] = "DummyShopApi",
                ["Jwt:ExpireDays"] = "1"
            })
            .Build();

        _service = new SecurityService(_configuration, _uow.Object);
    }

    [Fact]
    public async Task SuccessfulLogin()
    {
        const string username = "admin";
        const string password = "admin";

        var user = new User
        {
            Id = 1,
            FirstName = "admin",
            LastName = "admin",
            Role = ERole.Manager
        };

        _userDao
            .Setup(x => x.Login(username, password))
            .ReturnsAsync(true);

        _userDao
            .Setup(x => x.GetUserByUsername(username))
            .ReturnsAsync(user);

        var token = await _service.SignIn(username, password);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(username, jwt.Subject);

        Assert.Contains(jwt.Claims,
            c => c.Type == ClaimTypes.Role &&
                 c.Value == nameof(ERole.Manager));

        _userDao.Verify(x => x.Login(username, password), Times.Once);
        _userDao.Verify(x => x.GetUserByUsername(username), Times.Once);
    }

    [Fact]
    public async Task UnsuccessfulLogin()
    {
        const string username = "admin";
        const string password = "admin";

        _userDao
            .Setup(x => x.Login(username, password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidLoginException>(
            () => _service.SignIn(username, password));

        _userDao.Verify(x => x.Login(username, password), Times.Once);
        _userDao.Verify(x => x.GetUserByUsername(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UnknownUser_ShouldThrow()
    {
        const string username = "admin";
        const string password = "admin";

        _userDao
            .Setup(x => x.Login(username, password))
            .ReturnsAsync(true);

        _userDao
            .Setup(x => x.GetUserByUsername(username))
            .ThrowsAsync(new NotFoundEntityException(
                "Unable to find user with the specified username"));

        await Assert.ThrowsAsync<NotFoundEntityException>(
            () => _service.SignIn(username, password));

        _userDao.Verify(x => x.Login(username, password), Times.Once);
        _userDao.Verify(x => x.GetUserByUsername(username), Times.Once);
    }
}