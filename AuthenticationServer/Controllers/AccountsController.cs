using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Controllers
{
    public class AccountsController : Controller
    {
        [HttpGet]
        public  IActionResult Login()
        {
            return View();
        }
    }
}
