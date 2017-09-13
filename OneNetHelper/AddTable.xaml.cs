using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OneNetHelper
{
    /// <summary>
    /// AddTable.xaml 的交互逻辑
    /// </summary>
    public delegate void ChangeTextHandler(string Para_Name, string Para_Vaule);
    public partial class AddTable : Window
    {
        public String Para_Name, Para_Vaule;
        public event ChangeTextHandler ChangeTextEvent;
        public AddTable()
        {
            InitializeComponent();
            Comb_Para_Name.Items.Add("device_id");
            Comb_Para_Name.Items.Add("type");
            Comb_Para_Name.Items.Add("timeout");
            Comb_Para_Name.Items.Add("qos");
            Comb_Para_Name.SelectedIndex = 0;

        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Text_Para_Vaule.Text.Length != 0)
            {
                // Para_Name = Text_Para_Name.Text;
                Para_Name = Comb_Para_Name.SelectedItem.ToString();
                Para_Vaule = Text_Para_Vaule.Text;
                ChangeTextEvent(Para_Name, Para_Vaule);


            }
            this.Close();
            
        }
    }
}
