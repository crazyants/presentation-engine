﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private IArticleDal _articleDal;
        private IArticleService _articleService;
        public ArticlesController(IArticleDal articleDal, IArticleService articleService)
        {
          _articleDal = articleDal;
          _articleService = articleService;
        }

        [HttpGet]
        public IEnumerable<ArticleModel> Get()
        {
          return _articleDal.ListArticles();
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
          return this.Ok(_articleDal.GetArticleById(guid, null, null));
        }

        [HttpPost]
        public IActionResult InsertArticle([FromBody]ArticleModel article)
        {
          var errors = new List<string>();
          if (_articleService.UpsertArticle(article, ref errors))
          {
            return this.Ok(article);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [HttpPut]
        public IActionResult UpdateArticle([FromBody]ArticleModel article)
        {
          return InsertArticle(article);
        }

        [HttpDelete("{guid}")]
        public IActionResult DeleteArticle(Guid guid)
        {
          var errors = new List<string>();
          _articleDal.DeleteArticle(guid);
          return this.Ok();
        }
    }
}