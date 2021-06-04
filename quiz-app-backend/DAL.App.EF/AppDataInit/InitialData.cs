namespace DAL.App.EF.AppDataInit
{
    public static class InitialData
    {
        public static string[] Roles { get; } = {"Admin"};

        public static readonly (string name, string password, string firstName, string lastName, string? role)[] Users =
        {
            ("admin@ttu.ee", "Foobar1.", "Admin", "Admin", "Admin"),
            ("mabode@ttu.ee", "Foobar1.", "Marko", "Bode", null),
        };
    }
}