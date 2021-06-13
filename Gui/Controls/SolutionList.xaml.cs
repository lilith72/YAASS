using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.FrontEndInterface;
using YAASS.Gui.DataModel;
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
    /// Interaction logic for SolutionList.xaml
    /// </summary>
    public partial class SolutionList : UserControl
    {
        private static readonly RoutedEvent SolutionPinnedEvent = EventManager.RegisterRoutedEvent("SolutionPinned", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionList));
        private static readonly RoutedEvent SolutionRemovedEvent = EventManager.RegisterRoutedEvent("SolutionRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionList));
        private bool mCanPin;
        private bool mCanRemove;

        IList<SolutionData> solutions;
        public IList<SolutionData> Solutions
        {
            get
            {
                return solutions;
            }
            set
            {
                solutions = value;
                UpdateList();
            }
        }

        public int Count
        {
            get
            {
                return this.Solutions.Count;
            }
            private set { }
        }

        public SolutionList(bool canRemove = false, bool canPin = false)
        {
            InitializeComponent();

            this.mCanPin = canPin;
            this.mCanRemove = canRemove;
            this.Solutions = new List<SolutionData>();
        }

        public event RoutedEventHandler SolutionPinned
        {
            add { AddHandler(SolutionPinnedEvent, value); }
            remove { RemoveHandler(SolutionPinnedEvent, value); }
        }

        public event RoutedEventHandler SolutionRemoved
        {
            add { AddHandler(SolutionRemovedEvent, value); }
            remove { RemoveHandler(SolutionRemovedEvent, value); }
        }
        
        public void Add(Solution solution)
        {
            this.Solutions.Add(new SolutionData(solution, this.Solutions.Count, this.mCanPin, this.mCanRemove));
        }
        public void Add(IList<Solution> solutions)
        {
            foreach (Solution solution in solutions)
            {
                this.Solutions.Add(new SolutionData(solution, this.Solutions.Count, this.mCanPin, this.mCanRemove));
            }
        }

        public void Clear()
        {
            this.Solutions.Clear();
        }

        public void UpdateList()
        {
            lvSolutions.ItemsSource = null;
            lvSolutions.ItemsSource = solutions;
        }

        private void OnClick_Pin(object sender, RoutedEventArgs e)
        {
            (sender as Button).Visibility = Visibility.Hidden;
            RoutedEventArgs eventArgs = new RoutedEventArgs(SolutionList.SolutionPinnedEvent, sender);
            RaiseEvent(eventArgs);
        }
        private void OnClick_Remove(object sender, RoutedEventArgs e)
        {
            (sender as Button).Visibility = Visibility.Hidden;
            RoutedEventArgs eventArgs = new RoutedEventArgs(SolutionList.SolutionRemovedEvent, sender);
            RaiseEvent(eventArgs);
        }
    }
}
