using MahApps.Metro.Controls;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AllInOutForHR
{
    /// <summary>
    /// Interaction logic for UsersList.xaml
    /// </summary>
    public partial class UsersList : MetroWindow
    {
        SqlDataAdapter adapter, combodapter;
        DataTable table, comboTable;
        public UsersList()
        {
            InitializeComponent();
        }
        private void refreshDB()
        {
            table = new DataTable();
            adapter.Fill(table);
            dataGrid.ItemsSource = table.DefaultView;

        }
        private void SearchDivName()
        {
            string connectionString = @"Data Source=MN01;Initial Catalog=OrionLight;Integrated Security=True";
            string sql = @"select TabNumber, SurName + ' ' + FirstName + ' ' + SecondName as fio, d.DivName from List
                                left join Division d on d.ID = List.DivisionId
                                where DivisionId = '" + comboBox.SelectedValue + "' ORDER BY fio";
            SqlConnection sqlconnection = new SqlConnection();
            try
            {
                sqlconnection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, sqlconnection);
                adapter = new SqlDataAdapter(command);
                sqlconnection.Open();
                refreshDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlconnection.State == ConnectionState.Open)
                    sqlconnection.Close();
            }
        }
        private void SearchFio()
        {
        
            string connectionString = @"Data Source=MN01;Initial Catalog=OrionLight;Integrated Security=True";
            string sql = @"select TabNumber, SurName + ' ' + FirstName + ' ' + SecondName as fio, d.DivName from List
                                left join Division d on d.ID = List.DivisionId
                                left join Company c on c.Id = List.CompanyId
                                where SurName + ' ' + FirstName + ' ' + SecondName like '%" + textBox.Text + "%' and List.CompanyId = '1' ORDER BY fio";
            SqlConnection sqlconnection = new SqlConnection();
            try
            {
                sqlconnection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, sqlconnection);
                adapter = new SqlDataAdapter(command);
                sqlconnection.Open();
                refreshDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlconnection.State == ConnectionState.Open)
                    sqlconnection.Close();
            }
        }
    
        private void refreshCB()
        {
            comboTable = new DataTable();
            combodapter.Fill(comboTable);
            comboBox.SelectedValuePath = "ID";
            comboBox.DisplayMemberPath = "DivName";
            comboBox.ItemsSource = comboTable.DefaultView;
        }
        private void initializeCB()
        {
            string connectionString = @"Data Source=MN01;Initial Catalog=OrionLight;Integrated Security=True";
            string query = @"SELECT ID, DivName FROM [OrionLight].[dbo].[Division] ORDER BY DivName";

            SqlConnection sqlconnection = new SqlConnection();
            try
            {
                sqlconnection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(query, sqlconnection);
                combodapter = new SqlDataAdapter(command);
                refreshCB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlconnection.State == ConnectionState.Open)
                    sqlconnection.Close();
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            SearchDivName();
        }

        private void dataGrid_Initialized(object sender, EventArgs e)
        {
            SearchFio();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchDivName();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.Text != String.Empty)
                SearchFio();
            else
                SearchDivName();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGridCellTarget = (DataGridRow)sender;
            UserClass.Username = ((DataRowView)dataGrid.SelectedItem).Row["fio"].ToString();
            MainWindow mw = new MainWindow();
            mw.Show();
        }
        private void comboBox_Initialized(object sender, EventArgs e)
        {
            initializeCB();
        }
    }
}
