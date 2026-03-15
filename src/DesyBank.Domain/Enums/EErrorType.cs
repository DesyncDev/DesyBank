namespace DesyBank.Domain.Enums
{
    public enum EErrorType
    {
        ValidationError = 1,
        BusinessRuleError = 2,
        NotFoundError = 3,
        ConflictError = 4,
        UnauthorizedError = 5,
        ForbiddenError = 6,
        InfrastructureError = 7
    }
}