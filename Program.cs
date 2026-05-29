namespace KidsComputerTimeGuard;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var mode = ParseLaunchMode(args);

        if (mode == LaunchMode.Parent)
        {
            if (!ParentAuth.TryAuthenticate())
                return;

            Application.Run(new ParentConfigForm());
            return;
        }

        using var mutex = new Mutex(true, "KidsComputerTimeGuard_ChildMode", out var createdNew);
        if (!createdNew)
            return;

        Application.Run(new ChildModeContext());
    }

    private static LaunchMode ParseLaunchMode(string[] args)
    {
        foreach (var arg in args)
        {
            var a = arg.Trim().ToLowerInvariant();
            if (a is "--parent" or "-parent" or "/parent" or "parent")
                return LaunchMode.Parent;
            if (a is "--child" or "-child" or "/child" or "child")
                return LaunchMode.Child;
        }

        return LaunchMode.Child;
    }

    private enum LaunchMode
    {
        Parent,
        Child
    }
}
