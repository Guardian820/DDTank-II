﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using SqlDataProvider.Data;
using Bussiness;
using Road.Flash;
using System.IO;
using log4net;
using System.Reflection;

namespace Tank.Request
{
    ///http://192.168.0.4:828/LoadBoxTemp.ashx
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LoadBoxTemp : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(Bulid(context));
            }
            else
            {
                context.Response.Write("IP is not valid!");
            }
        }
        public static string Bulid(HttpContext context)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");

            try
            {

                using (ProduceBussiness db = new ProduceBussiness())
                {

                    ItemBoxInfo[] infos = db.GetItemBoxInfos();
                    foreach (ItemBoxInfo info in infos)
                    {
                        result.Add(Road.Flash.FlashUtils.CreateItemBoxInfo(info));
                    }
                }

                value = true;
                message = "Success!";
            }
            catch (Exception ex)
            {
                log.Error("LoadBoxTemp", ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return csFunction.CreateCompressXml(context, result, "LoadBoxTemp", true);

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