using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.FrontEndInterface;
using JustinsASS.Gui.Controls;
using JustinsASS.Gui.DataModel;
using JustinsASS.Gui;
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
using System.Windows.Shapes;


namespace JustinsASS.Gui.Windows
{
    /// <summary>
    /// Interaction logic for NewTalismanWindow.xaml
    /// </summary>
    public partial class NewTalismanWindow : Window
    {
        SlotSelector mSlotSelector;
        SkillSelector mSkillSelector;
        ASS mAss;

        public SkillContributor Result { set; get; }

        public NewTalismanWindow(ASS ass)
        {
            InitializeComponent();
            mAss = ass;

            mSkillSelector = new SkillSelector(ass);
            gridChoosers.Children.Add(mSkillSelector);
            mSkillSelector.Height = 400;
            Grid.SetColumn(mSkillSelector, 0);
            Grid.SetRow(mSkillSelector, 0);

            mSlotSelector = new SlotSelector(Helper.MAX_SLOT_SIZE, Helper.MAX_SLOTS);
            gridChoosers.Children.Add(mSlotSelector);
            Grid.SetColumn(mSlotSelector, 2);
            Grid.SetRow(mSlotSelector, 0);
        }

        private void OnClick_AddTalisman(object sender, RoutedEventArgs e)
        {
            if (tbTalismanName.Text.Length > 0)
            {
                IList<int> decoSlots = Helper.DecorationArrayToList(mSlotSelector.SelectedSlots);
                IList<SkillValue> skillList = mSkillSelector.SelectedSkills.Where(s => s.Value > 0).Select(kvp => new SkillValue(kvp.Key, kvp.Value)).ToList(); ;
                SkillContributor newTalisman = new SkillContributor(tbTalismanName.Text, 0, decoSlots, ArmorSlot.Talisman, skillList);
                this.Result = newTalisman;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
