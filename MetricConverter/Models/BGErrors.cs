using Microsoft.AspNetCore.Identity;
namespace MetricConverter.Models
{
    public class BGErrors : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        => new IdentityError { Code = nameof(PasswordTooShort), Description = $"Паролата трябва да бъде ПОНЕ {length} символа." };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Паролата трябва да съдържа поне една МАЛКА БУКВА." };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Паролата трябва да съдържа поне една ГЛАВНА БУКВА." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Паролата трябва да съдържа поне един СПЕЦИАЛЕН СИМВОЛ." };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Паролата трябва да съдържа поне една ЦИФРА." };

        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError { Code = nameof(DuplicateUserName), Description = $"Потребител с име '{userName}' ВЕЧЕ съществува." };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError { Code = nameof(InvalidUserName), Description = $"Потребителското име '{userName}' е НЕВАЛИДНО." };
    }
}
