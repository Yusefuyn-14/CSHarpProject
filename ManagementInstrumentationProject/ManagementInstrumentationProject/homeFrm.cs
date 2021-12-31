using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagementInstrumentationProject
{
    public partial class homeFrm : Form
    {
        public homeFrm()
        {
            InitializeComponent();
            ClassName className = new ClassName();
            comboBox1.Items.AddRange(className.getAllClassNameToArray());
            checkedListBox1.Items.AddRange(className.getAllClassNameToArray());
        }
        Manager manager = new Manager();
        private void homeFrm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            button5.PerformClick();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Concretes.WMIClass searchClasses = null;
            searchClasses = manager.getClass(comboBox1.Text);
            richTextBox1.Text = "";
            if (checkBox1.Checked == true)
                modifiedPrint(searchClasses);
            else if (checkBox1.Checked == false)
                normalPrint(searchClasses);

        }
        private void modifiedPrint(Concretes.WMIClass searchClasses)
        {
            String Code = @"#pragma namespace (""\\\\.\\root\\CIMv2"")" + "\rclass " + searchClasses.Name + "{\r";
            foreach (Concretes.WMIClassElement item in searchClasses.elements)
                Code += item.Type.ToString() + " " + item.Name + ";\n";
            Code += "};\n" + @"#pragma namespace (""\\\\.\\root\\CIMv2"")" + "\rinstance of " + searchClasses.Name + "{\r";
            foreach (Concretes.WMIClassElement item in searchClasses.elements)
            {
                string value = "null";
                if (item.Value != null)
                    value = item.Value.ToString();
                if (item.Type == CimType.String)
                    value = '"' + value + '"';
                Code += item.Name + " = " + value + ";\n";
            }
            richTextBox1.Text = Code + "};";
        }
        private void normalPrint(Concretes.WMIClass searchClasses)
        {
            foreach (Concretes.WMIClassElement item in searchClasses.elements)
            {
                string value = "null";
                if (item.Value != null)
                    value = item.Value.ToString();
                richTextBox1.Text += item.Type.ToString() + " " + item.Name + " = " + value + "\n";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            MessageBox.Show("Bu işlem makinanıza göre uzun sürebilir.\nİsterseniz \"Değiştirme\" penceresinden aranacaklar listesini kısaltıp daha hızlı sonuç alabilirsiniz !", "Bilgilendirme !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            pictureBox1.Visible = true;
            Task newTask = new Task(() => Search(textBox1.Text));
            newTask.Start();
        }

        List<Concretes.WMIClass> resultList = null;
        private async void Search(string Search)
        {
            List<string> searchList = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                    searchList.Add(checkedListBox1.Items[i].ToString());
            }
            foreach (Concretes.WMIClass classes in manager.getElementSearchValue(Search, searchList.ToArray<string>()).Result)
            {
                richTextBox1.Text += classes.Name + " ";

                foreach (Concretes.WMIClassElement item in classes.elements)
                {
                    richTextBox1.Text += item.Name + " ";
                }
                richTextBox1.Text += "\n";
            }
            richTextBox1.Text += "\nTarama bitti.";
            pictureBox1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MOF File|*.mof";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Uyarı = "Lütfen dikkat WMI üzerinde değişiklik yapmak tehlikeli ve geri dönüşü olmayan bir işelmdir ! Yedek almadan yapmayınız ve mof dosyanızı iyice kontrol ediniz.";
            if (MessageBox.Show(Uyarı, "Dikkat !", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            {
                string strCmdText = @"/c mofcomp -N:root\default " + textBox2.Text;
                Process.Start("CMD.exe", strCmdText);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Mof File|*.mof";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter write = new StreamWriter(sfd.FileName);
                write.Write(richTextBox1.Text);
                write.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                if (checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, false);
                else
                    checkedListBox1.SetItemChecked(i, true);
        }
    }
}
