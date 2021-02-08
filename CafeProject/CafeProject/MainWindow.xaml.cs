using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace CafeProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Invoice> InvoiceList = new List<Invoice>();
        private Invoice CurrentInvoice;

        private List<InvoiceItem> InvoiceItemsList = new List<InvoiceItem>();
        private InvoiceItem CurrentInvoiceItem;

        int k, temp = 0;


        bool isInvoiceValid = true;
        bool isInvoiceItemValid = true;


        private int CurrentSelectedIndexInvoice;
        private int CurrentSelectedIndexInvoiceItem;

        public MainWindow()
        {
            InitializeComponent();
            LoadInvoices();
        }
        void LoadInvoices()
        {
            // clear out the data list and listbox (fresh list)
            InvoiceList.Clear();
            InvoicesListBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection())
            {
                string connString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";

                connection.ConnectionString = connString;
                connection.Open();

                // Create a SQL Command object with a SQL Statement
                string sql = "SELECT * FROM Invoices ORDER BY InvoiceID";
                SqlCommand selectCommand = new SqlCommand(sql, connection);

                // Run the command and recieve a Reader with records
                using (SqlDataReader Reader = selectCommand.ExecuteReader())
                {

                    // loop through the Reader (reading each row 1 by 1) with Reader.Read() method
                    // when no rows, Reader returns false
                    while (Reader.Read() == true)
                    {

                        // for each row from Reader, make a new Customer object then load customer
                        // object into the List and listbox
                        Invoice NewInvoice = new Invoice((int)Reader["InvoiceID"],
                                                            (DateTime)Reader["InvoiceDate"],
                                                            (bool)Reader["Shipped"],
                                                            (string)Reader["CustomerName"],
                                                            (string)Reader["CustomerAddress"],
                                                            (string)Reader["CustomerEmail"]
                                                           );
                        // now add the new Invoice object to the list
                        InvoiceList.Add(NewInvoice);
                        // add to listbox
                        InvoicesListBox.Items.Add(NewInvoice);

                    }
                    Reader.Close();

                }
                connection.Close();

            }
        }

        private void InvoicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update the form with the currently selected record (in the listbox)
            CurrentInvoice = (Invoice)InvoicesListBox.SelectedItem;
            CurrentSelectedIndexInvoice = InvoicesListBox.SelectedIndex; // -1 if no item selected


            // display the CurrentCustomer in the form
            DisplayInvoice();
            LoadInvoicesItem();
            CalculateTotalPrice();


        }

        private void DisplayInvoice()
        {
            // protect from selectionchanged even from returning no selected object (null reference)
            if (CurrentInvoice != null)
            {
                invoiceIDTextBox.Text = CurrentInvoice.invoiceID.ToString();
                invoiceDateTextBox.Text = CurrentInvoice.invoiceDate.ToString();
                // shippedCheckBox.IsChecked = CurrentInvoice.shipped.CompareTo
                customerNameTextBox.Text = CurrentInvoice.customerName.ToString();
                customerAddressTextBox.Text = CurrentInvoice.customerAddress.ToString();
                customerEmailTextBox.Text = CurrentInvoice.customerEmail.ToString();

            }
            else
            {
                // no slected record to show - blank out form
                invoiceIDTextBox.Text = "";
                invoiceDateTextBox.Text = "";
                //shippedCheckBox.IsChecked = "";
                customerNameTextBox.Text = "";
                customerAddressTextBox.Text = "";
                customerEmailTextBox.Text = "";
            }


        }

        void LoadInvoicesItem()
        {
            if (CurrentInvoice != null)
            {
                // clear out the data list and listbox (fresh list)
                InvoiceItemsList.Clear();
                InvoicesItemsListBox.Items.Clear();

                using (SqlConnection connection = new SqlConnection())
                {
                    string connString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";

                    connection.ConnectionString = connString;
                    connection.Open();

                    // Create a SQL Command object with a SQL Statement
                    string sql = $"SELECT *FROM InvoiceItems WHERE InvoiceID = {CurrentInvoice.invoiceID}; ";
                    SqlCommand selectCommand = new SqlCommand(sql, connection);

                    // Run the command and recieve a Reader with records
                    using (SqlDataReader Reader = selectCommand.ExecuteReader())
                    {

                        // loop through the Reader (reading each row 1 by 1) with Reader.Read() method
                        // when no rows, Reader returns false
                        while (Reader.Read() == true)
                        {

                            // for each row from Reader, make a new Customer object then load customer
                            // object into the List and listbox
                            InvoiceItem NewInvoiceItem = new InvoiceItem((int)Reader["ItemID"],
                                                                (int)Reader["InvoiceID"],
                                                                (string)Reader["ItemName"],
                                                                (string)Reader["ItemDescription"],
                                                                (decimal)Reader["ItemPrice"],
                                                                (int)Reader["ItemQuantity"]
                                                               );
                            // now add the new Invoice object to the list
                            InvoiceItemsList.Add(NewInvoiceItem);
                            // add to listbox
                            InvoicesItemsListBox.Items.Add(NewInvoiceItem);

                        }
                        Reader.Close();

                    }
                    connection.Close();


                }
                CalculateTotalPrice();

            }
        }

        private void InvoicesItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update the form with the currently selected record (in the listbox)
            CurrentInvoiceItem = (InvoiceItem)InvoicesItemsListBox.SelectedItem;
            CurrentSelectedIndexInvoiceItem = InvoicesItemsListBox.SelectedIndex; // -1 if no item selected

            // display the CurrentCustomer in the form
            DisplayInvoiceItem();
        }

        private void DisplayInvoiceItem()
        {
            // protect from selectionchanged even from returning no selected object (null reference)
            if (CurrentInvoiceItem != null)
            {
                itemIDTextBox.Text = CurrentInvoiceItem.itemID.ToString();
                itemNameTextBox.Text = CurrentInvoiceItem.itemName.ToString();

                itemDescriptionTextBox.Text = CurrentInvoiceItem.itemDescription.ToString();
                itemPriceTextBox.Text = CurrentInvoiceItem.itemPrice.ToString();
                itemQuantityTextBox.Text = CurrentInvoiceItem.itemQuantity.ToString();

            }
            else
            {
                // no slected record to show - blank out form
                itemIDTextBox.Text = "";
                itemNameTextBox.Text = "";

                itemDescriptionTextBox.Text = "";
                itemPriceTextBox.Text = "";
                itemQuantityTextBox.Text = "";
            }


        }

        bool ReturnInvoiceValid()
        {
            if ((customerNameTextBox.Text == "") || (customerEmailTextBox.Text == "") || (invoiceDateTextBox.Text == ""))
            {
                isInvoiceValid = false;
            }

            else
            {
                isInvoiceValid = true;
            }

            return isInvoiceValid;

        }

        bool ReturnInvoiceItemValid()
        {
            if ((itemNameTextBox.Text == "") || (itemPriceTextBox.Text == "") || (Convert.ToDecimal(itemPriceTextBox.Text) <= 0) || (itemQuantityTextBox.Text == "") || (Convert.ToInt32(itemQuantityTextBox.Text) <= 0))
            {
                isInvoiceItemValid = false;
            }
            else
            {
                isInvoiceItemValid = true;

            }
            return isInvoiceItemValid;
        }


        private void saveInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnInvoiceValid() == false)
            {
                ErrorTextBox.Text = "You need to enter blank textbox";

            }
            else
            {
                ErrorTextBox.Text = "";

                k = CurrentSelectedIndexInvoice;
                if (k != -1)
                {

                    using (SqlConnection connection = new SqlConnection())
                    {
                        connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                        connection.Open();

                        string sql = $"UPDATE Invoices SET "
                        + $"InvoiceDate = '{Convert.ToDateTime(invoiceDateTextBox.Text)}', "
                        + $"CustomerName = '{customerNameTextBox.Text}', "
                        + $"CustomerEmail = '{customerEmailTextBox.Text}', "
                                + $"CustomerAddress = '{customerAddressTextBox.Text}', "
                                + $"Shipped = '{Convert.ToBoolean(shippedCheckBox.IsChecked)}' "
                                + $"WHERE InvoiceID = {CurrentInvoice.invoiceID}; ";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {

                            command.ExecuteNonQuery();

                        }

                        connection.Close();

                    }
                    LoadInvoices();


                    CurrentInvoice = InvoiceList[k];


                    customerNameTextBox.Text = InvoiceList[k].customerName;
                    customerEmailTextBox.Text = InvoiceList[k].customerEmail;
                    customerAddressTextBox.Text = InvoiceList[k].customerAddress;
                    invoiceDateTextBox.Text = Convert.ToString(InvoiceList[k].invoiceDate);
                    shippedCheckBox.IsChecked = InvoiceList[k].shipped;
                    invoiceIDTextBox.Text = Convert.ToString(InvoiceList[k].invoiceID);

                    temp = k;//assign the value of k for variable temp

                }

                else // k = -1 <-> no invoice is selected in the listbox, still keep the selected invoice before,
                //so user no need to click the same invoice all the time. If click different invoice, program will execute "IF" part
                //If not, program will execute "ELSE" part
                {
                    k = temp;//then variable temp is assigned for k again, so still keep the selected invoice before

                    using (SqlConnection connection = new SqlConnection())
                    {
                        connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                        connection.Open();

                        string sql = $"UPDATE Invoices SET "
                        + $"InvoiceDate = '{Convert.ToDateTime(invoiceDateTextBox.Text)}', "
                        + $"CustomerName = '{customerNameTextBox.Text}', "
                        + $"CustomerEmail = '{customerEmailTextBox.Text}', "
                                + $"CustomerAddress = '{customerAddressTextBox.Text}', "
                                + $"Shipped = '{Convert.ToBoolean(shippedCheckBox.IsChecked)}' "
                                + $"WHERE InvoiceID = {CurrentInvoice.invoiceID}; ";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {

                            command.ExecuteNonQuery();

                        }

                        connection.Close();

                    }
                    LoadInvoices();


                    CurrentInvoice = InvoiceList[k];


                    customerNameTextBox.Text = InvoiceList[k].customerName;
                    customerEmailTextBox.Text = InvoiceList[k].customerEmail;
                    customerAddressTextBox.Text = InvoiceList[k].customerAddress;
                    invoiceDateTextBox.Text = Convert.ToString(InvoiceList[k].invoiceDate);
                    shippedCheckBox.IsChecked = InvoiceList[k].shipped;
                    invoiceIDTextBox.Text = Convert.ToString(InvoiceList[k].invoiceID);




                }



            }

        }



        private void newInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnInvoiceValid() == false)
            {
                ErrorTextBox.Text = "You need to enter blank textbox";

            }
            else
            {

                ErrorTextBox.Text = "";
                using (SqlConnection connection = new SqlConnection())
                {

                    connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                    connection.Open();

                    string sql = "SELECT MAX(InvoiceID) FROM Invoices; ";
                    int IDMax = 0;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        IDMax = Convert.ToInt32(command.ExecuteScalar()) + 1;


                    }



                    //update in database
                    sql = "INSERT INTO Invoices "
                        + "(InvoiceID, InvoiceDate, Shipped, CustomerName, CustomerAddress, CustomerEmail) "
                        + "VALUES " + "(" + $"{IDMax}, " + $"'{Convert.ToDateTime(invoiceDateTextBox.Text)}', "
                        + $"'{Convert.ToBoolean(shippedCheckBox.IsChecked)}', " + $"'{customerNameTextBox.Text}', "
                        + $"'{customerAddressTextBox.Text}', " + $"'{customerEmailTextBox.Text}' " + "); ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                }

                LoadInvoices();

                //now change Current Invoice = new invoice
                CurrentInvoice = InvoiceList[InvoiceList.Count - 1];

                customerNameTextBox.Text = CurrentInvoice.customerName;
                customerEmailTextBox.Text = CurrentInvoice.customerEmail;
                customerAddressTextBox.Text = CurrentInvoice.customerAddress;
                invoiceDateTextBox.Text = Convert.ToString(CurrentInvoice.invoiceDate);
                shippedCheckBox.IsChecked = Convert.ToBoolean(CurrentInvoice.shipped);
                invoiceIDTextBox.Text = Convert.ToString(CurrentInvoice.invoiceID);


                //delete invoice item (of previous invoice) after having new invoice In InvoicelIstBox
                CurrentInvoiceItem = null;
                DisplayInvoiceItem();
                InvoicesItemsListBox.Items.Clear();



            }
        }

        private void deleteInvoiceButton_Click(object sender, RoutedEventArgs e)
        {

            //delete corresponding invoice item
            DeleteAllInvoiceItem();
            //load invoice item
            LoadInvoicesItem();

            k = CurrentSelectedIndexInvoice;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                string sql = $"DELETE FROM Invoices WHERE InvoiceID = {CurrentInvoice.invoiceID}; ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }


            //load invoice list
            LoadInvoices();

            //to delete Invoice continuously without clicking in textbox again
            if (InvoiceList.Count > 0)
            {
                if (k == InvoiceList.Count)
                {

                    CurrentInvoice = InvoiceList[k - 1];
                    CurrentSelectedIndexInvoice = k - 1;

                }
                else if (k == 0)
                {
                    CurrentInvoice = InvoiceList[0];
                    CurrentSelectedIndexInvoice = 0;
                }
                else
                {
                    CurrentInvoice = InvoiceList[k];
                    CurrentSelectedIndexInvoice = k;

                }

            }

            else if (InvoiceList.Count == 1)
            {
                return;
            }
            else
            {
                return;

            }


        }

        void DeleteAllInvoiceItem()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                string sql = $"DELETE FROM InvoiceItems WHERE InvoiceID= {CurrentInvoice.invoiceID}; ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();

            }


        }

        private void saveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnInvoiceItemValid() == false)
            {
                ErrorTextBox.Text = "You need to enter blank textbox";
            }
            else
            {
                ErrorTextBox.Text = "";
                k = CurrentSelectedIndexInvoiceItem;

                if (k != -1)
                {

                    using (SqlConnection connection = new SqlConnection())
                    {
                        connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                        connection.Open();

                        string sql = $"UPDATE InvoiceItems SET " + $"ItemName = '{itemNameTextBox.Text}', "
                            + $"ItemDescription = '{itemDescriptionTextBox.Text}', "
                            + $"ItemPrice = {Convert.ToDecimal(itemPriceTextBox.Text)}, "
                            + $"ItemQuantity = {Convert.ToInt32(itemQuantityTextBox.Text)} "
                            + $"WHERE ItemID = {CurrentInvoiceItem.itemID}; ";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                        }

                        connection.Close();

                    }
                    //load invoice item 
                    LoadInvoicesItem();

                    CurrentInvoiceItem = InvoiceItemsList[k];
                    DisplayInvoiceItem();

                    temp = k;
                }

                else// no invoice item is selected in the listbox, keep the selected invoice item before
                {
                    k = temp;
                    using (SqlConnection connection = new SqlConnection())
                    {
                        connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                        connection.Open();

                        string sql = $"UPDATE InvoiceItems SET " + $"ItemName = '{itemNameTextBox.Text}', "
                            + $"ItemDescription = '{itemDescriptionTextBox.Text}', "
                            + $"ItemPrice = {Convert.ToDecimal(itemPriceTextBox.Text)}, "
                            + $"ItemQuantity = {Convert.ToInt32(itemQuantityTextBox.Text)} "
                            + $"WHERE ItemID = {CurrentInvoiceItem.itemID}; ";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();

                        }

                        connection.Close();

                    }
                    //load invoice item 
                    LoadInvoicesItem();

                    CurrentInvoiceItem = InvoiceItemsList[k];
                    DisplayInvoiceItem();


                }


            }

        }

        private void newItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnInvoiceItemValid() == false)
            {
                ErrorTextBox.Text = "You need to enter blank textbox";
            }
            else
            {
                ErrorTextBox.Text = "";
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                    connection.Open();
                    int ItemIDMax = 0;

                    string sql = "SELECT MAX(ItemID) FROM InvoiceItems; ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        ItemIDMax = Convert.ToInt32(command.ExecuteScalar()) + 1;
                    }

                    sql = "INSERT INTO InvoiceItems "
                        + "(ItemID, InvoiceID, ItemName, ItemDescription, ItemPrice, ItemQuantity) "
                        + "VALUES " + "(" + $"{ItemIDMax}, " + $"{CurrentInvoice.invoiceID}, "
                        + $"'{itemNameTextBox.Text}', " + $"'{itemDescriptionTextBox.Text}', "
                        + $"{Convert.ToDecimal(itemPriceTextBox.Text)}, " + $"{Convert.ToInt32(itemQuantityTextBox.Text)} "
                        + "); ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }
                //load invoice item
                LoadInvoicesItem();

                CurrentInvoiceItem = InvoiceItemsList[InvoiceItemsList.Count - 1];
                DisplayInvoiceItem();
            }
        }



        private void deleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            k = CurrentSelectedIndexInvoiceItem;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                string sql = $"DELETE FROM InvoiceItems WHERE ItemID= {CurrentInvoiceItem.itemID}; ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();

            }

            //load invoice item after deleting
            LoadInvoicesItem();

            //If you want to continue deleting

            if (InvoiceItemsList.Count > 0)
            {
                if (k == InvoiceItemsList.Count)
                {
                    CurrentInvoiceItem = InvoiceItemsList[k - 1];
                    CurrentSelectedIndexInvoiceItem = k - 1;
                }
                else if (k == 0)
                {
                    CurrentInvoiceItem = InvoiceItemsList[0];
                    CurrentSelectedIndexInvoiceItem = 0;

                }
                else
                {
                    CurrentInvoiceItem = InvoiceItemsList[k];
                    CurrentSelectedIndexInvoiceItem = k;
                }
            }
            else if (InvoiceItemsList.Count == 1)
            {
                return;
            }
            else
            {
                return;
            }


        }

        void CalculateTotalPrice()
        {
            decimal SubTotal = 0;
            decimal GST = 0;
            decimal PST = 0;
            decimal ToTal = 0;

            //calculate SubTotal
            for (int i = 0; i < InvoiceItemsList.Count; i++)
            {
                SubTotal = SubTotal + InvoiceItemsList[i].itemPrice * InvoiceItemsList[i].itemQuantity;
            }

            GST = (SubTotal * 5) / 100;
            PST = (SubTotal * 6) / 100;

            ToTal = ToTal + SubTotal + GST + PST;


            //show info in textbox
            subTotalTextBox.Text = Convert.ToString(SubTotal);
            GSTTextBox.Text = Convert.ToString(GST);
            PSTTextBox.Text = Convert.ToString(PST);
            totalTextBox.Text = Convert.ToString(ToTal);

        }

    }
}
