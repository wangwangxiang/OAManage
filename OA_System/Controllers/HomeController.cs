
using OA_System.DTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OA_System.Controllers
{
    
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        /// <summary>
        /// 登录起始页
        /// </summary>
        /// <returns></returns>

        public ActionResult Login()
        {
            //加载时直接记住密码
            HttpCookie getCookie = Request.Cookies["userInfo"];

            if (getCookie != null)
            {
                //取出对象里面的东西

                ViewBag.userName = getCookie.Values["userName"];
                ViewBag.userPwd = getCookie.Values["userPwd"];
                ViewBag.userCode = "checked";

            }

         
            return View();
        }

        /// <summary>
        /// 验证登陆
        /// </summary>
          OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult ExeLogin() 
        {
            //调用记住密码方法
            string userName = Request["UserName"];
            string userPwd = Request["PassWord"];
            string userCode = Request["CodePwd"];
            string chose = Request["online"];
            resiger(userName,userPwd,chose);

           //获取系统验证码
            string Code = Session["codeNum"].ToString();

            //根据条件查询存在的用户信息
            UserInfo u = oai.UserInfo.Where(a => ((a.LoginName == userName) || string.IsNullOrEmpty(a.LoginName)) && ((a.UserPwd == userPwd) || string.IsNullOrEmpty(a.UserPwd))).FirstOrDefault();
            if (Code.ToLower() == userCode.ToLower())
            {
                if (u != null)
                {
                    Session.Timeout = 30000;
                    Session["user"] = u;
                    //return Json(new { message = "success", data = u }, JsonRequestBehavior.AllowGet);
                    return Redirect("/Home/Index");

                }
            }
            else
            {
                return Redirect("/Home");
            }

            return View();
            //return Json(new { message = "failure", data = "" }, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        /// 记住密码
        /// </summary>
        public void resiger(string userName, string userPwd, string chose)
        {
            //保存输入的状态

            if (chose == "on")
            {
                HttpCookie hc = new HttpCookie("userInfo");
                hc.Values["userName"] = userName;
                hc.Values["userPwd"] = userPwd;

                //保存到cookies中

                Response.Cookies.Add(hc);

                //设置有效时间

                hc.Expires = DateTime.Now.AddDays(2);

            }
            else
            {
                HttpCookie getCookie = Request.Cookies["userInfo"];
                if (getCookie != null)
                {
                    Response.Cookies["userInfo"].Expires = DateTime.Now.AddHours(-1);
                }


            }
        }

        /// <summary>
        /// 登陆成功的首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<PremissAsUser> plist = new List<PremissAsUser>();
            List<PremissAsUser> puList = new List<PremissAsUser>();
            HUMessage hm = new HUMessage();
            //取出登录人的信息

            UserInfo u = Session["user"] as UserInfo;

            //根据用户编号查询对应的权限


            puList = oai.Database.SqlQuery<PremissAsUser>("select * from PremissAsUser where PremissId in (select fatherId from PermissionsInfo where UserId='"+u.UserNum+"')").ToList();


            //查询职位
            hm.ZhiWei = oai.PositionInfo.Where(a => a.PositionId == u.ZhiWeiId).Select(b => b.PositionName).FirstOrDefault();

            //保存在viewBag中
            foreach (var item in puList)
            {
                if (oai.PremissAsUser.Where(a => a.PremissId == item.PremissId && a.ParenId == null).FirstOrDefault() != null)
                {
                    plist.Add(oai.PremissAsUser.Where(a => a.PremissId == item.PremissId && a.ParenId == null).FirstOrDefault());
                }
            }

           // plist = oai.PremissAsUser.Where(a => a.UserId==u.UserNum && a.ParenId==null).ToList();
            hm.UserName = u.UserName;
            ViewBag.User2 = hm;
            ViewBag.User = puList;
            ViewBag.User1 = plist;
           
            return View();
        }



    }
}
