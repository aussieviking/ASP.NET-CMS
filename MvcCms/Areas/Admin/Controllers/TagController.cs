﻿using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;
        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            return View(tags);
        }

        [HttpGet]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model);
            }
            catch (KeyNotFoundException e) { return HttpNotFound(); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string tag, string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");
                return View(tag);
            }

            var tags = _repository.GetAll();

            if (!tags.Contains(tag)) return HttpNotFound();
            if (tags.Contains(newTag)) return RedirectToAction("index");

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model);
            }
            catch (KeyNotFoundException e) { return HttpNotFound(); }
        }

        // added foo because the post signature needs to differ from get...!?!?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag, bool foo)
        {
            try
            {
                _repository.Delete(tag);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e) { return HttpNotFound(); }
        }
    }
}