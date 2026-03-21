using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.Transaction
{
    public sealed record OperationRequest(
        decimal Amount
    );
}