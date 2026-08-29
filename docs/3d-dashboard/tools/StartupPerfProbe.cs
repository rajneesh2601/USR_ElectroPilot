using System;
using System.Diagnostics;
using System.Windows.Forms;
using USR_ElectroPilot.Controls;
using USR_ElectroPilot.Data;

internal static class StartupPerfProbe
{
    [STAThread]
    private static int Main()
    {
        var sw = Stopwatch.StartNew();
        DatabaseHelper.InitializeDatabase();
        sw.Stop();
        Console.WriteLine("first_initialize_ms=" + sw.ElapsedMilliseconds);

        sw.Restart();
        for (var i = 0; i < 25; i++)
        {
            DatabaseHelper.InitializeDatabase();
        }

        sw.Stop();
        Console.WriteLine("next_25_initializes_ms=" + sw.ElapsedMilliseconds);

        Application.EnableVisualStyles();
        sw.Restart();
        using (var control = new Plant3DHostControl())
        {
            control.Width = 1500;
            control.Height = 900;
            control.CreateControl();
        }

        sw.Stop();
        Console.WriteLine("plant3d_host_eager_create_ms=" + sw.ElapsedMilliseconds);

        sw.Restart();
        using (var control = new Plant3DHostControl(true))
        {
            control.Width = 1500;
            control.Height = 900;
            control.CreateControl();
        }

        sw.Stop();
        Console.WriteLine("plant3d_host_deferred_create_ms=" + sw.ElapsedMilliseconds);
        return 0;
    }
}
