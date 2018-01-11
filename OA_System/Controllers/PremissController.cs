using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace OA_System.Controllers
{
    public class PremissController : Controller
    {
        //
        // GET: /Premiss/

        OA_ItemEntities oai = new OA_ItemEntities();
        public ActionResult Index()
        {

            //获取前台传过来的数据
            string preName = Request["preName"];
            int currentPage = Convert.ToInt32(Request["currentPage"]??"1");

            ViewBag.currentPage = currentPage;

            List<PremissAsUser> puList = new List<PremissAsUser>();

            //委托查询

            Func<PremissAsUser, bool> funck = a => (a.PremissName == preName) || string.IsNullOrEmpty(preName);

            puList = oai.PremissAsUser.Where(funck).OrderBy(a => a.ID).Skip((currentPage - 1) * 10).Take(currentPage * 10).ToList();

            //总页条数

            int countNum = oai.PremissAsUser.Where(funck).Select(a => a.ID).Count();
        
            //总页数

            int countPage = countNum / 10;

            if(countNum%10!=0)
            {
                countPage++;
            }

            ViewBag.data=puList;
            ViewBag.totalpage=countPage;

            return View();
        }
        /// <summary>
        /// 单条删除权限
        /// </summary>
        /// <returns></returns>
        public JsonResult DelOneById()
        {
            int preId = Convert.ToInt32(Request["preId"]);

            string preid = oai.PremissAsUser.Where(a=>a.ID==preId).Select(b=>b.PremissId).FirstOrDefault();

            int count = oai.PremissAsUser.Where(a => a.ParenId == preid).Count();

            if (count >= 1)
            {
                return Json(new { message="failure"},JsonRequestBehavior.AllowGet);
            }
            else
            {
                PremissAsUser pu = new PremissAsUser();
                pu.ID = preId;
                oai.Entry(pu).State = System.Data.EntityState.Deleted;
                int count1 = oai.SaveChanges();
                if (count1 >= 1)
                {
                    return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);
                }
            }

          

        }
        /// <summary>
        /// 多条删除
        /// </summary>
        /// <returns></returns>
        public JsonResult DelMuchPreById()
        {
            int count = 0;
            string[] preId = Request["PreIdStr"].Split(',');

            int premId = 0;
            List<string> preNameList = new List<string>();
           //根据查询找到为父级
            foreach (var item in preId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    premId = Convert.ToInt32(item);
                    if ((oai.PremissAsUser.Where(a => (a.ID == premId && a.ParenId == null)).Select(b => b.PremissId).FirstOrDefault()) != null)
                    {
                        preNameList.Add(oai.PremissAsUser.Where(a => (a.ID == premId && a.ParenId == null)).Select(b => b.PremissId).FirstOrDefault());

                    }
                }
                

            }
           // int preant1 = 0;
            string preant = null;
            foreach (var item1 in preNameList)
            {
                foreach (var item2 in preId)
                {
                    if (!string.IsNullOrEmpty(item2))
                    {
                        int id=Convert.ToInt32(item2);

                        preant = oai.PremissAsUser.Where(a => a.ID == id && a.ParenId == item1).Select(b=>b.ParenId).FirstOrDefault();

                        if (preant != null)
                        {
                            PremissAsUser pu = new PremissAsUser();
                            pu.ID = Convert.ToInt32(item2);

                            oai.Entry(pu).State = System.Data.EntityState.Deleted;

                            count = oai.SaveChanges();





                        }
                        

                        
                      
                    }
                }
            }
           
           

            if (count >= 1)
            {
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);
            }
          
        }
        /// <summary>
        /// 权限修改页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UpPreIndex()
        {

            int id = Convert.ToInt32(Request["preId"]);

            PremissAsUser pau = oai.PremissAsUser.Where(a=>a.ID==id).FirstOrDefault();

            return View(pau);
        }

        /// <summary>
        /// 给下拉列表框 赋值所有框架所有权限
        /// </summary>
        /// <returns></returns>
        public JsonResult selPreNameAll()
        {

            List<PremissAsUser> preList = new List<PremissAsUser>();

            preList = oai.PremissAsUser.Where(a=>a.ParenId==null).ToList();

            return Json(new {message="success",data=preList},JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据id修改权限
        /// </summary>
        /// <returns></returns>
        public JsonResult UpPreById()
        {
            string josnStr = Request["formData"];

            int radioVal = Convert.ToInt32(Request["radioVal"]);

            JavaScriptSerializer jss = new JavaScriptSerializer();

            PremissAsUser pau = jss.Deserialize<PremissAsUser>(josnStr);

            var update = oai.Entry(pau);

            update.State = System.Data.EntityState.Unchanged;

            update.Property("ID").IsModified = true;

            update.Property("PremissName").IsModified = true;

            update.Property("PremissUrl").IsModified = true;

            int count = oai.SaveChanges();

            if (count >= 1)
            {
                return Json(new { message="success"},JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "failure" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <returns></returns>
        public ActionResult addIndex()
        {

            return View();
        }

        /// <summary>
        /// 给页面下拉列表框添加权限
        /// </summary>
        /// <returns></returns>
        public JsonResult SelPressAdd()
        {

            List<PremissAsUser> paList = new List<PremissAsUser>();

            paList = oai.PremissAsUser.Where(a => a.ParenId == null).ToList();


            return Json(new { message="success",data=paList},JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///添加权限功能
        /// </summary>
        /// <returns></returns>
        public JsonResult Addpress()
        {
            //接收客户端传过来的值
            string parentId = Request["parentId"];
            string PremissName = Request["PremissName"];
            string PremissUrl = Request["PremissUrl"];
            int checkIs=Convert.ToInt32(Request["checkIs"]);
            //查询最后一个权限 的 权限编号

            PremissAsUser pau = oai.PremissAsUser.SqlQuery("select top 1* from PremissAsUser order by ID desc").FirstOrDefault();

            string preId = pau.PremissId;

            string num = preId.Substring(1,preId.Length-1);


           string  num1 = (Convert.ToInt32(num)+1).ToString();
           string[] num2 = num1.Split(',');
           int num3 = (Convert.ToInt32(num) + 1);
         
           string preStr = "P";
           for (int i = 0; i < num2.Length; i++)
           {
               if (num3 >= 10&&num3<100)
               {
                   preStr= "P00";
                   preStr += num2[i];
               }
               if (num3 < 10)
               {
                   preStr = "P000";
                   preStr += num2[i];
               }

               if (num3>=100&&num3<1000)
               {
                   preStr = "P0";
                   preStr += num2[i];
               }
               if (num3 < 10000&&num3>=1000)
               {
                   preStr = "P";
                   preStr += num2[i];
               }

             


           }

           PremissAsUser pau1 = new PremissAsUser();

           pau1.PremissId = preStr;
           pau1.PremissUrl = PremissUrl;
           pau1.PremissName = PremissName;
           if (checkIs == 0)
           {
               pau1.ParenId = null;
           }
           if (checkIs == 1)
           {
               pau1.ParenId = parentId;
           }

           oai.Entry(pau1).State = System.Data.EntityState.Added;

           int count = oai.SaveChanges();
           if (count >= 1)
           {
               return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
           }
           else
           {
               return Json("",JsonRequestBehavior.AllowGet);
           }
        }

    }
}
