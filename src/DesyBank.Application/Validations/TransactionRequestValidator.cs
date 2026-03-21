using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transaction;
using FluentValidation;

namespace DesyBank.Application.Validations
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
    {
        public TransactionRequestValidator()
        {
            RuleFor(x => x.UserId)
            .NotEmpty();

            RuleFor(a => a.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(1000000);

            RuleFor(t => t.Type)
            .NotEmpty();
        }
    }
}