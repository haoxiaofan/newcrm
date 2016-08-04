﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NewCRM.Domain.Entities.DomainModel.System;
using NewCRM.Domain.Entities.Repositories.IRepository.Account;
using NewCRM.Domain.Entities.Repositories.IRepository.System;
using NewCRM.Domain.Entities.ValueObject;
using NewCRM.Infrastructure.CommonTools.CustemException;

namespace NewCRM.Domain.Services.Impl
{
    [Export(typeof(IAppServices))]
    public class AppServices : BaseService, IAppServices
    {
        [Import]
        private IAppTypeRepository _appTypeRepository;

        [Import]
        private IAppRepository _appRepository;

        [Import]
        private IDeskRepository _deskRepository;

        public IDictionary<Int32, IList<dynamic>> GetUserDeskMembers(Int32 userId)
        {
            var userConfig = GetUser(userId);

            IDictionary<Int32, IList<dynamic>> desks = new Dictionary<Int32, IList<dynamic>>();

            foreach (var desk in userConfig.Config.Desks)
            {
                IList<dynamic> deskMembers = new List<dynamic>();

                var members = desk.Members.Where(member => member.IsDeleted == false).ToList();

                foreach (var member in members)
                {
                    if (member.MemberType == MemberType.Folder)
                    {
                        deskMembers.Add(new
                        {
                            type = member.MemberType.ToString().ToLower(),
                            memberId = member.Id,
                            appId = member.AppId,
                            name = member.Name,
                            icon = member.IconUrl,
                            width = member.Width,
                            height = member.Height,
                            isOnDock = member.IsOnDock,
                            isDraw = member.IsDraw,
                            isOpenMax = member.IsOpenMax,
                            isSetbar = member.IsSetbar,
                            apps = members.Where(m => m.FolderId == member.Id).Select(app => new
                            {
                                type = app.MemberType.ToString().ToLower(),
                                memberId = app.Id,
                                appId = app.AppId,
                                name = app.Name,
                                icon = app.IconUrl,
                                width = app.Width,
                                height = app.Height,
                                isOnDock = app.IsOnDock,
                                isDraw = app.IsDraw,
                                isOpenMax = app.IsOpenMax,
                                isSetbar = app.IsSetbar,
                            })
                        });
                    }
                    else
                    {
                        if (member.FolderId == 0)
                        {
                            var internalType = member.MemberType.ToString().ToLower();
                            deskMembers.Add(new
                            {
                                type = internalType,
                                memberId = member.Id,
                                appId = member.AppId,
                                name = member.Name,
                                icon = member.IconUrl,
                                width = member.Width,
                                height = member.Height,
                                isOnDock = member.IsOnDock,
                                isDraw = member.IsDraw,
                                isOpenMax = member.IsOpenMax,
                                isSetbar = member.IsSetbar
                            });
                        }
                    }
                }
                desks.Add(new KeyValuePair<Int32, IList<dynamic>>(desk.DeskNumber, deskMembers));
            }

            return desks;
        }

        public void ModifyAppDirection(Int32 userId, String direction)
        {
            var userResult = GetUser(userId);

            if (direction.ToLower() == "x")
            {
                userResult.Config.ModifyAppDirectionToX();
            }
            else if (direction.ToLower() == "y")
            {
                userResult.Config.ModifyAppDirectionToY();
            }
            else
            {
                throw new BusinessException($"未能识别的App排列方向:{direction.ToLower()}");
            }


            UserRepository.Update(userResult);
        }

        public void ModifyAppIconSize(Int32 userId, Int32 newSize)
        {
            var userResult = GetUser(userId);
            userResult.Config.ModifyDisplayIconLength(newSize);
            UserRepository.Update(userResult);
        }

        public void ModifyAppVerticalSpacing(Int32 userId, Int32 newSize)
        {
            var userResult = GetUser(userId);
            userResult.Config.ModifyAppVerticalSpacingLength(newSize);
            UserRepository.Update(userResult);
        }

        public void ModifyAppHorizontalSpacing(Int32 userId, Int32 newSize)
        {
            var userResult = GetUser(userId);
            userResult.Config.ModifyAppHorizontalSpacingLength(newSize);
            UserRepository.Update(userResult);
        }

        public List<AppType> GetAppTypes()
        {
            return _appTypeRepository.Entities.ToList();
        }

        public TodayRecommendAppModel GetTodayRecommend(Int32 userId)
        {
            var topApp = _appRepository.Entities.OrderByDescending(app => app.UserCount).FirstOrDefault();

            var userDesks = GetUser(userId).Config.Desks;

            Boolean isInstall = userDesks.Any(userDesk => userDesk.Members.Any(member => member.AppId == topApp.Id && member.IsDeleted == false));

            Double star = topApp.AppStars.Any(appStar => appStar.IsDeleted == false) ? (topApp.AppStars.Sum(s => s.StartNum) * 1.0) / (topApp.AppStars.Count(appStar => appStar.IsDeleted == false) * 1.0) : 0.0;

            return new TodayRecommendAppModel
            {
                AppId = topApp.Id,
                Name = topApp.Name,
                UserCount = topApp.UserCount,
                AppIcon = topApp.IconUrl,
                StartCount = star,
                IsInstall = isInstall,
                Remark = topApp.Remark,
                Style = topApp.AppStyle.ToString().ToLower()
            };
        }

        public Tuple<Int32, Int32> GetUserDevAppAndUnReleaseApp(Int32 userId)
        {
            var userApps = _appRepository.Entities.Where(app => app.UserId == userId);

            var userDevAppCount = userApps.Count();

            var userUnReleaseAppCount = userApps.Count(app => app.AppReleaseState == AppReleaseState.UnRelease);

            return new Tuple<Int32, Int32>(userDevAppCount, userUnReleaseAppCount);
        }

        public List<App> GetAllApps(Int32 userId, Int32 appTypeId, Int32 orderId, String searchText, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            var apps = _appRepository.Entities;

            if (appTypeId != 0 && appTypeId != -1)//全部app
            {
                apps = apps.Where(app => app.AppTypeId == appTypeId);
            }
            else
            {
                if (appTypeId == -1)//用户制作的app
                {
                    apps = apps.Where(app => app.UserId == userId);
                }
            }

            if (orderId == 1)//最新应用
            {
                apps = apps.OrderByDescending(app => app.AddTime);
            }
            else if (orderId == 2)//使用最多
            {
                apps = apps.OrderByDescending(app => app.UserCount);
            }
            else if (orderId == 3)//评价最高
            {
                apps = apps.OrderByDescending(app => app.AppStars.Any(appStar => appStar.IsDeleted == false) ? (app.AppStars.Sum(s => s.StartNum) * 1.0) / app.AppStars.Count(appStar => appStar.IsDeleted == false) * 1.0 : 0);
            }

            if ((searchText + "").Length > 0)//关键字搜索
            {
                apps = apps.Where(app => app.Name.Contains(searchText));
            }

            totalCount = apps.Count();

            return apps.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        }

        public App GetApp(Int32 appId)
        {
            return _appRepository.Entities.FirstOrDefault(app => app.Id == appId);
        }

        public void ModifyAppStar(Int32 userId, Int32 appId, Int32 starCount)
        {
            var userResult = GetUser(userId);

            if (!userResult.Config.Desks.Any(desk => desk.Members.Any(member => member.AppId == appId && member.IsDeleted == false)))
            {
                throw new BusinessException($"请安装这个应用后再打分");
            }

            var appResult = _appRepository.Entities.FirstOrDefault(app => app.Id == appId);

            appResult.AddStar(userId, starCount);

            _appRepository.Update(appResult);
        }

        public void InstallApp(Int32 userId, Int32 appId, Int32 deskNum)
        {
            var userResult = GetUser(userId);

            var realDeskId = GetRealDeskId(deskNum, userResult.Config);

            var appResult = _appRepository.Entities.FirstOrDefault(app => app.Id == appId);

            if (appResult == null)
            {
                throw new BusinessException($"应用添加失败，请刷新重试");
            }

            var newMember = new Member(appResult.Name, appResult.IconUrl, appResult.AppUrl, appResult.Id, appResult.Width, appResult.Height, appResult.IsLock, appResult.IsMax, appResult.IsFull, appResult.IsSetbar, appResult.IsOpenMax, appResult.IsFlash, appResult.IsDraw, appResult.IsResize);

            foreach (var desk in userResult.Config.Desks)
            {
                if (desk.Id == realDeskId)
                {
                    desk.Members.Add(newMember);
                    _deskRepository.Update(desk);

                    appResult.AddUserCount();
                    _appRepository.Update(appResult);

                    break;
                }
            }
        }

        public Boolean IsInstallApp(Int32 userId, Int32 appId)
        {
            var userResult = GetUser(userId);

            return userResult.Config.Desks.Any(desk => desk.Members.Any(member => member.AppId == appId && member.IsDeleted == false));
        }
    }


    /// <summary>
    /// app今日推荐模型
    /// </summary>
    public sealed class TodayRecommendAppModel
    {
        public Int32 AppId { get; set; }

        public String Name { get; set; }

        public Int32 UserCount { get; set; }

        public String AppIcon { get; set; }

        public Double StartCount { get; set; }

        public Boolean IsInstall { get; set; }

        public String Remark { get; set; }

        public String Style { get; set; }
    }
}
