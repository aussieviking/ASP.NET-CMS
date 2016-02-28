using MvcCms.Data;
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
        private readonly IPostRepository _repository;

        public PostController() : this(new PostRepository()) { }

        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Post
        [Route("")]
        public ActionResult Index()
        {
            var posts = _repository.GetAll();
            return View(posts);
        }

        // /admin/post/create
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new Post());
        }

        // /admin/post/create
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post model)
        {
            if (!ModelState.IsValid) return View(model);

            if (String.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();

            model.Created = DateTime.Now;

            model.AuthorId = "c57b52dc-7088-4b1f-a20f-c4e38399f335";

            try
            {
                _repository.Create(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View(model);
            }

            return RedirectToAction("index");
        }

        // /admin/post/edit/<postid>
        [HttpGet]
        [Route("edit/{postId}")]
        public ActionResult Edit(string postId)
        {
            var post = _repository.Get(postId);
            if (post == null) return HttpNotFound();

            return View(post);
        }

        // /admin/post/edit/<postid>
        [HttpPost]
        [Route("edit/{postId}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string postId, Post model)
        {
            if (!ModelState.IsValid) return View(model);

            if (String.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();

            try
            {
                _repository.Edit(postId, model);
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View(model);
            }

            return RedirectToAction("index");
        }

        // /admin/post/delete/<postid>
        [HttpGet]
        [Route("delete/{postId}")]
        public ActionResult Delete(string postId)
        {
            var post = _repository.Get(postId);
            if (post == null) return HttpNotFound();

            return View(post);
        }

        // /admin/post/delete/<postid>
        [HttpPost]
        [Route("delete/{postId}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string postId, string foo)
        {
            try
            {
                _repository.Delete(postId);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
        }
    }
}