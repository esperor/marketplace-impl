namespace course.Server.Configs.Enums
{
    [Flags]
    public enum EAccessTrait
    {
        Seller = 0x1,
        Client = 0x1 << 1,
    }
}
