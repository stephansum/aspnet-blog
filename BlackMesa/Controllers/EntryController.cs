﻿using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BlackMesa.Models;
using BlackMesa.Utilities;
using BlackMesa.ViewModels;
using PagedList;

namespace BlackMesa.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EntryController : BaseController
    {
        private readonly BlackMesaDb _db = new BlackMesaDb();

        [AllowAnonymous]
        public ActionResult Index(EntryIndexViewModel viewModel)
        {
            var language = ViewBag.CurrentLanguage as string;
            var model = User.IsInRole("Admin") ? _db.Entries.Where(e => e.Language == language) : _db.Entries.Where(e => e.Published && e.Language == language);

            // Filter
            var selectedTags = viewModel.SelectedTags;

            if (!String.IsNullOrEmpty(selectedTags))
            {
                var selectedTagList = selectedTags.Split(',').ToList();

                model = model.Where(e => selectedTagList.All(tagString => (e.Tags.Select(t => t.Name).Contains(tagString))));
            }
            /* SelectedTags has to be stored in the ViewBag to make it accessible in a PartialView */
            ViewBag.SelectedTags = viewModel.SelectedTags;
            ViewBag.EntriesFound = model.Count();


            // Order
            switch (viewModel.OrderBy)
            {
                case "date":
                    model = model.OrderByDescending(e => e.DateCreated);
                    break;
                case "comments":
                    model = model.OrderByDescending(e => e.Comments.Count);
                    break;
                case "views":
                    model = model.OrderByDescending(e => e.Comments.Count);
                    break;
                default:
                    model = model.OrderByDescending(e => e.DateCreated);
                    break;
            }
            ViewBag.OrderBy = viewModel.OrderBy; 


            // Paging
            const int pageSize = 3;
            ViewBag.PageSize = pageSize;
            var pageNumber = (viewModel.Page ?? 1);
            viewModel.Page = pageNumber;
            viewModel.Entries = model.ToPagedList(pageNumber, pageSize);


            if (Request.IsAjaxRequest())
                return PartialView(viewModel);

            return View(viewModel);


        }


        public ActionResult ReparseAllEntries()
        {
            var entries = _db.Entries;

            foreach (var entry in entries)
            {
                ParseEntry(entry);
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        private void ParseEntry(Entry entry)
        {
            // Read entry.Title and entry.Preview
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(entry.Content);

            if (htmlDoc.DocumentNode != null)
            {
                var headerNode = htmlDoc.DocumentNode.SelectSingleNode("article/header[1]");
                if (headerNode != null)
                {
                    var headerNodeHeading = headerNode.SelectSingleNode("h1");
                    if (headerNodeHeading != null)
                    {
                        entry.Title = headerNodeHeading.InnerHtml;
                        headerNode.RemoveChild(headerNodeHeading);

                        headerNode.SelectSingleNode("p[last()]").SetAttributeValue("class", "last-paragraph");
                        entry.Preview = headerNode.InnerHtml;
                    }
                }
            }


            // Read entry.Body
            var htmlDoc2 = new HtmlAgilityPack.HtmlDocument();
            htmlDoc2.LoadHtml(entry.Content);

            if (htmlDoc2.DocumentNode != null)
            {
                var headerNode = htmlDoc2.DocumentNode.SelectSingleNode("article/header[1]");
                if (headerNode != null)
                {
                    var headerNodeHeading = headerNode.SelectSingleNode("h1");
                    if (headerNodeHeading != null)
                    {
                        headerNode.RemoveChild(headerNodeHeading);

                        entry.Body = headerNode.OuterHtml;
                    }
                }


                var sectionNodes = htmlDoc2.DocumentNode.SelectNodes("article/section");

                if (sectionNodes != null)
                {
                    foreach (var sectionNode in sectionNodes)
                    {
                        entry.Body += sectionNode.OuterHtml;
                    }
                }
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(string title, int id = 0)
        {
            Entry entry = _db.Entries.Find(id);

            if (entry == null || (!entry.Published && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            /* if the title passed with the http-request doesnt equal the current entry title - we response with a moved permanent code to 
             * inform the search engines */

            var currentSeoFriendlyTitle = Utilities.Utilities.MakeUrlFriendly(entry.Title);
            if (title != currentSeoFriendlyTitle)
                return RedirectToActionPermanent("Details", new { Id = id, Title = currentSeoFriendlyTitle });

            return View(entry);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Details(Comment comment)
        {
            comment.DateCreated = DateTime.Now;
            comment.DateEdited = DateTime.Now;

            if (ModelState.IsValid)
            {
                _db.Comments.Add(comment);
                _db.SaveChanges();
                return RedirectToAction("Details", "Entry", new { Id = comment.EntryId });
            }

            return View("Details", _db.Entries.Single(c => c.Id == comment.EntryId));
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Entry entry)
        {
            ParseEntry(entry);

            if (entry.Title != null)
                ModelState["Title"].Errors.Clear();

            if (entry.Preview != null)
                ModelState["Preview"].Errors.Clear();

            if (entry.Body != null)
                ModelState["Body"].Errors.Clear();


            if (ModelState.IsValid)
            {
                var selectedTags = HttpContext.Request.Form["SelectedTags"];
                if (!String.IsNullOrEmpty(selectedTags))
                {
                    var selectedTagsList = selectedTags.Split(',').ToList();

                    entry.Tags = new Collection<Tag>();
                    foreach (var tag in selectedTagsList)
                    {
                        if (!_db.Tags.Where(t => t.Language == entry.Language).Select(t => t.Name).Contains(tag))
                        {
                            var newTag = new Tag { Name = tag, Language = entry.Language };
                            _db.Tags.Add(newTag);
                            entry.Tags.Add(newTag);
                        }
                        else
                        {
                            entry.Tags.Add(_db.Tags.Single(t => t.Name == tag && t.Language == entry.Language));
                        }
                    }
                }
                _db.Entries.Add(entry);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entry);
        }


        public ActionResult Edit(int id = 0)
        {
            Entry entry = _db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            var selectedTags = string.Join(",", entry.Tags.Select(t => t.Name));
            ViewBag.SelectedTags = selectedTags;
            return View(entry);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Entry entry)
        {
            ParseEntry(entry);

            if (entry.Title != null)
                ModelState["Title"].Errors.Clear();

            if (entry.Preview != null)
                ModelState["Preview"].Errors.Clear();

            if (entry.Body != null)
                ModelState["Body"].Errors.Clear();


            if (ModelState.IsValid)
            {
                var selectedTags = HttpContext.Request.Form["SelectedTags"];

                var dbEntry = _db.Entries.Find(entry.Id);

//                TryUpdateModel(dbEntry);  // tries to map the new values from the modelbinded entry to the passed model - this is the lazy way to go, instead of manually mapping all the properties

                dbEntry.Title = entry.Title;
                dbEntry.Preview = entry.Preview;
                dbEntry.Body = entry.Body;
                dbEntry.Content = entry.Content;
                dbEntry.DateCreated = entry.DateCreated;
                dbEntry.DateEdited = entry.DateEdited;
                dbEntry.Published = entry.Published;
                dbEntry.Language = entry.Language;

                dbEntry.Tags.Clear();

                if (!String.IsNullOrEmpty(selectedTags))
                {
                    var selectedTagsList = selectedTags.Split(',').ToList();

                    foreach (var tag in selectedTagsList)
                    {
                        if (!_db.Tags.Where(t => t.Language == entry.Language).Select(t => t.Name).Contains(tag))
                        {
                            var newTag = new Tag { Name = tag, Language = entry.Language };
                            _db.Tags.Add(newTag);
                            dbEntry.Tags.Add(newTag);
                        }
                        else
                        {
                            dbEntry.Tags.Add(_db.Tags.Single(t => t.Name == tag && t.Language == entry.Language));
                        }
                    }
                }
//                _db.Entry(dbEntry).State = EntityState.Modified; // not necessary cause dbEntry is tracked by the dbContext
                _db.SaveChanges();

                DeleteTagsWithNoEntries();

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entry);
        }


        private void DeleteTagsWithNoEntries()
        {
            var tagsToDelete = _db.Tags.Where(tag => tag.Entries.Count == 0);
            foreach (var tag in tagsToDelete)
            {
                _db.Tags.Remove(tag);
            }
        }


        public ActionResult Delete(int id = 0)
        {
            Entry entry = _db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entry entry = _db.Entries.Find(id);
            _db.Entries.Remove(entry);
            _db.SaveChanges();

            DeleteTagsWithNoEntries();
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}