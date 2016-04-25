using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebViewer
{
    /// <summary>
    /// UploadFile 的摘要说明
    /// </summary>
    public class UploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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