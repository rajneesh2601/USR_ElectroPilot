using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using USR_ElectroPilot.ThreeD.Components;
using USR_ElectroPilot.ThreeD.Materials;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.ThreeD.Views
{
    public class Plant3DView : UserControl, IDisposable
    {
        private readonly HelixViewport3D _viewport;
        private int? _selectedTankId;
        private double _plantLength = 16;
        private double _plantWidth = 5.4;
        private double _plantHeight = 2.9;
        private double _cameraPlantLength;
        private bool _cameraInitialized;
        private bool _disposed;
        private string _staticSceneKey;
        private TankLine3DResult _currentLine;
        private readonly List<Visual3D> _hoistVisuals = new List<Visual3D>();
        private readonly DispatcherTimer _animationTimer;
        private IList<TankModel> _currentHoistTanks = new List<TankModel>();
        private IList<HoistModel> _currentHoists = new List<HoistModel>();
        private bool _currentEmergencyStop;
        private bool _hoistVisualStateInitialized;
        private double _visualHoistPositionIndex;
        private double _targetHoistPositionIndex;
        private double _visualLiftProgress;
        private double _targetLiftProgress;
        private const double HoistPositionAnimationStep = 0.12;
        private const double LiftAnimationStep = 0.04;
        private const double AnimationEpsilon = 0.0001;

        public int StaticSceneBuildCount { get; private set; }
        public int HoistVisualBuildCount { get; private set; }

        public event EventHandler<int> TankSelected;

        public Plant3DView()
        {
            _viewport = new HelixViewport3D
            {
                Background = new SolidColorBrush(Color.FromRgb(4, 13, 18)),
                ShowCoordinateSystem = false,
                ShowViewCube = false,
                ShowFrameRate = false,
                IsHeadLightEnabled = false,
                IsRotationEnabled = true,
                IsPanEnabled = true,
                IsZoomEnabled = true,
                IsTouchZoomEnabled = true,
                IsInertiaEnabled = true,
                RotateAroundMouseDownPoint = true,
                ZoomAroundMouseDownPoint = true,
                CameraInertiaFactor = 0.65,
                RotationSensitivity = 1.25,
                ZoomSensitivity = 1.15,
                LeftRightPanSensitivity = 1.15,
                UpDownPanSensitivity = 1.15,
                RotateGesture = new MouseGesture(MouseAction.LeftClick),
                PanGesture = new MouseGesture(MouseAction.RightClick),
                PanGesture2 = new MouseGesture(MouseAction.MiddleClick),
                ZoomGesture = new MouseGesture(MouseAction.RightClick, ModifierKeys.Control),
                ResetCameraGesture = new MouseGesture(MouseAction.LeftDoubleClick),
                ZoomExtentsWhenLoaded = false,
                Camera = CreateCameraFromTarget(new Point3D(-7.8, -14.8, 5.05), GetSceneTarget(), 38)
            };

            Content = _viewport;
            _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        public void Dispose()
        {
            _disposed = true;
            _animationTimer.Stop();
            _animationTimer.Tick -= AnimationTimer_Tick;
            _viewport.Children.Clear();
            Content = null;
        }

        public void SelectTank(int tankId)
        {
            _selectedTankId = tankId;
        }

        public void FitPlant()
        {
            SetDefaultCamera(true);
        }

        public void ResetCamera()
        {
            SetDefaultCamera(true);
        }

        public void SetFrontView()
        {
            var distance = GetCameraDistance(1.05);
            _viewport.Camera = CreateCameraFromTarget(new Point3D(0, -distance, 3.8), GetSceneTarget(), 34);
        }

        public void SetTopView()
        {
            var distance = Math.Max(11, _plantLength * 0.78);
            _viewport.Camera = CreateCameraFromTarget(new Point3D(0, 0, distance), GetSceneTarget(), 34, new Vector3D(0, 1, 0));
        }

        public void SetLeftView()
        {
            var distance = GetCameraDistance(1.05);
            _viewport.Camera = CreateCameraFromTarget(new Point3D(-distance, -0.25, 3.8), GetSceneTarget(), 34);
        }

        public void SetRightView()
        {
            var distance = GetCameraDistance(1.05);
            _viewport.Camera = CreateCameraFromTarget(new Point3D(distance, -0.25, 3.8), GetSceneTarget(), 34);
        }

        protected virtual void OnTankSelected(int tankId)
        {
            var handler = TankSelected;
            if (handler != null)
            {
                handler(this, tankId);
            }
        }

        public void UpdatePlant(IList<TankModel> tanks, IList<ProcessStepModel> processSteps, IList<HoistModel> hoists, IList<JobModel> jobs, HoistStatusModel hoistStatus, double hoistPositionIndex, string currentStepName, int remainingSeconds, bool autoMode, bool emergencyStop)
        {
            if (_disposed)
            {
                return;
            }

            var tankList = tanks == null
                ? new List<TankModel>()
                : tanks.Where(t => t.IsActive).OrderBy(t => t.LineId <= 0 ? 1 : t.LineId).ThenBy(t => t.TankNo).ToList();
            if (tankList.Count == 0 && tanks != null)
            {
                tankList = tanks.OrderBy(t => t.LineId <= 0 ? 1 : t.LineId).ThenBy(t => t.TankNo).ToList();
            }

            var hoistList = hoists == null
                ? new List<HoistModel>()
                : hoists.OrderBy(h => string.Equals(h.HoistName, "H1", StringComparison.OrdinalIgnoreCase) ? 0 : 1).ThenBy(h => h.HoistId).Take(1).ToList();

            BuildScene(tankList, hoistList, hoistPositionIndex, emergencyStop);
        }

        private void BuildScene(IList<TankModel> tanks, IList<HoistModel> hoists, double fallbackHoistPosition, bool emergencyStop)
        {
            if (tanks.Count == 0)
            {
                _viewport.Children.Clear();
                _hoistVisuals.Clear();
                _currentLine = null;
                _staticSceneKey = null;
                return;
            }

            var sceneKey = BuildStaticSceneKey(tanks);
            if (!string.Equals(_staticSceneKey, sceneKey, StringComparison.Ordinal))
            {
                _viewport.Children.Clear();
                _hoistVisuals.Clear();
                AddSceneLights();

                _currentLine = TankLine3DBuilder.AddLine(_viewport, tanks, _selectedTankId);
                _plantLength = _currentLine.RailLength;
                _plantWidth = 5.45;
                _plantHeight = 2.95;
                _staticSceneKey = sceneKey;
                StaticSceneBuildCount++;

                EnsureCameraReady();
            }

            if (_currentLine == null || _currentLine.TankCount == 0)
            {
                return;
            }

            var activeHoists = hoists.Count == 0
                ? new List<HoistModel> { new HoistModel { HoistName = "H1", PositionIndex = fallbackHoistPosition, Status = "Idle", CurrentTankNo = Math.Max(1, Convert.ToInt32(fallbackHoistPosition) + 1) } }
                : hoists.Take(1).ToList();

            SetHoistAnimationTargets(tanks, activeHoists, emergencyStop);
        }

        private void SetHoistAnimationTargets(IList<TankModel> tanks, IList<HoistModel> hoists, bool emergencyStop)
        {
            _currentHoistTanks = tanks.ToList();
            _currentHoists = hoists.Select(CloneHoist).ToList();
            _currentEmergencyStop = emergencyStop;

            var primaryHoist = _currentHoists.Count == 0 ? null : _currentHoists[0];
            _targetHoistPositionIndex = primaryHoist == null ? 0 : ClampHoistPosition(primaryHoist.PositionIndex);
            _targetLiftProgress = GetTargetLiftProgress(primaryHoist, emergencyStop);

            if (!_hoistVisualStateInitialized)
            {
                _visualHoistPositionIndex = _targetHoistPositionIndex;
                _visualLiftProgress = _targetLiftProgress;
                _hoistVisualStateInitialized = true;
                RenderHoistLayer();
                return;
            }

            AdvanceHoistAnimationFrame();
            RenderHoistLayer();
            UpdateAnimationTimerState();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_disposed || _currentLine == null)
            {
                _animationTimer.Stop();
                return;
            }

            if (AdvanceHoistAnimationFrame())
            {
                RenderHoistLayer();
            }

            UpdateAnimationTimerState();
        }

        private bool AdvanceHoistAnimationFrame()
        {
            var positionNeedsMove = Math.Abs(_visualHoistPositionIndex - _targetHoistPositionIndex) > AnimationEpsilon;
            if (positionNeedsMove)
            {
                if (_visualLiftProgress > AnimationEpsilon)
                {
                    return Approach(ref _visualLiftProgress, 0, LiftAnimationStep);
                }

                return Approach(ref _visualHoistPositionIndex, _targetHoistPositionIndex, HoistPositionAnimationStep);
            }

            return Approach(ref _visualLiftProgress, _targetLiftProgress, LiftAnimationStep);
        }

        private static bool Approach(ref double current, double target, double step)
        {
            var distance = target - current;
            if (Math.Abs(distance) <= step)
            {
                if (Math.Abs(distance) <= 0.0001)
                {
                    return false;
                }

                current = target;
                return true;
            }

            current += distance > 0 ? step : -step;
            return true;
        }

        private void UpdateAnimationTimerState()
        {
            var active = Math.Abs(_visualHoistPositionIndex - _targetHoistPositionIndex) > 0.0001 ||
                Math.Abs(_visualLiftProgress - _targetLiftProgress) > AnimationEpsilon;

            if (active && !_animationTimer.IsEnabled)
            {
                _animationTimer.Start();
            }
            else if (!active && _animationTimer.IsEnabled)
            {
                _animationTimer.Stop();
            }
        }

        private void RenderHoistLayer()
        {
            if (_currentLine == null || _currentLine.TankCount == 0 || _currentHoistTanks.Count == 0)
            {
                RemoveHoistVisuals();
                return;
            }

            RemoveHoistVisuals();
            var hoist = _currentHoists.Count == 0
                ? new HoistModel { HoistName = "H1", Status = "Idle", CurrentTankNo = 1, PositionIndex = _visualHoistPositionIndex }
                : CloneHoist(_currentHoists[0]);

            var position = ClampHoistPosition(_visualHoistPositionIndex);
            hoist.PositionIndex = position;
            AddHoistVisual(hoist, _currentLine.GetTankX(position), _currentEmergencyStop, _visualLiftProgress);
        }

        private double ClampHoistPosition(double position)
        {
            var maxIndex = Math.Max(0, (_currentLine == null ? _currentHoistTanks.Count : _currentLine.TankCount) - 1);
            return Math.Max(0, Math.Min(maxIndex, position));
        }

        private double GetTargetLiftProgress(HoistModel hoist, bool emergencyStop)
        {
            if (emergencyStop)
            {
                return _hoistVisualStateInitialized ? _visualLiftProgress : 0;
            }

            var status = hoist == null ? "Idle" : hoist.Status;
            return string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase)
                ? 1
                : 0;
        }

        private static HoistModel CloneHoist(HoistModel hoist)
        {
            if (hoist == null)
            {
                return null;
            }

            return new HoistModel
            {
                HoistId = hoist.HoistId,
                LineId = hoist.LineId,
                LineName = hoist.LineName,
                HoistName = hoist.HoistName,
                CurrentTankNo = hoist.CurrentTankNo,
                TargetTankNo = hoist.TargetTankNo,
                Direction = hoist.Direction,
                HomeTank = hoist.HomeTank,
                FromTank = hoist.FromTank,
                ToTank = hoist.ToTank,
                Status = hoist.Status,
                CurrentJobId = hoist.CurrentJobId,
                IsAuto = hoist.IsAuto,
                PositionIndex = hoist.PositionIndex,
                StateTicks = hoist.StateTicks
            };
        }

        private string BuildStaticSceneKey(IList<TankModel> tanks)
        {
            var parts = tanks.Select(t => string.Join(":",
                t.Id,
                t.LineId,
                t.TankNo,
                t.TankNumber,
                t.Name,
                t.Status,
                Math.Round(t.CurrentLevelLiters, 1),
                Math.Round(t.CapacityLiters, 1),
                _selectedTankId.HasValue && _selectedTankId.Value == t.Id ? "S" : string.Empty));

            return string.Join("|", parts.ToArray());
        }

        private void RemoveHoistVisuals()
        {
            foreach (var visual in _hoistVisuals)
            {
                _viewport.Children.Remove(visual);
            }

            _hoistVisuals.Clear();
        }

        private void AddHoistVisual(HoistModel hoist, double hoistX, bool emergencyStop, double liftProgress)
        {
            var firstDynamicIndex = _viewport.Children.Count;
            PortalHoist3DBuilder.AddHoist(_viewport, hoist, hoistX, emergencyStop, liftProgress);

            for (var i = firstDynamicIndex; i < _viewport.Children.Count; i++)
            {
                _hoistVisuals.Add(_viewport.Children[i]);
            }

            HoistVisualBuildCount++;
        }

        private void SetDefaultCamera()
        {
            SetDefaultCamera(false);
        }

        private void SetDefaultCamera(bool force)
        {
            if (!force && _cameraInitialized && Math.Abs(_cameraPlantLength - _plantLength) <= 0.01)
            {
                return;
            }

            var distance = GetCameraDistance(1.65);
            var target = GetSceneTarget();
            _viewport.Camera = CreateCameraFromTarget(
                new Point3D(-distance * 0.38, -distance * 0.72, _plantHeight + 1.48),
                target,
                38);
            _cameraPlantLength = _plantLength;
            _cameraInitialized = true;
        }

        private void EnsureCameraReady()
        {
            if (!_cameraInitialized || Math.Abs(_cameraPlantLength - _plantLength) > 0.01)
            {
                SetDefaultCamera();
            }
        }

        private static PerspectiveCamera CreateCamera(Point3D position, Vector3D lookDirection, double fieldOfView)
        {
            return CreateCamera(position, lookDirection, fieldOfView, new Vector3D(0, 0, 1));
        }

        private static PerspectiveCamera CreateCamera(Point3D position, Vector3D lookDirection, double fieldOfView, Vector3D upDirection)
        {
            return new PerspectiveCamera(position, lookDirection, upDirection, fieldOfView)
            {
                NearPlaneDistance = 0.05,
                FarPlaneDistance = 250
            };
        }

        private PerspectiveCamera CreateCameraFromTarget(Point3D position, Point3D target, double fieldOfView)
        {
            return CreateCameraFromTarget(position, target, fieldOfView, new Vector3D(0, 0, 1));
        }

        private PerspectiveCamera CreateCameraFromTarget(Point3D position, Point3D target, double fieldOfView, Vector3D upDirection)
        {
            return CreateCamera(position, target - position, fieldOfView, upDirection);
        }

        private Point3D GetSceneTarget()
        {
            return new Point3D(0.15, -0.62, 0.78);
        }

        private double GetCameraDistance(double multiplier)
        {
            var diagonal = Math.Sqrt((_plantLength * _plantLength) + (_plantWidth * _plantWidth) + (_plantHeight * _plantHeight));
            return Math.Max(8.8, diagonal * multiplier);
        }

        private void AddSceneLights()
        {
            _viewport.Children.Add(new ModelVisual3D { Content = new AmbientLight(Color.FromRgb(42, 58, 68)) });
            _viewport.Children.Add(new ModelVisual3D { Content = new DirectionalLight(Color.FromRgb(255, 255, 248), new Vector3D(-0.42, 0.48, -0.76)) });
            _viewport.Children.Add(new ModelVisual3D { Content = new DirectionalLight(Color.FromRgb(86, 132, 168), new Vector3D(0.74, -0.26, -0.3)) });
            _viewport.Children.Add(new ModelVisual3D { Content = new DirectionalLight(Color.FromRgb(255, 205, 112), new Vector3D(-0.18, -0.88, -0.22)) });
        }

    }
}
