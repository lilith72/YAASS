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
        private IDictionary<string, int> mSkills = new Dictionary<string, int>();
        int[] mDecoSlots = new int[Helper.MAX_SLOTS];
        ASS mAss;

        public SkillContributor Result { set; get; }

        public NewTalismanWindow(ASS ass)
        {
            InitializeComponent();
            mAss = ass;
            cbAddSkill.ItemsSource = mAss.GetSkillNamesToMaxLevelMapping().Keys;
            for (int i = 0; i < Helper.MAX_SLOT_SIZE; i++)
            {
                Label newLabel = new Label();
                newLabel.Content = "Size " + (i + 1) + " Slots";
                UpDownControl newUdc = new UpDownControl();
                newUdc.Min = 0;
                newUdc.Max = Helper.MAX_SLOTS;
                newUdc.Value = 0;
                newUdc.Tag = i.ToString();
                newUdc.ValueChanged += OnChange_SlotValue;

                spSlots.Children.Add(newLabel);
                spSlots.Children.Add(newUdc);
            }
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
        private void OnKeyDown_AddSkill(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnClick_AddSkill(null, null);
            }
        }

        private void OnClick_RemoveSkill(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string skill = button.Tag.ToString();

            mSkills.Remove(skill);
            UpdateSkills();
        }
        private void OnChange_SkillValue(object sender, RoutedEventArgs e)
        {
            var upcSkill = (UpDownControl)sender;
            string skillID = upcSkill.Tag.ToString();
            mSkills[skillID] = upcSkill.Value;
        }

        private void OnChange_SlotValue(object sender, RoutedEventArgs e)
        {
            var upcSlot = (UpDownControl)sender;
            int slotNum = Int32.Parse(upcSlot.Tag.ToString());

            mDecoSlots[slotNum] = upcSlot.Value;
        }
        private void UpdateSkills()
        {
            IList<SkillValue> skillList = new List<SkillValue>();
            lvSkillList.ItemsSource = null;
            foreach (KeyValuePair<string, int> skill in mSkills.OrderBy(key => key.Key))
            {
                skillList.Add(new SkillValue(skill.Key, skill.Value));
            }
            IDictionary<string, SkillLvlMax> skills = Helper.GetSkillsWithMax(skillList, mAss);
            lvSkillList.ItemsSource = null;
            lvSkillList.ItemsSource = skills;
        }

        private void OnClick_AddTalisman(object sender, RoutedEventArgs e)
        {
            IList<int> decoSlots = Helper.DecorationArrayToList(mDecoSlots);
            IList<SkillValue> skillList = mSkills.Select(kvp => new SkillValue(kvp.Key, kvp.Value)).ToList(); ;
            SkillContributor newTalisman = new SkillContributor(tbTalismanName.Text, 0, decoSlots, ArmorSlot.Talisman, skillList);
            this.Result = newTalisman;
            this.DialogResult = true;
            this.Close();
        }
    }
}
