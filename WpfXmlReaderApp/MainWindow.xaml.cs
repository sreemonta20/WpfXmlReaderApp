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
using System.IO;
using Path = System.IO.Path;
using System.Xml;
using System.Globalization;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WpfXmlReaderApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variable declaration
        WebOrder oGlobalWebOrder = null;
        #endregion

        #region Constructor Initialization
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region All Methods
        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument oXmlDoc = new XmlDocument();
            XmlNodeList oListMasterXmlNode;
            int i = 0;
            string result = string.Empty;
            List<string> Operator = new List<string>();
            WebOrder oWebOrder = new WebOrder();
            List<WebOrderItem> oListWebOrderItem = new List<WebOrderItem>();
            oGlobalWebOrder = null;
            try
            {
                OpenFileDialog oFileDialog = new OpenFileDialog();
                oFileDialog.Filter = "XML Files (*.xml)|*.xml";
                oFileDialog.FilterIndex = 0;
                oFileDialog.DefaultExt = "xml";
                if (oFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!String.Equals(Path.GetExtension(oFileDialog.FileName), ".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("File Type is not supported", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        txtInputXml.Text = String.Format(@"{0}", oFileDialog.FileName);
                        oXmlDoc.Load(oFileDialog.FileName);
                        oListMasterXmlNode = oXmlDoc.GetElementsByTagName("WebOrder");
                        for (i = 0; i <= oListMasterXmlNode.Count - 1; i++)
                        {
                            if (oListMasterXmlNode[i].Attributes[0].Name.Equals("id"))
                            {
                                oWebOrder.Id = String.IsNullOrEmpty(oListMasterXmlNode[i].Attributes[0].Value) ? 0 : Convert.ToInt32(oListMasterXmlNode[i].Attributes[0].Value);

                            }
                            foreach (XmlNode item in oListMasterXmlNode[i].ChildNodes)
                            {
                                if (((XmlElement)item).Name.Equals("Customer"))
                                {
                                    oWebOrder.Customer = ((XmlElement)item).InnerText;
                                }
                                else if (((XmlElement)item).Name.Equals("Date"))
                                {
                                    string strDate = ((XmlElement)item).InnerText.Trim();
                                    string convertedDate = DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                    oWebOrder.Date = Convert.ToDateTime(convertedDate);
                                }
                                else if (((XmlElement)item).Name.Equals("Items"))
                                {

                                    foreach (XmlNode p in item.ChildNodes)
                                    {
                                        WebOrderItem oWebOrderItem = new WebOrderItem();
                                        if (p.Attributes.Count > 0)
                                        {
                                            foreach (XmlAttribute k in p.Attributes)
                                            {
                                                if (k.Name.Equals("id"))
                                                {
                                                    oWebOrderItem.Id = Convert.ToString(k.Value);
                                                }
                                                else
                                                {
                                                    oWebOrderItem.Description = Convert.ToString(k.Value);
                                                }
                                            }
                                        }
                                        foreach (XmlElement detail in p.ChildNodes)
                                        {
                                            if (detail.Name.Equals("Quantity"))
                                            {
                                                oWebOrderItem.Quantity = Convert.ToDecimal(detail.InnerText.Trim());
                                            }
                                            else if (detail.Name.Equals("Price"))
                                            {
                                                oWebOrderItem.Price = Convert.ToDecimal(detail.InnerText.Trim());
                                            }
                                        }
                                        oListWebOrderItem.Add(oWebOrderItem);
                                    }
                                    oWebOrder.Items = oListWebOrderItem;
                                }

                            }
                            //oListMasterXmlNode[i].ChildNodes.Item(0).InnerText.Trim();

                        }
                        oGlobalWebOrder = oWebOrder;

                    }
                }
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AssignResult(oGlobalWebOrder);
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }
        }

        public void AssignResult(WebOrder oWebOrder)
        {
            int count = 0;
            decimal sumOfPriceAvg = 0;
            decimal sumOfPriceTotal = 0;

            try
            {
                lblIdResult.Content = Convert.ToString(oWebOrder.Id);
                lblCustResult.Content = oWebOrder.Customer;
                lblDateResult.Content = Convert.ToDateTime(oWebOrder.Date).ToString("dd. MMMM. yyyy");

                if (oWebOrder.Items.Count > 0)
                {
                    count = oWebOrder.Items.Count;
                }

                foreach (var item in oWebOrder.Items)
                {
                    sumOfPriceAvg += item.Price;
                    sumOfPriceTotal += (item.Price * item.Quantity);
                }

                //lblPriceAvgResult.Content = string.Format("{0:N3}", sumOfPriceAvg / count);
                //lblTotalResult.Content = string.Format("{0:N3}", sumOfPriceTotal);
                //lblPriceAvgResult.Content = string.Format("{0:#.##0,000}", sumOfPriceAvg / count);
                //lblTotalResult.Content = string.Format("{0:#.##0,000}", sumOfPriceTotal);
                //lblPriceAvgResult.Content = (sumOfPriceAvg / count).ToString("#,###.##", new CultureInfo("da"));
                //lblTotalResult.Content = sumOfPriceTotal.ToString("#,###.##", new CultureInfo("da"));
                lblPriceAvgResult.Content = (sumOfPriceAvg / count).ToString("C3", new CultureInfo("da")).Replace("kr.", "");
                lblTotalResult.Content = sumOfPriceTotal.ToString("C3", new CultureInfo("da")).Replace("kr.", "");

            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }
        }
        #endregion
    }
}
