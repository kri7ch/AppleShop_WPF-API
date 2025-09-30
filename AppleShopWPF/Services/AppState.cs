namespace AppleShopWPF.Services
{
    public static class AppState
    {
        public static int CurrentUserId { get; set; }
        public static bool IsLoggedIn => CurrentUserId > 0;
    }
}