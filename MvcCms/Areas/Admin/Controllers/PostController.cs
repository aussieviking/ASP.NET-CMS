using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    // /admin/post

    [RouteArea("Admin")]
    [RoutePrefix("post")]
    public class PostController : Controller
    {
        // GET: Admin/Post
        public ActionResult Index()
        {
            return View();
        }

        // /admin/post/create
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            var model = new Post();

            return View(model);
        }

        // /admin/post/create
        [HttpPost]
        [Route("create")]
        public ActionResult Create(Post model)
        {
            if (!ModelState.IsValid) return View(model);

            // TODO: Create model in data store

            return RedirectToAction("index");
        }

        // /admin/post/edit/<postid>
        [HttpGet]
        [Route("create/{id}")]
        public ActionResult Edit(string id)
        {
            // TODO: retrieve model from the datastore
            var model = new Post();

            return View(model);
        }

        // /admin/post/edit/<postid>
        [HttpPost]
        [Route("edit/{id}")]
        public ActionResult Edit(Post model)
        {
            if (!ModelState.IsValid) return View(model);

            // TODO: Update model in data store

            return RedirectToAction("index");
        }
    }
}