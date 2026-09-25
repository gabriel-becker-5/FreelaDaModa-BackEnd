namespace _02_Application.Enums
{
    public enum CreateUserStatus
    {
        Success,
        EmailInUse,
        CpfInUse,
        CnpjInUse,
        InvalidData,
        DatabaseError,
        InvalidCPF,
        InvalidCNPJ
    }
}