﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewCRM.Domain.DomainModel.System;
using NewCRM.Infrastructure.Repositories.RepositoryProvide;

namespace NewCRM.Infrastructure.Repositories.Configuration.System.Imp
{
    internal partial class AppConfiguration : EntityTypeConfiguration<App>, IEntityMapper
    {

        public AppConfiguration()
        {
            HasKey(a => a.Id);



            HasRequired(a => a.AppType).WithMany(a => a.Apps).Map(a => a.MapKey("AppTypeId"));


            HasOptional(a => a.User).WithMany(a => a.Apps).Map(a => a.MapKey("UserId"));

        }

        public void RegistTo(ConfigurationRegistrar configurations) { configurations.Add(this); }
    }
}
