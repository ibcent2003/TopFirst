using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Models
{
    public class IndexViewModel
    {

        public List<ExchangeRate> CurrencyList{ get; set; }
        public List<News> NewsList { get; set; }
      // public List<Testimonial> TestimonialList { get; set; }

        public List<DateTime> newslistDate { get; set; }

        public List<News> CountNews { get; set; }
       // public News News { get; set; }

        public List<ExchangeRate> rateList { get; set; }
        public bool HasNewEvent { get; set; }

        public DateTime newsdate { get; set; }

        public News news { get; set; }
      

        public List<News> category { get; set; }
        public List<string> newsCategory { get; set; }

       
        public string PicturePath { get; set; }

        public string CoursePath { get; set; }

        public string masterURL { get; set; }


      
        public SelectList Currencies{get;set;}

        public CurrencyConverterForm CurrencyConverterForm{get;set;}

        public PreOrderForm preOrderForm { get; set; }


    }
  

    public class PreOrderForm
    {
        [Required(ErrorMessage = "Please enter your full Name")]
        [Display(Name = "Full Name")]
        public string fname { get; set; }

        [Required(ErrorMessage = "Please enter your Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter your Order Type")]
        [Display(Name = "Order Type")]
        public string BuyingOrSelling { get; set; }


        [Required(ErrorMessage = "Please enter your Currency Name")]
        [Display(Name = "Currency Name")]
        public int Country { get; set; }


        [Required(ErrorMessage = "Please enter the amount")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
    }

    public class CurrencyConverterForm
    {
        public int Country { get; set; }
        public double Amount { get; set; }
    }

}