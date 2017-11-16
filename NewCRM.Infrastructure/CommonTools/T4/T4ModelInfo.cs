﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NewCRM.Infrastructure.CommonTools.T4
{
    public class T4ModelInfo
    {
        /// <summary>
        /// 获取 是否使用模块文件夹
        /// </summary>
        public bool UseModuleDir { get; private set; }

        /// <summary>
        /// 获取 模型所在模块名称
        /// </summary>
        public String ModuleName { get; private set; }

        /// <summary>
        /// 获取 模型名称
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// 获取 模型描述
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public Type KeyType { get; set; }

        /// <summary>
        /// 主键类型名称
        /// </summary>
        public String KeyTypeName { get; set; }
        public IEnumerable<PropertyInfo> Properties { get; private set; }

        public T4ModelInfo(Type modelType, Boolean isinherit=false)
        {
            var @namespace = modelType.Namespace;
            if (@namespace == null)
            {
                return;
            }
            UseModuleDir = isinherit;
            if (UseModuleDir)
            {
                var index = @namespace.LastIndexOf('.') + 1;
                ModuleName = @namespace.Substring(index, @namespace.Length - index);
            }

            Name = modelType.Name;
            PropertyInfo keyProp = modelType.GetProperty("Id");
            KeyType = keyProp.PropertyType;
            KeyTypeName = KeyType.Name;

            var descAttributes = modelType.GetCustomAttributes(typeof(DescriptionAttribute), true);
            Description = descAttributes.Length == 1 ? ((DescriptionAttribute)descAttributes[0]).Description : Name;
            Properties = modelType.GetProperties();
        }
    }
}