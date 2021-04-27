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
using System.ComponentModel;
using System.Runtime.CompilerServices;

using JustinsASS.Engine.Contract.FrontEndInterface;
using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;

namespace JustinsASS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IList<string> smAllSorts = Enum.GetNames(typeof(SolutionSortCondition)).Where(s => !s.Equals("none", StringComparison.OrdinalIgnoreCase)).ToList();
        IList<Solution> mSearchResults = new List<Solution>();
        IDictionary<string, int> mSkills = new Dictionary<string, int>();
        IList<string> mSorts = new List<string>();
        ASS mAss = new ASS();
        public MainWindow()
        {
            InitializeComponent();
            cbAddSkill.ItemsSource = mAss.GetSkillNamesToMaxLevelMapping().Keys;
            cbAddSort.ItemsSource = smAllSorts;
        }

        private void SearchForSolutions(object sender, RoutedEventArgs e)
        {
            mSearchResults = mAss.GetSolutionsForSearch(mSkills);
            if (mSorts.Count > 0)
            {
                lvSearchResults.ItemsSource = mAss.SortSolutionsByGivenConditions(mSearchResults, mSorts.Select(x => (SolutionSortCondition)Enum.Parse(typeof(SolutionSortCondition), x)).ToList());
            }
            else
            {
                lvSearchResults.ItemsSource = mSearchResults;
            }
            lblNumResults.Content = mSearchResults.Count;
        }

        private void OnClick_AddSkill(object sender, RoutedEventArgs e)
        {
            if (cbAddSkill.SelectedItem != null)
            {
                string skill = cbAddSkill.SelectedItem.ToString();
                if (!mSkills.ContainsKey(skill))
                {
                    mSkills.Add(skill, 1);
                    UpdateSkills();
                }
            }
        }

        private void OnClick_RemoveSkill(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string skill = button.Tag.ToString();

            mSkills.Remove(skill);
            UpdateSkills();
        }
        private void OnClick_IncreaseSkill(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string skill = button.Tag.ToString();

            mSkills[skill] = mSkills[skill] >= mAss.GetSkillNamesToMaxLevelMapping()[skill] ? mAss.GetSkillNamesToMaxLevelMapping()[skill] : mSkills[skill] + 1;
            UpdateSkills();
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
                }
            }
        }

        private void OnClick_RemoveSort(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string sort = button.Tag.ToString();

            mSorts.Remove(sort);
            UpdateSorts();
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
        }

        private void OnClick_DecreaseSkill(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string skill = button.Tag.ToString();

            mSkills[skill] = mSkills[skill] <= 1 ? 1 : mSkills[skill] - 1;
            UpdateSkills();
        }

        private void UpdateSkills()
        {
            lvSkillList.ItemsSource = null;
            lvSkillList.ItemsSource = mSkills.OrderBy(key => key.Key);
        }
        private void UpdateSorts()
        {
            lvSortList.ItemsSource = null;
            lvSortList.ItemsSource = mSorts;
        }
    }
}
