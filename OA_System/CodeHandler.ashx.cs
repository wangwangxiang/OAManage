using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace OA_System
{
    /// <summary>
    /// CodeHandler 的摘要说明
    /// </summary>
    //在一般处理程序中如果要使用session存值的话必须使用实现接口IRequiresSessionState
    public class CodeHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //定义图片类型
            context.Response.ContentType = "image/png";

            Bitmap bm = new Bitmap(100,40);
            //创建图片
            Graphics gh = Graphics.FromImage(bm);
            //创建验证码图片格局
            gh.FillRectangle(new SolidBrush(Color.White),0,0,120,40);
            //创建生成验证码的数组
            string[] strNum = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "A", "B", "C", "D", "E", "F", "V", "v", "k", "K", "j"
                              ,"J","H","h","Y","y","I"};
            //创建随机对象

            Random rd = new Random();
            //创建一个存放数组的数

            string codeNum = "";

            //循环遍历数组（制造随机数）
            for (int i = 0; i <4; i++)
            {
                int num = rd.Next(0,strNum.Length-1);
 
                codeNum=codeNum+strNum[num];
            }

            //用session 存放生成的验证码

            context.Session["codeNum"] = codeNum;


            //设置字体大小

            Font font = new Font(FontFamily.GenericSerif,19);
            //验证码背景即字体大小
            gh.DrawString(codeNum,font,new SolidBrush(Color.Green),10,6);

            for (int i = 0; i < codeNum.Length; i++)
            {
                gh.FillEllipse(new SolidBrush(Color.Green),rd.Next(1, 56), rd.Next(1, 20), 10,6);
            }
            //验证码中干扰线
            Pen pen = new Pen(new SolidBrush(Color.Red));
            for (int i = 0; i < codeNum.Length; i++)
            {
                gh.DrawArc(pen, rd.Next(1, 80), rd.Next(1, 20), 120, 20, 50, 120);
            }
            bm.Save(context.Response.OutputStream, ImageFormat.Png);
            


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}