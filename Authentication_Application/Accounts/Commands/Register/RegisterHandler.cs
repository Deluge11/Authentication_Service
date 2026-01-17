
using Authentication_Core.Entities;
using Authentication_Core.Interfaces;
using MediatR;
using ConstantsLib.Events;
using ConstantsLib.Enums;
using ConstantsLib.Interfaces;
using ConstantsLib.Exchanges;

namespace Authentication_Application.Accounts.Commands.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, string>
    {
        public RegisterHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ITokenService tokenService,
            IEventBus eventBus
        )
        {
            UnitOfWork = unitOfWork;
            IdentityService = identityService;
            TokenService = tokenService;
            EventBus = eventBus;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IIdentityService IdentityService { get; }
        public ITokenService TokenService { get; }
        public IEventBus EventBus { get; }

        public async Task<string> Handle(RegisterCommand request, CancellationToken ct)
        {
            if (request.password != request.confirmPassword)
                return null;

            var accountExists = await UnitOfWork.Accounts.Exists(e => e.Email == request.email);
            if (accountExists)
                return null;

            var hashedPassword = IdentityService.GenerateHash(request.password);
            var defaultRole = await UnitOfWork.Roles.Find(r => r.Id == (int)EnRole.Customer);

            if (defaultRole != null)
            {
                UnitOfWork.Roles.Attach(defaultRole);
            }

            var newAccount = new Account
            {
                Name = request.name,
                Email = request.email,
                Password = hashedPassword,
                Roles = defaultRole == null ? new List<Role>() : new List<Role> { defaultRole }
            };

            await UnitOfWork.Accounts.Add(newAccount);

            if (await UnitOfWork.Complete() == 0)
                return null;

            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = newAccount.Id,
                Name = newAccount.Name,
                Email = newAccount.Email,
            };

            await EventBus.Publish(userCreatedEvent);

            var permissions = await UnitOfWork.Permissions.GetByUserId(newAccount.Id);

            return TokenService.GenerateJwtToken(newAccount.Id, permissions);
        }
    }
}
