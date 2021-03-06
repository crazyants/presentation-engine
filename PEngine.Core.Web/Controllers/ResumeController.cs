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
  public class ResumeController : Controller
  {
    private IResumeDal _resumeDal;
    private IResumeService _resumeService;

    public ResumeController(IResumeDal resumeDal, IResumeService resumeService)
    {
      _resumeDal = resumeDal;
      _resumeService = resumeService;
    }

    public async Task<IActionResult> Index()
    {
      var model = new PEngineGenericRecordModel<ResumeModel>(HttpContext, false);
      model.RecordData = await _resumeService.GetResume();
      return View(model);
    }
  }
}