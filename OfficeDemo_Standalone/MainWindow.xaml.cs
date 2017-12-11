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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Windows.Threading;

namespace OfficeDemo_Standalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (cboSupplier.SelectedIndex == -1)
            {
                MessageBox.Show("Var god välj Återförsäljare", "Ingen återförsäljare vald");
                cboSupplier.Focus();
                return;
            }
            if (cboMonth.SelectedIndex == -1)
            {
                MessageBox.Show("Var god välj månad", "Ingen månad vald");
                cboMonth.Focus();
                return;
            }

            int orders, costs, sales;

            if (!int.TryParse(txtSales.Text, out sales))
            {
                MessageBox.Show("Förstod inte försäljnings-beloppet.\nÄr det ett heltal?");
                txtSales.Focus();
                return;
            }

            if (!int.TryParse(txtOrders.Text, out orders))
            {
                MessageBox.Show("Förstod inte order-beloppet.\nÄr det ett heltal?");
                txtOrders.Focus();
                return;
            }

            if (!int.TryParse(txtCosts.Text, out costs))
            {
                MessageBox.Show("Förstod inte kostnads-beloppet.\nÄr det ett heltal?");
                txtCosts.Focus();
                return;
            }

            try
            {

                //package info
                var report = new ResellerReport();
                report.Reseller= cboSupplier.Text;
                report.Date = cboMonth.Text;
                report.Sales = Convert.ToInt32(txtSales.Text);
                report.Orders = Convert.ToInt32(txtOrders.Text);
                report.Costs = Convert.ToInt32(txtCosts.Text);
                report.Email = txtEmail.Text;

                //serialize to XML
                XmlSerializer writer = new XmlSerializer(typeof(ResellerReport));
                string path = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\OfficeDemo");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                path += "\\salesreport.xml";

                using (System.IO.FileStream file = System.IO.File.Create(path))
                {
                    writer.Serialize(file, report);
                }
                writer = null;

            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Något Gick Snett...\n{Ex.Message}");
                return;
            }

            btnSend.Content = "Rapport Skapad";
            //set up timer to reset the caption
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            btnSend.Content = "Rapportera";
        }
    }
}
