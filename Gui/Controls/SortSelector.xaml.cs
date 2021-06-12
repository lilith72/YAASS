using YAASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SortChooser.xaml
    /// </summary>
    public partial class SortSelector : UserControl
    {
        private static readonly RoutedEvent OnChangeEvent = EventManager.RegisterRoutedEvent("OnChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SortSelector));
        IList<string> mAllSorts = new List<string>();
        IList<string> mSorts = new List<string>();

        public IList<SolutionSortCondition> SelectedSorts
        {
            get
            {
                IList<string> sorts = new List<string>();

                // Convert back to the enums
                foreach (string sort in mSorts)
                {
                    sorts.Add(String.Concat(sort.Where(c => !Char.IsWhiteSpace(c))));
                }
                return sorts.Select(x => (SolutionSortCondition)Enum.Parse(typeof(SolutionSortCondition), x)).ToList();
            }
            private set { }
        }
        public SortSelector()
        {
            InitializeComponent();
            this.SelectedSorts = new List<SolutionSortCondition>();
            IList<string> allSorts = Enum.GetNames(typeof(SolutionSortCondition)).Where(s => !s.Equals("none", StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (string sort in allSorts)
            {
                // Make Readable
                mAllSorts.Add(string.Concat(sort.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' '));
            }
            UpdateSorts();
        }

        public event RoutedEventHandler OnChange
        {
            add { AddHandler(OnChangeEvent, value); }
            remove { RemoveHandler(OnChangeEvent, value); }
        }

        private void OnClick_AddSort(object sender, RoutedEventArgs e)
        {
            if (cbAddSort.SelectedItem != null)
            {
                string skill = cbAddSort.SelectedItem.ToString();
                if (!mSorts.Contains(skill))
                {
                    mSorts.Add(skill);
                    UpdateSorts();
                    RaiseOnChangeEvent();
                }
            }
        }
        private void OnKeyDown_AddSort(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnClick_AddSort(null, null);
            }
        }

        private void OnClick_RemoveSort(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string sort = button.Tag.ToString();

            mSorts.Remove(sort);
            UpdateSorts();
            RaiseOnChangeEvent();
        }

        private void OnClick_MoveSortUp(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string sort = button.Tag.ToString();

            int newIndex = mSorts.IndexOf(sort);
            newIndex = newIndex > 0 ? newIndex - 1 : mSorts.Count - 1;
            mSorts.Remove(sort);
            mSorts.Insert(newIndex, sort);
            UpdateSorts();
            RaiseOnChangeEvent();
        }

        private void OnClick_MoveSortDown(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string sort = button.Tag.ToString();

            int newIndex = mSorts.IndexOf(sort);
            newIndex = newIndex < mSorts.Count - 1 ? newIndex + 1 : 0;
            mSorts.Remove(sort);
            mSorts.Insert(newIndex, sort);
            UpdateSorts();
            RaiseOnChangeEvent();
        }

        private void RaiseOnChangeEvent()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(SortSelector.OnChangeEvent);
            RaiseEvent(eventArgs);
        }

        private void UpdateSorts()
        {
            // Update Dropdown
            cbAddSort.ItemsSource = mAllSorts.Where(s => !mSorts.Any(s2 => s2.Equals(s)));
            // Update Sort List
            lvSortList.ItemsSource = null;
            lvSortList.ItemsSource = mSorts;
        }
    }
}
