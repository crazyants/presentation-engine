using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Extensions;

namespace PEngine.Core.Web.Models
{
  public class PEngineStateModel
  {
    public const string COOKIE_ELITE = "elite";
    public const string COOKIE_THEME = "theme";
    public const string COOKIE_ACCESS_TOKEN = "access_token";

    private SettingsData _settings;
    private HttpContext _context;
    private ISubTitleModel _viewDataRecord;
    private IEnumerable<ISubTitleModel> _viewDataList;
    private bool _viewDataInList;

    public string Url { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool HideSubTitle { get; set; }
    public string FullTitle
    {
      get
      {
        if (!HideSubTitle && !string.IsNullOrEmpty(SubTitle))
        {
          return $"{Title} - {SubTitle}";
        }
        return Title;
      }
    }
    public List<KeyValuePair<string, string>> TopMenuButtons { get; set; }
    public List<KeyValuePair<string, string>> SubMenuButtons { get; set; }
    public bool HasAdmin { get; set; }
    public bool HasForumAdmin { get; set; }
    public string PEngineUserName { get; set; }
    public string PEngineUserType { get; set; }
    public string CurrentSection { get; set; }
    public int? CurrentPage { get; set; }
    public bool IsElite { get; set; }
    public bool IsForum { get; set; }
    public string Theme { get; set; }
    public string ThemePath
    {
      get
      {
        return $"themes/{Theme}/{Theme}.css";
      }
    }
    public string SummaryTitle { get; set; }
    public string SummaryDescription { get; set; }
    public string SummaryUrl { get; set; }
    public string SummaryImage { get; set; }
    public string SummarySite { get; set; }
    public bool HasSummary { get; set; }

    public PEngineStateModel(SettingsData settings, HttpContext context, bool hideSubTitle = false, bool isForum = false, ISubTitleModel viewDataRecord = null, string currentSection = null, int? currentPage = null)
    {
      _settings = settings;
      _context = context;
      HideSubTitle = hideSubTitle;
      IsForum = isForum;
      CurrentSection = currentSection;
      CurrentPage = currentPage;

      UpdateData(viewDataRecord);
    }

    public PEngineStateModel(SettingsData settings, HttpContext context, bool hideSubTitle = false, bool isForum = false, IEnumerable<ISubTitleModel> viewDataList = null, string currentSection = null, int? currentPage = null)
    {
      _settings = settings;
      _context = context;
      HideSubTitle = hideSubTitle;
      IsForum = isForum;
      CurrentSection = currentSection;
      CurrentPage = currentPage;

      UpdateData(viewDataList);
    }

    public void UpdateData(ISubTitleModel viewDataRecord)
    {
      _viewDataRecord = viewDataRecord;
      _viewDataList = null;
      _viewDataInList = false;

      Init();
    }

    public void UpdateData(IEnumerable<ISubTitleModel> viewDataList)
    {
      _viewDataList = viewDataList;
      _viewDataRecord = _viewDataList?.FirstOrDefault();
      _viewDataInList = true;

      Init();
    }

    public void Init()
    {
      Url = _context.Request.Path;
      Title = _settings.DefaultTitle;
      HasAdmin = false;
      HasForumAdmin = false;
      PEngineUserName = string.Empty;
      PEngineUserType = "Anonymous";
      Theme = string.Empty;
      var themeList = Helpers.Rendering.ThemeList;
      if (themeList.Any() && !themeList.Any(t => t.Equals(_settings.DefaultTheme, StringComparison.OrdinalIgnoreCase)))
      {
        Theme = themeList.First();
      }
      
      //Process Cookies
      if (_context.Request?.Cookies != null)
      {
        var requestedTheme = _context.Request.Cookies.ContainsKey(COOKIE_THEME) ? _context.Request.Cookies[COOKIE_THEME] : Theme;
        if (themeList.Any(t => t.Equals(requestedTheme, StringComparison.OrdinalIgnoreCase)))
        {
          Theme = requestedTheme;
        }
        IsElite = _context.Request.Cookies.ContainsKey(COOKIE_ELITE);
      }

      //Process Authentication
      if (_context.Request?.HttpContext?.User != null && _context.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        HasAdmin = _context.Request.HttpContext.User.IsInRole("PEngineAdmin");
        HasForumAdmin = _context.Request.HttpContext.User.IsInRole("ForumAdmin");
        PEngineUserName = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
        PEngineUserType = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;
      }

      //Process Record
      SubTitle = null;
      TopMenuButtons = new List<KeyValuePair<string, string>>();
      SubMenuButtons = new List<KeyValuePair<string, string>>();

      TopMenuButtons.Add(new KeyValuePair<string, string>(_settings.LabelHomeButton, "/"));
      if (!IsForum && !_settings.DisableResume)
      {
        TopMenuButtons.Add(new KeyValuePair<string, string>(_settings.LabelResumeButton, "/resume"));
      }
      if (!_settings.DisableForum)
      {
        TopMenuButtons.Add(new KeyValuePair<string, string>(_settings.LabelForumButton, "/forum"));
      }

      if (!IsForum)
      {
        var articleDal = Startup.ServiceProvider.GetRequiredService<IArticleDal>();
        var articleCategories = articleDal.ListArticles(null).Result
          .Where(a => a.VisibleFlag || HasAdmin)
          .Select(a => $"{a.Category}|{a.ContentURL}")
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .OrderBy(a => a);

        foreach (var articleCategory in articleCategories)
        {
          var categoryElements = articleCategory.Split('|');
          var categoryUrl = $"/article/category/{categoryElements[0]}";
          if (!string.IsNullOrWhiteSpace(categoryElements[1]))
          {
            categoryUrl = categoryElements[1];
          }
          Console.WriteLine($"Adding Top Button for {categoryElements[0]} to {categoryUrl}");
          TopMenuButtons.Add(new KeyValuePair<string, string>(categoryElements[0], categoryUrl));
        }
      }

      var requestUri = new Uri(_context.Request.GetDisplayUrl());
      SummaryTitle = string.Empty;
      SummaryDescription = string.Empty;
      SummaryUrl = $"{_settings.ExternalBaseUrl.TrimEnd('/')}{requestUri.PathAndQuery}";
      SummarySite = _settings.DefaultTitle;
      SummaryImage = $"{_settings.ExternalBaseUrl.TrimEnd('/')}/images/system/{_settings.LogoFrontPage}";

      if (_viewDataRecord != null)
      {
        SubTitle = _viewDataRecord.GetSubTitle(_viewDataInList, CurrentSection, CurrentPage);
        
        if (!_viewDataInList && _viewDataRecord is ArticleModel)
        {
          var articleData = (ArticleModel)_viewDataRecord;
          if (string.IsNullOrWhiteSpace(articleData.ContentURL))
          {
            var displayedSection = (CurrentSection ?? articleData.DefaultSection);
            var currentSectionData = (displayedSection != null) 
              ? articleData.Sections.FirstOrDefault(s => s.UniqueName.Equals(displayedSection, StringComparison.OrdinalIgnoreCase))
              : null;
            var articleSections = articleData.Sections.OrderBy(s => s.SortOrder);
            currentSectionData = currentSectionData ?? articleSections.First();
            CurrentSection = currentSectionData.UniqueName;
            foreach (var section in articleData.Sections)
            {
              SubMenuButtons.Add(new KeyValuePair<string, string>(section.Name, $"/article/view/{articleData.UniqueName}/{section.UniqueName}"));
            }
            SummaryTitle = !HideSubTitle ? SubTitle : _settings.DefaultTitle;
            SummaryDescription = Helpers.Rendering.DataTruncate(currentSectionData?.Data ?? articleData.Description, -1);
          }
        }

        if (!_viewDataInList && _viewDataRecord is PostModel)
        {
          var postData = (PostModel)_viewDataRecord;
          SummaryTitle = !HideSubTitle ? SubTitle : _settings.DefaultTitle;
          SummaryDescription = Helpers.Rendering.DataTruncate(postData.Data, -1);
        }
      }

      HasSummary = !String.IsNullOrWhiteSpace(SummaryTitle);
    }

    public void EliteToggle()
    {
      if (_context.Request != null)
      {
        if (_context.Request.Cookies.ContainsKey(COOKIE_ELITE))
        {
          _context.Response.Cookies.Delete(COOKIE_ELITE);
        }
        else
        {
          _context.Response.Cookies.Append(COOKIE_ELITE, "1");
        }
        IsElite = !IsElite;
      }
    }

    public void ThemeChange(string requestedTheme)
    {
      if (_context.Request != null)
      {
        var themeList = Helpers.Rendering.ThemeList;
        if (themeList.Any(t => t.Equals(requestedTheme, StringComparison.OrdinalIgnoreCase)))
        {
          Theme = requestedTheme;
          _context.Response.Cookies.Append(COOKIE_THEME, requestedTheme);
        }
      }
    }

    public void LogOut()
    {

    }
  }
}