using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Extensions;

namespace PEngine.Core.Web.Models
{
  public class PEngineSearchResultModel : ISubTitleModel
  {
    public string Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public string Creator { get; set; }
    public string Link { get; set; }

    public PEngineSearchResultModel(PostModel post)
    {
      Type = "Post";
      Title = post.Name;
      Content = Helpers.Rendering.DataTruncate(post.Data, 75);
      CreatedUTC = post.CreatedUTC;
      Creator = "Admin";
      Link = $"/post/view/{post.CreatedYear}/{post.CreatedMonth}/{post.UniqueName}";
    }

    public PEngineSearchResultModel(ArticleModel article)
    {
      Type = "Article";
      Title = article.Name;
      Content = Helpers.Rendering.DataTruncate(article.Description, 75);
      CreatedUTC = article.CreatedUTC;
      Creator = "Admin";
      Link = $"/article/view/{article.UniqueName}";
    }

    public PEngineSearchResultModel(ForumThreadPostModel forumThreadPost)
    {
      Type = "Forum";
      Title = forumThreadPost.ForumThreadName;
      Content = Helpers.Rendering.DataTruncate(forumThreadPost.Data, 75);
      CreatedUTC = forumThreadPost.CreatedUTC;
      Creator = forumThreadPost.ForumUserId;
      Link = $"/forum/thread/{forumThreadPost.ForumUniqueName}/{forumThreadPost.ForumThreadUniqueName}";
    }

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      if (currentPage.HasValue)
      {
        return $"Search Results for \"{currentSection}\" - Page {currentPage.Value}";
      }
      else
      {
        return $"Search Results for \"{currentSection}\"";
      }
    }
  }
}