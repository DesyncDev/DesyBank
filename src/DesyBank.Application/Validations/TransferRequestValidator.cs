using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transfer;
using FluentValidation;

namespace DesyBank.Application.Validations
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(fa => fa.FromAccountUserId)
            .NotEmpty();

            RuleFor(ta => ta.ToAccountNumber)
            .NotEmpty();

            RuleFor(a => a.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(1000000);
        }
    }
}