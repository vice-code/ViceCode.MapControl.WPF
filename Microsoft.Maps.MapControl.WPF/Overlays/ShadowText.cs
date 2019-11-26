using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public partial class ShadowText : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ShadowText), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));
        public static readonly DependencyProperty ForegroundTopProperty = DependencyProperty.Register(nameof(ForegroundTop), typeof(Brush), typeof(ShadowText), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));
        public static readonly DependencyProperty ForegroundBottomProperty = DependencyProperty.Register(nameof(ForegroundBottom), typeof(Brush), typeof(ShadowText), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));
        public ShadowText() => InitializeComponent();

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush ForegroundTop
        {
            get => (Brush)GetValue(ForegroundTopProperty);
            set => SetValue(ForegroundTopProperty, value);
        }

        public Brush ForegroundBottom
        {
            get => (Brush)GetValue(ForegroundBottomProperty);
            set => SetValue(ForegroundBottomProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ShadowText)d).OnTextChanged();

        private void OnTextChanged()
        {
            Text1.Text = Text;
            Text2.Text = Text;
            if (ForegroundTop is null || ForegroundBottom is null)
                return;
            Text1.Foreground = ForegroundBottom;
            Text2.Foreground = ForegroundTop;
        }
    }
}
