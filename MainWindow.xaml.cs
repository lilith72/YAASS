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

using YAASS.Engine.Contract.FrontEndInterface;
using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;
using System.Text.RegularExpressions;
using System.Globalization;
using YAASS.Gui.Windows;
using YAASS.Gui.DataModel;
using YAASS.Gui;
using YAASS.Gui.Controls;
using System.Diagnostics;

namespace YAASS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SkillSelector mSkillSelector;
        SortSelector mSortSelector;
        SlotSelector mWeaponSlotSelector;
        ASS mAss = new ASS();

        IList<Solution> mSearchResults;
        SolutionList mSolutionList;
        SolutionList mPinnedSolutionList;

        const int SEARCH_COL_WIDTH = 350;
        const int SEARCH_COL_HEIGHT = 500;
        private const string MUSHROOMANCER_NAME = "Mushroomancer";

        public MainWindow()
        {
            InitializeComponent();
            mSkillSelector = new SkillSelector(mAss)
            {
                Height = SEARCH_COL_HEIGHT,
                Width = SEARCH_COL_WIDTH
            };
            spSearchConditions.Children.Add(mSkillSelector);

            mSortSelector = new SortSelector()
            {
                Height = SEARCH_COL_HEIGHT,
                Width = SEARCH_COL_WIDTH
            };
            mSortSelector.OnChange += OnChange_Sort;
            spSearchConditions.Children.Add(mSortSelector);

            mWeaponSlotSelector = new SlotSelector(Helper.MAX_SLOT_SIZE, Helper.MAX_WEAPON_SLOTS, "Weapon Decoration Slots");
            spWeaponSlots.Children.Add(mWeaponSlotSelector);

            mSolutionList = new SolutionList(false, true);
            mSolutionList.SolutionPinned += PinSolution;
            spResults.Children.Add(mSolutionList);
            mSolutionList.Height = 610;
            mSolutionList.SetBinding(SolutionList.WidthProperty, "{Binding RelativeSource={...}, Path=ActualWidth}");

            mPinnedSolutionList = new SolutionList(true, false);
            mPinnedSolutionList.SetBinding(SolutionList.HeightProperty, "{Binding RelativeSource={...}, Path=ActualHeight}");
            mPinnedSolutionList.SolutionRemoved += RemovePinnedSolution;
            gridPinnedSolutions.Children.Add(mPinnedSolutionList);

            UpdatedPinned();
        }

        private void SearchForSolutions(object sender, RoutedEventArgs e)
        {
            // Prevent search spam while working
            btnSearch.IsEnabled = false;
            IList<int> weaponSlots = (bool)cbUseWeaponSlot.IsChecked ? Helper.DecorationArrayToList(mWeaponSlotSelector.SelectedSlots) : null;

            if (mSkillSelector.SelectedSkills.Keys.Any(key => key.Equals(MUSHROOMANCER_NAME, StringComparison.OrdinalIgnoreCase))
                && mSkillSelector.SelectedSkills[MUSHROOMANCER_NAME] > 0)
            {
                MessageBox.Show("Don't build around mushroomancer you redditor");
            }

            // Do search
            IList<SkillContributor> talismans = (bool)cbUseTalismans.IsChecked ? mAss.GetAllCustomTalismans().Select(kvp => kvp.Value).ToList() : null;
            mSearchResults = mAss.GetSolutionsForSearch(mSkillSelector.SelectedSkills, weaponSlots, null, talismans);

            // Show on Gui
            UpdateSolutions();

            // Re-enable search
            btnSearch.IsEnabled = true;
        }

        private void UpdateSolutions()
        {
            // Sort Search
            if (mSearchResults != null)
            {
                if (mSortSelector.SelectedSorts.Count > 0 && mSearchResults.Count > 1)
                {
                    mSearchResults = mAss.SortSolutionsByGivenConditions(mSearchResults, mSortSelector.SelectedSorts);
                }
                // Update Gui
                mSolutionList.Clear();
                mSolutionList.Add(mSearchResults);

                Stopwatch stopwatch = Stopwatch.StartNew();
                lblNumResults.Content = mSolutionList.Count;
                mSolutionList.UpdateList();

                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
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

        private void OnChange_Sort(object sender, RoutedEventArgs e)
        {
            UpdateSolutions();
        }

        private void UpdateTalismans()
        {
            IList<TalismanData> talismans = new List<TalismanData>();
            foreach (KeyValuePair<string, SkillContributor> talisman in mAss.GetAllCustomTalismans())
            {
                talismans.Add(new TalismanData(talisman.Key, talisman.Value));
            }
            lvTalismans.ItemsSource = talismans;
        }

        private void UpdatedPinned()
        {
            mPinnedSolutionList.Clear();
            mPinnedSolutionList.Add(mAss.GetAllPinnedSolutions().ToList());
            mPinnedSolutionList.UpdateList();
            tabPinnedSolutions.Visibility = 0 < mPinnedSolutionList.Count ? Visibility.Visible : Visibility.Hidden;
        }

        private void PinSolution(object sender, RoutedEventArgs e)
        {
            int index = (int)(e.OriginalSource as Button).Tag;
            mAss.PinSolution(mSolutionList.Solutions[index].GetAsEngineSolution());
            UpdatedPinned();
        }
        private void RemovePinnedSolution(object sender, RoutedEventArgs e)
        {
            int index = (int)(e.OriginalSource as Button).Tag;
            mAss.TryUnpinSolution(mPinnedSolutionList.Solutions[index].GetAsEngineSolution(), out string errorMessage);
            UpdatedPinned();
        }
    }
}
