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

namespace JustinsASS.Gui.Controls
{
    /// <summary>
    /// Interaction logic for SlotChooser.xaml
    /// </summary>
    public partial class SlotSelector : UserControl
    {
        public int[] SelectedSlots { get; private set; }
        public SlotSelector(int slotMaxSize, int slotMaxValue, string chooserTitle = "Decoration Slots")
        {
            InitializeComponent();
            lblTitle.Content = chooserTitle;
            this.SelectedSlots = new int[slotMaxSize];
            for (int i = slotMaxSize - 1; i >= 0 ; i--)
            {
                Label newLabel = new Label();
                newLabel.Content = "Size " + (i + 1) + " Slots";
                UpDownControl newUdc = new UpDownControl();
                newUdc.Min = 0;
                newUdc.Max = slotMaxValue;
                newUdc.Value = 0;
                newUdc.Tag = i.ToString();
                newUdc.ValueChanged += OnChange_SlotValue;

                spSlots.Children.Add(newLabel);
                spSlots.Children.Add(newUdc);
            }
        }

        private void OnChange_SlotValue(object sender, RoutedEventArgs e)
        {
            var upcSlot = (UpDownControl)sender;
            int slotNum = Int32.Parse(upcSlot.Tag.ToString());

            this.SelectedSlots[slotNum] = upcSlot.Value;
        }
    }
}
