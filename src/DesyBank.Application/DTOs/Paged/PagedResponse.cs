using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.Paged
{
    public sealed record PagedResponse<T>
    (
        IReadOnlyList<T> Items,
        int Page,
        int TotalPages,
        int TotalRegisters
    );
}