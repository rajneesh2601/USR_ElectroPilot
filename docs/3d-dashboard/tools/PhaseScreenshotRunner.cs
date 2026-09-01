using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using USR_ElectroPilot.Controls;
using USR_ElectroPilot.Forms;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;
using USR_ElectroPilot.ThreeD.Views;

internal static class PhaseScreenshotRunner
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: PhaseScreenshotRunner <appOutputDirectory> <screenshotPath>");
            return 2;
        }

        var appOutputDirectory = Path.GetFullPath(args[0]);
        var screenshotPath = Path.GetFullPath(args[1]);
        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));

        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs eventArgs)
        {
            var assemblyName = new AssemblyName(eventArgs.Name).Name;
            var dllCandidate = Path.Combine(appOutputDirectory, assemblyName + ".dll");
            if (File.Exists(dllCandidate))
            {
                return Assembly.LoadFrom(dllCandidate);
            }

            var exeCandidate = Path.Combine(appOutputDirectory, assemblyName + ".exe");
            return File.Exists(exeCandidate) ? Assembly.LoadFrom(exeCandidate) : null;
        };
        Assembly.LoadFrom(Path.Combine(appOutputDirectory, "USR_ElectroPilot.exe"));

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var user = new UserModel
        {
            Username = "phase0",
            DisplayName = "Phase 0",
            Role = "Admin",
            IsActive = true
        };

        if (args.Length >= 3 && string.Equals(args[2], "h1", StringComparison.OrdinalIgnoreCase))
        {
            SaveWpfDashboard(screenshotPath, true, false);
            Console.WriteLine("Screenshot saved: " + screenshotPath);
            return 0;
        }

        if (args.Length >= 3 && string.Equals(args[2], "tank", StringComparison.OrdinalIgnoreCase))
        {
            SaveWpfDashboard(screenshotPath, false, true);
            Console.WriteLine("Screenshot saved: " + screenshotPath);
            return 0;
        }

        if (args.Length >= 3 && string.Equals(args[2], "wpf", StringComparison.OrdinalIgnoreCase))
        {
            SaveWpfDashboard(screenshotPath, false, false);
            Console.WriteLine("Screenshot saved: " + screenshotPath);
            return 0;
        }

        if (args.Length >= 3 && string.Equals(args[2], "host", StringComparison.OrdinalIgnoreCase))
        {
            SaveHostDashboard(screenshotPath);
            Console.WriteLine("Screenshot saved: " + screenshotPath);
            return 0;
        }

        using (var form = new MainForm(user))
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(80, 80);
            form.Size = new Size(1500, 900);
            form.Show();

            for (var i = 0; i < 30; i++)
            {
                Application.DoEvents();
                Thread.Sleep(150);
            }

            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                if (args.Length >= 3 && string.Equals(args[2], "winforms-draw", StringComparison.OrdinalIgnoreCase))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                }
                else
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(form.Location, Point.Empty, form.Size);
                    }
                }

                bitmap.Save(screenshotPath, ImageFormat.Png);
            }

            form.Close();
        }

        Console.WriteLine("Screenshot saved: " + screenshotPath);
        return 0;
    }

    private static void SaveWpfDashboard(string screenshotPath, bool focusH1, bool focusTank)
    {
        var view = new Plant3DView
        {
            Width = 1500,
            Height = 900
        };

        var tanks = new System.Collections.Generic.List<TankModel>();
        for (var i = 1; i <= 18; i++)
        {
            tanks.Add(new TankModel
            {
                Id = i,
                LineId = 1,
                TankNo = i,
                Name = "T" + i,
                ChemicalName = i == 4 ? "Acid Dip" : i == 5 ? "Copper" : "Process",
                CapacityLiters = 1000,
                CurrentLevelLiters = i == 8 ? 840 : 750,
                TemperatureCelsius = 30 + i,
                CurrentAmps = i == 5 ? 138 : 90 + i,
                Voltage = i == 5 ? 12.1 : 9.5 + (i * 0.1),
                Status = i == 5 ? "Running" : "Normal",
                IsActive = true
            });
        }

        var hoists = new System.Collections.Generic.List<HoistModel>
        {
            new HoistModel
            {
                HoistId = 1,
                LineId = 1,
                HoistName = "H1",
                CurrentTankNo = 5,
                TargetTankNo = 5,
                Status = "Running",
                IsAuto = true,
                PositionIndex = 4
            }
        };

        view.UpdatePlant(
            tanks,
            new System.Collections.Generic.List<ProcessStepModel>(),
            hoists,
            new System.Collections.Generic.List<JobModel>(),
            null,
            4,
            "Copper",
            0,
            true,
            false,
            new SimulatorService().CreateEquipmentSnapshot(tanks, 1, DateTime.UtcNow));

        if (focusH1)
        {
            var viewportField = typeof(Plant3DView).GetField("_viewport", BindingFlags.Instance | BindingFlags.NonPublic);
            var viewport = (HelixToolkit.Wpf.HelixViewport3D)viewportField.GetValue(view);
            var target = new Point3D(-6.525, 0, 1.75);
            var position = new Point3D(-12.9, -0.55, 3.65);
            viewport.Camera = new PerspectiveCamera(position, target - position, new Vector3D(0, 0, 1), 42)
            {
                NearPlaneDistance = 0.05,
                FarPlaneDistance = 100
            };
        }
        else if (focusTank)
        {
            var viewportField = typeof(Plant3DView).GetField("_viewport", BindingFlags.Instance | BindingFlags.NonPublic);
            var viewport = (HelixToolkit.Wpf.HelixViewport3D)viewportField.GetValue(view);
            var target = new Point3D(-2.175, -0.28, 0.48);
            var position = new Point3D(-5.45, -6.35, 2.75);
            viewport.Camera = new PerspectiveCamera(position, target - position, new Vector3D(0, 0, 1), 43)
            {
                NearPlaneDistance = 0.05,
                FarPlaneDistance = 100
            };
        }

        view.Measure(new System.Windows.Size(view.Width, view.Height));
        view.Arrange(new System.Windows.Rect(0, 0, view.Width, view.Height));
        view.UpdateLayout();

        var bitmap = new RenderTargetBitmap((int)view.Width, (int)view.Height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
        bitmap.Render(view);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using (var stream = File.Create(screenshotPath))
        {
            encoder.Save(stream);
        }

        view.Dispose();
    }

    private static void SaveHostDashboard(string screenshotPath)
    {
        using (var control = new Plant3DHostControl())
        {
            control.Width = 1500;
            control.Height = 900;
            control.CanOperateTanks = true;
            control.CanEngineerTanks = true;

            var tanks = CreateReferenceTanks();
            tanks[9].Status = "Fault";
            var hoists = new System.Collections.Generic.List<HoistModel>
            {
                new HoistModel
                {
                    HoistId = 1,
                    LineId = 1,
                    HoistName = "H1",
                    CurrentTankNo = 5,
                    TargetTankNo = 5,
                    Status = "Running",
                    IsAuto = true,
                    PositionIndex = 4
                }
            };

            control.BindData(
                tanks,
                new System.Collections.Generic.List<WagonModel>(),
                new System.Collections.Generic.List<ProcessStepModel>(),
                hoists,
                new System.Collections.Generic.List<JobModel>(),
                null,
                4,
                "Processing",
                42,
                true,
                false,
                1,
                new SimulatorService().CreateEquipmentSnapshot(tanks, 1, DateTime.UtcNow));
            control.SelectTank(4);

            control.CreateControl();
            control.PerformLayout();

            for (var i = 0; i < 20; i++)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            using (var bitmap = new Bitmap(control.Width, control.Height))
            {
                control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));
                bitmap.Save(screenshotPath, ImageFormat.Png);
            }
        }
    }

    private static System.Collections.Generic.List<TankModel> CreateReferenceTanks()
    {
        var tanks = new System.Collections.Generic.List<TankModel>();
        for (var i = 1; i <= 18; i++)
        {
            tanks.Add(new TankModel
            {
                Id = i,
                LineId = 1,
                TankNo = i,
                Name = "T" + i,
                ChemicalName = i == 4 ? "Acid Dip" : i == 5 ? "Copper" : "Process",
                CapacityLiters = 1000,
                CurrentLevelLiters = i == 8 ? 840 : 750,
                TemperatureCelsius = 30 + i,
                CurrentAmps = i == 5 ? 138 : 90 + i,
                Voltage = i == 5 ? 12.1 : 9.5 + (i * 0.1),
                Status = i == 5 || i <= 9 ? "Running" : "Normal",
                IsActive = true
            });
        }

        return tanks;
    }
}
