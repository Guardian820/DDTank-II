﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Specialized;
using log4net;
using System.Reflection;
using Bussiness;
using System.Configuration;
using Tank.Request.Illegalcharacters;
using Bussiness.Interface;
using Road.Flash;
using System.Text;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class RenameNick : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool value = false;
            string message = LanguageMgr.GetTranslation("Tank.Request.RenameNick.Fail1");// "Login Fail!";

            XElement result = new XElement("Result");
            try
            {
                BaseInterface inter = BaseInterface.CreateInterface();
                string p = context.Request["p"];
                string site = context.Request["site"] == null ? "" : HttpUtility.UrlDecode(context.Request["site"]);
                //string nickname = context.Request["nickname"] == null ? "" : HttpUtility.UrlDecode(context.Request["nickname"]);
                //string newNickname = context.Request["newNickname"] == null ? "" : HttpUtility.UrlDecode(context.Request["newNickname"]);
                string IP = context.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(p))
                {
                    //解密
                    byte[] src = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, p);
                    string[] strList = Encoding.UTF8.GetString(src, 7, src.Length - 7).Split(',');
                    if (strList.Length == 5)
                    {
                        string name = strList[0];
                        string pwd = strList[1];
                        string newPwd = strList[2];
                        string nickname = strList[3];
                        string newNickname = strList[4];

                        if (PlayerManager.Login(name, pwd))
                        {
                            
                            using (PlayerBussiness db = new PlayerBussiness())
                            {
                                if (db.RenameNick(name, nickname, newNickname, ref message))
                                {
                                    PlayerManager.Update(name, newPwd);
                                    value = true;
                                    message = LanguageMgr.GetTranslation("Tank.Request.RenameNick.Success");
                                }
                            }
                        }                   
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("RenameNick", ex);
                value = false;
                message = LanguageMgr.GetTranslation("Tank.Request.RenameNick.Fail2");
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
