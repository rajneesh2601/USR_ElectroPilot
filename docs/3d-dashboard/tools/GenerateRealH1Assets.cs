using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

internal static class GenerateRealH1Assets
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private static int Main(string[] args)
    {
        var outputFolder = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "USR_ElectroPilot", "Assets", "Models"));

        Directory.CreateDirectory(outputFolder);
        File.WriteAllText(Path.Combine(outputFolder, "h1_industrial.mtl"), CreateMaterialLibrary());

        BuildPortal().Save(Path.Combine(outputFolder, "hoist_h1.obj"));
        BuildMotors().Save(Path.Combine(outputFolder, "motor_gearbox.obj"));
        BuildLiftRack().Save(Path.Combine(outputFolder, "plate_rack.obj"));
        BuildWorkpieces().Save(Path.Combine(outputFolder, "hanging_plate.obj"));
        BuildTank().Save(Path.Combine(outputFolder, "tank_standard.obj"));
        BuildPipeManifold().Save(Path.Combine(outputFolder, "pipe_manifold.obj"));
        BuildPumpMotor().Save(Path.Combine(outputFolder, "pump.obj"));
        BuildWalkway().Save(Path.Combine(outputFolder, "walkway.obj"));

        Console.WriteLine("Generated reference-matched H1 assets in " + outputFolder);
        return 0;
    }

    private static ObjBuilder BuildPortal()
    {
        var model = new ObjBuilder("H1 fabricated portal frame");

        // Main fabricated bridge and two rail-supported columns.
        model.AddBeveledBox("main_bridge", new V3(0, 0, 2.62), new V3(0.58, 3.18, 0.42), 0.055, "SafetyYellow");
        model.AddBeveledBox("left_column", new V3(0, -1.35, 2.17), new V3(0.40, 0.42, 0.82), 0.035, "SafetyYellow");
        model.AddBeveledBox("right_column", new V3(0, 1.35, 2.17), new V3(0.40, 0.42, 0.82), 0.035, "SafetyYellow");
        model.AddBeveledBox("left_end_plate", new V3(0, -1.47, 2.62), new V3(0.72, 0.08, 0.50), 0.018, "YellowEdge");
        model.AddBeveledBox("right_end_plate", new V3(0, 1.47, 2.62), new V3(0.72, 0.08, 0.50), 0.018, "YellowEdge");

        // Lower tie beam and diagonal gussets make the portal read as welded machinery.
        model.AddBeveledBox("lower_tie", new V3(0, 0, 2.23), new V3(0.28, 2.62, 0.18), 0.025, "SafetyYellow");
        model.AddBeveledBox("front_stiffener", new V3(-0.23, 0, 2.47), new V3(0.09, 2.56, 0.28), 0.018, "YellowEdge");
        AddGusset(model, -1.18);
        AddGusset(model, 1.18);

        // Four-wheel bogies, bearing blocks, axles, and rail shoes.
        AddBogie(model, -1.35, -0.13);
        AddBogie(model, -1.35, 0.13);
        AddBogie(model, 1.35, -0.13);
        AddBogie(model, 1.35, 0.13);

        // Bolted bridge end plates and column base plates.
        foreach (var y in new[] { -1.47, 1.47 })
        {
            foreach (var x in new[] { -0.22, 0.22 })
            {
                foreach (var z in new[] { 2.48, 2.76 })
                {
                    model.AddCylinder("bridge_bolt", new V3(x, y - Math.Sign(y) * 0.047, z), new V3(0, 1, 0), 0.025, 0.025, 12, "BoltSteel");
                    model.AddCylinder("bridge_washer", new V3(x, y - Math.Sign(y) * 0.043, z), new V3(0, 1, 0), 0.038, 0.009, 18, "Stainless");
                }
            }
        }

        // Cable tray and flexible power cable cue along the rear of the bridge.
        model.AddBeveledBox("cable_tray", new V3(0.18, 0, 2.91), new V3(0.16, 2.72, 0.07), 0.012, "DarkSteel");
        model.AddCylinder("power_cable", new V3(0.29, 0, 2.91), new V3(0, 1, 0), 0.026, 2.52, 16, "Rubber");

        // Beam-applied machine name, built as shallow raised geometry instead of floating UI.
        model.AddPixelText("USR ELECTROPILOT H1", new V3(-0.302, -1.18, 2.62), 0.036, 0.010, "BrandBlue");

        // Compact status stack on the front-right column.
        model.AddBeveledBox("status_mount", new V3(-0.23, -1.48, 2.18), new V3(0.07, 0.08, 0.30), 0.008, "DarkSteel");
        model.AddCylinder("status_green", new V3(-0.27, -1.525, 2.27), new V3(1, 0, 0), 0.035, 0.035, 18, "StatusGreen");
        model.AddCylinder("status_amber", new V3(-0.27, -1.525, 2.18), new V3(1, 0, 0), 0.035, 0.035, 18, "StatusAmber");
        model.AddCylinder("status_red", new V3(-0.27, -1.525, 2.09), new V3(1, 0, 0), 0.035, 0.035, 18, "StatusRed");

        return model;
    }

    private static ObjBuilder BuildMotors()
    {
        var model = new ObjBuilder("H1 travel and lift drives");

        // Side-mounted geared travel motor, compactly attached to the front column/beam end.
        model.AddBeveledBox("travel_mount", new V3(0.02, -1.63, 2.62), new V3(0.52, 0.18, 0.48), 0.035, "SafetyYellow");
        model.AddBeveledBox("travel_gearbox", new V3(-0.02, -1.76, 2.62), new V3(0.38, 0.26, 0.38), 0.045, "GearboxGrey");
        model.AddCylinder("travel_coupling", new V3(0, -1.92, 2.62), new V3(0, 1, 0), 0.09, 0.14, 24, "DarkSteel");
        model.AddCylinder("travel_motor_body", new V3(0, -2.11, 2.62), new V3(0, 1, 0), 0.18, 0.34, 32, "MotorGrey");
        model.AddCylinder("travel_fan_cover", new V3(0, -2.31, 2.62), new V3(0, 1, 0), 0.195, 0.08, 32, "DarkSteel");
        for (var i = 0; i < 7; i++)
        {
            model.AddCylinder("travel_cooling_fin", new V3(0, -1.97 - (i * 0.045), 2.62), new V3(0, 1, 0), 0.205, 0.014, 32, "DarkSteel");
        }
        model.AddCylinder("travel_shaft", new V3(0, -1.60, 2.62), new V3(0, 1, 0), 0.045, 0.18, 18, "Stainless");
        AddGearboxBolts(model, new V3(-0.02, -1.905, 2.62), new V3(0, 1, 0));

        // Vertically mounted lift motor, gearbox and drum connection.
        model.AddBeveledBox("lift_mount", new V3(0, 0, 2.82), new V3(0.44, 0.50, 0.10), 0.018, "SafetyYellow");
        model.AddBeveledBox("lift_gearbox", new V3(0, 0, 2.92), new V3(0.34, 0.38, 0.20), 0.035, "GearboxGrey");
        model.AddCylinder("lift_motor_body", new V3(0, 0, 3.16), new V3(0, 0, 1), 0.15, 0.34, 30, "MotorGrey");
        model.AddCylinder("lift_motor_cap", new V3(0, 0, 3.36), new V3(0, 0, 1), 0.16, 0.08, 30, "DarkSteel");
        for (var i = 0; i < 6; i++)
        {
            model.AddCylinder("lift_cooling_fin", new V3(0, 0, 3.03 + (i * 0.052)), new V3(0, 0, 1), 0.175, 0.014, 30, "DarkSteel");
        }
        model.AddCylinder("lift_drive_shaft", new V3(0, 0, 2.70), new V3(0, 0, 1), 0.055, 0.28, 20, "Stainless");
        model.AddCylinder("lift_drum", new V3(0, 0, 2.50), new V3(1, 0, 0), 0.12, 0.46, 28, "DarkSteel");
        for (var i = -4; i <= 4; i++)
        {
            model.AddTorus("drum_groove", new V3(i * 0.045, 0, 2.50), new V3(1, 0, 0), 0.125, 0.007, 24, 6, "Stainless");
        }

        return model;
    }

    private static ObjBuilder BuildLiftRack()
    {
        var model = new ObjBuilder("H1 moving lift frame and carrier");

        // Secondary moving frame held inside the portal by four guide rollers.
        model.AddBeveledBox("lift_upper_beam", new V3(0, 0, 2.24), new V3(0.26, 2.34, 0.18), 0.025, "SafetyYellow");
        model.AddBeveledBox("lift_lower_beam", new V3(0, 0, 2.04), new V3(0.22, 2.18, 0.14), 0.022, "SafetyYellow");
        model.AddBeveledBox("lift_left_guide", new V3(0, -1.02, 2.14), new V3(0.22, 0.16, 0.42), 0.018, "SafetyYellow");
        model.AddBeveledBox("lift_right_guide", new V3(0, 1.02, 2.14), new V3(0.22, 0.16, 0.42), 0.018, "SafetyYellow");
        model.AddBeveledBox("central_drive_block", new V3(0, 0, 2.31), new V3(0.30, 0.30, 0.22), 0.025, "YellowEdge");

        foreach (var y in new[] { -1.10, 1.10 })
        {
            foreach (var z in new[] { 2.03, 2.25 })
            {
                model.AddCylinder("guide_roller", new V3(-0.15, y, z), new V3(1, 0, 0), 0.055, 0.10, 18, "Rubber");
                model.AddCylinder("guide_axle", new V3(-0.22, y, z), new V3(1, 0, 0), 0.022, 0.16, 14, "Stainless");
            }
        }

        // Copper carrier bars, support hangers and real linked-chain approximations.
        model.AddBeveledBox("carrier_spine", new V3(0, 0, 1.92), new V3(0.16, 1.92, 0.10), 0.014, "Copper");
        model.AddCylinder("carrier_front", new V3(-0.18, 0, 1.86), new V3(0, 1, 0), 0.035, 1.84, 20, "Copper");
        model.AddCylinder("carrier_rear", new V3(0.18, 0, 1.86), new V3(0, 1, 0), 0.035, 1.84, 20, "Copper");

        foreach (var y in new[] { -0.82, 0.82 })
        {
            AddVerticalChain(model, new V3(0, y, 2.03), 4, 0.055);
            model.AddBeveledBox("carrier_clamp", new V3(0, y, 1.84), new V3(0.22, 0.10, 0.08), 0.012, "Stainless");
        }

        return model;
    }

    private static ObjBuilder BuildWorkpieces()
    {
        var model = new ObjBuilder("H1 PCB workpiece load");
        const int boardCount = 12;
        const double firstY = -0.82;
        const double spacing = 1.64 / (boardCount - 1);

        for (var i = 0; i < boardCount; i++)
        {
            var y = firstY + (i * spacing);
            var xOffset = ((i % 3) - 1) * 0.006;
            var zOffset = (i % 2) * 0.006;

            AddBoardHanger(model, i, new V3(xOffset, y, 1.97 + zOffset));
            AddPcb(model, i, new V3(xOffset, y, 1.59 + zOffset));
        }

        return model;
    }

    private static ObjBuilder BuildTank()
    {
        var model = new ObjBuilder("Reusable open electroplating process tank");
        const double halfX = 0.575;
        const double halfY = 0.925;

        // Open fabricated/polypropylene shell with a single clear tank opening.
        model.AddBeveledBox("tank_front_wall", new V3(0, -halfY, 0.52), new V3(1.15, 0.085, 1.04), 0.018, "TankPoly");
        model.AddBeveledBox("tank_rear_wall", new V3(0, halfY, 0.52), new V3(1.15, 0.085, 1.04), 0.018, "TankPoly");
        model.AddBeveledBox("tank_left_wall", new V3(-halfX, 0, 0.52), new V3(0.085, 1.85, 1.04), 0.018, "TankPoly");
        model.AddBeveledBox("tank_right_wall", new V3(halfX, 0, 0.52), new V3(0.085, 1.85, 1.04), 0.018, "TankPoly");
        model.AddBeveledBox("tank_floor", new V3(0, 0, 0.035), new V3(1.15, 1.85, 0.07), 0.014, "TankInterior");

        // Thick welded rim with bevels to catch factory lighting.
        model.AddBeveledBox("front_rim", new V3(0, -0.965, 1.07), new V3(1.28, 0.13, 0.13), 0.028, "TankEdge");
        model.AddBeveledBox("rear_rim", new V3(0, 0.965, 1.07), new V3(1.28, 0.13, 0.13), 0.028, "TankEdge");
        model.AddBeveledBox("left_rim", new V3(-0.615, 0, 1.07), new V3(0.13, 1.95, 0.13), 0.028, "TankEdge");
        model.AddBeveledBox("right_rim", new V3(0.615, 0, 1.07), new V3(0.13, 1.95, 0.13), 0.028, "TankEdge");

        // External ribs and reinforcement bands make the shell read as manufactured equipment.
        for (var i = 0; i < 5; i++)
        {
            var x = -0.46 + i * 0.23;
            model.AddBeveledBox("front_vertical_rib", new V3(x, -0.985, 0.53), new V3(0.055, 0.055, 0.82), 0.010, "TankEdge");
            model.AddBeveledBox("rear_vertical_rib", new V3(x, 0.985, 0.53), new V3(0.055, 0.055, 0.82), 0.010, "TankEdge");
        }
        model.AddBeveledBox("front_band", new V3(0, -1.015, 0.72), new V3(1.08, 0.055, 0.075), 0.012, "Stainless");
        model.AddBeveledBox("rear_band", new V3(0, 1.015, 0.72), new V3(1.08, 0.055, 0.075), 0.012, "Stainless");
        foreach (var x in new[] { -0.47, 0.47 })
        foreach (var y in new[] { -0.78, 0.78 })
        {
            model.AddBeveledBox("tank_support", new V3(x, y, -0.11), new V3(0.16, 0.18, 0.24), 0.022, "DarkSteel");
            model.AddBeveledBox("tank_mounting_pad", new V3(x, y, -0.24), new V3(0.25, 0.28, 0.045), 0.012, "Stainless");
        }

        // Front instrument/label plate and dynamic status-lamp bezel.
        model.AddBeveledBox("label_backplate", new V3(0, -1.015, 0.84), new V3(0.62, 0.055, 0.22), 0.020, "LabelPlate");
        model.AddCylinder("status_bezel", new V3(-0.43, -1.055, 0.76), new V3(0, 1, 0), 0.075, 0.055, 24, "DarkSteel");

        // Sight glass with stainless end fittings.
        model.AddCylinder("sight_glass", new V3(0.43, -1.055, 0.54), new V3(0, 0, 1), 0.025, 0.52, 18, "SightGlass");
        model.AddCylinder("sight_glass_top", new V3(0.43, -1.055, 0.82), new V3(0, 0, 1), 0.045, 0.07, 18, "Stainless");
        model.AddCylinder("sight_glass_bottom", new V3(0.43, -1.055, 0.26), new V3(0, 0, 1), 0.045, 0.07, 18, "Stainless");

        // Drain nozzle, flange and capped process connection.
        model.AddCylinder("tank_outlet", new V3(0.68, -0.54, 0.27), new V3(1, 0, 0), 0.052, 0.20, 20, "PipeSteel");
        model.AddCylinder("tank_outlet_flange", new V3(0.78, -0.54, 0.27), new V3(1, 0, 0), 0.092, 0.045, 24, "Stainless");
        for (var i = 0; i < 6; i++)
        {
            var angle = Math.PI * 2.0 * i / 6.0;
            model.AddCylinder("outlet_flange_bolt", new V3(0.81, -0.54 + Math.Cos(angle) * 0.067, 0.27 + Math.Sin(angle) * 0.067), new V3(1, 0, 0), 0.010, 0.035, 10, "BoltSteel");
        }

        return model;
    }

    private static ObjBuilder BuildPipeManifold()
    {
        var model = new ObjBuilder("Per-tank process manifold and valve module");

        // Repeatable manifold segments join into two continuous process lines.
        model.AddCylinder("main_process_pipe", new V3(0, -1.52, 0.39), new V3(1, 0, 0), 0.055, 1.45, 28, "PipeSteel");
        model.AddCylinder("return_process_pipe", new V3(0, -1.66, 0.24), new V3(1, 0, 0), 0.038, 1.45, 24, "PipeSteel");
        foreach (var x in new[] { -0.69, 0.69 })
        {
            model.AddCylinder("main_union", new V3(x, -1.52, 0.39), new V3(1, 0, 0), 0.078, 0.045, 24, "Stainless");
            model.AddCylinder("return_union", new V3(x, -1.66, 0.24), new V3(1, 0, 0), 0.058, 0.040, 22, "Stainless");
        }

        model.AddCylinder("tank_branch", new V3(0, -1.27, 0.39), new V3(0, 1, 0), 0.043, 0.50, 24, "PipeSteel");
        model.AddCylinder("branch_riser", new V3(0, -1.03, 0.56), new V3(0, 0, 1), 0.043, 0.34, 24, "PipeSteel");
        model.AddCylinder("branch_flange", new V3(0, -1.02, 0.72), new V3(0, 0, 1), 0.084, 0.045, 24, "Stainless");
        model.AddCylinder("valve_body", new V3(0, -1.20, 0.39), new V3(0, 1, 0), 0.092, 0.12, 26, "ValveOrange");
        model.AddCylinder("valve_stem", new V3(0, -1.20, 0.52), new V3(0, 0, 1), 0.022, 0.22, 16, "Stainless");
        model.AddTorus("valve_handwheel", new V3(0, -1.20, 0.65), new V3(0, 0, 1), 0.105, 0.014, 24, 8, "ValveOrange");
        for (var i = 0; i < 4; i++)
        {
            var angle = Math.PI * 2.0 * i / 4.0;
            model.AddCylinder("handwheel_spoke", new V3(Math.Cos(angle) * 0.052, -1.20 + Math.Sin(angle) * 0.052, 0.65), new V3(Math.Cos(angle), Math.Sin(angle), 0), 0.010, 0.10, 10, "ValveOrange");
        }

        // Galvanized support bracket and flange fasteners.
        model.AddBeveledBox("manifold_support", new V3(0, -1.54, 0.08), new V3(0.24, 0.28, 0.08), 0.014, "DarkSteel");
        model.AddBeveledBox("manifold_stanchion", new V3(0, -1.54, 0.22), new V3(0.08, 0.10, 0.28), 0.012, "Stainless");
        return model;
    }

    private static ObjBuilder BuildPumpMotor()
    {
        var model = new ObjBuilder("Per-tank pump and finned electric motor assembly");

        model.AddBeveledBox("pump_skid", new V3(0.30, -1.28, 0.11), new V3(0.72, 0.42, 0.08), 0.018, "DarkSteel");
        model.AddBeveledBox("pump_skid_left_foot", new V3(0.04, -1.28, 0.04), new V3(0.20, 0.34, 0.08), 0.014, "Stainless");
        model.AddBeveledBox("pump_skid_right_foot", new V3(0.54, -1.28, 0.04), new V3(0.20, 0.34, 0.08), 0.014, "Stainless");

        // Volute, inlet/outlet flanges and guarded coupling.
        model.AddCylinder("pump_volute", new V3(-0.02, -1.28, 0.36), new V3(0, 1, 0), 0.17, 0.20, 30, "PumpBlue");
        model.AddCylinder("pump_hub", new V3(-0.02, -1.39, 0.36), new V3(0, 1, 0), 0.075, 0.08, 24, "Stainless");
        model.AddCylinder("pump_inlet", new V3(-0.02, -1.51, 0.36), new V3(0, 1, 0), 0.055, 0.20, 24, "PipeSteel");
        model.AddCylinder("pump_inlet_flange", new V3(-0.02, -1.62, 0.36), new V3(0, 1, 0), 0.095, 0.045, 24, "Stainless");
        model.AddCylinder("pump_outlet", new V3(-0.02, -1.28, 0.57), new V3(0, 0, 1), 0.052, 0.25, 24, "PipeSteel");
        model.AddCylinder("pump_outlet_flange", new V3(-0.02, -1.28, 0.70), new V3(0, 0, 1), 0.092, 0.045, 24, "Stainless");
        model.AddCylinder("pump_shaft", new V3(0.15, -1.28, 0.36), new V3(1, 0, 0), 0.035, 0.34, 18, "Stainless");
        model.AddBeveledBox("coupling_guard", new V3(0.19, -1.28, 0.36), new V3(0.28, 0.22, 0.20), 0.035, "SafetyYellow");

        // Separate finned motor body, fan housing, terminal box and mounting feet.
        model.AddCylinder("pump_motor_body", new V3(0.45, -1.28, 0.36), new V3(1, 0, 0), 0.145, 0.42, 32, "MotorBlue");
        model.AddCylinder("pump_motor_front", new V3(0.23, -1.28, 0.36), new V3(1, 0, 0), 0.158, 0.07, 30, "DarkSteel");
        model.AddCylinder("pump_motor_fan", new V3(0.68, -1.28, 0.36), new V3(1, 0, 0), 0.165, 0.09, 32, "DarkSteel");
        for (var i = 0; i < 8; i++)
        {
            model.AddCylinder("pump_motor_fin", new V3(0.29 + i * 0.052, -1.28, 0.36), new V3(1, 0, 0), 0.172, 0.014, 32, "DarkSteel");
        }
        model.AddBeveledBox("motor_terminal_box", new V3(0.45, -1.28, 0.55), new V3(0.22, 0.22, 0.12), 0.022, "MotorBlue");
        model.AddBeveledBox("motor_front_foot", new V3(0.31, -1.28, 0.18), new V3(0.16, 0.25, 0.08), 0.014, "DarkSteel");
        model.AddBeveledBox("motor_rear_foot", new V3(0.59, -1.28, 0.18), new V3(0.16, 0.25, 0.08), 0.014, "DarkSteel");

        return model;
    }

    private static ObjBuilder BuildWalkway()
    {
        var model = new ObjBuilder("Repeatable galvanized service walkway module");

        // Open grating grid, not a solid floor panel.
        for (var i = 0; i < 8; i++)
        {
            var y = -2.34 + i * 0.085;
            model.AddBeveledBox("walkway_longitudinal_grate", new V3(0, y, -0.12), new V3(1.43, 0.022, 0.035), 0.006, "GratingGalv");
        }
        for (var i = 0; i < 11; i++)
        {
            var x = -0.68 + i * 0.136;
            model.AddBeveledBox("walkway_cross_grate", new V3(x, -2.04, -0.105), new V3(0.022, 0.66, 0.028), 0.005, "GratingGalv");
        }

        model.AddBeveledBox("walkway_inner_beam", new V3(0, -1.72, -0.15), new V3(1.45, 0.09, 0.13), 0.018, "FrameBlue");
        model.AddBeveledBox("walkway_outer_beam", new V3(0, -2.39, -0.15), new V3(1.45, 0.09, 0.13), 0.018, "FrameBlue");
        foreach (var x in new[] { -0.68, 0.68 })
        {
            model.AddCylinder("guard_post", new V3(x, -2.39, 0.22), new V3(0, 0, 1), 0.025, 0.74, 18, "FrameBlue");
            model.AddBeveledBox("walkway_support", new V3(x, -2.05, -0.32), new V3(0.09, 0.62, 0.34), 0.016, "FrameBlue");
        }
        model.AddCylinder("upper_guard_rail", new V3(0, -2.39, 0.56), new V3(1, 0, 0), 0.030, 1.45, 20, "FrameBlue");
        model.AddCylinder("middle_guard_rail", new V3(0, -2.39, 0.31), new V3(1, 0, 0), 0.025, 1.45, 18, "FrameBlue");
        model.AddBeveledBox("toe_plate", new V3(0, -2.35, -0.015), new V3(1.45, 0.035, 0.16), 0.010, "FrameBlue");

        return model;
    }

    private static void AddGusset(ObjBuilder model, double y)
    {
        model.AddBeveledBox("column_gusset", new V3(0.20, y, 2.40), new V3(0.12, 0.30, 0.34), 0.020, "YellowEdge");
        model.AddBeveledBox("base_plate", new V3(0, y, 1.78), new V3(0.58, 0.54, 0.10), 0.018, "SafetyYellow");
    }

    private static void AddBogie(ObjBuilder model, double y, double x)
    {
        model.AddBeveledBox("bogie_block", new V3(x, y, 1.68), new V3(0.34, 0.30, 0.16), 0.025, "DarkSteel");
        model.AddBeveledBox("bearing_housing", new V3(x, y - Math.Sign(y) * 0.18, 1.68), new V3(0.20, 0.12, 0.20), 0.025, "GearboxGrey");
        model.AddCylinder("rail_wheel", new V3(x, y, 1.58), new V3(0, 1, 0), 0.105, 0.24, 28, "WheelSteel");
        model.AddCylinder("wheel_axle", new V3(x, y, 1.58), new V3(0, 1, 0), 0.035, 0.36, 18, "Stainless");
        model.AddCylinder("bearing_cap", new V3(x, y - Math.Sign(y) * 0.20, 1.68), new V3(0, 1, 0), 0.060, 0.035, 18, "Stainless");
    }

    private static void AddGearboxBolts(ObjBuilder model, V3 center, V3 axis)
    {
        foreach (var x in new[] { -0.13, 0.13 })
        {
            foreach (var z in new[] { -0.13, 0.13 })
            {
                model.AddCylinder("gearbox_bolt", new V3(center.X + x, center.Y, center.Z + z), axis, 0.022, 0.028, 12, "BoltSteel");
            }
        }
    }

    private static void AddVerticalChain(ObjBuilder model, V3 top, int links, double spacing)
    {
        for (var i = 0; i < links; i++)
        {
            var normal = i % 2 == 0 ? new V3(0, 1, 0) : new V3(1, 0, 0);
            model.AddTorus("chain_link", new V3(top.X, top.Y, top.Z - (i * spacing)), normal, 0.030, 0.008, 14, 6, "ChainSteel");
        }
    }

    private static void AddBoardHanger(ObjBuilder model, int index, V3 top)
    {
        AddVerticalChain(model, top, 3, 0.048);
        model.AddTorus("pcb_hook_" + index, new V3(top.X, top.Y, top.Z - 0.15), new V3(0, 1, 0), 0.038, 0.009, 16, 6, "ChainSteel");
        model.AddBeveledBox("pcb_clamp_" + index, new V3(top.X, top.Y, top.Z - 0.205), new V3(0.12, 0.055, 0.065), 0.010, "Stainless");
    }

    private static void AddPcb(ObjBuilder model, int index, V3 center)
    {
        model.AddBeveledBox("pcb_" + index, center, new V3(0.46, 0.022, 0.62), 0.018, "PcbGreen");
        model.AddBeveledBox("pcb_top_contact_" + index, new V3(center.X, center.Y - 0.013, center.Z + 0.255), new V3(0.34, 0.009, 0.045), 0.006, "Copper");
        model.AddBeveledBox("pcb_left_trace_" + index, new V3(center.X - 0.14, center.Y - 0.014, center.Z + 0.02), new V3(0.025, 0.009, 0.36), 0.004, "Copper");
        model.AddBeveledBox("pcb_right_trace_" + index, new V3(center.X + 0.13, center.Y - 0.014, center.Z - 0.02), new V3(0.022, 0.009, 0.30), 0.004, "Copper");
        model.AddBeveledBox("pcb_cross_trace_" + index, new V3(center.X, center.Y - 0.014, center.Z - 0.13), new V3(0.31, 0.009, 0.022), 0.004, "Copper");
        model.AddCylinder("pcb_hole_left_" + index, new V3(center.X - 0.16, center.Y - 0.018, center.Z + 0.25), new V3(0, 1, 0), 0.018, 0.012, 14, "DarkSteel");
        model.AddCylinder("pcb_hole_right_" + index, new V3(center.X + 0.16, center.Y - 0.018, center.Z + 0.25), new V3(0, 1, 0), 0.018, 0.012, 14, "DarkSteel");
        model.AddCylinder("pcb_lower_pin_" + index, new V3(center.X, center.Y, center.Z - 0.37), new V3(0, 0, 1), 0.012, 0.16, 12, "Copper");
    }

    private static string CreateMaterialLibrary()
    {
        return
            "# USR ElectroPilot H1 industrial materials\r\n" +
            Material("SafetyYellow", 0.94, 0.57, 0.015, 0.28, 0.22, 0.08, 85) +
            Material("YellowEdge", 0.72, 0.35, 0.005, 0.22, 0.18, 0.05, 70) +
            Material("DarkSteel", 0.08, 0.10, 0.11, 0.42, 0.44, 0.46, 120) +
            Material("BoltSteel", 0.04, 0.05, 0.055, 0.48, 0.50, 0.52, 150) +
            Material("Stainless", 0.52, 0.56, 0.58, 0.72, 0.74, 0.76, 190) +
            Material("WheelSteel", 0.10, 0.12, 0.13, 0.52, 0.54, 0.56, 135) +
            Material("GearboxGrey", 0.20, 0.22, 0.23, 0.38, 0.40, 0.42, 95) +
            Material("MotorGrey", 0.12, 0.14, 0.15, 0.34, 0.36, 0.38, 90) +
            Material("Rubber", 0.018, 0.022, 0.024, 0.05, 0.05, 0.05, 20) +
            Material("ChainSteel", 0.24, 0.27, 0.28, 0.65, 0.67, 0.68, 170) +
            Material("Copper", 0.58, 0.24, 0.055, 0.68, 0.37, 0.15, 125) +
            Material("PcbGreen", 0.025, 0.22, 0.105, 0.22, 0.32, 0.24, 80) +
            Material("BrandBlue", 0.015, 0.12, 0.27, 0.16, 0.23, 0.30, 90) +
            Material("StatusGreen", 0.02, 0.72, 0.20, 0.18, 0.70, 0.24, 110) +
            Material("StatusAmber", 1.00, 0.58, 0.02, 0.75, 0.50, 0.10, 100) +
            Material("StatusRed", 0.92, 0.035, 0.025, 0.70, 0.14, 0.10, 100) +
            Material("TankPoly", 0.48, 0.52, 0.50, 0.42, 0.46, 0.45, 95) +
            Material("TankEdge", 0.68, 0.71, 0.68, 0.60, 0.63, 0.62, 125) +
            Material("TankInterior", 0.25, 0.30, 0.29, 0.24, 0.28, 0.27, 65) +
            Material("LabelPlate", 0.82, 0.83, 0.79, 0.32, 0.34, 0.33, 70) +
            Material("SightGlass", 0.20, 0.55, 0.58, 0.55, 0.75, 0.78, 140) +
            Material("PipeSteel", 0.28, 0.31, 0.30, 0.62, 0.65, 0.64, 155) +
            Material("ValveOrange", 0.90, 0.23, 0.025, 0.58, 0.30, 0.08, 85) +
            Material("PumpBlue", 0.015, 0.22, 0.48, 0.18, 0.42, 0.70, 115) +
            Material("MotorBlue", 0.025, 0.19, 0.38, 0.16, 0.36, 0.58, 105) +
            Material("GratingGalv", 0.30, 0.33, 0.32, 0.58, 0.61, 0.60, 145) +
            Material("FrameBlue", 0.015, 0.17, 0.35, 0.12, 0.35, 0.58, 105);
    }

    private static string Material(string name, double r, double g, double b, double sr, double sg, double sb, int ns)
    {
        return string.Format(Invariant,
            "newmtl {0}\r\nKa {1:0.###} {2:0.###} {3:0.###}\r\nKd {1:0.###} {2:0.###} {3:0.###}\r\nKs {4:0.###} {5:0.###} {6:0.###}\r\nNs {7}\r\nd 1.0\r\nillum 2\r\n\r\n",
            name, r, g, b, sr, sg, sb, ns);
    }

    private sealed class ObjBuilder
    {
        private readonly List<V3> _vertices = new List<V3>();
        private readonly List<Face> _faces = new List<Face>();
        private readonly string _description;
        private int _shapeIndex;

        public ObjBuilder(string description)
        {
            _description = description;
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine("# " + _description);
                writer.WriteLine("mtllib h1_industrial.mtl");
                writer.WriteLine("s off");
                foreach (var vertex in _vertices)
                {
                    writer.WriteLine(string.Format(Invariant, "v {0:0.######} {1:0.######} {2:0.######}", vertex.X, vertex.Y, vertex.Z));
                }

                string currentMaterial = null;
                foreach (var face in _faces)
                {
                    if (!string.Equals(currentMaterial, face.Material, StringComparison.Ordinal))
                    {
                        currentMaterial = face.Material;
                        writer.WriteLine("usemtl " + currentMaterial);
                    }
                    writer.Write("f");
                    foreach (var index in face.Indices)
                    {
                        writer.Write(" " + index.ToString(Invariant));
                    }
                    writer.WriteLine();
                }
            }
        }

        public void AddBeveledBox(string name, V3 center, V3 size, double bevel, string material)
        {
            _shapeIndex++;
            var hx = size.X / 2.0;
            var hy = size.Y / 2.0;
            var hz = size.Z / 2.0;
            var b = Math.Max(0.001, Math.Min(bevel, Math.Min(hx, Math.Min(hy, hz)) * 0.45));

            foreach (var sx in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X + sx * hx, center.Y - hy + b, center.Z - hz + b), new V3(center.X + sx * hx, center.Y + hy - b, center.Z - hz + b), new V3(center.X + sx * hx, center.Y + hy - b, center.Z + hz - b), new V3(center.X + sx * hx, center.Y - hy + b, center.Z + hz - b) }, center, material);
            }
            foreach (var sy in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X - hx + b, center.Y + sy * hy, center.Z - hz + b), new V3(center.X + hx - b, center.Y + sy * hy, center.Z - hz + b), new V3(center.X + hx - b, center.Y + sy * hy, center.Z + hz - b), new V3(center.X - hx + b, center.Y + sy * hy, center.Z + hz - b) }, center, material);
            }
            foreach (var sz in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X - hx + b, center.Y - hy + b, center.Z + sz * hz), new V3(center.X + hx - b, center.Y - hy + b, center.Z + sz * hz), new V3(center.X + hx - b, center.Y + hy - b, center.Z + sz * hz), new V3(center.X - hx + b, center.Y + hy - b, center.Z + sz * hz) }, center, material);
            }

            foreach (var sx in new[] { -1.0, 1.0 })
            foreach (var sy in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X + sx * hx, center.Y + sy * (hy - b), center.Z - hz + b), new V3(center.X + sx * (hx - b), center.Y + sy * hy, center.Z - hz + b), new V3(center.X + sx * (hx - b), center.Y + sy * hy, center.Z + hz - b), new V3(center.X + sx * hx, center.Y + sy * (hy - b), center.Z + hz - b) }, center, material);
            }
            foreach (var sx in new[] { -1.0, 1.0 })
            foreach (var sz in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X + sx * hx, center.Y - hy + b, center.Z + sz * (hz - b)), new V3(center.X + sx * (hx - b), center.Y - hy + b, center.Z + sz * hz), new V3(center.X + sx * (hx - b), center.Y + hy - b, center.Z + sz * hz), new V3(center.X + sx * hx, center.Y + hy - b, center.Z + sz * (hz - b)) }, center, material);
            }
            foreach (var sy in new[] { -1.0, 1.0 })
            foreach (var sz in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X - hx + b, center.Y + sy * hy, center.Z + sz * (hz - b)), new V3(center.X - hx + b, center.Y + sy * (hy - b), center.Z + sz * hz), new V3(center.X + hx - b, center.Y + sy * (hy - b), center.Z + sz * hz), new V3(center.X + hx - b, center.Y + sy * hy, center.Z + sz * (hz - b)) }, center, material);
            }

            foreach (var sx in new[] { -1.0, 1.0 })
            foreach (var sy in new[] { -1.0, 1.0 })
            foreach (var sz in new[] { -1.0, 1.0 })
            {
                AddFaceAuto(new[] { new V3(center.X + sx * hx, center.Y + sy * (hy - b), center.Z + sz * (hz - b)), new V3(center.X + sx * (hx - b), center.Y + sy * hy, center.Z + sz * (hz - b)), new V3(center.X + sx * (hx - b), center.Y + sy * (hy - b), center.Z + sz * hz) }, center, material);
            }
        }

        public void AddCylinder(string name, V3 center, V3 axis, double radius, double length, int segments, string material)
        {
            _shapeIndex++;
            var n = axis.Normalized();
            var helper = Math.Abs(n.Z) < 0.9 ? new V3(0, 0, 1) : new V3(0, 1, 0);
            var u = V3.Cross(n, helper).Normalized();
            var v = V3.Cross(n, u).Normalized();
            var halfAxis = n * (length / 2.0);
            var first = new V3[segments];
            var second = new V3[segments];
            for (var i = 0; i < segments; i++)
            {
                var angle = (Math.PI * 2.0 * i) / segments;
                var radial = (u * Math.Cos(angle) + v * Math.Sin(angle)) * radius;
                first[i] = center - halfAxis + radial;
                second[i] = center + halfAxis + radial;
            }
            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                AddFaceAuto(new[] { first[i], first[next], second[next], second[i] }, center, material);
            }
            AddFaceAuto(first, center - halfAxis, material);
            AddFaceAuto(second, center + halfAxis, material);
        }

        public void AddTorus(string name, V3 center, V3 normal, double majorRadius, double tubeRadius, int majorSegments, int tubeSegments, string material)
        {
            _shapeIndex++;
            var n = normal.Normalized();
            var helper = Math.Abs(n.Z) < 0.9 ? new V3(0, 0, 1) : new V3(0, 1, 0);
            var u = V3.Cross(n, helper).Normalized();
            var v = V3.Cross(n, u).Normalized();
            var points = new V3[majorSegments, tubeSegments];
            for (var i = 0; i < majorSegments; i++)
            {
                var a = (Math.PI * 2.0 * i) / majorSegments;
                var radial = u * Math.Cos(a) + v * Math.Sin(a);
                for (var j = 0; j < tubeSegments; j++)
                {
                    var b = (Math.PI * 2.0 * j) / tubeSegments;
                    points[i, j] = center + radial * (majorRadius + tubeRadius * Math.Cos(b)) + n * (tubeRadius * Math.Sin(b));
                }
            }
            for (var i = 0; i < majorSegments; i++)
            for (var j = 0; j < tubeSegments; j++)
            {
                var ni = (i + 1) % majorSegments;
                var nj = (j + 1) % tubeSegments;
                AddFaceRaw(new[] { points[i, j], points[ni, j], points[ni, nj], points[i, nj] }, material);
            }
        }

        public void AddPixelText(string text, V3 origin, double pixel, double depth, string material)
        {
            var cursor = origin.Y;
            foreach (var character in text.ToUpperInvariant())
            {
                string[] glyph;
                if (!Glyphs.TryGetValue(character, out glyph))
                {
                    cursor += pixel * 4;
                    continue;
                }
                for (var row = 0; row < glyph.Length; row++)
                for (var column = 0; column < glyph[row].Length; column++)
                {
                    if (glyph[row][column] == '1')
                    {
                        AddBeveledBox("brand_pixel", new V3(origin.X, cursor + column * pixel, origin.Z + (3 - row) * pixel), new V3(depth, pixel * 0.78, pixel * 0.78), depth * 0.2, material);
                    }
                }
                cursor += pixel * (glyph[0].Length + 1);
            }
        }

        private void AddFaceAuto(IList<V3> points, V3 center, string material)
        {
            if (points.Count >= 3)
            {
                var normal = V3.Cross(points[1] - points[0], points[2] - points[0]);
                var centroid = V3.Zero;
                foreach (var point in points) centroid += point;
                centroid /= points.Count;
                if (V3.Dot(normal, centroid - center) < 0)
                {
                    var reversed = new List<V3>(points);
                    reversed.Reverse();
                    AddFaceRaw(reversed, material);
                    return;
                }
            }
            AddFaceRaw(points, material);
        }

        private void AddFaceRaw(IList<V3> points, string material)
        {
            var indices = new int[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                _vertices.Add(points[i]);
                indices[i] = _vertices.Count;
            }
            _faces.Add(new Face(material, indices));
        }

        private static readonly Dictionary<char, string[]> Glyphs = new Dictionary<char, string[]>
        {
            { 'A', new[] { "010", "101", "111", "101" } }, { 'B', new[] { "110", "101", "110", "101" } },
            { 'C', new[] { "111", "100", "100", "111" } }, { 'E', new[] { "111", "110", "100", "111" } },
            { 'H', new[] { "101", "101", "111", "101" } }, { 'I', new[] { "111", "010", "010", "111" } },
            { 'L', new[] { "100", "100", "100", "111" } }, { 'O', new[] { "111", "101", "101", "111" } },
            { 'P', new[] { "110", "101", "110", "100" } }, { 'R', new[] { "110", "101", "110", "101" } },
            { 'S', new[] { "111", "100", "011", "111" } }, { 'T', new[] { "111", "010", "010", "010" } },
            { 'U', new[] { "101", "101", "101", "111" } }, { '1', new[] { "010", "110", "010", "111" } },
            { ' ', new[] { "0", "0", "0", "0" } }
        };

        private sealed class Face
        {
            public Face(string material, int[] indices) { Material = material; Indices = indices; }
            public string Material { get; private set; }
            public int[] Indices { get; private set; }
        }
    }

    private struct V3
    {
        public static readonly V3 Zero = new V3(0, 0, 0);
        public V3(double x, double y, double z) { X = x; Y = y; Z = z; }
        public double X;
        public double Y;
        public double Z;
        public double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public V3 Normalized() { var length = Length; return length < 0.000001 ? new V3(0, 0, 1) : this / length; }
        public static V3 Cross(V3 a, V3 b) { return new V3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X); }
        public static double Dot(V3 a, V3 b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }
        public static V3 operator +(V3 a, V3 b) { return new V3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static V3 operator -(V3 a, V3 b) { return new V3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static V3 operator *(V3 a, double value) { return new V3(a.X * value, a.Y * value, a.Z * value); }
        public static V3 operator /(V3 a, double value) { return new V3(a.X / value, a.Y / value, a.Z / value); }
    }
}
