using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PEngine.Core.Shared;

namespace PEngine.Core.Web.Controllers
{
  [Route("[controller]")]
  public class HashController : Controller
  {    
    public HashController()
    {
    }

    [HttpGet("{*hashedPath}")]
    public IActionResult GetHashedFileName(string hashedPath)
    {
      Console.WriteLine($"Hash File Request: {hashedPath}");
      string[] elements = hashedPath.Split('.');
      if (elements.Length >= 3)
      {
        string originalPath = null;
        switch (elements.Last().ToLower())
        {
          //Certain file types are assumed to be exempt from hash mapping, including .map files
          case "map":
            originalPath = hashedPath;
            break;
          default:
            originalPath = String.Join(".", elements.Where((e, i) => i != elements.Length - 2));
            break;
        }
        string originalFullPath = System.IO.Path.Combine(Startup.ContentRootPath, "wwwroot", originalPath);
        if (System.IO.File.Exists(originalFullPath))
        {
          string contentType;
          new FileExtensionContentTypeProvider().TryGetContentType(originalFullPath, out contentType);
          return File(System.IO.File.OpenRead(originalFullPath), contentType ?? "application/octet-stream");
        }
        return NotFound();
      }
      return BadRequest();
    }
  }
}