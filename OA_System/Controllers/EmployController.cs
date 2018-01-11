using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace OA_System.Controllers
{
    public class EmployController : Controller
    {
        //
        // GET: /Employ/
        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {

           

            string name = Request["userName"];
            string num = Request["userNum"];
            int currentPage =Convert.ToInt32(Request["currentPage"] ?? "1");
            ViewBag.currentPage = currentPage;
            int checknum;
            if (Request["checktype"] == null)
            {
                checknum=-1;
                ViewBag.chose =checknum;

            }
            else
            {
                checknum=Convert.ToInt32(Request["checktype"]);
                ViewBag.chose =checknum;
            }
            List<UserInfo> uList = new List<UserInfo>();
            Func<UserInfo, bool> SearFun = a => (a.UserName == name || string.IsNullOrEmpty(name)) &&
                                      (string.IsNullOrEmpty(num) || a.UserNum == num) &&
                                       (checknum == -1 || a.OccupationalStatus == checknum);
            uList = oai.UserInfo.Where(SearFun).OrderBy(a => a.ID).Skip((currentPage - 1) * 10).Take(10).ToList();

            //查询总条数数

            int allpage = oai.UserInfo.Where(SearFun).Select(a => a.ID).Count();
            int count = allpage / 10;

            if(allpage%10!=0){
                count++;
            }

           ViewBag.count = count;

            ViewBag.ulist = uList;

            return View();
        }

        /// <summary>
        /// 单个员工解雇
        /// </summary>
        /// <returns></returns>
        public JsonResult FireEmployByNum(int userId)
        {

            UserInfo u = new UserInfo();
            u.OccupationalStatus = 1;
            u.ID = userId;

            string time = DateTime.Now.ToShortDateString();
            u.QuitId = time;
            var update = oai.Entry(u);

           
        
            update.State = System.Data.EntityState.Unchanged;

            update.Property("ID").IsModified = true;
            update.Property("OccupationalStatus").IsModified = true;
            update.Property("QuitId").IsModified = true;
         
            int count = oai.SaveChanges();

            if (count >= 1)
            {
                return Json(new {message="success"},JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 批量删除员工
        /// </summary>
        /// <returns></returns>
        public JsonResult DelEmployByNum()
        {

            string[] UserId = Request["userId"].Split(',');
            int count=0;
            foreach (var item in UserId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    UserInfo u = new UserInfo();
                    u.ID = Convert.ToInt32(item);
                    oai.Entry(u).State = System.Data.EntityState.Deleted;
                    count = oai.SaveChanges();
                }
            }

            if (count >= 1)
            {
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据Id查询员工信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult SeachEmployById(int Id)
        {
            UserInfo user = new UserInfo();

            user = oai.UserInfo.Where(a=>a.ID==Id).FirstOrDefault();
            //用model的方式传值
            return View(user);
        }
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult UpdateEmployById()
        {

            string frmdate = Request["frmdate"];
            JavaScriptSerializer jss = new JavaScriptSerializer();

            UserInfo user = jss.Deserialize<UserInfo>(frmdate);
            
            var update = oai.Entry(user);

            update.State = System.Data.EntityState.Unchanged;

            update.Property("ID").IsModified = true;
            update.Property("LoginName").IsModified = true;
            update.Property("UserName").IsModified = true;
            update.Property("UserNum").IsModified = true;
            update.Property("UserPwd").IsModified = true;
            update.Property("UserSex").IsModified = true;
            update.Property("UserTel").IsModified = true;
            update.Property("UserEmail").IsModified = true;
            update.Property("XueLi").IsModified = true;
            update.Property("UserAge").IsModified = true;
            update.Property("UserAddress").IsModified = true;

            int count = oai.SaveChanges();

            if (count >= 1)
            {
                return Json(new { message="scuccess"},JsonRequestBehavior.AllowGet);
            }

            return Json("",JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 添加部门下拉列表框内容
        /// </summary>
        /// <returns></returns>
        public JsonResult adddepartment()
        {
            List<departmentInfo> dlist = new List<departmentInfo>();

            dlist = oai.departmentInfo.ToList();

            if (dlist != null)
            {
                return Json(new {message="success",data=dlist },JsonRequestBehavior.AllowGet);
            }

            return Json("",JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加职位下拉列表框
        /// </summary>
        /// <returns></returns>
        public JsonResult addPositionInfo()
        {
            List<PositionInfo> plist = new List<PositionInfo>();
            plist = oai.PositionInfo.ToList();
            if (plist != null)
            {
                return Json(new { message="success",data=plist},JsonRequestBehavior.AllowGet);
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 员工入职界面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEmp()
        {
            return View();
        }
        /// <summary>
        /// 员工入职功能
        /// </summary>
        /// <returns></returns>
        public JsonResult addemploy()
        {



            string jonstr = Request["jonstr"];
            JavaScriptSerializer jss = new JavaScriptSerializer();
            UserInfo u = jss.Deserialize<UserInfo>(jonstr);

            UserInfo u1 = oai.Database.SqlQuery<UserInfo>("select top 1* from UserInfo order by ID desc").FirstOrDefault();
            string userNum = u1.UserNum;

            string num = userNum.Substring(1, userNum.Length - 1);


            string num1 = (Convert.ToInt32(num) + 1).ToString();
            string[] num2 = num1.Split(',');
            int num3 = (Convert.ToInt32(num) + 1);

            string userNum3 = "";

            for (int i = 0; i < num2.Length; i++)
            {
                if (num3 >= 10 && num3 < 100)
                {
                    userNum3 = "D000";
                    userNum3 += num2[i];
                }
                if (num3 < 10)
                {
                    userNum3 = "D0000";
                    userNum3 += num2[i];
                }
                if (num3 >= 100 && num3 < 1000)
                {
                    userNum3 = "D00";
                    userNum3 += num2[i];
                }

                if (num3 >= 1000 && num3 < 10000)
                {
                    userNum3 = "D0";
                    userNum3 += userNum[i];
                }
                if (num3 >= 10000 && num3 < 100000)
                {
                    userNum3 = "D";
                    userNum3 += num2[i];
                }

                u.UserNum = userNum3;

            }



            int count1 = oai.UserInfo.Where(a => a.LoginName == u.LoginName && a.UserNum == u.UserNum).Count();

            if (count1 >= 1)
            {
                return Json(new { message = "failure" });
            }
            else
            {

                //获取当前时间

                string time = DateTime.Now.ToShortDateString();

                u.EntryTime = time;

                oai.Entry(u).State = System.Data.EntityState.Added;
                int count = oai.SaveChanges();

                if (count >= 1)
                {
                    return Json(new { message = "success" });
                }
                return Json("");
            }




        }

    }
}
