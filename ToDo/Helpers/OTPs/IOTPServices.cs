namespace ToDo.Helpers.OTPs
{
    public interface IOTPServices
    {
        Task<string> GenerateSecretKey(int UserId);
    }
}
