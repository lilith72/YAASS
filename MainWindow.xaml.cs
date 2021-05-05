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
using System.Text.RegularExpressions;
using System.Globalization;
using JustinsASS.Gui.Windows;
using JustinsASS.Gui.DataModel;
using JustinsASS.Gui;
using JustinsASS.Gui.Controls;

namespace JustinsASS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IList<string> smAllSorts = Enum.GetNames(typeof(SolutionSortCondition)).Where(s => !s.Equals("none", StringComparison.OrdinalIgnoreCase)).ToList();
        IList<SolutionData> mSearchResults = new List<SolutionData>();
        IDictionary<string, int> mSkills = new Dictionary<string, int>();
        IList<string> mSorts = new List<string>();
        int[] mWeaponSlots= new int[Helper.MAX_SLOTS];
        ASS mAss = new ASS();

        public MainWindow()
        {
            InitializeComponent();
            cbAddSkill.ItemsSource = mAss.GetSkillNamesToMaxLevelMapping().Keys;
            cbAddSort.ItemsSource = smAllSorts;

            for (int i = 0; i < Helper.MAX_SLOT_SIZE; i++)
            {
                Label newLabel = new Label();
                newLabel.Content = "Size " + (i + 1) + " Slots";
                UpDownControl newUdc = new UpDownControl();
                newUdc.Min = 0;
                newUdc.Max = Helper.MAX_SLOTS;
                newUdc.Value = 0;
                newUdc.Tag = i.ToString();
                newUdc.ValueChanged += OnChange_WeaponSlotValue;

                spWeaponSlots.Children.Add(newLabel);
                spWeaponSlots.Children.Add(newUdc);
            }
        }

        private void SearchForSolutions(object sender, RoutedEventArgs e)
        {
            // Prevent search spam while working
            btnSearch.IsEnabled = false;
            IList<int> decoSlots = (bool)cbUseWeaponSlot.IsChecked ? Helper.DecorationArrayToList(mWeaponSlots) : null;
            IList<Solution> searchSolutions = mAss.GetSolutionsForSearch(mSkills, decoSlots);
            if (mSorts.Count > 0)
            {
                searchSolutions = mAss.SortSolutionsByGivenConditions(searchSolutions, mSorts.Select(x => (SolutionSortCondition)Enum.Parse(typeof(SolutionSortCondition), x)).ToList());
            }
            mSearchResults.Clear();
            foreach (Solution solution in searchSolutions)
            {
                mSearchResults.Add(new SolutionData(solution, mAss));
            }
            lvSearchResults.ItemsSource = null;
            lvSearchResults.ItemsSource = mSearchResults;
            lblNumResults.Content = mSearchResults.Count;
            // Re-enable search
            btnSearch.IsEnabled = true;
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
        private void OnClick_AddNewTalisman(object sender, RoutedEventArgs e)
        {
            NewTalismanWindow dialog = new NewTalismanWindow(mAss);
            if (dialog.ShowDialog().Value)
            {
                SkillContributor newTalisman = dialog.Result;
                lblTestAddTalisman.Content = newTalisman.ToString();
            }
        }
        private void OnChange_SkillValue(object sender, RoutedEventArgs e)
        {
            var upcSkill = (UpDownControl)sender;
            string skillID = upcSkill.Tag.ToString();
            mSkills[skillID] = upcSkill.Value;
        }
        private void OnChange_WeaponSlotValue(object sender, RoutedEventArgs e)
        {
            var upcSlot = (UpDownControl)sender;
            int slotNum = Int32.Parse(upcSlot.Tag.ToString());

            mWeaponSlots[slotNum] = upcSlot.Value;
        }

        private void UpdateSkills()
        {
            IList<SkillValue> skillList = new List<SkillValue>();
            lvSkillList.ItemsSource = null;
            foreach(KeyValuePair<string, int> skill in mSkills.OrderBy(key => key.Key))
            {
                skillList.Add(new SkillValue(skill.Key, skill.Value));
            }
            IDictionary<string, SkillLvlMax> skills = Helper.GetSkillsWithMax(skillList, mAss);
            lvSkillList.ItemsSource = null;
            lvSkillList.ItemsSource = skills;
        }
        private void UpdateSorts()
        {
            lvSortList.ItemsSource = null;
            lvSortList.ItemsSource = mSorts;
        }
    }
}
