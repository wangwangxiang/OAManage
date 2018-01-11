using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OA_System.Controllers
{
    public class EmployMoveController : Controller
    {
        //
        // GET: /EmployMove/
        /// <summary>
        ///人事变动首页
        /// </summary>
        /// <returns></returns>
        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {
            //获取查询条件

            string userName=Request["userName"];
            string userNum=Request["userNum"];
            //当前页
            int currentPage=Convert.ToInt32(Request["currentPage"]?? "1");
            ViewBag.currentPage = currentPage;
            int chokNum;
            if(Request["selStatus"]==null)
            {
                chokNum=-1;
              

            }else{
                chokNum=Convert.ToInt32(Request["selStatus"]);
                
            }



            //分页查询语句

            List<UserInfo> userList = new List<UserInfo>();
            //linq委托实现分页
            Func<UserInfo, bool> func = a => (a.UserName == userName || string.IsNullOrEmpty(userName)) &&
                (a.UserNum == userNum || string.IsNullOrEmpty(userNum)) &&
                (a.OccupationalStatus == chokNum || chokNum == -1);

            //存放userList集合中
            userList = oai.UserInfo.Where(func).OrderBy(a => a.ID).Skip((currentPage - 1) * 10).Take(currentPage*10).ToList();

            //查询总条数

            int countpage = oai.UserInfo.Where(func).Select(a=>a.ID).Count();

            int count;
            count = countpage / 10;
            if (countpage %10 != 0)
            {
                count++;
            }
            //创建员工所在部门集合
            List<departmentInfo> dlist = new List<departmentInfo>();
            //创建员工所处职位的集合
            List<PositionInfo> plist = new List<PositionInfo>();


            dlist = oai.departmentInfo.ToList();
            plist = oai.PositionInfo.ToList();
           
            ViewBag.dlist = dlist;
            ViewBag.plist = plist;

            //存放变量保存

            ViewBag.userList = userList;
            ViewBag.tocount = count;




            return View();
        }
        /// <summary>
        ///员工退休
        /// </summary>
        /// <returns></returns>
        public JsonResult UpStatusById()
        {
            ///获取字符串并分割成数组
            string[] strUserId = Request["strUserId"].Split(',');

            //接收修改成功的行数
            int count = 0;
            foreach (var item in strUserId)
            {

                if (!string.IsNullOrEmpty(item))
                {
                    UserInfo u = new UserInfo();
                    u.ID = Convert.ToInt32(item);
                    u.OccupationalStatus=3;
                    var update = oai.Entry(u);

                    update.State = System.Data.EntityState.Unchanged;

                    update.Property("ID").IsModified = true;

                    update.Property("OccupationalStatus").IsModified = true;

                    count = oai.SaveChanges();



                }
                
            }

            if (count >= 1)
            {
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 员工批量离职
        /// </summary>

        public JsonResult UpEmployStatusById()
        {
            string[] strUserId = Request["strUserId"].Split(',');
            int count=0;
            foreach (var item in strUserId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    UserInfo u = new UserInfo();
                    u.ID = Convert.ToInt32(item);
                    u.OccupationalStatus = 1;
                    string time = DateTime.Now.ToShortDateString();
                    u.QuitId = time;
                    var update = oai.Entry(u);
                    update.State = System.Data.EntityState.Unchanged;

                    update.Property("ID").IsModified=true;
                    update.Property("OccupationalStatus").IsModified = true;
                    update.Property("QuitId").IsModified = true;
                    count = oai.SaveChanges();

                }
            }

            if (count >= 1)
            {
                return Json(new { message="success"}, JsonRequestBehavior.AllowGet);
            }
            return Json(new {mesage="failure" },JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 员工改为实习
        /// </summary>
        /// <returns></returns>
        public JsonResult UpStatusByIdShi()
        {
            string[] strUserId = Request["strUserId"].Split(',');
            int count = 0;
            foreach (var item in strUserId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    UserInfo u = new UserInfo();
                    u.ID = Convert.ToInt32(item);
                    u.OccupationalStatus = 2;

                    var update = oai.Entry(u);
                    update.State = System.Data.EntityState.Unchanged;
                    update.Property("ID").IsModified = true;
                    update.Property("OccupationalStatus").IsModified = true;
                    count = oai.SaveChanges();
                }
            }
            if (count >= 1)
            {
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 变更员工所在的部门以及职位
        /// </summary>
        /// <returns></returns>
        public JsonResult upEmployBZById()
        {
            //获取前台传过来的值

            int userId = Convert.ToInt32(Request["userId"]);
            string bumenId = Request["bumenId"];
            int zhiweiId = Convert.ToInt32(Request["zhiweiId"]);

            UserInfo u = new UserInfo();
            u.ID = userId;
            u.BuMenId = bumenId;
            u.ZhiWeiId = zhiweiId;

            var update = oai.Entry(u);

            update.State = System.Data.EntityState.Unchanged;

            update.Property("ID").IsModified=true;
            update.Property("BuMenId").IsModified = true;
            update.Property("ZhiWeiId").IsModified = true;

            int count = oai.SaveChanges();
            if (count >= 1)
            {
                return Json(new{message="success"},JsonRequestBehavior.AllowGet);
            }

            return Json(new {message="failure"},JsonRequestBehavior.AllowGet);
        }

    }
}
