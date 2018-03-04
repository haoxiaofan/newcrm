﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NewCRM.Application.Services.Interface;
using NewCRM.Domain.ValueObject;
using NewCRM.Dto;
using NewCRM.Infrastructure.CommonTools;
using NewCRM.Infrastructure.CommonTools.CustomHelper;
using NewCRM.Web.Controllers.ControllerHelper;
using NewLib;
using Nito.AsyncEx;

namespace NewCRM.Web.Controllers
{
    public class IndexController : BaseController
    {
        private readonly IDeskServices _deskServices;

        public IndexController(IDeskServices deskServices) => _deskServices = deskServices;

        #region 页面

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Desktop()
        {
            ViewBag.Title = "桌面";
            if(Request.Cookies["memberID"] != null)
            {
                var account = Account;
                account.AccountFace = ProfileManager.FileUrl + account.AccountFace;
                ViewData["Account"] = account;
                ViewData["AccountConfig"] = AccountServices.GetConfig(account.Id);
                ViewData["Desks"] = AccountServices.GetConfig(account.Id).DefaultDeskCount;

                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        #endregion

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnlockScreen(String unlockPassword)
        {
            #region 参数验证
            Parameter.Validate(unlockPassword);
            #endregion

            var response = new ResponseModel();
            var result = AccountServices.UnlockScreen(Account.Id, unlockPassword);
            if(result)
            {
                response.IsSuccess = true;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 账户登出
        /// </summary>
        [HttpPost]
        public void Logout()
        {
            AccountServices.Logout(Account.Id);
            InternalLogout();
        }

        /// <summary>
        /// 初始化皮肤
        /// </summary>
        [HttpGet]
        public ActionResult GetSkin()
        {
            var response = new ResponseModel<String>();
            var skinName = AccountServices.GetConfig(Account.Id).Skin;
            response.IsSuccess = true;
            response.Model = skinName;
            response.Message = "初始化皮肤成功";

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 初始化壁纸
        /// </summary>
        [HttpGet]
        public ActionResult GetWallpaper()
        {
            var response = new ResponseModel<ConfigDto>();
            var result = AccountServices.GetConfig(Account.Id);

            if(result.IsBing)
            {
                result.WallpaperSource = WallpaperSource.Bing.ToString().ToLower();
                result.WallpaperUrl = AsyncContext.Run(BingHelper.GetEverydayBackgroundImageAsync);
            }

            response.IsSuccess = true;
            response.Message = "初始化壁纸成功";
            response.Model = result;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 初始化应用码头
        /// </summary>
        [HttpGet]
        public ActionResult GetDockPos()
        {
            var response = new ResponseModel<String>();
            var result = AccountServices.GetConfig(Account.Id).DockPosition;
            response.IsSuccess = true;
            response.Message = "初始化应用码头成功";
            response.Model = result;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取我的应用
        /// </summary>
        [HttpGet]
        public ActionResult GetAccountDeskMembers()
        {
            var response = new ResponseModel<IDictionary<String, IList<dynamic>>>();
            var result = _deskServices.GetDeskMembers(Account.Id);
            response.IsSuccess = true;
            response.Message = "获取我的应用成功";
            response.Model = result;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        [HttpGet]
        public ActionResult GetAccountFace()
        {
            var response = new ResponseModel<String>();
            var result = AccountServices.GetConfig(Account.Id).AccountFace;
            response.IsSuccess = true;
            response.Message = "获取用户头像成功";
            response.Model = ProfileManager.FileUrl + result;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建一个窗口
        /// </summary>
        [HttpPost]
        public ActionResult CreateWindow(Int32 id, String type)
        {
            var response = new ResponseModel<dynamic>();
            var internalMemberResult = type == "folder" ? _deskServices.GetMember(Account.Id, id, true) : _deskServices.GetMember(Account.Id, id);
            response.IsSuccess = true;
            response.Message = "创建一个窗口成功";
            response.Model = new
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
                url = internalMemberResult.AppUrl,
                isResize = internalMemberResult.IsResize
            };

            return Json(response);
        }

    }
}