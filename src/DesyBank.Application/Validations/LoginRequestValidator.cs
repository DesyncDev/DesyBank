using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Auth.Login;
using FluentValidation;

namespace DesyBank.Application.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(e => e.Email)
            .NotEmpty().WithMessage("Email can't be empty.")
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("Please, put a valid email adress.")
            .MinimumLength(10).WithMessage("Email - Min 10 characters.")
            .MaximumLength(254).WithMessage("Email - Max 254 characters.");

            RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password can't be empty.")
            .MinimumLength(8).WithMessage("Password - Min 8 characters.")
            .MaximumLength(16).WithMessage("Password - Max 16 characters.");
        }
    }
}