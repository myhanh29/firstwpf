using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.view.UserControls;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Reflection.Metadata;



namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      

        
        public MainWindow()
        {
            InitializeComponent();
            panel1.Visibility = Visibility.Visible;
            panel2.Visibility = Visibility.Collapsed;
        }

        private void saveruser_Click_1(object sender, RoutedEventArgs e)
        {
            DateTime? datepicker1 = dp.SelectedDate;
            string formatted = datepicker1.HasValue ? datepicker1.Value.ToString("yyyy-MM-dd") : string.Empty; 
            

            // Code zum Anzeigen des Benutzersteuerelements in einem Popup-Fenster
            string[] labels = { "Vorname: ", "Nachname: ", "Geburtdatum: ", "Anschrift: ", "Handynummer: " };
            string[] datas = { textbox1.Text, textbox2.Text, formatted, textbox3.Text, textbox4.Text };

             Window window = new Window();

            // Create a StackPanel to hold the text blocks
            StackPanel stackPanel = new StackPanel();

            for (int i = 0; i < labels.Length; i++)
            {
                TextBlock labelBlock = new TextBlock
                {
                    Text = labels[i],
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                };
                stackPanel.Children.Add(labelBlock);

                TextBlock dataBlock = new TextBlock
                {
                    Text = datas[i],
                    Margin = new Thickness(5),
                };
                stackPanel.Children.Add(dataBlock);

               
            }
            Button button1 = new Button
            {
                Content = "Save",
                Margin = new Thickness(5)
            };
            button1.Click += (s, args) => { SaveButonnClick(s, args, window); };
            stackPanel.Children.Add(button1);

            Button button2 = new Button
            {
                Content = "Delete",
                Margin = new Thickness(5)
            };
            button2.Click += (s, args) => { CancelButonnClick(s, args, window); };
            stackPanel.Children.Add(button2);

            window.Content = stackPanel;

            window.ShowDialog();
        }

        private void clear_Click_1(object sender, RoutedEventArgs e)
        {
            textbox1.Clear();
            textbox2.Clear();
            dp.SelectedDate = DateTime.Now;
            textbox3.Clear();
            textbox4.Clear();
        }
        private void SaveButonnClick(object sender, RoutedEventArgs e, Window window)
        {
            SaveToDB(sender, e, window);
            panel1.Visibility = Visibility.Collapsed;
            panel2.Visibility = Visibility.Visible;
            LoadData();
            
        }
        private void SearchinTable(object sender, EventArgs e, TextBox searchText, DataGrid dgv)
        {

            string filter = string.Empty;
            foreach (DataGridColumn column in dgv.Columns)
            {
                if (column.DisplayIndex == 0)
                {
                    filter += string.Format("Vorname LIKE '%{0}%'", searchText.Text);
                }
                else
                {
                    filter += string.Format(" OR Vorname LIKE '%{0}%'", searchText.Text);
                }
            }
          (dgv.ItemsSource as DataTable).DefaultView.RowFilter = filter;

        }
        public void LoadData()
        {
            panel2.Visibility = Visibility.Visible;
            panel2.Children.Clear();

            // Verbindungszeichenfolge aus der Konfiguration abrufen
            string con_string = @"Server=LAPTOP-5OQP0KBS\SQLEXPRESS;Database=wpf;Trusted_Connection=True;";
            DataGrid dgv = new DataGrid
            {
                AutoGenerateColumns = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            panel2.Children.Add(dgv);
            // using-Anweisungen zur korrekten Freigabe der Ressourcen verwenden
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
                string CommandText = "SELECT * FROM userdata";
                using (SqlCommand cmd = new SqlCommand(CommandText, sqlConnection))
                {
                   
                    sqlConnection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    dgv.ItemsSource = ds.Tables[0].DefaultView;

                }
            }

            StackPanel searchpanel = new StackPanel
            {
               
                Width = dgv.Width / 3,
                Height = 30,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10)

            };

            TextBox searchText = new TextBox
            {
                Width = 150,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                Text ="ne"
            };
            searchpanel.Children.Add(searchText);

            Button buttonsearch = new Button
            {
                Width = 100,
                Height = searchText.Height + 3,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                Content = "Search"
            };
            buttonsearch.Click += (s, eventArgs) => { SearchinTable(s, eventArgs, searchText,dgv); };
            searchpanel.Children.Add(buttonsearch);

            panel2.Children.Add(searchpanel);

            Button button5 = new Button
            {
                Content = "Save to PDF",
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            panel2.Children.Add(button5);
            button5.Click += (s, eventArgs) => { SaveToPDF(s, eventArgs); };

            Button button6 = new Button
            {
                Content = "Save to EXCEL",
                VerticalAlignment = VerticalAlignment.Bottom
            };

            panel2.Children.Add(button6);
            button6.Click += (s, eventArgs) => { SaveToEXCEL(s, eventArgs); };
        }

        private void SaveToPDF(object sender, EventArgs e)
        {

          
                

            

        }
        // Ereignishandler für den Klick auf die "Speichern als EXCEL"-Schaltfläche
        private void SaveToEXCEL(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };
          
        }
        private async void SaveToDB(object sender, RoutedEventArgs e, Window window)
        {
          
                string connectionString = @"Server=LAPTOP-5OQP0KBS\SQLEXPRESS;Database=wpf;Trusted_Connection=True;";

                // SQL-Befehl zum Einfügen von Daten in die Tabelle Userdatas
                string myinsert = "INSERT INTO userdata (Vorname, Nachname, Geburtsdatum, Anschrift, Handynummer) VALUES (@Vorname, @Nachname, @Geburtdatum, @Anschrift, @Handynummer)";

            // using-Anweisungen zur korrekten Freigabe der Ressourcen verwenden
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand mycom = new SqlCommand(myinsert, sqlConnection))
                {


                    mycom.Parameters.AddWithValue("@Vorname", textbox1.Text);
                    mycom.Parameters.AddWithValue("@Nachname", textbox2.Text);
                    DateTime? datepicker1 = dp.SelectedDate;
                    if (datepicker1.HasValue)
                    {
                        mycom.Parameters.AddWithValue("@Geburtdatum", datepicker1.Value);
                    }
                    else
                    {
                        mycom.Parameters.AddWithValue("@Geburtdatum", DBNull.Value);
                    }
                    mycom.Parameters.AddWithValue("@Anschrift", textbox3.Text);
                    mycom.Parameters.AddWithValue("@Handynummer", textbox4.Text);
                    // Verbindung öffnen und SQL-Befehl ausführen
                    sqlConnection.Open();
                    await mycom.ExecuteNonQueryAsync();
                    MessageBox.Show("Save sucessfull");
                    window.Close();
                   
                }
            }
           
        }
       
        private void CancelButonnClick(object sender, RoutedEventArgs e, Window window)
        {
            window.Close();
        }


        private void EditUser_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
