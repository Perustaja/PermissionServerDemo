using System;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Authorization;
using PermissionServerDemo.Identity.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PermissionServer;

namespace UnitTests
{
    public class RemoteAuthorizationEvaluatorTests
    {
        [Theory]
        [InlineData("awdoinwadin")]
        [InlineData("")]
        public async void Returns_Unauthorized_If_Perms_Invalid(string invalidPerm)
        {
            // Enum is not a valid enum, it should be parsed prior and cause an error
            var expected = new AuthorizeDecision() { Allowed = false, FailureReason = AuthorizeFailureReason.PermissionFormat };

            // OrganizationManager says that the user has access and the org exists
            var orgManMock = new Mock<IOrganizationManager>();
            orgManMock.Setup(m => m.UserHasAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            orgManMock.Setup(m => m.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            // PermissionService says that the user has permission(s)
            var permSvcMock = AlwaysXPermissionService(true);

            var evaluator = new AuthorizationEvaluator(MockAuthLogger(), orgManMock.Object, permSvcMock.Object);
            var actual = await evaluator.EvaluateAsync(GuidAsStr(), GuidAsStr(), invalidPerm);

            Assert.True(DecisionsAreEqual(expected, actual));
        }

        [Fact]
        public async void Returns_Unauthorized_If_User_Doesnt_Have_Perms()
        {
            var expected = new AuthorizeDecision() { Allowed = false, FailureReason = AuthorizeFailureReason.Unauthorized };

            // OrganizationManager says the user has access and the org exists
            var orgManMock = new Mock<IOrganizationManager>();
            orgManMock.Setup(m => m.UserHasAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            orgManMock.Setup(m => m.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            // but the Permission Service says it does not have permission
            var permSvcMock = AlwaysXPermissionService(false);

            var evaluator = new AuthorizationEvaluator(MockAuthLogger(), orgManMock.Object, permSvcMock.Object);
            var actual = await evaluator.EvaluateAsync(GuidAsStr(), GuidAsStr(), PermissionEnum.AircraftCreate.ToString());

            Assert.True(DecisionsAreEqual(expected, actual));
        }

        [Fact]
        public async void Returns_Authorized_If_User_Has_Perms()
        {
            var expected = new AuthorizeDecision() { Allowed = true };

            // OrganizationManager says the user has access and the org exists
            var orgManMock = new Mock<IOrganizationManager>();
            orgManMock.Setup(m => m.UserHasAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            orgManMock.Setup(m => m.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            // And the PermissionService says it has the given permission
            var permSvcMock = AlwaysXPermissionService(true);

            var evaluator = new AuthorizationEvaluator(MockAuthLogger(), orgManMock.Object, permSvcMock.Object);
            var actual = await evaluator.EvaluateAsync(GuidAsStr(), GuidAsStr(), PermissionEnum.AircraftCreate.ToString());

            Assert.True(DecisionsAreEqual(expected, actual));
        }

        [Fact]
        public async void Returns_Authorized_If_User_Has_Access_And_No_Perms_Specified()
        {
            var expected = new AuthorizeDecision() { Allowed = true };

            // OrganizationManager says the user has access and the org exists
            var orgManMock = new Mock<IOrganizationManager>();
            orgManMock.Setup(m => m.UserHasAccessAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            orgManMock.Setup(m => m.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            // And the PermissionService says it has the given permission
            var permSvcMock = AlwaysXPermissionService(true);

            var evaluator = new AuthorizationEvaluator(MockAuthLogger(), orgManMock.Object, permSvcMock.Object);
            var actual = await evaluator.EvaluateAsync(GuidAsStr(), GuidAsStr());

            Assert.True(DecisionsAreEqual(expected, actual));
        }

        private string GuidAsStr() => Guid.NewGuid().ToString();
        private bool DecisionsAreEqual(AuthorizeDecision dec1, AuthorizeDecision dec2)
            => (dec1.Allowed == dec2.Allowed && dec1.FailureReason == dec2.FailureReason);

        private Mock<IPermissionService> AlwaysXPermissionService(bool x)
        {
            var permSvcMock = new Mock<IPermissionService>();
            permSvcMock.Setup(m => m.UserHasPermissionsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string[]>())).ReturnsAsync(x);
            return permSvcMock;
        }

        private ILogger<AuthorizationEvaluator> MockAuthLogger() 
            => new Mock<ILogger<AuthorizationEvaluator>>().Object;
    }
}
