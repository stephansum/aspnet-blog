﻿using System.Web.Mvc;

namespace BlackMesa.Website.Main.Controllers
{
    public class ArchiveController : BaseController
    {
        //
        // GET: /About/

        public ActionResult Index()
        {
//            if (ViewBag.CurrentLanguage == "de")
//                return RedirectToActionPermanent("Details", "Entry", new {Id = 1});
//
//            if (ViewBag.CurrentLanguage == "en")
//                return RedirectToActionPermanent("Details", "Entry", new { Id = 2 });
//
//            return HttpNotFound();

            return View();
        }

    }
}
