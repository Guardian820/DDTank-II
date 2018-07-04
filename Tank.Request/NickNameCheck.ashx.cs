﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using SqlDataProvider.Data;
using Bussiness;
using log4net;
using System.Reflection;
using System.Configuration;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class NickNameCheck : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool value = false;
            //string message = "Name is Exist!";
            string message = LanguageMgr.GetTranslation("Tên nhân vật đã được sử dụng."); //string message = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Exist");
            XElement result = new XElement("Result");

            try
            {
                string nickName = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
                if (System.Text.Encoding.Default.GetByteCount(nickName) <= 14)
                {
                    if (!string.IsNullOrEmpty(nickName))
                    {
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            if (db.GetUserSingleByNickName(nickName) == null)
                            {
                                value = true;
                                //message = "You can register!";
                                message = LanguageMgr.GetTranslation("Kiểm tra tên nhân vật.", new object[0]); //message = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Right", new object[0]);
                               
                            }
                        }
                    }
                }
                else
                {
                    message = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Long", new object[0]);
                }
            }
            catch(Exception ex)
            {
                log.Error("NickNameCheck", ex);
                value = false;
            }

            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));

            context.Response.ContentType = "text/plain";
            context.Response.Write(result.ToString(false));
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
