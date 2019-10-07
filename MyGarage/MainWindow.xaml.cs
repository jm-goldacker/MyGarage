using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyGarage
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MyGarageDataSet ds;
        MyGarageDataSetTableAdapters.carsTableAdapter carsTableAdapter;


        public MainWindow()
        {
            InitializeComponent();

            Sql.ConnectToCarsTable(dataGrid);
        }

        internal void Refresh_DataGrid()
        {
            carsTableAdapter.Fill(ds.cars);
        }

        private void Filter_btn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Add_car_btn_Click(object sender, RoutedEventArgs e)
        {
            AddCarWindow addCarWindow = new AddCarWindow();
            addCarWindow.SetMainWindow(this);
            addCarWindow.Show();
        }

        private void Delete_car_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                DataRowView view = (DataRowView)dataGrid.SelectedItem;
                String license_plate = view.Row["license_plate"].ToString();

                String message = "Möchten Sie das Fahrzeug mit den Kennzeichen " + license_plate + " wirklich löschen?";

                MessageBoxResult result = MessageBox.Show(message, "Löschen", MessageBoxButton.YesNo);

                if (result.Equals(MessageBoxResult.Yes))
                {
                    Sql.DeleteCar(license_plate);
                }
            }   
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                delete_car_btn.IsEnabled = true;
                details_btn.IsEnabled = true;
            }

            else
            {
                delete_car_btn.IsEnabled = false;
                details_btn.IsEnabled = false;
            }
        }

        private void Details_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                DataRowView view = (DataRowView)dataGrid.SelectedItem;
                String license_plate = view.Row["license_plate"].ToString();

                DetailsWindow detailsWindow = new DetailsWindow(license_plate);
                detailsWindow.Show();
            }
        }

        private void Exit_Menu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
           
            switch (e.Key)
            {
                case var _ when e.Key.Equals(Key.F6):
                    details_btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case var _ when e.Key.Equals(Key.F7):
                    add_car_btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case var _ when e.Key.Equals(Key.F8):
                    delete_car_btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
            }
        }
    }
}
