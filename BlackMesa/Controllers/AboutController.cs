﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlackMesa.Controllers
{
    public class AboutController : BaseController
    {
        //
        // GET: /About/

        public ActionResult Index()
        {
            return RedirectToActionPermanent("Details", "Entry", new {Id = 1});
        }

    }
}
