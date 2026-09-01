using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Forms;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Controls;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;
using USR_ElectroPilot.ThreeD.Materials;
using USR_ElectroPilot.ThreeD.Views;

internal static class Phase11RuntimeVerifier
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: Phase11RuntimeVerifier <appOutputDirectory>");
            return 2;
        }

        var appOutputDirectory = Path.GetFullPath(args[0]);
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

        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            VerifyHoistAnimationDoesNotRebuildStaticScene();
            VerifyHelixMouseControls();
            VerifyDashboardBindings();
            VerifyDashboardMenuShell();
            VerifyEquipmentTelemetryContract();
            VerifySafePlcWriteGuard();

            Console.WriteLine("PHASE11 PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("PHASE11 FAIL: " + ex);
            return 1;
        }
    }

    private static void VerifyHoistAnimationDoesNotRebuildStaticScene()
    {
        var view = new Plant3DView
        {
            Width = 1000,
            Height = 600
        };

        try
        {
            var tanks = CreateTanks(false);
            var telemetry = new SimulatorService().CreateEquipmentSnapshot(tanks, 1, DateTime.UtcNow);
            telemetry.Motors[9].CommandOn = true;
            telemetry.Motors[9].RunFeedback = false;
            telemetry.Motors[9].Fault = true;
            var hoist = new HoistModel
            {
                HoistId = 1,
                LineId = 1,
                HoistName = "H1",
                CurrentTankNo = 1,
                TargetTankNo = 5,
                Status = "Moving",
                IsAuto = true,
                PositionIndex = 0
            };
            var hoists = new List<HoistModel> { hoist };

            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, 0, "Loading", 60, true, false, telemetry);
            var staticBuilds = view.StaticSceneBuildCount;
            var hoistBuilds = view.HoistVisualBuildCount;
            var tankDynamicBuilds = view.TankDynamicBuildCount;
            AssertTrue(view.IsUsingImportedHoistAssets, "H1 should use the complete imported model set when all required assets are present.");
            AssertContains(view.RealHoistAssetReadinessReport, "Loaded=hoist_h1,motor_gearbox,plate_rack,hanging_plate", "Real H1 readiness report should list all imported parts as loaded.");
            AssertContains(view.RealHoistAssetReadinessReport, "Missing=none", "Real H1 should not report missing model parts.");
            AssertContains(view.RealHoistAssetReadinessReport, "Failed=none", "Real H1 should not report failed model parts.");
            VerifyImportedPlantAssets(view, tanks.Count);
            VerifyReadableAcidTankLabel(view);
            VerifyMotorStatusIndicators(view);
            VerifySingleTankOpening(view, tanks.Count);
            VerifyImportedH1Assets(view);
            VerifyRaisedPlateRackClearsTankRim(view);
            var startPlateX = GetAveragePlateRackX(view);

            for (var i = 1; i <= 40; i++)
            {
                hoist.PositionIndex = Math.Min(4, i * 0.2);
                hoist.CurrentTankNo = Math.Max(1, Convert.ToInt32(Math.Round(hoist.PositionIndex, MidpointRounding.AwayFromZero)) + 1);
                view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Moving", 50 - i, true, false);
            }

            AssertEqual(staticBuilds, view.StaticSceneBuildCount, "Static scene rebuilt during hoist X movement.");
            AssertTrue(view.HoistVisualBuildCount > hoistBuilds, "Hoist visual layer did not refresh during movement.");
            VerifyImportedH1Assets(view);
            VerifyRaisedPlateRackClearsTankRim(view);
            AssertTrue(GetAveragePlateRackX(view) > startPlateX + 2.0, "Hanging plate rack did not move with the H1 hoist X position.");

            var raisedBottom = GetLowestPlateRackBottom(view);
            hoist.Status = "Lowering";
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Lowering", 30, true, false);
            var firstLoweringBottom = GetLowestPlateRackBottom(view);
            AssertTrue(firstLoweringBottom < raisedBottom - 0.02, "Lowering should begin moving the H1 plates downward.");
            AssertTrue(firstLoweringBottom > 0.9, "Lowering should not instantly jump the H1 plates into the tank.");

            for (var i = 0; i < 28; i++)
            {
                view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Lowering", 30, true, false);
            }

            VerifyLoweredPlateRackEntersTank(view);

            hoist.Status = "Processing";
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Processing", 30, true, false);
            VerifyLoweredPlateRackEntersTank(view);
            VerifyMoveTargetWaitsForRaisedRack(view, tanks, hoist, hoists);

            var loweredBottom = GetLowestPlateRackBottom(view);
            hoist.Status = "Lifting";
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Lifting", 30, true, false);
            var firstLiftingBottom = GetLowestPlateRackBottom(view);
            AssertTrue(firstLiftingBottom > loweredBottom + 0.02, "Lifting should begin moving the H1 plates upward.");
            AssertTrue(firstLiftingBottom < raisedBottom - 0.02, "Lifting should not instantly jump the H1 plates to the raised position.");

            for (var i = 0; i < 28; i++)
            {
                view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Lifting", 30, true, false);
            }

            VerifyRaisedPlateRackClearsTankRim(view);

            AssertEqual(staticBuilds, view.StaticSceneBuildCount, "Static scene rebuilt during lift state changes.");

            view.SelectTank(5);
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Processing", 30, true, false);
            AssertEqual(staticBuilds, view.StaticSceneBuildCount, "Tank selection should update the dynamic highlight without rebuilding imported static plant assets.");
            AssertEqual(tankDynamicBuilds + 1, view.TankDynamicBuildCount, "Tank selection did not rebuild the dynamic tank-state layer.");

            var initialLiquidZ = GetLeftmostLiquidSurfaceZ(view);
            tanks[0].CurrentLevelLiters += 25;
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Processing", 30, true, false);
            AssertEqual(staticBuilds, view.StaticSceneBuildCount, "Tank liquid/value change rebuilt imported static plant assets.");
            AssertEqual(tankDynamicBuilds + 2, view.TankDynamicBuildCount, "Tank liquid/value change did not refresh the dynamic tank-state layer.");
            AssertTrue(GetLeftmostLiquidSurfaceZ(view) > initialLiquidZ, "Tank liquid surface did not move when the bound level changed.");

            tanks[0].Status = "Fault";
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Processing", 30, true, false);
            AssertEqual(staticBuilds, view.StaticSceneBuildCount, "Tank status change rebuilt imported static plant assets.");
            AssertEqual(tankDynamicBuilds + 3, view.TankDynamicBuildCount, "Tank status change did not refresh the dynamic tank-state layer.");
            AssertTrue(HasFaultLampAtLeftmostTank(view), "Tank fault status did not update the imported tank's dynamic status lamp.");
            VerifySingleTankOpening(view, tanks.Count);
        }
        finally
        {
            view.Dispose();
        }
    }

    private static void VerifyImportedPlantAssets(Plant3DView view, int expectedTankCount)
    {
        AssertTrue(view.IsUsingImportedPlantAssets, "Plant should use imported tank, manifold, pump, and walkway assets when the complete set is available.");
        AssertContains(view.RealPlantAssetReadinessReport, "Loaded=tank_standard,pipe_manifold,pump,walkway", "Real plant readiness report should list all imported plant parts as loaded.");
        AssertContains(view.RealPlantAssetReadinessReport, "Missing=none", "Real plant should not report missing model parts.");
        AssertContains(view.RealPlantAssetReadinessReport, "Failed=none", "Real plant should not report failed model parts.");
        AssertEqual(expectedTankCount, view.GetImportedPlantInstanceCount("tank_standard"), "Imported tank shell count should match the configured tank count.");
        AssertEqual(expectedTankCount, view.GetImportedPlantInstanceCount("pipe_manifold"), "Imported manifold module count should match the configured tank count.");
        AssertEqual(expectedTankCount, view.GetImportedPlantInstanceCount("pump"), "Imported pump/motor count should match the configured tank count.");
        AssertEqual(expectedTankCount, view.GetImportedPlantInstanceCount("walkway"), "Imported walkway module count should match the configured tank count.");
        AssertTrue(view.TankDynamicBuildCount > 0, "Imported tanks should have a separate dynamic liquid/status layer.");
        VerifyStaticPlantGeometryIsShared(view, expectedTankCount);
    }

    private static void VerifyStaticPlantGeometryIsShared(Plant3DView view, int expectedTankCount)
    {
        var contentCounts = new Dictionary<Model3D, int>();
        foreach (var visual in GetViewport(view).Children)
        {
            var modelVisual = visual as ModelVisual3D;
            if (modelVisual == null || modelVisual.Content == null)
            {
                continue;
            }

            int count;
            contentCounts.TryGetValue(modelVisual.Content, out count);
            contentCounts[modelVisual.Content] = count + 1;
        }

        var sharedPlantPartCount = 0;
        foreach (var count in contentCounts.Values)
        {
            if (count == expectedTankCount)
            {
                sharedPlantPartCount++;
            }
        }

        AssertTrue(sharedPlantPartCount >= 4, "Imported tank, manifold, pump, and walkway meshes should be shared across the configured tank instances.");
    }

    private static void VerifyReadableAcidTankLabel(Plant3DView view)
    {
        foreach (var visual in GetViewport(view).Children)
        {
            var label = visual as BillboardTextVisual3D;
            if (label == null || label.Text == null || label.Text.IndexOf("ACID", StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            AssertContains(label.Text, "T04", "Acid label should include its two-digit tank number.");
            AssertTrue(label.FontSize >= 12, "Acid label text should remain readable at the dashboard camera distance.");
            AssertTrue(label.Width >= 0.5 && label.Height >= 0.2, "Acid label billboard should have a readable camera-facing area.");
            AssertTrue(label.Background != null && label.Background != System.Windows.Media.Brushes.Transparent, "Acid label should use a high-contrast background.");
            return;
        }

        throw new InvalidOperationException("Missing camera-facing Acid Dip tank label.");
    }

    private static void VerifyMotorStatusIndicators(Plant3DView view)
    {
        BillboardTextVisual3D running = null;
        BillboardTextVisual3D fault = null;
        foreach (var visual in GetViewport(view).Children)
        {
            var label = visual as BillboardTextVisual3D;
            if (label == null)
            {
                continue;
            }

            if (string.Equals(label.Text, "M RUN", StringComparison.Ordinal))
            {
                running = label;
            }
            else if (string.Equals(label.Text, "M FAULT", StringComparison.Ordinal))
            {
                fault = label;
            }
        }

        AssertTrue(running != null, "Running motor should have a green camera-facing status indicator.");
        AssertTrue(fault != null, "Faulted motor should have a red camera-facing status indicator.");
        AssertTrue(running.Position.Y < -1.3 && fault.Position.Y < -1.3, "Motor indicators should stay anchored beside the front pump/motor row.");
        AssertTrue(running.Background != null && fault.Background != null, "Motor status indicators should use readable high-contrast backgrounds.");
    }

    private static void VerifyEquipmentTelemetryContract()
    {
        var now = DateTime.UtcNow;
        var runningMotor = new MotorTelemetryModel
        {
            EquipmentId = "M-T04-P01",
            TankNo = 4,
            Name = "Tank 04 circulation motor",
            CommandOn = true,
            RunFeedback = true,
            CurrentAmps = 7.4,
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var stoppedPump = new PumpTelemetryModel
        {
            EquipmentId = "P-T04-01",
            TankNo = 4,
            Name = "Tank 04 circulation pump",
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var mismatchedValve = new ValveTelemetryModel
        {
            EquipmentId = "V-T04-01",
            TankNo = 4,
            Name = "Tank 04 inlet valve",
            CommandOn = true,
            RunFeedback = false,
            OpenCommand = true,
            OpenFeedback = false,
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var trippedHeater = new HeaterTelemetryModel
        {
            EquipmentId = "H-T04-01",
            TankNo = 4,
            Name = "Tank 04 heater",
            CommandOn = true,
            OverTemperature = true,
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var staleRectifier = new RectifierTelemetryModel
        {
            EquipmentId = "R-T05-01",
            TankNo = 5,
            Name = "Tank 05 rectifier",
            CommandOn = true,
            RunFeedback = true,
            LastUpdatedUtc = now.AddSeconds(-20),
            Quality = TelemetryQuality.Good
        };

        var provider = new SimulationTelemetryProvider();
        provider.Publish(new PlantTelemetrySnapshot(
            1,
            now,
            new[] { runningMotor },
            new[] { stoppedPump },
            new[] { mismatchedValve },
            new[] { trippedHeater },
            new[] { staleRectifier }));

        var snapshot = provider.GetLatestSnapshot();
        AssertEqual(1L, snapshot.Sequence, "Simulation telemetry provider did not retain the latest sequence.");
        AssertEqual(1, snapshot.Motors.Count, "Motor telemetry was not retained in the plant snapshot.");
        AssertEqual(1, snapshot.Pumps.Count, "Pump telemetry was not retained in the plant snapshot.");
        AssertEqual(1, snapshot.Valves.Count, "Valve telemetry was not retained in the plant snapshot.");
        AssertEqual(1, snapshot.Heaters.Count, "Heater telemetry was not retained in the plant snapshot.");
        AssertEqual(1, snapshot.Rectifiers.Count, "Rectifier telemetry was not retained in the plant snapshot.");

        var staleAfter = TimeSpan.FromSeconds(5);
        AssertEqual(EquipmentOperatingState.Running, EquipmentTelemetryStateResolver.Resolve(runningMotor, now, staleAfter), "Confirmed motor feedback should resolve to Running.");
        AssertEqual(EquipmentOperatingState.Stopped, EquipmentTelemetryStateResolver.Resolve(stoppedPump, now, staleAfter), "Healthy pump without feedback should resolve to Stopped.");
        AssertEqual(EquipmentOperatingState.Warning, EquipmentTelemetryStateResolver.Resolve(mismatchedValve, now, staleAfter), "Valve command/feedback mismatch should resolve to Warning.");
        AssertEqual(EquipmentOperatingState.Fault, EquipmentTelemetryStateResolver.Resolve(trippedHeater, now, staleAfter), "Heater over-temperature should resolve to Fault.");
            AssertEqual(EquipmentOperatingState.Stale, EquipmentTelemetryStateResolver.Resolve(staleRectifier, now, staleAfter), "Old rectifier telemetry should resolve to Stale.");

        VerifyEquipmentAlarmRules(now);
    }

    private static void VerifyEquipmentAlarmRules(DateTime now)
    {
        var startFailure = new MotorTelemetryModel
        {
            EquipmentId = "M-T01-P01",
            CommandOn = true,
            RunFeedback = false,
            CommandChangedAtUtc = now.AddSeconds(-5),
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var overload = new MotorTelemetryModel
        {
            EquipmentId = "M-T02-P01",
            CommandOn = true,
            RunFeedback = false,
            Overload = true,
            CommandChangedAtUtc = now,
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var local = new MotorTelemetryModel
        {
            EquipmentId = "M-T03-P01",
            IsLocalMode = true,
            LastUpdatedUtc = now,
            Quality = TelemetryQuality.Good
        };
        var staleMotor = new MotorTelemetryModel
        {
            EquipmentId = "M-T04-P01",
            LastUpdatedUtc = now.AddSeconds(-10),
            Quality = TelemetryQuality.Good
        };
        var snapshot = new PlantTelemetrySnapshot(
            3,
            now,
            new[] { startFailure, overload, local, staleMotor },
            null,
            null,
            null,
            null);
        var evaluator = new EquipmentTelemetryAlarmService();
        var alarms = evaluator.Evaluate(snapshot, now, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3));
        AssertEqual(4, alarms.Count, "Equipment alarm rules should produce one alarm for each distinct failure mode.");
        AssertTrue(alarms.Any(a => a.Message.IndexOf("failed to start", StringComparison.OrdinalIgnoreCase) >= 0), "Missing motor start-failure alarm.");
        AssertTrue(alarms.Any(a => a.Message.IndexOf("overload", StringComparison.OrdinalIgnoreCase) >= 0), "Missing motor overload alarm.");
        AssertTrue(alarms.Any(a => a.Message.IndexOf("local mode", StringComparison.OrdinalIgnoreCase) >= 0), "Missing motor local-mode alarm.");
        AssertTrue(alarms.Any(a => a.Message.IndexOf("stale", StringComparison.OrdinalIgnoreCase) >= 0), "Missing stale motor feedback alarm.");
        AssertEqual(alarms.Count, alarms.Select(a => a.Source + "|" + a.Message).Distinct(StringComparer.OrdinalIgnoreCase).Count(), "Equipment alarm candidates should be deduplicated by source and message.");

        var communicationAlarm = evaluator.Evaluate(
            new PlantTelemetrySnapshot(4, now.AddSeconds(-10), snapshot.Motors, null, null, null, null),
            now,
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(3));
        AssertEqual(1, communicationAlarm.Count, "A stale plant snapshot should suppress individual equipment alarms.");
        AssertContains(communicationAlarm[0].Message, "heartbeat is stale", "Missing plant communication stale alarm.");
    }

    private static double GetLeftmostLiquidSurfaceZ(Plant3DView view)
    {
        BoxVisual3D leftmost = null;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !IsLiquidMaterial(box.Material))
            {
                continue;
            }

            if (leftmost == null || box.Center.X < leftmost.Center.X)
            {
                leftmost = box;
            }
        }

        if (leftmost == null)
        {
            throw new InvalidOperationException("No dynamic tank liquid surface found.");
        }

        return leftmost.Center.Z;
    }

    private static bool HasFaultLampAtLeftmostTank(Plant3DView view)
    {
        SphereVisual3D leftmost = null;
        foreach (var visual in GetViewport(view).Children)
        {
            var sphere = visual as SphereVisual3D;
            if (sphere == null || sphere.Radius < 0.045 || sphere.Radius > 0.06)
            {
                continue;
            }

            if (leftmost == null || sphere.Center.X < leftmost.Center.X)
            {
                leftmost = sphere;
            }
        }

        return leftmost != null && object.ReferenceEquals(leftmost.Material, PlantMaterialLibrary.StatusFault);
    }

    private static bool IsLiquidMaterial(System.Windows.Media.Media3D.Material material)
    {
        return object.ReferenceEquals(material, PlantMaterialLibrary.LiquidBlue) ||
            object.ReferenceEquals(material, PlantMaterialLibrary.LiquidGreen) ||
            object.ReferenceEquals(material, PlantMaterialLibrary.LiquidBrown);
    }

    private static void VerifyImportedH1Assets(Plant3DView view)
    {
        var portal = view.GetImportedHoistPartBounds("hoist_h1");
        var motor = view.GetImportedHoistPartBounds("motor_gearbox");
        var rack = view.GetImportedHoistPartBounds("plate_rack");
        var plates = view.GetImportedHoistPartBounds("hanging_plate");

        AssertTrue(!portal.IsEmpty && portal.SizeY > 2.8 && portal.SizeZ > 1.1, "Imported H1 portal should have the reference-style full-width fabricated frame.");
        AssertTrue(!motor.IsEmpty && motor.SizeY > 1.8 && motor.SizeZ > 0.7, "Imported H1 should include separate travel and vertical lift motor geometry.");
        AssertTrue(!rack.IsEmpty && rack.SizeY > 1.8 && rack.SizeZ > 0.4, "Imported H1 should include the mechanically connected moving lift frame and carrier.");
        AssertTrue(!plates.IsEmpty && plates.SizeY > 1.5 && plates.SizeX > 0.4 && plates.SizeZ > 0.7, "Imported H1 should include a distributed rack of vertical PCB workpieces.");

        var portalCenterX = portal.X + portal.SizeX / 2.0;
        var motorCenterX = motor.X + motor.SizeX / 2.0;
        var rackCenterX = rack.X + rack.SizeX / 2.0;
        var plateCenterX = plates.X + plates.SizeX / 2.0;
        AssertTrue(Math.Abs(motorCenterX - portalCenterX) < 0.4, "Imported motor assembly should stay attached to the H1 portal.");
        AssertTrue(Math.Abs(rackCenterX - portalCenterX) < 0.4, "Imported lift frame should stay centered inside the H1 portal.");
        AssertTrue(Math.Abs(plateCenterX - rackCenterX) < 0.2, "Imported PCB load should stay bound to its carrier.");
    }

    private static void VerifyMoveTargetWaitsForRaisedRack(Plant3DView view, List<TankModel> tanks, HoistModel hoist, List<HoistModel> hoists)
    {
        var lowX = GetAveragePlateRackX(view);
        var lowBottom = GetLowestPlateRackBottom(view);
        hoist.Status = "Moving";
        hoist.PositionIndex += 1.0;
        hoist.CurrentTankNo = Math.Max(1, Convert.ToInt32(Math.Round(hoist.PositionIndex, MidpointRounding.AwayFromZero)) + 1);
        view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Moving", 30, true, false);

        var firstMoveX = GetAveragePlateRackX(view);
        var firstMoveBottom = GetLowestPlateRackBottom(view);
        AssertTrue(Math.Abs(firstMoveX - lowX) < 0.08, "H1 should not move horizontally while plates are still lifting out of the tank.");
        AssertTrue(firstMoveBottom > lowBottom + 0.02, "H1 should lift plates before accepting horizontal travel.");

        for (var i = 0; i < 45; i++)
        {
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Moving", 30, true, false);
        }

        VerifyRaisedPlateRackClearsTankRim(view);
        AssertTrue(GetAveragePlateRackX(view) > lowX + 1.0, "H1 should begin horizontal travel after the plates are fully raised.");

        hoist.PositionIndex -= 1.0;
        hoist.CurrentTankNo = Math.Max(1, Convert.ToInt32(Math.Round(hoist.PositionIndex, MidpointRounding.AwayFromZero)) + 1);
        hoist.Status = "Processing";
        for (var i = 0; i < 45; i++)
        {
            view.UpdatePlant(tanks, new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, hoist.PositionIndex, "Processing", 30, true, false);
        }

        VerifyLoweredPlateRackEntersTank(view);
    }

    private static void VerifyParallelHangingPlateRackGeometry(Plant3DView view)
    {
        var plateCount = 0;
        var verticalPlateCount = 0;
        var minY = double.MaxValue;
        var maxY = double.MinValue;
        var minX = double.MaxValue;
        var maxX = double.MinValue;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.PlateMetal))
            {
                continue;
            }

            plateCount++;
            if (box.Length >= 0.25 && box.Width <= 0.06 && box.Height >= 0.35)
            {
                verticalPlateCount++;
            }

            minY = Math.Min(minY, box.Center.Y);
            maxY = Math.Max(maxY, box.Center.Y);
            minX = Math.Min(minX, box.Center.X);
            maxX = Math.Max(maxX, box.Center.X);
        }

        AssertTrue(plateCount >= 10, "Expected multiple real-style hanging plates on the H1 carrier.");
        AssertEqual(plateCount, verticalPlateCount, "Work pieces should hang vertically from the H1 carrier.");
        AssertTrue(maxY - minY > 0.9, "Hanging plates should be distributed along the tank-parallel carrier.");
        AssertTrue(maxX - minX < 0.05, "Hanging plates should stay centered on one H1 carrier, not spread across tank positions.");
    }

    private static void VerifySingleTankOpening(Plant3DView view, int expectedTankCount)
    {
        var liquidSurfaceCount = 0;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null)
            {
                continue;
            }

            if (IsLiquidMaterial(box.Material) && box.Height <= 0.06)
            {
                liquidSurfaceCount++;
            }

            var isNestedLinerWall = object.ReferenceEquals(box.Material, PlantMaterialLibrary.TankBase) &&
                box.Height > 0.4 && box.Center.Z > 0.25;
            AssertTrue(!isNestedLinerWall, "Tank should not render inner wall panels that make a second tank inside the main tank.");
        }

        AssertEqual(expectedTankCount, liquidSurfaceCount, "Each tank should render one liquid surface only.");
    }

    private static void VerifyPlateRackHasNoSolidCover(Plant3DView view)
    {
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.BrushedSteel))
            {
                continue;
            }

            var looksLikePlateCover = box.Length < 0.08 && box.Width > 1.0 && box.Height > 0.45 &&
                box.Center.Z > 0.9 && box.Center.Z < 1.9;
            AssertTrue(!looksLikePlateCover, "Plate rack should use open rods only; solid cover panels hide the hanging plates.");
        }
    }

    private static void VerifyNoBlackPadBelowPlateRack(Plant3DView view)
    {
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.SoftContactShadow))
            {
                continue;
            }

            var isInsideTankOpening = box.Center.Z > 0.7 && box.Center.Z < 1.2 && box.Length > 1.0 && box.Width > 1.0;
            AssertTrue(!isInsideTankOpening, "H1 should not render a black shadow pad below the hanging plates.");
        }
    }

    private static void VerifyHoistNameOnFrontBeam(Plant3DView view)
    {
        foreach (var visual in GetViewport(view).Children)
        {
            var label = visual as TextVisual3D;
            if (label == null || !string.Equals(label.Text, "USR H1 500KG", StringComparison.Ordinal))
            {
                continue;
            }

            AssertTrue(label.Position.Y > -1.9 && label.Position.Y < -1.45, "Hoist name should sit on the H1 front beam, not float beside the machine.");
            AssertTrue(label.Position.Z > 2.5 && label.Position.Z < 2.75, "Hoist name should align with the upper yellow H1 beam.");
            AssertTrue(label.Height > 0.1, "Hoist name should use readable beam lettering, not a tiny tag.");
            AssertTrue(label.TextDirection.X > 0.9 && Math.Abs(label.TextDirection.Y) < 0.01, "Hoist name should be laid along the bridge beam.");
            return;
        }

        throw new InvalidOperationException("Missing H1 front beam name label.");
    }

    private static void VerifyRealisticHoistDriveMotor(Plant3DView view)
    {
        var labelPosition = GetHoistNameLabelPosition(view);
        var hoistCenterX = labelPosition.X + 0.48;
        var gearboxFound = false;
        var motorBodyFound = false;
        var coolingFinCount = 0;
        var maxDriveX = double.MinValue;

        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box != null && object.ReferenceEquals(box.Material, PlantMaterialLibrary.HoistMotor))
            {
                if (box.Center.X > hoistCenterX + 0.45 &&
                    box.Center.Y > -1.75 && box.Center.Y < -1.4 &&
                    box.Center.Z > 2.35 && box.Center.Z < 2.75 &&
                    box.Length >= 0.18 && box.Height >= 0.3)
                {
                    gearboxFound = true;
                    maxDriveX = Math.Max(maxDriveX, box.Center.X + (box.Length / 2.0));
                }
            }

            var modelVisual = visual as ModelVisual3D;
            var geometryModel = modelVisual == null ? null : modelVisual.Content as GeometryModel3D;
            if (geometryModel == null)
            {
                continue;
            }

            var bounds = geometryModel.Bounds;
            var centerX = bounds.X + bounds.SizeX / 2.0;
            var centerY = bounds.Y + bounds.SizeY / 2.0;
            var centerZ = bounds.Z + bounds.SizeZ / 2.0;
            var isDriveArea = centerX > hoistCenterX + 0.65 &&
                centerY > -1.85 && centerY < -1.35 &&
                centerZ > 2.35 && centerZ < 2.8;

            if (!isDriveArea)
            {
                continue;
            }

            maxDriveX = Math.Max(maxDriveX, bounds.X + bounds.SizeX);

            if (object.ReferenceEquals(geometryModel.Material, PlantMaterialLibrary.HoistMotor) &&
                bounds.SizeX > 0.22 && bounds.SizeX < 0.42 && bounds.SizeY > 0.22 && bounds.SizeZ > 0.22)
            {
                motorBodyFound = true;
            }

            if (object.ReferenceEquals(geometryModel.Material, PlantMaterialLibrary.DarkMetal) &&
                bounds.SizeX < 0.05 && bounds.SizeY > 0.25 && bounds.SizeZ > 0.25)
            {
                coolingFinCount++;
            }
        }

        AssertTrue(gearboxFound, "H1 bridge drive should include a gearbox mounted on the yellow beam.");
        AssertTrue(motorBodyFound, "H1 bridge drive should include a connected horizontal cylindrical motor.");
        AssertTrue(coolingFinCount >= 4, "H1 bridge motor should include visible cooling fins, not a plain cylinder only.");
        AssertTrue(maxDriveX < hoistCenterX + 1.42, "H1 bridge motor should stay compact and attached to the hoist body, not protrude far outside it.");
    }

    private static Point3D GetHoistNameLabelPosition(Plant3DView view)
    {
        foreach (var visual in GetViewport(view).Children)
        {
            var label = visual as TextVisual3D;
            if (label != null && string.Equals(label.Text, "USR H1 500KG", StringComparison.Ordinal))
            {
                return label.Position;
            }
        }

        throw new InvalidOperationException("Missing H1 front beam name label.");
    }

    private static void VerifyRaisedPlateRackClearsTankRim(Plant3DView view)
    {
        if (view.IsUsingImportedHoistAssets)
        {
            var bounds = view.GetImportedHoistPartBounds("hanging_plate");
            AssertTrue(!bounds.IsEmpty && bounds.Z > 1.05, "Raised imported H1 plates should clear the tank rim instead of staying inside the tank.");
            return;
        }

        var minBottom = double.MaxValue;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.PlateMetal))
            {
                continue;
            }

            minBottom = Math.Min(minBottom, box.Center.Z - (box.Height / 2.0));
        }

        AssertTrue(minBottom > 1.05, "Raised H1 plates should clear the tank rim instead of staying inside the tank.");
    }

    private static void VerifyLoweredPlateRackEntersTank(Plant3DView view)
    {
        if (view.IsUsingImportedHoistAssets)
        {
            var bounds = view.GetImportedHoistPartBounds("hanging_plate");
            AssertTrue(!bounds.IsEmpty && bounds.Z < 0.55, "Lowered imported H1 plates should reach into the tank liquid area.");
            return;
        }

        var maxTop = double.MinValue;
        var minBottom = double.MaxValue;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.PlateMetal))
            {
                continue;
            }

            maxTop = Math.Max(maxTop, box.Center.Z + (box.Height / 2.0));
            minBottom = Math.Min(minBottom, box.Center.Z - (box.Height / 2.0));
        }

        AssertTrue(maxTop <= 1.08, "Lowered/processing H1 plates should enter the tank instead of remaining above the rim.");
        AssertTrue(minBottom < 0.55, "Lowered/processing H1 plates should reach into the tank liquid area.");
    }

    private static double GetAveragePlateRackX(Plant3DView view)
    {
        if (view.IsUsingImportedHoistAssets)
        {
            var bounds = view.GetImportedHoistPartBounds("hanging_plate");
            if (bounds.IsEmpty)
            {
                throw new InvalidOperationException("No imported plate rack geometry found.");
            }

            return bounds.X + bounds.SizeX / 2.0;
        }

        var count = 0;
        var total = 0.0;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.PlateMetal))
            {
                continue;
            }

            total += box.Center.X;
            count++;
        }

        if (count == 0)
        {
            throw new InvalidOperationException("No plate rack geometry found.");
        }

        return total / count;
    }

    private static double GetLowestPlateRackBottom(Plant3DView view)
    {
        if (view.IsUsingImportedHoistAssets)
        {
            var bounds = view.GetImportedHoistPartBounds("hanging_plate");
            if (bounds.IsEmpty)
            {
                throw new InvalidOperationException("No imported plate rack geometry found.");
            }

            return bounds.Z;
        }

        var minBottom = double.MaxValue;
        foreach (var visual in GetViewport(view).Children)
        {
            var box = visual as BoxVisual3D;
            if (box == null || !object.ReferenceEquals(box.Material, PlantMaterialLibrary.PlateMetal))
            {
                continue;
            }

            minBottom = Math.Min(minBottom, box.Center.Z - (box.Height / 2.0));
        }

        if (minBottom == double.MaxValue)
        {
            throw new InvalidOperationException("No plate rack geometry found.");
        }

        return minBottom;
    }

    private static HelixViewport3D GetViewport(Plant3DView view)
    {
        var field = typeof(Plant3DView).GetField("_viewport", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException("Missing Plant3DView _viewport field.");
        }

        var viewport = field.GetValue(view) as HelixViewport3D;
        if (viewport == null)
        {
            throw new InvalidOperationException("Plant3DView _viewport is not HelixViewport3D.");
        }

        return viewport;
    }

    private static void VerifyHelixMouseControls()
    {
        using (var view = new Plant3DView())
        {
            var viewport = GetViewport(view);

            AssertTrue(viewport.IsRotationEnabled, "Helix rotation should be enabled.");
            AssertTrue(viewport.IsPanEnabled, "Helix pan should be enabled.");
            AssertTrue(viewport.IsZoomEnabled, "Helix zoom should be enabled.");
            AssertTrue(viewport.RotateAroundMouseDownPoint, "Helix should rotate around the mouse-down point.");
            AssertTrue(viewport.ZoomAroundMouseDownPoint, "Helix should zoom around the mouse-down point.");
            AssertEqual(MouseAction.LeftClick, GetMouseGesture(viewport.RotateGesture, "RotateGesture").MouseAction, "Left drag should rotate the plant view.");
            AssertEqual(MouseAction.RightClick, GetMouseGesture(viewport.PanGesture, "PanGesture").MouseAction, "Right drag should pan the plant view.");
            AssertEqual(MouseAction.MiddleClick, GetMouseGesture(viewport.PanGesture2, "PanGesture2").MouseAction, "Middle drag should pan the plant view.");
            AssertEqual(MouseAction.LeftDoubleClick, GetMouseGesture(viewport.ResetCameraGesture, "ResetCameraGesture").MouseAction, "Double-click should reset the plant view.");
        }
    }

    private static MouseGesture GetMouseGesture(InputGesture gesture, string name)
    {
        var mouseGesture = gesture as MouseGesture;
        if (mouseGesture == null)
        {
            throw new InvalidOperationException(name + " is not a MouseGesture.");
        }

        return mouseGesture;
    }

    private static void VerifyDashboardBindings()
    {
        using (var control = new Plant3DHostControl())
        {
            control.Width = 1400;
            control.Height = 850;
            control.CanOperateTanks = true;
            control.CanEngineerTanks = true;
            control.CreateControl();

            var tanks = CreateTanks(false);
            var hoists = new List<HoistModel>
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

            var telemetry = new SimulatorService().CreateEquipmentSnapshot(tanks, 1, DateTime.UtcNow);
            control.BindData(tanks, new List<WagonModel>(), new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, 4, "Copper", 42, true, false, 1, telemetry);
            control.SelectTank(4);

            AssertContains(GetLabel(control, "_plantStateLabel").Text, "Normal", "Plant normal state not bound.");
            AssertEqual("18", GetLabel(control, "_totalTankValue").Text, "Total tank count not bound.");
            AssertEqual("9", GetLabel(control, "_runningTankValue").Text, "Running tank count not bound.");
            AssertContains(GetLabel(control, "_hoistPositionValue").Text, "Tank 05", "Hoist position not bound.");
            AssertContains(GetLabel(control, "_currentStepValue").Text, "Copper", "Current process step not bound.");
            AssertContains(GetLabel(control, "_currentStepValue").Text, "42s", "Remaining process time not bound.");
            AssertContains(GetLabel(control, "_selectedEquipmentTitle").Text, "T04", "Selected equipment panel did not bind the tank number.");
            AssertContains(GetLabel(control, "_selectedEquipmentTitle").Text, "Acid", "Selected equipment panel did not bind the chemical name.");
            AssertContains(GetLabel(control, "_selectedLevelTemperatureValue").Text, "Temp", "Selected equipment panel did not bind temperature.");
            AssertContains(GetLabel(control, "_selectedElectricalValue").Text, "V", "Selected equipment panel did not bind electrical values.");
            AssertContains(GetLabel(control, "_selectedMotorValue").Text, "RUNNING", "Selected equipment panel did not bind confirmed motor feedback.");
            AssertContains(GetLabel(control, "_selectedQualityValue").Text, "GOOD", "Selected equipment panel did not bind telemetry quality.");
            AssertContains(GetLabel(control, "_communicationLabel").Text, "GOOD", "Telemetry heartbeat did not report healthy data.");
            AssertContains(GetLabel(control, "_communicationLabel").Text, "#1", "Telemetry heartbeat did not report the snapshot sequence.");
            AssertEqual("0", GetLabel(control, "_activeAlarmValue").Text, "Active alarm count should be zero in normal reference state.");
            AssertCommandState(control, "Auto", true, true);
            AssertCommandState(control, "Manual", true, false);
            AssertCommandState(control, "Start Job", true, false);
            AssertCommandState(control, "Stop", true, false);
            AssertCommandState(control, "Emergency Stop", true, false);

            foreach (var motor in telemetry.Motors)
            {
                motor.LastUpdatedUtc = DateTime.UtcNow.AddSeconds(-10);
            }
            var staleTelemetry = new PlantTelemetrySnapshot(
                2,
                DateTime.UtcNow.AddSeconds(-10),
                telemetry.Motors,
                telemetry.Pumps,
                telemetry.Valves,
                telemetry.Heaters,
                telemetry.Rectifiers);
            control.BindData(tanks, new List<WagonModel>(), new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, 4, "Copper", 42, true, false, 1, staleTelemetry);
            control.SelectTank(4);
            AssertContains(GetLabel(control, "_communicationLabel").Text, "STALE", "Old telemetry snapshot should change the heartbeat to STALE.");
            AssertContains(GetLabel(control, "_selectedMotorValue").Text, "STALE", "Old motor feedback should not remain green/running in the selected equipment panel.");

            var activeJobs = new List<JobModel>
            {
                new JobModel
                {
                    JobId = 1001,
                    LineId = 1,
                    CurrentStep = 5,
                    CurrentTank = 5,
                    Status = "Processing",
                    RemainingSeconds = 42
                }
            };
            control.BindData(tanks, new List<WagonModel>(), new List<ProcessStepModel>(), hoists, activeJobs, null, 4, "Copper", 42, true, false, 1);
            AssertCommandState(control, "Start Job", false, true);
            AssertCommandState(control, "Stop", true, false);

            var alarmTanks = CreateTanks(true);
            hoists[0].Status = "EmergencyStop";
            control.BindData(alarmTanks, new List<WagonModel>(), new List<ProcessStepModel>(), hoists, new List<JobModel>(), null, 4, "Emergency", 0, false, true, 1);

            AssertContains(GetLabel(control, "_plantStateLabel").Text, "Emergency Stop", "Emergency state not bound.");
            AssertContains(GetLabel(control, "_modeLabel").Text, "Manual", "Manual mode not bound.");
            AssertContains(GetLabel(control, "_hoistLabel").Text, "EmergencyStop", "Hoist emergency status not bound.");
            AssertEqual("3", GetLabel(control, "_activeAlarmValue").Text, "Emergency/alarm count not bound.");
            AssertCommandState(control, "Auto", true, false);
            AssertCommandState(control, "Manual", true, false);
            AssertCommandState(control, "Start Job", false, false);
            AssertCommandState(control, "Stop", false, false);
            AssertCommandState(control, "Emergency Stop", false, true);
            AssertCommandState(control, "Reset", true, false);
        }
    }

    private static void VerifySafePlcWriteGuard()
    {
        using (var adapter = new SafePlcAdapter())
        {
            AssertTrue(!adapter.WritesEnabled, "PLC writes should be disabled by default.");

            var blocked = false;
            try
            {
                adapter.SendTextAsync("START").GetAwaiter().GetResult();
            }
            catch (InvalidOperationException)
            {
                blocked = true;
            }

            AssertTrue(blocked, "PLC raw write was not blocked while writes were disabled.");

            var reasonRequired = false;
            try
            {
                adapter.SetWritesEnabled(true, string.Empty);
            }
            catch (InvalidOperationException)
            {
                reasonRequired = true;
            }

            AssertTrue(reasonRequired, "PLC write enable did not require a confirmation reason.");
        }
    }

    private static void VerifyDashboardMenuShell()
    {
        using (var control = new Plant3DHostControl())
        {
            control.Width = 1500;
            control.Height = 900;
            control.CanOperateTanks = true;
            control.CanEngineerTanks = true;
            control.CreateControl();

            AssertMissingField<Plant3DHostControl>("_dashboardMenu", "Old grouped dashboard menu should be removed.");
            var viewMenu = GetContextMenu(control, "_viewMenu");
            AssertEqual(5, viewMenu.Items.Count, "View sidebar menu has unexpected item count.");
            AssertMenuItem(viewMenu, "Fit Plant", "Fit Plant");
            AssertMenuItem(viewMenu, "Front View", "Front View");
            AssertMenuItem(viewMenu, "Top View", "Top View");
            AssertMenuItem(viewMenu, "Left View", "Left View");
            AssertMenuItem(viewMenu, "Right View", "Right View");

            var tankMenu = GetContextMenu(control, "_tankMenu");
            AssertEqual(3, tankMenu.Items.Count, "Tanks sidebar menu has unexpected item count.");
            AssertMenuItem(tankMenu, "Add Tank", "Add Tank");
            AssertMenuItem(tankMenu, "Edit Tank", "Edit Tank");
            AssertMenuItem(tankMenu, "Remove Tank", "Remove Tank");

            var recipeMenu = GetContextMenu(control, "_recipeMenu");
            AssertEqual(2, recipeMenu.Items.Count, "Recipe sidebar menu has unexpected item count.");
            AssertMenuItem(recipeMenu, "Create Recipe", "Recipe");
            AssertMenuItem(recipeMenu, "Edit Recipe", "Recipe");

            var settingsMenu = GetContextMenu(control, "_settingsMenu");
            AssertEqual(1, settingsMenu.Items.Count, "Settings sidebar menu has unexpected item count.");
            AssertMenuItem(settingsMenu, "User Management", "Users");

            var accountMenu = GetContextMenu(control, "_accountMenu");
            AssertEqual(2, accountMenu.Items.Count, "Operator account menu should contain only account actions.");
            AssertMenuItem(accountMenu, "Login Another", "Login");
            AssertMenuItem(accountMenu, "Logout", "Logout");
            AssertNoMenuItemText(viewMenu, "Plant Operations");
            AssertNoMenuItemText(tankMenu, "Production");
            AssertNoMenuItemText(recipeMenu, "Engineering");
            AssertNoMenuItemText(settingsMenu, "Engineering");
            var brandHeader = FindControl<ScadaBrandHeader>(control);
            AssertTrue(brandHeader != null, "Brand header was not found.");
            AssertTrue(brandHeader.ContextMenuStrip == null, "Grouped dashboard menu should not be attached to the top header brand.");
            AssertTrue(!FindControlText(control, "Menu"), "Generic sidebar Menu entry should be removed.");
            AssertNavigationMenu(control, "View", viewMenu);
            AssertNavigationMenu(control, "Tanks", tankMenu);
            AssertNavigationMenu(control, "Recipe", recipeMenu);
            AssertNavigationMenu(control, "Settings", settingsMenu);
            AssertTrue(InvokeLocalDashboardCommand(control, "Top View"), "Top View command should be handled by the 3D dashboard host.");
            AssertTrue(InvokeLocalDashboardCommand(control, "Fit Plant"), "Fit Plant command should be handled by the 3D dashboard host.");
            AssertTrue(FindControlText(control, "View All Alarms"), "Right sidebar View All Alarms action was not found.");
            AssertTrue(FindControlText(control, "IP Connection"), "Bottom IP Connection command was not found.");
            AssertTrue(!FindControlText(control, "Fit Plant"), "Viewport camera commands should be grouped in the menu, not shown as a visible toolbar.");
            AssertContains(GetLabel(control, "_operatorLabel").Text, "Operator", "Operator header label not found.");
        }
    }

    private static List<TankModel> CreateTanks(bool includeFault)
    {
        var tanks = new List<TankModel>();
        for (var i = 1; i <= 18; i++)
        {
            tanks.Add(new TankModel
            {
                Id = i,
                LineId = 1,
                TankNo = i,
                Name = i == 4 ? "Acid Dip" : i == 5 ? "Copper" : "Process " + i,
                ChemicalName = i == 4 ? "Acid Dip" : i == 5 ? "Copper" : "Process",
                CapacityLiters = 1000,
                CurrentLevelLiters = i == 8 ? 840 : 750,
                TemperatureCelsius = 30 + i,
                CurrentAmps = i == 5 ? 138 : 90 + i,
                Voltage = i == 5 ? 12.1 : 9.5 + (i * 0.1),
                Status = includeFault && i == 10 ? "Fault" : i <= 9 ? "Running" : "Normal",
                IsActive = true
            });
        }

        return tanks;
    }

    private static Label GetLabel(Plant3DHostControl control, string fieldName)
    {
        var field = typeof(Plant3DHostControl).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException("Missing label field: " + fieldName);
        }

        var label = field.GetValue(control) as Label;
        if (label == null)
        {
            throw new InvalidOperationException("Field is not a Label: " + fieldName);
        }

        return label;
    }

    private static ContextMenuStrip GetContextMenu(Plant3DHostControl control, string fieldName)
    {
        var field = typeof(Plant3DHostControl).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException("Missing context menu field: " + fieldName);
        }

        var menu = field.GetValue(control) as ContextMenuStrip;
        if (menu == null)
        {
            throw new InvalidOperationException("Field is not a ContextMenuStrip: " + fieldName);
        }

        return menu;
    }

    private static void AssertMenuItem(ContextMenuStrip menu, string text, string command)
    {
        foreach (ToolStripItem item in menu.Items)
        {
            if (string.Equals(item.Text, text, StringComparison.OrdinalIgnoreCase))
            {
                AssertEqual(command, Convert.ToString(item.Tag), "Menu item command mismatch: " + text);
                return;
            }
        }

        throw new InvalidOperationException("Missing account menu item: " + text);
    }

    private static void AssertNavigationMenu(Plant3DHostControl control, string text, ContextMenuStrip expectedMenu)
    {
        var button = FindNavButton(control, text);
        AssertTrue(button != null, "Sidebar " + text + " entry was not found.");
        AssertTrue(object.ReferenceEquals(expectedMenu, button.ContextMenuStrip), "Sidebar " + text + " entry is not wired to the expected submenu.");
    }

    private static ScadaNavButton FindNavButton(Control control, string text)
    {
        var button = control as ScadaNavButton;
        if (button != null && string.Equals(button.Text, text, StringComparison.OrdinalIgnoreCase))
        {
            return button;
        }

        foreach (Control child in control.Controls)
        {
            button = FindNavButton(child, text);
            if (button != null)
            {
                return button;
            }
        }

        return null;
    }

    private static void AssertNoMenuItemText(ContextMenuStrip menu, string text)
    {
        foreach (ToolStripItem item in menu.Items)
        {
            if (string.Equals(item.Text, text, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Unexpected menu item remains: " + text);
            }
        }
    }

    private static void AssertMissingField<T>(string fieldName, string message)
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        AssertTrue(field == null, message);
    }

    private static bool FindControlText(Control control, string text)
    {
        if (string.Equals(control.Text, text, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (Control child in control.Controls)
        {
            if (FindControlText(child, text))
            {
                return true;
            }
        }

        return false;
    }

    private static void AssertCommandState(Plant3DHostControl control, string command, bool expectedEnabled, bool expectedActive)
    {
        var button = FindCommandButton(control, command);
        AssertTrue(button != null, "Missing dashboard command button: " + command);
        AssertEqual(expectedEnabled, button.Enabled, "Command enabled state mismatch: " + command);

        var scadaButton = button as ScadaCommandButton;
        AssertTrue(scadaButton != null, "Dashboard command should use ScadaCommandButton: " + command);
        AssertEqual(expectedActive, scadaButton.Active, "Command active state mismatch: " + command);
    }

    private static Button FindCommandButton(Control control, string command)
    {
        var button = control as Button;
        if (button != null && string.Equals(Convert.ToString(button.Tag), command, StringComparison.OrdinalIgnoreCase))
        {
            return button;
        }

        foreach (Control child in control.Controls)
        {
            button = FindCommandButton(child, command);
            if (button != null)
            {
                return button;
            }
        }

        return null;
    }

    private static bool InvokeLocalDashboardCommand(Plant3DHostControl control, string command)
    {
        var method = typeof(Plant3DHostControl).GetMethod("HandleLocalDashboardCommand", BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException("Missing HandleLocalDashboardCommand method.");
        }

        return Convert.ToBoolean(method.Invoke(control, new object[] { command }));
    }

    private static T FindControl<T>(Control control) where T : Control
    {
        var match = control as T;
        if (match != null)
        {
            return match;
        }

        foreach (Control child in control.Controls)
        {
            match = FindControl<T>(child);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static void AssertContains(string actual, string expected, string message)
    {
        if (actual == null || actual.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0)
        {
            throw new InvalidOperationException(message + " Expected contains '" + expected + "', actual '" + actual + "'.");
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!object.Equals(expected, actual))
        {
            throw new InvalidOperationException(message + " Expected '" + expected + "', actual '" + actual + "'.");
        }
    }

    private static void AssertTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
