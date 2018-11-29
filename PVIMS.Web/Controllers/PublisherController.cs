using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class PublisherController : BaseController
    {
        private static string CurrentMenuItem = "PublishAdmin";

        private readonly IUnitOfWorkInt _unitOfWork;

        public PublisherController(IUnitOfWorkInt unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Cohort
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeleteMetaPage(int metaPageId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            // Prepare clinical event
            var metaPage = _unitOfWork.Repository<MetaPage>()
                .Queryable()
                .SingleOrDefault(p => p.Id == metaPageId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaPage.metapage_guid;

            if (metaPage == null)
            {
                ViewBag.Entity = "Meta Page";
                return View("NotFound");
            }

            // Prepare model
            var model = new MetaPageDeleteModel
            {
                MetaPageId = metaPage.Id,
                PageName = Server.HtmlDecode(metaPage.PageName)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteMetaPage(MetaPageDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaPage = _unitOfWork.Repository<MetaPage>()
                .Queryable()
                .SingleOrDefault(p => p.Id == model.MetaPageId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaPage.metapage_guid;

            if (metaPage != null)
            {
                if (ModelState.IsValid)
                {
                    var pageInUse = _unitOfWork.Repository<MetaWidget>().Queryable().Any(w => w.MetaPage.Id == model.MetaPageId);

                    if (pageInUse)
                    {
                        ModelState.AddModelError("PageName", "Unable to delete the Page as it is currently in use.");
                    }
                    if(metaPage.IsSystem)
                    {
                        ModelState.AddModelError("PageName", "SYSTEM PAGE. Unable to delete.");
                    }

                    if (ModelState.IsValid)
                    {
                        _unitOfWork.Repository<MetaPage>().Delete(metaPage);
                        _unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Page deleted successfully";
                        Response.Cookies.Add(cookie);

                        return Redirect("/Publisher/PageViewer.aspx?guid=a63e9f29-a22f-43df-87a0-d0c8dec50548");
                    }
                }
            }

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeleteMetaWidget(int metaWidgetId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaWidget = _unitOfWork.Repository<MetaWidget>()
                .Queryable()
                .Include(mw => mw.MetaPage)
                .SingleOrDefault(p => p.Id == metaWidgetId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid;

            if (metaWidget == null)
            {
                ViewBag.Entity = "Meta Widget";
                return View("NotFound");
            }

            // Prepare model
            var model = new MetaWidgetDeleteModel
            {
                MetaWidgetId = metaWidget.Id,
                PageName = Server.HtmlDecode(metaWidget.MetaPage.PageName),
                WidgetName = Server.HtmlDecode(metaWidget.WidgetName),
                WidgetType = metaWidget.WidgetType.Description
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteMetaWidget(MetaWidgetDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaWidget = _unitOfWork.Repository<MetaWidget>()
                .Queryable()
                .Include(mw => mw.MetaPage)
                .SingleOrDefault(p => p.Id == model.MetaWidgetId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid;

            if (metaWidget != null)
            {
                var returnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid.ToString();
                if (ModelState.IsValid)
                {
                    _unitOfWork.Repository<MetaWidget>().Delete(metaWidget);
                    _unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Widget deleted successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect(returnUrl);
                }
            }

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult MoveMetaWidget(int metaWidgetId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaWidget = _unitOfWork.Repository<MetaWidget>()
                .Queryable()
                .Include(mw => mw.MetaPage)
                .SingleOrDefault(p => p.Id == metaWidgetId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid;

            if (metaWidget == null)
            {
                ViewBag.Entity = "Meta Widget";
                return View("NotFound");
            }

            // Prepare model
            var model = new MetaWidgetMoveModel
            {
                MetaWidgetId = metaWidget.Id,
                WidgetName = Server.HtmlDecode(metaWidget.WidgetName),
                CurrentPageName = Server.HtmlDecode(metaWidget.MetaPage.PageName)
            };

            var pages = new List<SelectListItem>();
            pages.AddRange(_unitOfWork.Repository<MetaPage>()
                .Queryable()
                .Where(c => c.Id != metaWidget.MetaPage.Id)
                .OrderBy(c => c.PageName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.PageName
                })
                .ToArray());
            ViewBag.DestinationPages = pages;

            return View(model);
        }

        [HttpPost]
        public ActionResult MoveMetaWidget(MetaWidgetMoveModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaWidget = _unitOfWork.Repository<MetaWidget>()
                .Queryable()
                .Include(mw => mw.MetaPage)
                .Include(mw2 => mw2.WidgetType)
                .SingleOrDefault(p => p.Id == model.MetaWidgetId);
            ViewBag.ReturnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid;

            if (metaWidget != null)
            {
                var returnUrl = "/Publisher/PageViewer.aspx?guid=" + metaWidget.MetaPage.metapage_guid.ToString();
                if (ModelState.IsValid)
                {
                    metaWidget.MetaPage = _unitOfWork.Repository<MetaPage>().Queryable().Single(mp => mp.Id == model.DestinationPageId);
                    _unitOfWork.Repository<MetaWidget>().Update(metaWidget);
                    _unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Widget moved successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect(returnUrl);
                }
            }

            var pages = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            pages.AddRange(_unitOfWork.Repository<MetaPage>()
                .Queryable()
                .Where(c => c.Id != metaWidget.MetaPage.Id)
                .OrderBy(c => c.PageName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.PageName
                })
                .ToArray());
            ViewBag.DestinationPages = pages;

            return View(model);
        }

        [HttpPost]
        public JsonResult AddMetaPage(string pageName, string widgetName)
        {
            var success = "OK";
            var message = "";

            try
            {
                if(String.IsNullOrWhiteSpace(pageName))
                {
                    pageName = "** New Page For Widget **";
                }

                var encodedName = AntiXssEncoder.HtmlEncode(pageName, false);

                var metaPage = new MetaPage { Breadcrumb = string.Empty, IsSystem = false, MetaDefinition = "", PageDefinition = widgetName, PageName = encodedName, metapage_guid = Guid.NewGuid(), IsVisible = false };
                _unitOfWork.Repository<MetaPage>().Save(metaPage);

            }
            catch (Exception ex)
            {
                success = "FAILED";
                message = ex.Message;
            }

            var result = new { Success = success, Message = message };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}