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
using System.Diagnostics;

namespace JustinsASS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IList<SolutionData> mSearchResults = new List<SolutionData>();
        SkillSelector mSkillSelector;
        SortSelector mSortSelector;
        SlotSelector mWeaponSlotSelector;
        ASS mAss = new ASS();

        public MainWindow()
        {
            InitializeComponent();
            mSkillSelector = new SkillSelector(mAss);
            spSearchConditions.Children.Add(mSkillSelector);

            mSortSelector = new SortSelector();
            spSearchConditions.Children.Add(mSortSelector);

            mWeaponSlotSelector = new SlotSelector(Helper.MAX_SLOT_SIZE, Helper.MAX_WEAPON_SLOTS, "Weapon Decoration Slots");
            spWeaponSlots.Children.Add(mWeaponSlotSelector);
        }

        private void SearchForSolutions(object sender, RoutedEventArgs e)
        {
            // Prevent search spam while working
            btnSearch.IsEnabled = false;
            IList<int> weaponSlots = (bool)cbUseWeaponSlot.IsChecked ? Helper.DecorationArrayToList(mWeaponSlotSelector.SelectedSlots) : null;

            // Do search
            IList<Solution> searchSolutions = mAss.GetSolutionsForSearch(mSkillSelector.SelectedSkills, weaponSlots, null, mAss.GetAllCustomTalismans().Select(kvp => kvp.Value).ToList());

            // Sort Search
            if (mSortSelector.SelectedSorts.Count > 0)
            {
                searchSolutions = mAss.SortSolutionsByGivenConditions(searchSolutions, mSortSelector.SelectedSorts);
            }

            // Add to Gui data model and present
            mSearchResults.Clear();
            foreach (Solution solution in searchSolutions)
            {
                mSearchResults.Add(new SolutionData(solution, mAss));
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            lvSearchResults.ItemsSource = null;
            lvSearchResults.ItemsSource = mSearchResults;
            lblNumResults.Content = mSearchResults.Count;
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            // Re-enable search
            btnSearch.IsEnabled = true;
        }

        private void OnClick_AddNewTalisman(object sender, RoutedEventArgs e)
        {
            NewTalismanWindow dialog = new NewTalismanWindow(mAss);
            if (dialog.ShowDialog().Value)
            {
                SkillContributor newTalisman = dialog.Result;
                mAss.PersistCustomInventoryAddition(newTalisman);
            }
            UpdateTalismans();
        }

        private void OnClick_RemoveTalisman(object sender, RoutedEventArgs e)
        {
            Button removeBtn = sender as Button;
            string talismanId = removeBtn.Tag.ToString();
            mAss.TryPersistCustomInventoryDeletion(talismanId);
            UpdateTalismans();
        }

        private void OnCheck_UseTalismans(object sender, RoutedEventArgs e)
        {
            UpdateTalismans();
        }

        //private void OnChange_WeaponSlotValue(object sender, RoutedEventArgs e)
        //{
        //    var upcSlot = (UpDownControl)sender;
        //    int slotNum = Int32.Parse(upcSlot.Tag.ToString());

        //    mWeaponSlots[slotNum] = upcSlot.Value;
        //}

        private void UpdateTalismans()
        {
            //IDictionary<string, SkillContributor> talismans = mAss.GetAllCustomTalismans();
            IList<TalismanData> talismans = new List<TalismanData>();
            foreach(KeyValuePair<string, SkillContributor> talisman in mAss.GetAllCustomTalismans())
            {
                talismans.Add(new TalismanData(talisman.Key, talisman.Value));
            }
            lvTalismans.ItemsSource = talismans;
        }
    }
}
