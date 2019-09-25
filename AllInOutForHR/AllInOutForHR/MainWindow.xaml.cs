using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AllInOutForHR
{
    public partial class MainWindow : MetroWindow
    {
        SqlDataAdapter adapter;
        DataTable table;
        bool changed = false;
        public MainWindow()
        {
            InitializeComponent();
            this.Title = UserClass.Username;
        }

        private void refreshDB()
        {
            table = new DataTable();
            adapter.Fill(table);
            dataGrid.ItemsSource = table.DefaultView;
        }
        private void initializeDB()
        {
            string connectionString = @"Data Source=MN01;Initial Catalog=OrionLight;Integrated Security=True";

            SqlConnection sqlconnection = new SqlConnection();
            try
            {
                sqlconnection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(@"SELECT TabNumber, SurName + ' ' + FirstName + ' ' + SecondName as fio, TimeVal,
                                                         case [Mode] 
                                                              when 1 then 'Вход'
                                                              when 2 then 'Выход'
                                                          end as ModeInOut,
                                                            CASE DATEDIFF(DAY,0, TimeVal)%7
								                              WHEN 0 THEN 'Понедельник'
								                              WHEN 1 THEN 'Вторник'
								                              WHEN 2 THEN 'Среда'
								                              WHEN 3 THEN 'Четверг'
								                              WHEN 4 THEN 'Пятница'
								                              WHEN 5 THEN 'Суббота'
								                              WHEN 6 THEN 'Воскресенье'
								                            END [DayOfTheWeek],
                                                          DivName, PointName FROM [OrionLight].[dbo].[AllInOutForHR] 
                                                          WHERE SurName + ' ' + FirstName + ' ' + SecondName = '" + UserClass.Username + "' ORDER BY TimeVal", sqlconnection);
                adapter = new SqlDataAdapter(command);
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
            if (datePicker2.SelectedDate != null && datePicker1.SelectedDate != null)
            {
                var date1 = Convert.ToDateTime(datePicker1.Text).ToString("yyyy/MM/dd");
                var date2 = Convert.ToDateTime(datePicker2.Text).ToString("yyyy/MM/dd");
                string connectionString = @"Data Source=MN01;Initial Catalog=OrionLight;Integrated Security=True";
                string sql = @"SELECT TabNumber, SurName + ' ' + FirstName + ' ' + SecondName as fio, TimeVal,
                             case [Mode] 
                                  when 1 then 'Вход'
                                  when 2 then 'Выход'
                              end as ModeInOut, 
							    CASE DATEDIFF(DAY,0, TimeVal)%7
									WHEN 0 THEN 'Понедельник'
									WHEN 1 THEN 'Вторник'
									WHEN 2 THEN 'Среда'
									WHEN 3 THEN 'Четверг'
									WHEN 4 THEN 'Пятница'
									WHEN 5 THEN 'Суббота'
									WHEN 6 THEN 'Воскресенье'
								END [DayOfTheWeek],
                              DivName, PointName FROM [OrionLight].[dbo].[AllInOutForHR] WHERE 
                              SurName + ' ' + FirstName + ' ' + SecondName = '" + UserClass.Username + "' and TimeVal between '" + date1 + "' and '" + date2 + "' ORDER BY TimeVal";
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
        }
        private void dataGrid_Initialized(object sender, EventArgs e)
        {
            initializeDB();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (changed == true)
            {
                SearchFio();
            }
            else
            {
                MessageBox.Show("Введите данные для поиска", "Ошибка");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            datePicker1.SelectedDate = null;
            datePicker2.SelectedDate = null;
            changed = false;
            initializeDB();
        }
        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            changed = true;
        }
        private void data2Exel(DataGrid dataGrid)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.DisplayAlerts = false;
                excel.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                sheet1.Name = "Посещения";
                Microsoft.Office.Interop.Excel.Range cellRange;

                System.Data.DataTable tempDt = table;
                dataGrid.ItemsSource = tempDt.DefaultView;
                sheet1.Cells.Font.Size = 11;
                int rowcount = 1;
                for (int i = 1; i <= tempDt.Columns.Count; i++) //Имена заголовков
                {
                    sheet1.Cells[1, i] = dataGrid.Columns[i - 1].Header;
                }
                foreach (System.Data.DataRow row in tempDt.Rows) //Все строки
                {
                    rowcount += 1;
                    for (int i = 0; i < tempDt.Columns.Count; i++) //Каждая колонка
                    {
                        sheet1.Cells[rowcount, i + 1] = row[i].ToString();
                    }
                }
                cellRange = sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[rowcount, tempDt.Columns.Count]];
                sheet1.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                sheet1.Columns.EntireColumn.AutoFit();
                SaveFileDialog saveDialog = new SaveFileDialog(); // Диалог сохранения файла
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;
                if (saveDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveDialog.FileName);
                }

                workbook.Close(); excel.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void buttonExcel_Click(object sender, RoutedEventArgs e)
        {
            data2Exel(dataGrid);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UsersList ul = new UsersList();
            ul.Close();
        }
    }
}
