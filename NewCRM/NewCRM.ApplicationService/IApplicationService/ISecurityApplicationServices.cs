﻿using System;
using System.Collections.Generic;
using NewCRM.Dto.Dto;

namespace NewCRM.Application.Services.IApplicationService
{
    public interface ISecurityApplicationServices
    {
        /// <summary>
        /// 获取全部的角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<RoleDto> GetAllRoles(String roleName, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="roleId"></param>
        void RemoveRole(Int32 roleId);
    }
}
