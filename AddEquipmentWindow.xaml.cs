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
using System.Globalization;


namespace EquipmentRent
{
    /// <summary>
    /// Логика взаимодействия для AddEquipmentWindow.xaml
    /// </summary>
    public partial class AddEquipmentWindow : Window
    {
        public Equipment NewEquipment { get; private set; }
        public AddEquipmentWindow(Equipment equipment = null)
        {
            InitializeComponent();
            if (equipment != null)
            {
                NameBox.Text = equipment.Name;
                PriceBox.Text = equipment.Price.ToString(CultureInfo.InvariantCulture);
                StatusBox.Text = equipment.StatusID.ToString();
                NewEquipment = equipment;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewEquipment = new Equipment
                {
                    Name = NameBox.Text,
                    Price = decimal.Parse(PriceBox.Text, CultureInfo.InvariantCulture),
                    StatusID = int.Parse(StatusBox.Text)
                };
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода данных: " + ex.Message);
            }
        }
    }
}
