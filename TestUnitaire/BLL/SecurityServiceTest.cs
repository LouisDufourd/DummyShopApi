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


public class SecurityServiceTest
{
    [Fact]
    public async Task SuccesfullLogin()
    {
        string username = "admin";
        string password = "admin";

        var user = new User()
        {
            Id = 1,
            FirstName = "admin",
            LastName = "admin",
            Role = ERole.Manager
        };

        IUserDAO userDao = Mock.Of<IUserDAO>();
        Mock.Get(userDao)
            .Setup(userDao => userDao.Login(username, password))
            .ReturnsAsync(() => true);
        Mock.Get(userDao)
            .Setup(userDao => userDao.GetUserByUsername(username))
            .ReturnsAsync(user);

        IUOW UOW = Mock.Of<IUOW>();
        Mock.Get(UOW)
            .Setup(UOW => UOW.User)
            .Returns(userDao);

        IConfiguration configuration = Mock.Of<IConfiguration>();
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:Key"])
            .Returns("6kMdarlfRQRWA3Sj9/gQEIef6JsYXjctdeGCVVW/nX0=");
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:ExpireDays"])
            .Returns("1");
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:Issuer"])
            .Returns("DummyShopApi");

        var service = new SecurityService(configuration, UOW);

        string token = await service.SignIn(username, password);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(username, jwt.Subject);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Manager");

        return;
    }

    [Fact]
    public async Task UnsuccessfullLogin()
    {
        string username = "admin";
        string password = "admin";

        var user = new User()
        {
            Id = 1,
            FirstName = "admin",
            LastName = "admin",
            Role = ERole.Manager
        };

        IUserDAO userDao = Mock.Of<IUserDAO>();
        Mock.Get(userDao)
            .Setup(userDao => userDao.Login(username, password))
            .ReturnsAsync(() => false);
        Mock.Get(userDao)
            .Setup(userDao => userDao.GetUserByUsername(username))
            .ReturnsAsync(user);

        IUOW UOW = Mock.Of<IUOW>();
        Mock.Get(UOW)
            .Setup(UOW => UOW.User)
            .Returns(userDao);

        IConfiguration configuration = Mock.Of<IConfiguration>();

        var service = new SecurityService(configuration, UOW);

        await Assert.ThrowsAsync<InvalidLoginException>(() => service.SignIn(username, password));
    }

    [Fact]
    public async Task UnkownUser()
    {
        //NotFoundEntityException
        string username = "admin";
        string password = "admin";

        var user = new User()
        {
            Id = 1,
            FirstName = "admin",
            LastName = "admin",
            Role = ERole.Manager
        };

        IUserDAO userDao = Mock.Of<IUserDAO>();
        Mock.Get(userDao)
            .Setup(userDao => userDao.Login(username, password))
            .ReturnsAsync(() => false);
        Mock.Get(userDao)
            .Setup(userDao => userDao.GetUserByUsername(username))
            .ThrowsAsync(new NotFoundEntityException("Unable to find user with the specidied username"));

        IUOW UOW = Mock.Of<IUOW>();
        Mock.Get(UOW)
            .Setup(UOW => UOW.User)
            .Returns(userDao);

        IConfiguration configuration = Mock.Of<IConfiguration>();

        var service = new SecurityService(configuration, UOW);

        await Assert.ThrowsAsync<InvalidLoginException>(() => service.SignIn(username, password));
    }

    /*public async Task TokenGeneration()
    {
        IUOW UOW = Mock.Of<IUOW>();

        IConfiguration configuration = Mock.Of<IConfiguration>();
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:Key"])
            .Returns("6kMdarlfRQRWA3Sj9/gQEIef6JsYXjctdeGCVVW/nX0=");
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:ExpireDays"])
            .Returns("1");
        Mock.Get(configuration)
            .Setup(configuration => configuration["Jwt:Issuer"])
            .Returns("DummyShopApi");

        var service = new SecurityService(configuration, UOW);
    }*/
}
