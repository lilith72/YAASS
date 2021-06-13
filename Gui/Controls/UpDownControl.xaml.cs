using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YAASS.Gui.Controls
{
    /// <summary>
    /// Interaction logic for UpDownControl.xaml
    /// </summary>
    public partial class UpDownControl : UserControl
    {
        private static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UpDownControl));
        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(int),
                typeof(UpDownControl),
                new PropertyMetadata(default(int), OnPropertyChanged));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                "Max",
                typeof(int),
                typeof(UpDownControl),
                new PropertyMetadata(default(int), OnPropertyChanged));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(
                "Min",
                typeof(int),
                typeof(UpDownControl),
                new PropertyMetadata(default(int), OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpDownControl control = d as UpDownControl;
            // TODO: Is there a way to do this that doesn't cause issues due to binding order?
            //switch (e.Property.Name)
            //{
            //    case "Max":
            //        if (control.Max < control.Min)
            //            control.Max = (int)e.OldValue;
            //        break;
            //    case "Min":
            //        if (control.Min > control.Max)
            //            control.Min = (int)e.OldValue;
            //        break;
            //    default:
            //        break;
            //}
            //if (control.Value > control.Max)
            //{
            //    control.Value = control.Max;
            //}
            //else if (control.Value < control.Min)
            //{
            //    control.Value = control.Min;
            //}
            control.UpdateProgressBar();
        }

        public UpDownControl()
        {
            InitializeComponent();
            pbValueBar.Foreground = (LinearGradientBrush)FindResource("brushProgressBar");
            UpdateProgressBar();
        }

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void RaiseValueChangedEvent()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(UpDownControl.ValueChangedEvent);
            RaiseEvent(eventArgs);
        }

        public void UpdateProgressBar()
        {
            if (this.Max != 0)
                pbValueBar.Value = this.Value * 100 / this.Max;

            tbValue.Text = this.Value.ToString();
        }

        private void OnClick_Down(object sender, RoutedEventArgs e)
        {
            if (this.Value > this.Min)
            {
                this.Value--;
                RaiseValueChangedEvent();
            }
        }

        private void OnClick_Up(object sender, RoutedEventArgs e)
        {
            if (this.Value < this.Max)
            {
                this.Value++;
                RaiseValueChangedEvent();
            }
        }
    }
}
