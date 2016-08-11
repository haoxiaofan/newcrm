﻿using System;
using System.Collections.Generic;

namespace NewCRM.Dto.Dto
{
    public sealed class RoleDto : BaseDto
    {
        public String Name { get; set; }

        public List<PowerDto> Powers { get; set; }
    }
}