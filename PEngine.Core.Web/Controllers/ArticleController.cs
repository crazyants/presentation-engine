using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Models;

namespace PEngine.Core.Web.Controllers
{
  [Route("article")]
  public class ArticleController : Controller
  {
    private IArticleDal _articleDal;
    private IArticleService _articleService;

    public ArticleController(IArticleDal articleDal, IArticleService articleService)
    {
      _articleDal = articleDal;
      _articleService = articleService;
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> Category(string category, [FromQuery]PagingModel paging = null)
    {
      var model = new PEngineGenericListModel<ArticleModel>(HttpContext, false);
      if (paging != null)
      {
        paging.Count = paging.Count > 0 ? paging.Count : model.Settings.PerPagePostArchived;
        if (string.IsNullOrEmpty(paging.SortField))
        {
          paging.SortField = "Name";
          paging.SortAscending = false;
        }
      }
      var articles = (await _articleService.ListArticles(category, model.State.HasAdmin)).ToList();
      if (!articles.Any())
      {
        return this.NotFound();
      }
      if (articles.Count > 1 || model.State.HasAdmin)
      {
        model.ListData = PagingUtils.Paginate<ArticleModel>(ref paging, articles);
        model.Paging = paging;
        return View("List", model);
      }
      else
      {
        return Redirect($"/article/view/{articles.First().UniqueName}");
      }
    }

    [HttpGet("view/{uniqueName}")]
    public async Task<IActionResult> ViewArticle(string uniqueName)
    {
      return await ViewArticleSection(uniqueName, null);
    }

    [HttpGet("view/{uniqueName}/{sectionUniqueName}")]
    public async Task<IActionResult> ViewArticleSection(string uniqueName, string sectionUniqueName)
    {
      var model = new PEngineGenericRecordModel<ArticleModel>(HttpContext, false, false, null, sectionUniqueName);
      var article = await _articleService.GetArticleById(null, null, uniqueName, model.State.HasAdmin);
      if (article == null || article.Guid == Guid.Empty)
      {
        return this.NotFound();
      }
      model.RecordData = article;

      if (!string.IsNullOrWhiteSpace(sectionUniqueName) && !model.State.CurrentSection.Equals(sectionUniqueName))
      {
        return this.NotFound();
      }
      
      return View("View", model);
    }

    [HttpPost("view/{uniqueName}")]
    public IActionResult SwitchArticleSection(string uniqueName, [FromForm] string newSectionUniqueName)
    {
      return Redirect($"/article/view/{uniqueName}/{newSectionUniqueName}");
    }

    [HttpPost("view/{uniqueName}/{sectionUniqueName}")]
    public IActionResult SwitchArticleSection(string uniqueName, string sectionUniqueName, [FromForm] string newSectionUniqueName)
    {
      return Redirect($"/article/view/{uniqueName}/{newSectionUniqueName}");
    }
  }
}