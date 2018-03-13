﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NewCRM.Domain.Entitys.Agent
{
    public class AccountRole : DomainModelBase
    {
        #region private field
        private Int32 _accountId;

        private Int32 _roleId;
        #endregion

        #region public property
        [Required]
        public Int32 AccountId
        {
            get
            {
                return _accountId;
            }
            private set
            {
                if(_accountId != value)
                {
                    _accountId = value;
                    OnPropertyChanged(nameof(AccountId));
                }
            }
        }

        [Required]
        public Int32 RoleId
        {
            get
            {
                return _roleId;
            }
            private set
            {
                if(_roleId != value)
                {
                    _roleId = value;
                    OnPropertyChanged(nameof(RoleId));
                }
            }
        }

        #endregion

        #region ctor

        public AccountRole(Int32 accountId, Int32 roleId)
        {
            AccountId = accountId;
            RoleId = roleId;
        }

        public AccountRole() { }

        public void Remove() => IsDeleted = true;

        #endregion
    }
}
