using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();           
            model.NewsList = db.News.Where(x => x.IsPublished == true && x.IsDeleted == false).ToList();
            model.PicturePath = Properties.Settings.Default.PhotoPath;
            model.rateList = db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).ToList();

            model.Currencies = new SelectList(db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).Take(18), "Buying", "CurrencyName");

            model.CurrencyList = db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).Take(18).ToList();
            // model.TestimonialList = db.Testimonial.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();           
            return View(model);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult News()
        {
            IndexViewModel model = new IndexViewModel();
            model.NewsList = db.News.Where(x => x.IsPublished == true && x.IsDeleted == false).OrderByDescending(x=>x.CreatedDate).ToList();
            return View(model);
        }

        public ActionResult NewsDetails(int Id)
        {
            IndexViewModel model = new IndexViewModel();
            var news = db.News.Where(x => x.Id == Id).FirstOrDefault();
            int add = 1;
            int oldCount = news.NoOfView;
            int totalCount = add + oldCount;
            news.NoOfView = totalCount;
            db.SaveChanges();

            model.news = news;
            model.PicturePath = Settings.Default.PhotoPath;
            model.NewsList = (from n in db.News select n).Take(5).ToList();
            var GetNew = (from s in db.News select s).Distinct().ToList();
            List<DateTime> result = GetNew.Select(d => new DateTime(d.CreatedDate.Year, d.CreatedDate.Month, 1)).Distinct().ToList();
            model.newslistDate = result;
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CashPurchase()
        {
            return View();
        }

        public ActionResult CreditCardPayments()
        {
            return View();
        }

        public ActionResult ForeignPayments()
        {
            return View();
        }

        public ActionResult Inflow()
        {
            return View();
        }

        public ActionResult PreOrder()
        {
            IndexViewModel model = new IndexViewModel();
            model.Currencies = new SelectList(db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).Take(18), "Id", "CurrencyName");
            return View(model);

        }
        [HttpPost]
        public ActionResult PreOrder(IndexViewModel model)
        {
            try
            {
                model.Currencies = new SelectList(db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).Take(18), "Id", "CurrencyName");
                if (ModelState.IsValid)
                {
                    PreOrder add = new PreOrder
                    {
                        Name = model.preOrderForm.fname,
                        Country = model.preOrderForm.Country,
                        Amount = model.preOrderForm.Amount,
                        BuyingOrSelling=model.preOrderForm.BuyingOrSelling,
                        EmailAddress = model.preOrderForm.Email,
                        MobileNo=model.preOrderForm.MobileNumber,
                        OrderDate = DateTime.Now


                    };
                    db.PreOrder.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b> Dear " + model.preOrderForm.fname + "</b> your order has been submitted successfully. One of our staff will contact you. You can also contact us on 08066111112. Our whats app no 07034595351";
                    return RedirectToAction("PreOrder");
                }
                return View(model);
            }
            catch (Exception ex)
            {

                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View(model);
            }
        
       }

        public ActionResult PrivacyPolicy()
        {
           
            return View();
        }

        public ActionResult TermsCondition()
        {
            return View();
        }
    }
}
