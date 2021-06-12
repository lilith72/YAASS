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

using YAASS.Gui;

namespace YAASS.Gui.Controls
{
    /// <summary>
    /// Interaction logic for DecoSlot.xaml
    /// </summary>
    public partial class DecoSlot : UserControl
    {
        public DecoSlot()
        {
            InitializeComponent();
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }

            set { SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(
                "Size",
                typeof(int),
                typeof(DecoSlot),
                new PropertyMetadata(default(int), OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecoSlot control = d as DecoSlot;
            control.UpdateSlot();
        }

        private void UpdateSlot()
        {
            if (this.Size > 0 && this.Size <= Helper.MAX_SLOT_SIZE)
            {
                string imgUri = "pack://application:,,,/Gui/Media/Decoration" + this.Size + ".png";
                imgDecoSlot.Source = new BitmapImage(new Uri(imgUri));
                imgDecoSlot.ToolTip = "Empty Deco Size " + this.Size;
            }
        }
    }
}
