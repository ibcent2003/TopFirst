using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class ExchangeRateViewModel
    {
        public string PicturePath { get; set; }
        public IList<ExchangeRate> Rows { get; set; }
        public ExchangeRate Rate { get; set; }

        public ExchangeForm Exchangeform { get; set; }
    }

    public class ExchangeForm
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }        
        public HttpPostedFileBase Photo { get; set; }
        public decimal Buying { get; set; }
        public decimal Selling { get; set; }
        public DateTime WeeklyDate { get; set; }
    }
}