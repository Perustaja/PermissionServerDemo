using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CoreMultiTenancy.Identity.Services
{
    /// <summary>
    /// Required for Identity, even if not used directly within the application code.
    /// </summary>
    public class UserSignInManager : SignInManager<User>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserSignInManager(
        UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            ApplicationDbContext dbContext,
            IAuthenticationSchemeProvider schemeProvider,
            IUserConfirmation<User> userConfirmation
            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, userConfirmation)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
    }
}