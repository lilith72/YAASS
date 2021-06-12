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
    /// Interaction logic for SkillPointBar.xaml
    /// </summary>
    public partial class SkillPointBar : UserControl
    {
        public int Max
        {
            get { return (int)GetValue(MaxProperty); }

            set { SetValue(MaxProperty, value); }
        }
        public int Points
        {
            get { return (int)GetValue(PointsProperty); }

            set { SetValue(PointsProperty, value); }
        }
        public SkillPointBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                "Max",
                typeof(int),
                typeof(SkillPointBar),
                new PropertyMetadata(default(int), OnPropertyChanged));

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                "Points",
                typeof(int),
                typeof(SkillPointBar),
                new PropertyMetadata(default(int), OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SkillPointBar control = d as SkillPointBar;
            control.UpdatePoints();
        }

        private void UpdatePoints()
        {
            spPoints.Children.Clear();
            for (int i = 0; i < this.Points; i++)
            {
                Image pointImage = new Image();
                pointImage.Width = 20;
                pointImage.Source = new BitmapImage(new Uri("pack://application:,,,/Gui/Media/SkillPoint.png"));
                spPoints.Children.Add(pointImage);
            }
            for (int i = 0; i < (this.Max - this.Points); i++)
            {
                Image pointImage = new Image();
                pointImage.Width = 20;
                pointImage.Source = new BitmapImage(new Uri("pack://application:,,,/Gui/Media/SkillPoint_Empty.png"));
                spPoints.Children.Add(pointImage);
            }
        }
    }
}
