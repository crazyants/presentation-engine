using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using PEngine.Core.Shared;
using Markdig;

namespace PEngine.Core.Web.Helpers
{
  public static class Rendering
  {
    private static readonly char[] _bannedChars = { '/', '\\', '?', '!', ';', ':', '\"', '\'', '(', ')', '&', '$', '%', '#', '@', '*', '|', ',', '-' };

    public static string FormatDate(object dateobj)
    {
      return FormatDate(dateobj, false);
    }

    public static string FormatDate(object dateobj, bool skipPresent)
    {
      DateTime tempobj = DateTime.MinValue;
      if (dateobj is DateTime)
      {
        tempobj = (DateTime)dateobj;
      }
      else if (dateobj is string)
      {
        tempobj = DateTime.Parse((string)dateobj);
      }
      if (tempobj != DateTime.MinValue)
      {
        if (tempobj <= DateTime.UtcNow || skipPresent)
        {
          return tempobj.ToString("MM/dd/yyyy") + (tempobj.TimeOfDay.TotalSeconds > 0 ? " " + tempobj.ToString("HH:mm:ss") : string.Empty);
        }
        else
        {
          return "Present";
        }
      }
      else
      {
        return string.Empty;
      }
    }

    public static string MarkupSubheader(string text)
    {
      return MarkupSubheader(text, true);
    }

    public static string MarkupSubheader(string text, bool anchorFlag)
    {
      return MarkupSubheader(text, anchorFlag, anchorFlag ? text.Replace(" ", string.Empty) : string.Empty);
    }

    public static string MarkupSubheader(string text, bool anchorFlag, string anchorName)
    {
      return (anchorFlag ? $"<a name=\"{anchorName}\"></a>" : string.Empty)
        + $"<div class=\"sub-header\">{text}</div>";
    }

    public static string MarkupIcon(string url)
    {
      return !string.IsNullOrEmpty(url)
        ? string.Format($"<img src=\"images/icons/{url}\" class=\"post-icon\" alt=\"Post Icon\" />")
        : string.Empty;
    }

    public static string MarkupMenuButton(string text, string url)
    {
      if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(url))
      {
        return $"<a class=\"menu-button\" href=\"{url}\">{text}</a>";
      }
      else
      {
        return "<div class=\"menu-separator\"></div>";
      }
    }

    public static string MarkupArticle(string secdata, bool forum)
    {
      var pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions();

      if (forum)
      {
        pipeline.DisableHtml();
      }
      return Markdown.ToHtml(secdata, pipeline.Build());
    }

    public static string LogoPath
    {
      get
      {
        return $"images/system/{Settings.Current.LogoFrontPage}";
      }
    }

    public static bool LogoEnabled
    {
      get
      {
        return !string.IsNullOrWhiteSpace(Settings.Current.LogoFrontPage);
      }
    }

    public static string DataTruncate(string data, int length = 75)
    {
      return data.DataTruncate(length);
    }

    public static IEnumerable<string> ThemeList
    {
      get
      {
        var themePath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}");
        if (System.IO.Directory.Exists(themePath))
        {
          return new DirectoryInfo(themePath).GetDirectories()
            .Where(d => System.IO.File.Exists($"{d.FullName}{Path.DirectorySeparatorChar}{d.Name}.css"))
            .Select(d => d.Name);
        }
        return new List<string>();
      }
    }

    public static IEnumerable<string> IconList
    {
      get
      {
        var iconPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}icons{Path.DirectorySeparatorChar}");
        if (System.IO.Directory.Exists(iconPath))
        {
          return new DirectoryInfo(iconPath).GetFiles()
            .Select(f => f.Name);
        }
        return new List<string>();
      }
    }
  }
}