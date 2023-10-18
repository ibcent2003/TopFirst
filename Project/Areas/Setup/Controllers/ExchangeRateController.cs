using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class ExchangeRateController : Controller
    {
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;
        public ExchangeRateController()
        {
            this.util = new ProcessUtility();
            this.filePath = Properties.Settings.Default.PhotoPath;
        }

        public ActionResult Index()
        {
            try
            {
                ExchangeRateViewModel model = new ExchangeRateViewModel();
                model.Rows = db.ExchangeRate.OrderByDescending(x => x.WeeklyDate).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult NewRate()
        {
            try
            {
                ExchangeRateViewModel model = new ExchangeRateViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult NewRate(ExchangeRateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //check for duplicate
                    //var validate = db.ExchangeRate.Where(x => x.CurrencyName == model.Exchangeform.Name).ToList();
                    //if (validate.Any())
                    //{
                    //    TempData["message"] = "The Document Format " + model.Documentformatform.Name + " already exist. Please enter different name";
                    //    TempData["messageType"] = "danger";
                    //    return View(model);
                    //}

                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);
                    //  List<DocumentInfo> uploadedResume = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.Exchangeform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        //model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Country Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.Exchangeform.Photo.ContentLength > max_upload)
                    {
                        // model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.Exchangeform.Photo.FileName);
                    model.Exchangeform.Photo.SaveAs(ResumePath + filename);

                    #endregion


                    ExchangeRate add = new ExchangeRate
                    {
                        CurrencyName = model.Exchangeform.CurrencyName,
                        Logo= filename,
                        Buying = model.Exchangeform.Buying,
                        Selling = model.Exchangeform.Selling,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                        WeeklyDate = model.Exchangeform.WeeklyDate
                    };
                    db.ExchangeRate.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "" + model.Exchangeform.CurrencyName + " has been added successful for "+model.Exchangeform.WeeklyDate.ToShortDateString()+" .";
                    return RedirectToAction("Index");

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult EditRate(int Id)
        {
            try
            {
                ExchangeRateViewModel model = new ExchangeRateViewModel();
                var getrate = db.ExchangeRate.Where(x => x.Id == Id).FirstOrDefault();
                if (getrate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document format object"));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                model.Exchangeform = new  ExchangeForm();
                model.Exchangeform.CurrencyName = getrate.CurrencyName;
                model.Exchangeform.Buying = getrate.Buying;
                model.Exchangeform.Selling = getrate.Selling;
                model.Exchangeform.WeeklyDate = getrate.WeeklyDate;
                model.Exchangeform.Id = getrate.Id;
                model.PicturePath = Properties.Settings.Default.PhotoPath;
                model.Rate = getrate;
                return View(model);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult EditRate(ExchangeRateViewModel model)
        {
            try
            {
                var GetRate = db.ExchangeRate.Where(x => x.Id == model.Exchangeform.Id).FirstOrDefault();
                if (GetRate == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cant find document format object"));
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                if (model.Exchangeform.Photo != null && model.Exchangeform.Photo.ContentLength > 0)
                {
                    if (GetRate.Logo != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(Properties.Settings.Default.FullPhotoPath + GetRate.Logo);
                        fi.Delete();
                    }

                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);
                    //  List<DocumentInfo> uploadedResume = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.Exchangeform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        //model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Country Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.Exchangeform.Photo.ContentLength > max_upload)
                    {
                        // model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.Exchangeform.Photo.FileName);
                    model.Exchangeform.Photo.SaveAs(ResumePath + filename);

                    #endregion


                    GetRate.CurrencyName = model.Exchangeform.CurrencyName;
                    GetRate.Buying = model.Exchangeform.Buying;
                    GetRate.Selling = model.Exchangeform.Selling;
                    GetRate.ModifiedBy = User.Identity.Name;
                    GetRate.ModifiedDate = DateTime.Now;
                    GetRate.Logo = filename;
                    GetRate.WeeklyDate = model.Exchangeform.WeeklyDate;
                    db.SaveChanges();
                    TempData["message"] = "" + model.Exchangeform.CurrencyName + " has been updated successful for " + model.Exchangeform.WeeklyDate.ToShortDateString() + " .";

                    return RedirectToAction("Index");

                }
                else
                {
                    GetRate.CurrencyName = model.Exchangeform.CurrencyName;
                    GetRate.Buying = model.Exchangeform.Buying;
                    GetRate.Selling = model.Exchangeform.Selling;
                    GetRate.ModifiedBy = User.Identity.Name;
                    GetRate.ModifiedDate = DateTime.Now;
                    GetRate.WeeklyDate = model.Exchangeform.WeeklyDate;
                    db.SaveChanges();
                    TempData["message"] = "" + model.Exchangeform.CurrencyName + " has been updated successful for " + model.Exchangeform.WeeklyDate.ToShortDateString() + " .";

                    return RedirectToAction("Index");

                }
               
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                TempData["messageType"] = "danger";
                return RedirectToAction("Index");
            }
        }
    }
}
