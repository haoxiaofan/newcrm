﻿using NewCRM.Web.Controllers.ControllerHelper;
using System;
using System.ComponentModel.Composition;
using System.Web;
using System.Web.Mvc; 

namespace NewCRM.Web.Controllers
{
    [Export]
    public class IndexController : BaseController
    {

        #region 页面

        // GET: Index
        /// <summary>
        /// 桌面
        /// </summary>
        /// <returns></returns>
        public ActionResult Desktop()
        {
            ViewBag.Title = "桌面";

            if (Request.Cookies["Account"] != null)
            {
                AccountConfig = AccountApplicationServices.GetConfig(Int32.Parse(Request.Cookies["Account"].Value));

                ViewData["Account"] = Account;

                ViewData["AccountConfig"] = AccountConfig;

                return View();
            }

            return RedirectToAction("Login", "Index");
        }

        /// <summary>
        /// 登陆页
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        #endregion
        
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="passWord"></param>
        /// <param name="isRememberPasswrod"></param>
        /// <returns></returns>
        public ActionResult Landing(String accountName, String passWord, Boolean isRememberPasswrod = false)
        {
            var accountResult = AccountApplicationServices.Login(accountName, passWord);
            
            Response.SetCookie(new HttpCookie("Account")
            {
                Value = accountResult.Id.ToString(),
                Expires = isRememberPasswrod ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(30)
            });

            Account = accountResult;

            return Json(new { success = 1 });
        }

        /// <summary>
        /// 账户登出
        /// </summary>
        public void Logout()
        {
            Response.Cookies.Add(new HttpCookie("Account")
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        /// <summary>
        /// 初始化皮肤
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSkin()
        {
            var skinName = AccountConfig.Skin;

            return Json(new { data = skinName }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 初始化壁纸
        ///// </summary>
        ///// <returns></returns>
        public ActionResult GetWallpaper()
        {
            var config = AccountApplicationServices.GetConfig(Account.Id);

            return Json(new
            {
                data = new
                {
                    config.WallpaperSource,
                    config.WallpaperUrl,
                    config.WallpaperHeigth,
                    config.WallpaperMode,
                    config.WallpaperWidth
                }
            }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 初始化应用码头
        ///// </summary>
        ///// <returns></returns>
        public ActionResult GetDockPos()
        {
            var dockPos = AccountConfig.DockPosition;

            return Json(new { data = dockPos }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 获取我的应用
        ///// </summary>
        ///// <returns></returns>
        public ActionResult GetAccountDeskMembers()
        {
            var app = AppApplicationServices.GetAccountDeskMembers(Account.Id);

            return Json(new { app }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 获取用户头像
        ///// </summary>
        ///// <returns></returns>
        public ActionResult GetAccountFace()
        {
            return Json(new { data = AccountConfig.AccountFace }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 创建一个窗口
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        public ActionResult CreateWindow(Int32 id = 0, String type = "")
        {
            var internalMemberResult = type == "folder" ? DeskApplicationServices.GetMember(Account.Id, id, true) : DeskApplicationServices.GetMember(Account.Id, id);

            return Json(new
            {
                data = new
                {
                    type = internalMemberResult.MemberType.ToLower(),
                    memberId = internalMemberResult.Id,
                    appId = internalMemberResult.AppId,
                    name = internalMemberResult.Name,
                    icon = internalMemberResult.IconUrl,
                    width = internalMemberResult.Width,
                    height = internalMemberResult.Height,
                    isOnDock = internalMemberResult.IsOnDock,
                    isDraw = internalMemberResult.IsDraw,
                    isOpenMax = internalMemberResult.IsOpenMax,
                    isSetbar = internalMemberResult.IsSetbar,
                    url = internalMemberResult.AppUrl
                }
            });
        }
    }
}