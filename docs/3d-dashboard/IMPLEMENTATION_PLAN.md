# USR ElectroPilot 3D Implementation Plan

Work one numbered phase at a time. Do not begin the next phase until the current phase has build and verification results recorded in PROGRESS.md.

## Phase 0 - Baseline and Repository Analysis

Create persistent documentation, inspect current project structure, inspect dashboard/3D/communication/simulation code, search for 3D assets, build the current solution, and record findings.

## Phase 1 - Correct WinForms Dashboard Shell

Build the dashboard shell around the 3D viewport using WinForms controls. The WPF/Helix viewport must stay inside the central panel and must not own dashboard text/cards/navigation.

## Phase 2 - Camera, Scene Bounds and Rendering Foundation

Create plant bounds, fit/reset/view commands, correct camera composition, clipping, and background.

## Phase 3 - Material and Lighting Foundation

Create reusable industrial material and lighting libraries.

## Phase 4 - Detailed Reusable Tank

Create one approved detailed reusable tank component before generating the full line.

## Phase 5 - Complete Configurable Tank Line

Create the complete configured tank line using shared tank geometry and real project configuration where available.

## Phase 6 - Walkway and Supporting Structure

Add frame, walkway, grating, handrails, posts, stairs, and equipment supports.

## Phase 7 - Front and Rear Hoist Travel Rails

Create exactly two mechanically aligned longitudinal rails.

## Phase 8 - Correct Portal-Hoist Structure

Create a mechanically correct portal hoist with one shared X transform for the whole portal.

## Phase 9 - Vertical Lift and Plating Parts

Create lift assembly, hooks, carrier bar, and individual hanging metal plates with Z-only lift motion.

## Phase 10 - Machine Animation and Simulation

Implement safe sequence animation and simulated machine states without blocking UI threads.

## Phase 11 - Live Dashboard Binding

Bind live/simulated values to dashboard panels, indicators, alarm table, and control availability.

## Phase 12 - Safe PLC Adapter

Preserve existing PLC/IP communication and keep unconfirmed writes disabled.

## Phase 13 - Performance and Lifecycle

Optimize geometry reuse, animation transforms, disposal, frame rate, and form lifecycle.

## Phase 14 - Final Visual Comparison

Capture the required reference state and complete the visual acceptance checklist.

## Phase 15 - Final Review and Handoff

Run final builds/tests/simulation, review changes, remove unused prototype code, update docs, and produce final report.
