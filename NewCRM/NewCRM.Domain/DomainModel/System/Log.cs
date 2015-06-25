﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace NewCRM.Domain.DomainModel.System
{
    [Description("日志")]
    [Serializable]
    public class Log : EntityBase<Int32>
    {
        #region private field
        private String _level;
        private String _logger;
        private String _message;
        private String _exception;
        #endregion

        #region ctor

        public Log(String level, String logger, String message, String exception)
            : this()
        {
            _level = level;
            _logger = logger;
            _message = message;
            _exception = exception;
        }

        public Log()
        {
            // TODO: Complete member initialization
        }

        #endregion

        #region public attribute



        [StringLength(20)]
        public String Levels
        {
            get { return _level; }
            set { _level = value; }
        }

        [StringLength(200)]
        public String Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        [StringLength(4000)]
        public String Message
        {
            get { return _message; }
            set { _message = value; }
        }

        [StringLength(4000)]
        public String Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        #endregion
    }
}
