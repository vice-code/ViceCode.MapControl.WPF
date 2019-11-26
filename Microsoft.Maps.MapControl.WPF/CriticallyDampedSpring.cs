using System;

namespace Microsoft.Maps.MapControl.WPF
{
    internal class CriticallyDampedSpring
    {
        private const double Epsilon = 1E-06;
        private readonly double omegaNought;
        private double currentVelocity;
        private double targetValue;
        private double targetSetTime;
        private double A;
        private double B;

        public CriticallyDampedSpring()
        {
            omegaNought = 20.0;
            Initialize();
        }

        public CriticallyDampedSpring(double springConstant, double mass)
        {
            if (springConstant <= 0.0 || mass <= 0.0)
                throw new ArgumentException("mass and spring constant must be positive");
            omegaNought = Math.Sqrt(springConstant / mass);
            Initialize();
        }

        public double CurrentValue { get; private set; }

        public double TargetValue
        {
            get => targetValue;
            set
            {
                if (Math.Abs(targetValue - value) <= 1E-06)
                    return;
                Update();
                var num = CurrentValue - value;
                A = num;
                B = currentVelocity + num * omegaNought;
                targetValue = value;
                targetSetTime = CurrentTime;
            }
        }

        public void SnapToValue(double value)
        {
            CurrentValue = value;
            currentVelocity = 0.0;
            targetValue = value;
            targetSetTime = CurrentTime;
        }

        public bool Update()
        {
            if (CurrentValue == targetValue)
                return false;
            var relativeTime = CurrentTime - targetSetTime;
            var currentValue = CalculateCurrentValue(relativeTime);
            var currentVelocity = CalculateCurrentVelocity(relativeTime);
            if (Math.Abs(currentValue - targetValue) < 1E-06 && Math.Abs(this.currentVelocity) < 1E-06)
            {
                CurrentValue = targetValue;
                this.currentVelocity = 0.0;
            }
            else
            {
                CurrentValue = currentValue;
                this.currentVelocity = currentVelocity;
            }
            return true;
        }

        private double CurrentTime => DateTime.Now.Ticks / 10000000.0;

        private double CalculateCurrentValue(double relativeTime) => (A + B * relativeTime) * Math.Exp(-omegaNought * relativeTime) + targetValue;

        private double CalculateCurrentVelocity(double relativeTime) => (B - (A + B * relativeTime) * omegaNought) * Math.Exp(-omegaNought * relativeTime);

        private void Initialize()
        {
            CurrentValue = 0.0;
            currentVelocity = 0.0;
            targetValue = 0.0;
            targetSetTime = CurrentTime;
        }
    }
}
