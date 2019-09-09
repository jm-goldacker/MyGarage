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

namespace MyGarage
{
    /// <summary>
    /// Interaktionslogik für NewRepairWindow.xaml
    /// </summary>
    public partial class NewRepairWindow : Window
    {
    
        private String license_plate;
        public NewRepairWindow(String license_plate)
        {
            InitializeComponent();

            this.license_plate = license_plate;
        }

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate != null)
            {
                ok_btn.IsEnabled = true;
            }
            else
            {
                ok_btn.IsEnabled = false;
            }
        }

        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            MyGarageDataSet ds = new MyGarageDataSet();
            MyGarageDataSet.repairsRow row = ds.repairs.NewrepairsRow();

            row.license_plate = license_plate;
            row.repair_date = datePicker.SelectedDate.HasValue ? datePicker.SelectedDate.Value : datePicker.DisplayDate;
            row.kilometer = kilometer_textBox.Text != "" ? int.Parse(kilometer_textBox.Text) : 0;
            row.operating_hours = operation_hours_textBox.Text != "" ? int.Parse(operation_hours_textBox.Text) : 0;
            row.descriptions = description_textBox.Text;
            row._operator = operator_textBox.Text;
            row.is_main_inspection = main_inspection_checkBox.IsChecked.HasValue ? main_inspection_checkBox.IsChecked.Value : false;
            row.is_safety_inspection = safety_inspection_checkBox.IsChecked.HasValue ? safety_inspection_checkBox.IsChecked.Value : false;

            Sql.AddRepairsRow(row, ds, license_plate);
            Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
