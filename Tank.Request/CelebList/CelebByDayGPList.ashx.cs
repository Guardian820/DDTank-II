﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using Bussiness;
using SqlDataProvider.Data;
using Road.Flash;
using log4net;
using System.Reflection;

namespace Tank.Request.CelebList
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CelebByDayGPList : IHttpHandler
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(Build(context));
        }

        public static string Build(HttpContext context)
        {
            if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
                return "CelebByDayGPList Fail!";

            return Build();
        }

        public static string Build()
        {
            return csFunction.BuildCelebUsers("CelebByDayGPList", 2, "CelebForUsersByDay");

            //bool value = false;
            //string message = "Fail!";
            //XElement result = new XElement("Result");

            //try
            //{
            //    int page = 1;
            //    int size = 50;
            //    int order = 2;
            //    int userID = -1;
            //    int total = 0;
            //    bool resultValue = false;

            //    using (PlayerBussiness db = new PlayerBussiness())
            //    {
            //        PlayerInfo[] infos = db.GetPlayerPage(page, size, ref total, order, userID, ref resultValue);
            //        if (resultValue)
            //        {
            //            foreach (PlayerInfo info in infos)
            //            {
            //                result.Add(FlashUtils.CreateCelebInfo(info));
            //            }
            //            value = true;
            //            message = "Success!";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.Error("CelebByDayGPList is fail!", ex);
            //}

            //result.Add(new XAttribute("value", value));
            //result.Add(new XAttribute("message", message));

            //return csFunction.CreateCompressXml(result, "CelebByDayGPList", true);
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
