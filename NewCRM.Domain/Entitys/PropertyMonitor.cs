﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace NewCRM.Domain.Entitys
{
    public class PropertyArgs : EventArgs
    {
        public String PropertyName { get; private set; }

        public Object PropertyValue { get; private set; }

        public PropertyArgs(String propertyName, Object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }

    public abstract class PropertyMonitor
    {
        public delegate void PropertyChangeEventHandler(Object sender, PropertyArgs args);

        public event PropertyChangeEventHandler PropertyChanged;

        private IDictionary<String, Object> _propertyValues = new Dictionary<String, Object>();

        public PropertyMonitor()
        {
            if(PropertyChanged == null)
            {
                PropertyChanged += PropertyMonitor_PropertyChanged;
            }
        }

        private void PropertyMonitor_PropertyChanged(Object sender, PropertyArgs e)
        {
            var instanceName = $@"{GetType().FullName}.{e.PropertyName}";
            if(!_propertyValues.Keys.Contains(instanceName))
            {
                _propertyValues.Add(new KeyValuePair<String, Object>(instanceName, e.PropertyValue));
            }
        }

        public void OnPropertyChanged(String propertyName)
        {
            var temp = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            var property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if(temp != null)
            {
                temp(this, new PropertyArgs(propertyName, property.GetValue(this)));
            }
        }

        public IDictionary<String, Object> GetPropertyValues()
        {
            return _propertyValues;
        }
    }
}