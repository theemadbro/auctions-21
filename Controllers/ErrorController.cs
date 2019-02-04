using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace beltfix.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Default()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
    
}