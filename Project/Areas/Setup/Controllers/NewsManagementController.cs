using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;



namespace Project.Areas.Setup.Controllers
{
    public class NewsManagementController : Controller
    {
        //
        // GET: /Setup/NewsManagement/
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;
        public NewsManagementController()
        {
            this.util = new ProcessUtility();
            this.filePath = Properties.Settings.Default.PhotoPath;
        }


        public ActionResult ReOrderList()
        {
            try
            {
                var rowsToShow = db.PreOrder.ToList();
                var viewModel = new NewsManagementViewModel
                {
                    preorder = rowsToShow.OrderByDescending(x => x.OrderDate).ToList(),
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.News.ToList();
                var viewModel = new NewsManagementViewModel
                {
                    Rows = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList(),
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult Create()
        {
            try
            {
                NewsManagementViewModel model = new NewsManagementViewModel();
                //model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        //      
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewsManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var validate = (from m in db.News where m.NewsHeadline == model.newsform.NewsHeadline select m).ToList();
                    if (validate.Any())
                    {                      
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Headline" + model.newsform.NewsHeadline + " already exist. Please try different headline";
                        return View(model);
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
                    var fileResume = System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        //model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for News Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.newsform.Photo.ContentLength > max_upload)
                    {
                       // model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    model.newsform.Photo.SaveAs(ResumePath + filename);
                 
                    #endregion
                    //add to news table 
                    
                        News add = new News
                        {
                            NewsHeadline = model.newsform.NewsHeadline,
                            NewsContent = model.newsform.NewsContent,                         
                            Photo = filename,
                            CreatedBy = User.Identity.Name,                            
                            CreatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                            IsDeleted = model.newsform.IsDeleted,
                            IsPublished = model.newsform.IsPublished,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            NoOfView = 0
                        };
                        db.News.AddObject(add);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully created";
                        return RedirectToAction("Index");
                  
                  

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



        public ActionResult Edit(int Id)
        {
            try
            {
                NewsManagementViewModel model = new NewsManagementViewModel();             
                var GetNews = db.News.Where(x => x.Id == Id).FirstOrDefault();
                model.news = GetNews;
                model.newsform = new  NewsForm();               
                model.newsform.NewsHeadline = model.news.NewsHeadline;              
                model.newsform.NewsContent = model.news.NewsContent;              
                model.newsform.IsPublished = model.news.IsPublished;
                model.newsform.IsDeleted = model.news.IsDeleted;
                model.newsform.Id = Id;

                model.PicturePath = Properties.Settings.Default.PhotoPath;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewsManagementViewModel model)
        {
            try
            {
                var GetNews = db.News.Where(x => x.Id == model.newsform.Id).FirstOrDefault();             
                model.news = GetNews;
                if (model.newsform.Photo != null && model.newsform.Photo.ContentLength > 0)
                {
                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(this.filePath);
                  
                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for News Photo ";
                        return View(model);

                    }
                    else if (model.newsform.Photo.ContentLength > max_upload)
                    {

                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    model.newsform.Photo.SaveAs(ResumePath + filename);
                    
                    #endregion

                    GetNews.NewsHeadline = model.newsform.NewsHeadline;                   
                    GetNews.NewsContent = model.newsform.NewsContent;
                    GetNews.IsPublished = model.newsform.IsPublished;
                    GetNews.IsDeleted = model.newsform.IsDeleted;
                    GetNews.Photo = filename;
                    GetNews.ModifiedBy = User.Identity.Name;
                    GetNews.ModifiedDate = DateTime.Now;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    GetNews.NewsHeadline = model.newsform.NewsHeadline;                   
                    GetNews.NewsContent = model.newsform.NewsContent;
                    GetNews.IsPublished = model.newsform.IsPublished;
                    GetNews.IsDeleted = model.newsform.IsDeleted;
                    GetNews.ModifiedBy = User.Identity.Name;
                    GetNews.ModifiedDate = DateTime.Now;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }


    }
}
