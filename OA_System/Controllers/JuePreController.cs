using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OA_System.Controllers
{
    public class JuePreController : Controller
    {
        //
        // GET: /JuePre/
        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {
            //查询所有权限

            List<PremissAsUser> pList = oai.PremissAsUser.ToList();

            //查询父级权限

            List<PremissAsUser> puList = oai.PremissAsUser.Where(a => a.ParenId == null).ToList();

            //把权限保存在 viewBag中

            ViewBag.pList = pList;

            ViewBag.puList = puList;
            return View();
        }

        /// <summary>
        /// 查询所有职位
        /// </summary>
        /// <returns></returns>
        public JsonResult SelPosition()
        {
            List<PositionInfo> plist = new List<PositionInfo>();

            plist = oai.PositionInfo.ToList();

            if (plist.Count > 0)
            {
                return Json(new {message="success",data=plist },JsonRequestBehavior.AllowGet);
            }

            return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 角色授权
        /// </summary>
        /// <returns></returns>
        public JsonResult JueAddPremiss()
        {
            int posId =Convert.ToInt32(Request["PosId"]);

            string[] preStr = Request["preStr"].Split(',');

            List<string> PreList = oai.userAsPersion.Where(a => a.PositionId == posId).Select(b=>b.preId).ToList();

            int count = 0;

            foreach (var item in preStr)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!PreList.Contains(item))
                    {
                        userAsPersion up = new userAsPersion();
                        up.PositionId = posId;
                        up.preId = item;
                        oai.Entry(up).State = System.Data.EntityState.Added;
                        count = oai.SaveChanges();
                    }
                }
            }

            if (count >= 1)
            {
                return Json(new { message="success"},JsonRequestBehavior.AllowGet);
            }

            return Json(new { message="failUre"},JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 角色授权
        /// </summary>
        /// <returns></returns>
        public JsonResult addpositionPre()
        {
            string buMenId = Request["bunMenNum"];

            int zhiId =Convert.ToInt32(Request["zhiNum"]);
            //根据部门id 职位Id 查找员工ID
            string userNum = oai.UserInfo.Where(a => a.ZhiWeiId == zhiId && a.BuMenId == buMenId).Select(b => b.UserNum).FirstOrDefault();

            //根据职位Id查询 角色拥有权限

            List<string> uList = oai.userAsPersion.Where(a => a.PositionId == zhiId).Select(b=>b.preId).ToList();

            //根据员工编号 查询所拥有的权限

            List<string> preList = oai.PermissionsInfo.Where(a => a.UserId == userNum).Select(b => b.fatherId).ToList();
            int count = 0;
            if (userNum != null)
            {
                foreach (var item in uList)
                {
                    if (!preList.Contains(item))
                    {
                        PermissionsInfo p = new PermissionsInfo();
                        p.fatherId = item;
                        p.UserId = userNum;

                        oai.Entry(p).State = System.Data.EntityState.Added;

                        count = oai.SaveChanges();
                    }
                }
            }
            if (count >= 1)
            {
                return Json(new {message="success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 角色授权首页
        /// </summary>
        /// <returns></returns>
        public ActionResult PositionPreIndex()
        {
            return View();
        }

    }
}
