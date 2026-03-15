using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Auth;
using FluentValidation;

namespace DesyBank.Application.Validations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(fn => fn.FullName)
            .NotEmpty().WithMessage("Your name can't be empty.")
            .MinimumLength(5).WithMessage("FullName - Min 5 characters.")
            .MaximumLength(30).WithMessage("FullName - Max 30 characters.");

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