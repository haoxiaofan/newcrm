﻿using NewCRM.Domain.Entitys.Security;
using NewCRM.Domain.Repositories.IRepository.Security;
using NewCRM.Repository.DataBaseProvider;

namespace NewCRM.Repository.RepositoryImpl.Security
{
    public class RoleRepository : EntityFrameworkProvider<Role>,IRoleRepository
    {
    }
}
