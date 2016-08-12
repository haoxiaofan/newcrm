﻿using System;

namespace NewCRM.Domain.Entities.DomainModel.Security
{
    public partial class Power
    {
        #region public method

        /// <summary>
        /// 修改权限名称
        /// </summary>
        /// <param name="newPowerName"></param>
        public Power ModifyPowerName(String newPowerName)
        {
            Name = newPowerName;
            return this;
        }

        /// <summary>
        /// 修改权限的标识
        /// </summary>
        /// <param name="newPowerIdentity"></param>
        /// <returns></returns>
        public Power ModifyPowerIdentity(String newPowerIdentity)
        {
            PowerIdentity = newPowerIdentity;
            return this;
        }

        /// <summary>
        /// 移除权限
        /// </summary>
        public void Remove()
        {
            IsDeleted = true;
        }


        #endregion
    }
}
