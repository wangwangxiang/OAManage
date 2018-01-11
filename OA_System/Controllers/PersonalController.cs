using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OA_System.Controllers
{
    public class PersonalController : Controller
    {
        //
        // GET: /Personal/
        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {
            UserInfo u = Session["user"] as UserInfo;

            UserInfo user = new UserInfo();

            user = oai.UserInfo.Where(a => a.ID ==u.ID).FirstOrDefault();
            //用model的方式传值
            return View(user);
        }

    }
}
