﻿using System;
using NewCRM.Domain.ValueObject;

namespace NewCRM.Dto
{
    public sealed class TodayRecommendAppDto : BaseDto
    {
        public String Name { get; set; }

        public Int32 UseCount { get; set; }

        public String AppIcon { get; set; }

        public Double AppStars { get; set; }

        public Boolean IsInstall { get; set; }

        public String Remark { get; set; }

        public AppStyle Style { get; set; }

        public Boolean IsIconByUpload { get; set; }
    }
}
