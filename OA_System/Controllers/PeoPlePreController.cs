using OA_System.DTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OA_System.Controllers
{
    public class PeoPlePreController : Controller
    {
        //
        // GET: /PeoPlePre/
        /// <summary>
        /// 权限分配首页
        /// </summary>
        /// <returns></returns>
        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {
            //查询所有权限

            List<PremissAsUser> pList = oai.PremissAsUser.ToList();

            //查询父级权限

            List<PremissAsUser> puList = oai.PremissAsUser.Where(a=>a.ParenId==null).ToList();

            //把权限保存在 viewBag中

            ViewBag.pList = pList;

            ViewBag.puList = puList;

            return View();
        }

        /// <summary>
        /// 搜索职位
        /// </summary>
        /// <returns></returns>
        public JsonResult SelPosition()
        {
            string bumen = Request["bumen"];

            List<PositionInfo> plist = oai.PositionInfo.Where(a => a.DepteId == bumen).ToList();

            if (plist.Count > 0)
            {
                return Json(new { message = "success",data=plist }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 搜索人员
        /// </summary>
        /// <returns></returns>
        public JsonResult SelEmployee()
        {
            int posId =Convert.ToInt32(Request["posId"]);
            string bumen = Request["bumen"];

            List<UserInfo> ulist = oai.UserInfo.Where(a => a.BuMenId == bumen && a.ZhiWeiId == posId).ToList();

            if (ulist.Count > 0)
            {
                return Json(new { message="success",data=ulist},JsonRequestBehavior.AllowGet);
            }

            return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 个人授权
        /// </summary>
        /// <returns></returns>
        public JsonResult perSonalAdd()
        {

            string buMen = Request["bumen"];
            string renyuan = Request["renyuan"];

            //定义存放权限的数组

            string [] preStr = Request["preDate"].Split(',');
            
            //根据人员搜寻他 也有的权限


            List<string> plist = oai.PermissionsInfo.Where(a => a.UserId == renyuan).Select(b=>b.fatherId).ToList();
            int count = 0;

            if (plist.Count > 0)
            {
               

              
                    foreach (var item1 in preStr)
                    {
                        if (!string.IsNullOrEmpty(item1))
                        {
                            if (!plist.Contains(item1))
                            {
                                PermissionsInfo p = new PermissionsInfo();
                                p.fatherId = item1;
                                p.UserId = renyuan;

                                oai.Entry(p).State = System.Data.EntityState.Added;
                                count = oai.SaveChanges();
                            }
                        }
                    }
                
            }
            else 
            {
                foreach (var item in preStr)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        PermissionsInfo p = new PermissionsInfo();
                        p.fatherId = item;
                        p.UserId = renyuan;

                        oai.Entry(p).State = System.Data.EntityState.Added;
                        count = oai.SaveChanges();
                    }
                }
            }

            if (count >= 1)
            {
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failrue" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 给部门下拉列表框添加内容
        /// </summary>
        /// <returns></returns>
        public JsonResult addDepetContent()
        {
            List<departmentInfo> dList = new List<departmentInfo>();
            dList = oai.departmentInfo.ToList();

            if (dList != null)
            {
                return Json(new {message="success",data=dList },JsonRequestBehavior.AllowGet);
            }

            return Json(new {message="failure"},JsonRequestBehavior.AllowGet);
           
        }
        /// <summary>
        /// 给职位下拉列表添加值
        /// </summary>
        /// <returns></returns>
        public JsonResult addPosotionContent()
        {
            string depetId = Request["depetId"];

            List<UserInfo> pList = new List<UserInfo>();
            pList = oai.UserInfo.Where(a=>a.BuMenId==depetId).ToList();

            if (pList != null)
            {
                return Json(new { message="success",data=pList},JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 取消权限 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult DelPreIndex()
        {

            return View();
        }
        /// <summary>
        /// 查询用户 所拥有的权限
        /// </summary>
        /// <returns></returns>
        public JsonResult SelEmployeePre()
        {
            string employeeNum = Request["employeeNum"];

            //查询所有权限

            List<PremissAsUser> pList = oai.Database.SqlQuery<PremissAsUser>("select*from PremissAsUser where PremissId in(select fatherId from PermissionsInfo where UserId='" + employeeNum + "')").ToList();

            //查询父级权限

            List<PremissAsUser> puList = oai.Database.SqlQuery<PremissAsUser>("select * from PremissAsUser where ParenId is NULL and PremissId in (select fatherId from PermissionsInfo where UserId='" + employeeNum + "')").ToList();

            //把权限保存在 viewBag中

            PreMiss pm = new PreMiss();

            pm.plist = pList;
            pm.Pulist = puList;

            if (pList.Count > 0)
            {
                return Json(new { message="success",data=pm},JsonRequestBehavior.AllowGet);
            }


            return Json(new { message = "failure"}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解除权限
        /// </summary>
        /// <returns></returns>
        public JsonResult DelPre()
        {
            //接受页面传来的值
            string employeeNum = Request["employeeNum"];

            string[] PreStr = Request["PreStr"].Split(',');

            int count = 0;

            foreach (var item in PreStr)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    PermissionsInfo p = new PermissionsInfo();
                    int preId = oai.PermissionsInfo.Where(a => a.fatherId == item&&a.UserId==employeeNum).Select(b => b.ID).FirstOrDefault();

                    p.ID = preId;
                    p.UserId = employeeNum;

                    oai.Entry(p).State = System.Data.EntityState.Deleted;

                    count = oai.SaveChanges();

                }
            }

            if (count >= 1)
            {
                return Json(new {message="success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);

        }

    }
}
