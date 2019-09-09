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
    /// Interaktionslogik für AddCarWindow.xaml
    /// </summary>
    public partial class AddCarWindow : Window
    {

        private MainWindow mainWindow;

        public AddCarWindow()
        {
            InitializeComponent();
        }

        private void Abort_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Lost_focus(object sender, RoutedEventArgs e)
        {
            if (isFormValid())
            {
                add_btn.IsEnabled = true;
            }
            else
            {
                add_btn.IsEnabled = false;
            }


        }

        private bool isFormValid()
        {
            return license_plate_textBox.Text.Length != 0 && manufacturer_textBox.Text.Length != 0
                && model_textBox.Text.Length != 0 && type_textBox.Text.Length != 0
                && main_inspection_datePicker.Text.Length != 0 && vid_textBox.Text.Length == 17
                && isManufacturerTypeIdValid() && isManufacturerIdInt()
                && isHorsePowerInt() && isKiloWattInt();
        }

        private bool isManufacturerTypeIdValid()
        {
            return (manufacturer_id_textBox.Text.Length == 0 && type_id_textBox.Text.Length == 0)
                || (manufacturer_id_textBox.Text.Length == 4 && type_id_textBox.Text.Length == 3);
        }

        private bool isManufacturerIdInt()
        {
            int tmp;
            return int.TryParse(manufacturer_id_textBox.Text, out tmp) || manufacturer_id_textBox.Text.Length == 0;
        }

        private bool isHorsePowerInt()
        {
            int tmp;
            return int.TryParse(horse_power_textBox.Text, out tmp) || horse_power_textBox.Text.Length == 0;
        }

        private bool isKiloWattInt()
        {
            int tmp;
            return int.TryParse(kilo_watt_textBox.Text, out tmp) || kilo_watt_textBox.Text.Length == 0;
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isFormValid())
            {
                MyGarageDataSet ds = new MyGarageDataSet();
                MyGarageDataSet.carsRow row = ds.cars.NewcarsRow();

                row.license_plate = license_plate_textBox.Text;
                row.manufacturer = manufacturer_textBox.Text;
                row.model = model_textBox.Text;
                row.car_type = type_textBox.Text;
                row.identification_number = vid_textBox.Text;
                row.manufacturer_id = manufacturer_id_textBox.Text != "" ? int.Parse(manufacturer_id_textBox.Text) : 0;
                row.typ_id = type_id_textBox.Text;
                row.first_registered = DateTime.Parse(first_registered_textBox.Text);
                row.horse_power = horse_power_textBox.Text != "" ? int.Parse(horse_power_textBox.Text) : 0;
                row.kilo_watt = kilo_watt_textBox.Text != "" ? int.Parse(kilo_watt_textBox.Text) : 0;
                row.next_main_inspection = main_inspection_datePicker.DisplayDate;

                if (safety_inspection_datePicker.SelectedDate != null)
                {
                    row.next_safety_inspection = safety_inspection_datePicker.DisplayDate;
                }

                Sql.AddCarsRow(row, ds);

                this.Close();
            }

            else
            {
                MessageBox.Show("Einige Paramater sind nicht korrekt. Bitte überprüfen Sie Ihre Eingabe");
            }
        }

        internal void SetMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
    }
}
