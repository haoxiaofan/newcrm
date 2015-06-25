﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NewCRM.Domain.DomainModel.System;

namespace NewCRM.Domain.DomainModel.Security
{
    [Description("权限")]
    [Serializable]
    public class Power : EntityBase<Int32>, IAggregationRoot
    {
        #region private field
        private String _name;
        private String _groupName;
        private String _title;
        private String _remark;
        private PowerType _powerType;
        private ICollection<Role> _roles;
        private ICollection<Folder> _folders;
        #endregion

        #region ctor

        public Power(String name, String title, String remake)
            : this()
        {
            _name = name;
            _title = title;
            _remark = remake;
        }

        private Power()
        {
            
        }

        #endregion

        #region public attribute
        public PowerType PowerType
        {
            get
            {
                return _powerType;
            }
            set { _powerType = value; }
        }

        public ICollection<Folder> Folders
        {
            get { return _folders; }
            set { _folders = value; }
        }

        [Required, StringLength(50)]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [StringLength(50)]
        public String GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        [StringLength(200)]
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [StringLength(500)]
        public String Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
        public virtual ICollection<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }
        #endregion
    }
}
