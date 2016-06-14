using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("tag")]
    [Authorize(Roles = "admin, editor")]
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController() : this(new TagRepository()) { }

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        [Route("")]
        
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            return View(tags);
        }

        [HttpGet]
        [Route("edit/{tag}")]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException) { return HttpNotFound(); }
        }

        [HttpPost]
        [Route("edit/{tag}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string tag, string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");
                return View(model: tag);
            }

            var tags = _repository.GetAll();

            newTag = newTag.MakeUrlFriendly();

            if (!tags.Contains(tag)) return HttpNotFound();
            if (tags.Contains(newTag)) return RedirectToAction("index");

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("delete/{tag}")]
        public ActionResult Delete(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException) { return HttpNotFound(); }
        }

        // added foo because the post signature needs to differ from get...!?!?
        [HttpPost]
        [Route("delete/{tag}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag, string foo)
        {
            try
            {
                _repository.Delete(tag);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException) { return HttpNotFound(); }
        }
    }
}