using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyGarage
{
    static class Sql
    {
        private static MyGarageDataSet ds;
        private static MyGarageDataSetTableAdapters.carsTableAdapter carsTableAdapter;
        private static MyGarageDataSetTableAdapters.repairsTableAdapter repairsTableAdapter;

        static Sql()
        {
            ds = new MyGarageDataSet();

            carsTableAdapter = new MyGarageDataSetTableAdapters.carsTableAdapter();
            carsTableAdapter.Fill(ds.cars);

            repairsTableAdapter = new MyGarageDataSetTableAdapters.repairsTableAdapter();
        }

        public static void ConnectToCarsTable(DataGrid dataGrid)
        {
            dataGrid.ItemsSource = ds.cars.DefaultView;
        }

        public static void ConnectToRepairsTable(DataGrid dataGrid, string license_plate)
        {
            repairsTableAdapter.FillByLicensePlate(ds.repairs, license_plate);
            dataGrid.ItemsSource = ds.repairs.DefaultView;
        }

        public static MyGarageDataSet.carsDataTable GetCarTableByLicensePlate(String license_plate)
        {
            return carsTableAdapter.GetCarByLicensePlate(license_plate);
        }

        public static void DeleteCar(String license_plate)
        {
            carsTableAdapter.DeleteCar(license_plate);
            RefreshCars();
        }

        public static void AddCarsRow(MyGarageDataSet.carsRow row, MyGarageDataSet dataSet)
        {
            dataSet.cars.Rows.Add(row);

            carsTableAdapter.Update(dataSet.cars);

            Sql.RefreshCars();
        }

        public static void AddRepairsRow(MyGarageDataSet.repairsRow row, MyGarageDataSet dataSet, String license_plate)
        {
            dataSet.repairs.Rows.Add(row);

            repairsTableAdapter.Update(dataSet.repairs);

            Sql.RefreshRepairs(license_plate);
        }

        public static void RefreshCars()
        {
            carsTableAdapter.Fill(ds.cars);
        }

        public static void RefreshRepairs(String license_plate)
        {
            repairsTableAdapter.FillByLicensePlate(ds.repairs, license_plate);
        }
    }
}
