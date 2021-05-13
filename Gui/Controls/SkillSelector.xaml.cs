using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.FrontEndInterface;
using JustinsASS.Gui.DataModel;
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
    /// Interaction logic for SkillSelector.xaml
    /// </summary>
    public partial class SkillSelector : UserControl
    {
        public IDictionary<string, int> SelectedSkills {
            get
            {
                IDictionary<string, int> selectedSkills = new Dictionary<string, int>();
                selectedSkills = mSkills.ToDictionary(x => x.Key, x => x.Value.Level);
                return selectedSkills;
            }
            private set {}
        }
        private IDictionary<string, SkillLvlMax> mSkills;

        public SkillSelector(ASS ass)
        {
            InitializeComponent();
            this.SelectedSkills = new Dictionary<string, int>();
            mSkills = new Dictionary<string, SkillLvlMax>();

            foreach (KeyValuePair<string, int> skill in ass.GetSkillNamesToMaxLevelMapping())
            {
                mSkills.Add(skill.Key, new SkillLvlMax(0, skill.Value));
            }
            UpdateSkills();
        }

        private void UpdateSkills()
        {
            // Update dropdown
            cbAddSkill.ItemsSource = mSkills.Where(s => s.Value.Level == 0).OrderBy(s => s.Key).Select(s2 => s2.Key).ToList();
            // Update List of Skills
            lvSkillList.ItemsSource = mSkills.Where(s => s.Value.Level > 0).OrderBy(s => s.Key);
        }

        private void OnClick_AddSkill(object sender, RoutedEventArgs e)
        {
            if (cbAddSkill.SelectedItem != null)
            {
                string skill = cbAddSkill.SelectedItem.ToString();
                if (mSkills.ContainsKey(skill))
                {
                    mSkills[skill].Level = 1;
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

            if (mSkills.ContainsKey(skill))
            {
                mSkills[skill].Level = 0;
            }
            UpdateSkills();
        }

        private void OnChange_SkillValue(object sender, RoutedEventArgs e)
        {
            var upcSkill = (UpDownControl)sender;
            string skillID = upcSkill.Tag.ToString();
            mSkills[skillID].Level = upcSkill.Value;
        }
    }
}
