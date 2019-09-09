using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaktionslogik für DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {

        private String license_plate;
        private MyGarageDataSetTableAdapters.repairsTableAdapter repairsTableAdapter;
        public DetailsWindow(String license_plate)
        {
            InitializeComponent();

            this.license_plate = license_plate;

            Sql.ConnectToRepairsTable(dataGrid, license_plate);

            MyGarageDataSet.carsDataTable carsTable = Sql.GetCarTableByLicensePlate(license_plate);

            license_plate_textBox.Text = carsTable[0].license_plate;
            vid_textBox.Text = carsTable[0].Isidentification_numberNull() ? "" : carsTable[0].identification_number;
            manufacturer_textBox.Text = carsTable[0].manufacturer;
            model_textBox.Text = carsTable[0].model;
            manufacturer_id_textBox.Text = carsTable[0].Ismanufacturer_idNull() ? "" : carsTable[0].manufacturer_id.ToString();
            type_id_textBox.Text = carsTable[0].Istyp_idNull() ? "" : carsTable[0].typ_id;
            type_textBox.Text = carsTable[0].car_type;
            first_registered_datePicker.SelectedDate = carsTable[0].Isfirst_registeredNull() ? first_registered_datePicker.SelectedDate : carsTable[0].first_registered;
            horse_power_textBox.Text = carsTable[0].Ishorse_powerNull() ? "" : carsTable[0].horse_power.ToString();
            kilo_watt_textBox.Text = carsTable[0].Iskilo_wattNull() ? "" : carsTable[0].kilo_watt.ToString();
            main_inspection_datePicker.SelectedDate = carsTable[0].next_main_inspection;
            safety_inspection_datePicker.SelectedDate = carsTable[0].Isnext_safety_inspectionNull() ? safety_inspection_datePicker.SelectedDate : carsTable[0].next_safety_inspection;
            
        }

        private void Lost_focus(object sender, RoutedEventArgs e)
        {
           
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            MyGarageDataSet ds = new MyGarageDataSet();
            MyGarageDataSet.repairsRow row = ds.repairs.NewrepairsRow();
            DataRowView view = (DataRowView)dataGrid.SelectedItem;
            String license_plate = view.Row["license_plate"].ToString();
            DateTime date = (DateTime)view.Row["repair_date"];

            String message = "Möchten Sie die Reparatur wirklich löschen?";

            MessageBoxResult result = MessageBox.Show(message, "Löschen", MessageBoxButton.YesNo);

            if (result.Equals(MessageBoxResult.Yes))
            {
                repairsTableAdapter.DeleteRepair(license_plate, date);
                MyGarageDataSet.repairsDataTable repairsTable = repairsTableAdapter.GetRepairByLicensePlate(license_plate);
                dataGrid.ItemsSource = repairsTable;
            }
        }

        private void New_btn_Click(object sender, RoutedEventArgs e)
        {
            NewRepairWindow repairWindow = new NewRepairWindow(license_plate);
            repairWindow.Show();
        }
    }
}
