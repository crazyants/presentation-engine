using System;
using System.Collections.Generic;
using System.IO;
using PEngine.Core.Shared;
using Newtonsoft.Json;

namespace PEngine.Core.Web.Models
{
  public class PEngineFileModel
  {
    public string Name { get; set; }
    public double Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    [JsonIgnore]
    public bool Valid { get; set; }
    [JsonIgnore]
    public string FullPath { get; set; }
    public string AbsolutePath
    {
      get
      {
        var rootPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}");
        var relativePath = FullPath.Substring(rootPath.TrimEnd(Path.DirectorySeparatorChar).Length).Replace(Path.DirectorySeparatorChar, '/');
        return $"{Settings.Current.ExternalBaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
      } 
    }
    public string RelativePath
    {
      get
      {
        var rootPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}");
        return $".{FullPath.Substring(rootPath.TrimEnd(Path.DirectorySeparatorChar).Length).Replace(Path.DirectorySeparatorChar, '/')}";
      } 
    }

    public PEngineFileModel(string webFolderPath)
    {
      var relativePath = webFolderPath.Replace('/', Path.DirectorySeparatorChar).TrimStart('/');
      var fullFilePath = $"{Startup.ContentRootPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}{relativePath}";
      if (System.IO.File.Exists(webFolderPath))
      {
        Init(new FileInfo(fullFilePath));
      }
      else
      {
        Valid = false;  
      }
    }

    public PEngineFileModel(FileInfo current)
    {
      Init(current);
    }

    private void Init(FileInfo current)
    {
      FullPath = current.FullName;
      Name = current.Name;
      Size = current.Length;
      Created = current.CreationTimeUtc;
      Modified = current.LastWriteTimeUtc;
      Valid = true;
    }
  }
}