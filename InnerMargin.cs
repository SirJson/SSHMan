﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SSHMan
{
    public static class InnerMargin
    {
        private static Thickness GetLastItemMargin(Panel obj)
        {
            if(obj is null) { throw new ArgumentNullException(nameof(obj)); }
            return (Thickness)obj.GetValue(LastItemMarginProperty);
        }

        public static Thickness GetMargin(DependencyObject obj)
        {
            if(obj is null) { throw new ArgumentNullException(nameof(obj)); }
            return (Thickness)obj.GetValue(MarginProperty);
        }

        private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure this is put on a panel
            if (!(sender is Panel panel)) return;

            // Avoid duplicate registrations
            panel.Loaded -= OnPanelLoaded;
            panel.Loaded += OnPanelLoaded;

            if (panel.IsLoaded)
            {
                OnPanelLoaded(panel, null);
            }
        }

        private static void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            var panel = (Panel)sender;

            // Go over the children and set margin for them:
            for (var i = 0; i < panel.Children.Count; i++)
            {
                var child = panel.Children[i];
                if (!(child is FrameworkElement fe)) continue;

                bool isLastItem = i == panel.Children.Count - 1;
                fe.Margin = isLastItem ? GetLastItemMargin(panel) : GetMargin(panel);
            }
        }

        public static void SetLastItemMargin(DependencyObject obj, Thickness value)
        {
            if(obj is null) { throw new ArgumentNullException(nameof(obj)); }
            obj.SetValue(LastItemMarginProperty, value);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            if(obj is null) { throw new ArgumentNullException(nameof(obj)); }
            obj.SetValue(MarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(InnerMargin),
                new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        public static readonly DependencyProperty LastItemMarginProperty =
            DependencyProperty.RegisterAttached("LastItemMargin", typeof(Thickness), typeof(InnerMargin),
                new UIPropertyMetadata(new Thickness(), MarginChangedCallback));
    }
}
