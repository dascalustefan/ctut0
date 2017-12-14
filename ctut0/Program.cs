using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace ctut0
{
   
    class Program
    {
       static private string EnumerateView(DataView view)
        {
            var buffer = new System.Text.StringBuilder();
            foreach (DataColumn dc in view.Table.Columns)
            {
                buffer.AppendFormat("{0,15} ", dc.ColumnName);
            }
            buffer.Append("\r\n");
            foreach (DataRowView dr in view)
            {
                foreach (DataColumn dc in view.Table.Columns)
                {
                    buffer.AppendFormat("{0,15} ", dr.Row[dc]);
                }
                buffer.Append("\r\n");
            }
            return buffer.ToString();
        }
        static public string EnumerateTable(DataTable cars)
        {
            var buffer = new System.Text.StringBuilder();
            foreach (DataColumn dc in cars.Columns)
            {
                buffer.AppendFormat("{0,15} ", dc.ColumnName);
            }
            buffer.Append("\r\n");
            foreach (DataRow dr in cars.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    buffer.Append("Deleted Row");
                }
                else
                {
                    foreach (DataColumn dc in cars.Columns)
                    {
                        buffer.AppendFormat("{0,15} ", dr[dc]);
                    }
                }
                buffer.Append("\r\n");
            }
           return buffer.ToString();
        }
        static private string GetDataRowInfo(DataRow row, string columnName)
        {
            string retVal = string.Format(
            "RowState: {0} \r\n",
            row.RowState);
            foreach (string versionString in Enum.GetNames(typeof(DataRowVersion)))
            {
                DataRowVersion version = (
                DataRowVersion)Enum.Parse(
                typeof(DataRowVersion), versionString);
                if (row.HasVersion(version))
                {
                    retVal += string.Format(
                    "Version: {0} Value: {1} \r\n",
                    version, row[columnName, version]);
                }
                else
                {
                    retVal += string.Format(
                    "Version: {0} does not exist.\r\n",
                    version);
                }
            }
            return retVal;
        }

        static void Main(string[] args)
        {
            //Create the DataTable named "Cars"
            DataTable cars = new DataTable("Cars");
            //Add the DataColumn using all properties
            DataColumn vin = new DataColumn("Vin");
            vin.DataType = typeof(string);
            vin.MaxLength = 23;
            vin.Unique = true;
            vin.AllowDBNull = false;
            vin.Caption = "VIN";
            cars.Columns.Add(vin);
            //Add the DataColumn using defaults
            DataColumn make = new DataColumn("Make");
            make.MaxLength = 35;
            make.AllowDBNull = false;
            cars.Columns.Add(make);
            DataColumn year = new DataColumn("Year", typeof(int));
            year.AllowDBNull = false;
            cars.Columns.Add(year);
            //Derived column using expression
            DataColumn yearMake = new DataColumn("Year and Make");
            yearMake.DataType = typeof(string);
            yearMake.MaxLength = 70;
            yearMake.Expression = "Year + ' ' + Make";
            cars.Columns.Add(yearMake);
            //Set the Primary Key
            cars.PrimaryKey = new DataColumn[] { vin };
            //Add New DataRow by creating the DataRow first
            DataRow newCar = cars.NewRow();
            newCar["Vin"] = "123456789ABCD";
            newCar["Make"] = "Ford";
            newCar["Year"] = 2002;
            cars.Rows.Add(newCar);
            //Add New DataRow by simply adding the values
            cars.Rows.Add("987654321XYZ", "Buick", 2001);
            //Load DataRow, replacing existing contents, if existing
            //cars.LoadDataRow(new object[]
            //{ "987654321XYZ", "Jeep", 2002 }, LoadOption.OverwriteChanges);
            cars.AcceptChanges();
            newCar.BeginEdit();//makes proposed
           // System.Console.Write(newCar.RowState.ToString()); rowstate
            //cars.Rows.Remove(newCar);
            for (int i = cars.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = cars.Rows[i];
                if (dr["make"] == "Buick")
                    cars.Rows.Remove(dr);
            }
            System.Console.Write(GetDataRowInfo(newCar, "vin"));
            System.Console.Write(EnumerateTable(cars));
            DataView view = new DataView(cars);
            view.RowFilter = "Make like 'B%' and Year > 2000";
            view.RowStateFilter = DataViewRowState.Deleted;
            System.Console.Write(EnumerateView(view));
        }

    }
}
