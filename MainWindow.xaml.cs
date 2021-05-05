﻿using System;
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
        ASS mAss = new ASS();

        public MainWindow()
        {
            InitializeComponent();
            cbAddSkill.ItemsSource = mAss.GetSkillNamesToMaxLevelMapping().Keys;
            cbAddSort.ItemsSource = smAllSorts;
        }

        private void SearchForSolutions(object sender, RoutedEventArgs e)
        {
            // Prevent search spam while working
            btnSearch.IsEnabled = false;
            IList<Solution> searchSolutions = mAss.GetSolutionsForSearch(mSkills);
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

        public class SolutionData
        {
            public IList<String> ContributorIds { get; private set; }
            public IDictionary<string, SkillLvlMax> Skills { get; private set; }
            public int ArmorPoints { get; private set; }
            public int FireRes { get; private set; }
            public int WaterRes { get; private set; }
            public int IceRes { get; private set; }
            public int ThunderRes { get; private set; }
            public int DragonRes { get; private set; }

            public SolutionData(Solution solution, ASS ass)
            {
                Regex vacantPattern = new Regex(@"vacant.*slot");
                this.ContributorIds = new List<string>();
                this.ArmorPoints = solution.GetTotalArmorPoints();
                this.FireRes = solution.GetTotalFireResistance();
                this.WaterRes = solution.GetTotalWaterResistance();
                this.IceRes = solution.GetTotalIceResistance();
                this.ThunderRes = solution.GetTotalThunderResistance();
                this.DragonRes = solution.GetTotalDragonResistance();
                Skills = Helper.GetSkillsWithMax(solution.GetSkillValues(), ass);
                foreach (SkillContributor contributor in solution.Contributors)
                {
                    if (!vacantPattern.IsMatch(contributor.SkillContributorId.ToLower()))
                    {
                        ContributorIds.Add(contributor.SkillContributorId);
                    }
                }
            }
        }
    }
}
