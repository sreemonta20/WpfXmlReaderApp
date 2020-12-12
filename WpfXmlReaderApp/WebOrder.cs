using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfXmlReaderApp
{
    public class WebOrder
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public DateTime? Date { get; set; }
        public List<WebOrderItem> Items { get; set; }
    }
}
