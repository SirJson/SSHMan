using System.Windows;
using System.Windows.Controls;


namespace SSHMan
{
    public class Spacing
    {
        public static double GetHorizontal(DependencyObject obj) => (double)obj.GetValue(HorizontalProperty);

        public static double GetVertical(DependencyObject obj) => (double)obj.GetValue(VerticalProperty);

        private static void HorizontalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var space = (double)e.NewValue;
            var obj = (DependencyObject)sender;

            InnerMargin.SetMargin(obj, new Thickness(0, 0, space, 0));
            InnerMargin.SetLastItemMargin(obj, new Thickness(0));
        }


        public static void SetHorizontal(DependencyObject obj, double space) => obj.SetValue(HorizontalProperty, space);


        public static void SetVertical(DependencyObject obj, double value) => obj.SetValue(VerticalProperty, value);

        private static void VerticalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var space = (double)e.NewValue;
            var obj = (DependencyObject)sender;
            InnerMargin.SetMargin(obj, new Thickness(0, 0, 0, space));
            InnerMargin.SetLastItemMargin(obj, new Thickness(0));
        }

        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.RegisterAttached("Vertical", typeof(double), typeof(Spacing),
                new UIPropertyMetadata(0d, VerticalChangedCallback));

        public static readonly DependencyProperty HorizontalProperty =
            DependencyProperty.RegisterAttached("Horizontal", typeof(double), typeof(Spacing),
                new UIPropertyMetadata(0d, HorizontalChangedCallback));
    }
}
