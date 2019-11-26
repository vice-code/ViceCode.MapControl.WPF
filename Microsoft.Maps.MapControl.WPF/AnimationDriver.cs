using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Maps.MapControl.WPF
{
    internal class AnimationDriver : DependencyObject
    {
        public static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(nameof(AnimationProgress), typeof(double), typeof(AnimationDriver));
        private readonly Storyboard storyboard;

        public event EventHandler AnimationProgressChanged;

        public event EventHandler AnimationStopped;

        public event EventHandler AnimationCompleted;

        public AnimationDriver()
        {
            DependencyPropertyDescriptor.FromProperty(AnimationProgressProperty, typeof(AnimationDriver)).AddValueChanged(this, new EventHandler(OnAnimationProgressChanged));
            var doubleAnimation = new DoubleAnimation()
            {
                From = new double?(0.0),
                To = new double?(1.0)
            };
            Storyboard.SetTarget(doubleAnimation, this);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(AnimationProgress)", new object[0]));
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            this.storyboard = storyboard;
            this.storyboard.Completed += new EventHandler(StoryboardCompleted);
        }

        public void Start(Duration duration)
        {
            storyboard.Stop();
            storyboard.Duration = duration;
            storyboard.Children[0].Duration = duration;
            storyboard.Begin();
            IsAnimating = true;
        }

        public void Stop()
        {
            storyboard.Stop();
            IsAnimating = false;
            if (AnimationStopped is null)
                return;
            AnimationStopped(this, EventArgs.Empty);
        }

        public double AnimationProgress => (double)GetValue(AnimationProgressProperty);

        public bool IsAnimating { get; private set; }

        private void OnAnimationProgressChanged(object sender, EventArgs e)
        {
            if (!IsAnimating || AnimationProgressChanged is null)
                return;
            AnimationProgressChanged(this, EventArgs.Empty);
        }

        private void StoryboardCompleted(object sender, EventArgs e)
        {
            SetValue(AnimationProgressProperty, 1.0);
            if (AnimationProgressChanged is object)
                AnimationProgressChanged(this, EventArgs.Empty);
            if (AnimationCompleted is object)
                AnimationCompleted(this, EventArgs.Empty);
            IsAnimating = false;
        }
    }
}
