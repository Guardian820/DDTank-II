﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Xml.Linq;
using Road;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using Road.Flash;
using System.Web.SessionState;
using Bussiness;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Bussiness.Interface;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Login : IHttpHandler, IRequiresSessionState
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {

            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool value = false;
            string message = LanguageMgr.GetTranslation("Đăng nhập thất bại.");//string message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1");// "Login Fail!";
            string style = "";
            bool isError = false;

            XElement result = new XElement("Result");

            try
            {
                BaseInterface inter = BaseInterface.CreateInterface();
                string p = context.Request["p"];//?
                string site = context.Request["site"] == null ? "" : HttpUtility.UrlDecode(context.Request["site"]);
                //string nickname = context.Request["nickname"] == null ? "" : HttpUtility.UrlDecode(context.Request["nickname"]);
                string IP = context.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(p))
                {
                    //解密
                    byte[] src = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, p);                   
                    string[] strList = Encoding.UTF8.GetString(src, 7, src.Length - 7).Split(',');
                    if (strList.Length == 4)
                    {
                        string name = strList[0];
                        string pwd = strList[1];
                        string newPwd = strList[2];
                        string nickname = strList[3];
                        
                            if (true)//(PlayerManager.Login(name, pwd)) 
                            {
                            int isFirst = 0;
                            bool isActive = false;
                            bool firstValidate = PlayerManager.GetByUserIsFirst(name);
                            //using (PlayerBussiness db = new PlayerBussiness())
                            //{
                            //    PlayerInfo[] infos = db.GetUserLoginList(name);

                            //    if (infos.Length <= 0)
                            //    {
                            //        throw new Exception();
                            //    }
                            //}
                            PlayerInfo player = inter.CreateLogin(name, newPwd, ref message, ref isFirst, IP, ref isError, firstValidate, ref isActive, site, nickname);
                                                       
                            if (player != null && !isError)
                            {
                                if (isFirst == 0)
                                {
                                    PlayerManager.Update(name, newPwd);
                                }
                                else
                                {
                                    PlayerManager.Remove(name);
                                }
                                style = string.IsNullOrEmpty(player.Style) ? ",,,,,,,," : player.Style;
                                player.Colors = string.IsNullOrEmpty(player.Colors) ? ",,,,,,,," : player.Colors;

                                XElement node = new XElement("Item", 
                                    new XAttribute("ID", player.ID),
                                    new XAttribute("IsFirst", isFirst),
                                    new XAttribute("NickName", player.NickName),
                                    new XAttribute("Date", ""),
                                    new XAttribute("IsConsortia", 0),
                                    new XAttribute("ConsortiaID", player.ConsortiaID),
                                    new XAttribute("Sex", player.Sex),
                                    new XAttribute("WinCount", player.Win),
                                    new XAttribute("TotalCount", player.Total),
                                    new XAttribute("EscapeCount", player.Escape),
                                     new XAttribute("DutyName", player.DutyName == null ? "" : player.DutyName),
                                    new XAttribute("GP", player.GP),
                                    new XAttribute("Honor", ""),
                                    new XAttribute("Style", player.Style == null ? style : player.Style),
                                    new XAttribute("Gold", player.Gold),
                                    new XAttribute("Colors", player.Colors == null ? "" : player.Colors),
                                    new XAttribute("Attack", player.Attack),
                                    new XAttribute("Defence", player.Defence),
                                    new XAttribute("Agility", player.Agility),
                                    new XAttribute("Luck", player.Luck),
                                    new XAttribute("Grade", player.Grade),
                                    new XAttribute("Hide", player.Hide),
                                    new XAttribute("Repute", player.Repute),
                                    new XAttribute("ConsortiaName", player.ConsortiaName == null ? "" : player.ConsortiaName),
                                    new XAttribute("Offer", player.Offer),
                                    new XAttribute("Skin", player.Skin == null ? "" : player.Skin),
                                    new XAttribute("ReputeOffer", player.ReputeOffer),
                                    new XAttribute("ConsortiaHonor", player.ConsortiaHonor),
                                    new XAttribute("ConsortiaLevel", player.ConsortiaLevel),
                                    new XAttribute("ConsortiaRepute", player.ConsortiaRepute),
                                    new XAttribute("Money", player.Money),
                                    new XAttribute("AntiAddiction", player.AntiAddiction),
                                    new XAttribute("IsMarried", player.IsMarried),
                                    new XAttribute("SpouseID", player.SpouseID),
                                    new XAttribute("SpouseName", player.SpouseName == null ? "" : player.SpouseName),
                                    new XAttribute("MarryInfoID", player.MarryInfoID),
                                    new XAttribute("IsCreatedMarryRoom", player.IsCreatedMarryRoom),
                                    new XAttribute("IsGotRing", player.IsGotRing),
                                    new XAttribute("LoginName", player.UserName == null ? "" : player.UserName),
                                    new XAttribute("Nimbus", player.Nimbus),
                                    new XAttribute("FightPower", player.FightPower),
                                    new XAttribute("AnswerSite", player.AnswerSite),
                                    new XAttribute("WeaklessGuildProgressStr", player.WeaklessGuildProgressStr == null ? "" : player.WeaklessGuildProgressStr),
                                    new XAttribute("IsOldPlayer", player.IsOldPlayer));

                                result.Add(node);
                                value = true;
                                message = LanguageMgr.GetTranslation("Đăng nhập thành công."); ; //message = LanguageMgr.GetTranslation("Tank.Request.Login.Success"); ;

                            }
                            else
                            {
                                PlayerManager.Remove(name);
                            }
                        }
                        else
                        {
                            message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Try");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("User Login error", ex);
                value = false;
                message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail2");
            }
            finally
            {
                result.Add(new XAttribute("value", value));
                result.Add(new XAttribute("message", message));
                context.Response.ContentType = "text/plain";
                context.Response.Write(result.ToString(false));
            }

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
