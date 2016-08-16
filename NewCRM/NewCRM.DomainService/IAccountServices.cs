﻿using System;
using System.Collections.Generic;
using NewCRM.Domain.Entities.DomainModel.Account;

namespace NewCRM.Domain.Services
{
    public interface IAccountServices
    {
        /// <summary>
        /// 验证用户是否合法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User Validate(String userName, String password);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userId"></param>
        void Logout(Int32 userId);

        /// <summary>
        /// 用户禁用
        /// </summary>
        /// <param name="userId"></param>
        void Disable(Int32 userId);

        /// <summary>
        /// 用户启用
        /// </summary>
        /// <param name="userId"></param>
        void Enable(Int32 userId);

        /// <summary>
        /// 获取用户的配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserConfig(Int32 userId);

        /// <summary>
        /// 获取所有的用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<dynamic> GetAllUsers(String userName, Int32 userType, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 根据userId获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUser(Int32 userId);

    }
}
