using System;
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
using SqlDataProvider.Data;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for FarmGetUserFieldInfos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FarmGetUserFieldInfos : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            //farmgetuserfieldinfos.ashx?selfid=5&key=f79ed73008e2a7b20f96ad31562c9d52
            //<Result value="true" message="Success!">
            //<Item UserID="740619823">
            //<Item SeedID="332111" AcclerateDate="0" GrowTime="2012-08-21T12:07:48" />
            //<Item SeedID="332111" AcclerateDate="0" GrowTime="2012-08-21T12:07:44" />
            //<Item SeedID="332111" AcclerateDate="0" GrowTime="2012-08-21T12:07:49" />
            //<Item SeedID="332112" AcclerateDate="0" GrowTime="2012-08-21T12:07:56" />
            //<Item SeedID="332112" AcclerateDate="0" GrowTime="2012-08-21T12:07:53" />
            //<Item SeedID="332112" AcclerateDate="0" GrowTime="2012-08-21T12:07:54" />
            //<Item SeedID="332112" AcclerateDate="0" GrowTime="2012-08-21T12:07:56" />
            //</Item>
            int selfid = Convert.ToInt32(context.Request["selfid"]);
            string key = context.Request["key"];
            bool value = true;

            string message = "Success!";
            XElement result = new XElement("Result"); 
            using (PlayerBussiness db = new PlayerBussiness())
            {
                FriendInfo[] infos = db.GetFriendsAll(selfid);

                foreach (FriendInfo g in infos)
                {
                    XElement node = new XElement("Item");
                    UserFieldInfo[] fields = db.GetSingleFields(g.FriendID);
                    foreach (UserFieldInfo f in fields)
                    {
                        XElement Item = new XElement("Item",
                            new XAttribute("SeedID", f.SeedID),
                            new XAttribute("AcclerateDate", f.AccelerateTime),
                            new XAttribute("GrowTime", f.PlantTime.ToString("yyyy-mm-ddTHH:mm:ss")));//"2012-08-21T12:07:48" 
                        node.Add(Item);
                    }
                    node.Add(new XAttribute("UserID", g.FriendID));
                    result.Add(node);
                }
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