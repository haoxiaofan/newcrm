﻿using System;
using System.Linq;
using NewCRM.Domain.Entitys.Agent;
using NewCRM.Domain.Entitys.System;
using NewCRM.Domain.Services.Interface;
using NewCRM.Domain.ValueObject;
using NewCRM.Infrastructure.CommonTools.CustomException;
using NewCRM.Domain.Repositories.IRepository.System;
using NewCRM.Domain.Repositories.IRepository.Agent;

namespace NewCRM.Domain.Services.BoundedContextMember
{
    public sealed class ModifyDockPostionServices : BaseServiceContext, IModifyDockPostionServices
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IAccountRepository _accountRepository;

        public ModifyDockPostionServices(IDeskRepository deskRepository, IAccountRepository accountRepository)
        {
            _deskRepository = deskRepository;
            _accountRepository = accountRepository;
        }

        public void ModifyDockPosition(Int32 accountId, Int32 defaultDeskNumber, String newPosition)
        {
            ValidateParameter.Validate(accountId).Validate(defaultDeskNumber).Validate(newPosition);

            var accountResult = DatabaseQuery.FindOne(FilterFactory.Create((Account account) => account.Id == accountId));
            DockPostion dockPostion;

            if (Enum.TryParse(newPosition, true, out dockPostion))
            {
                if (dockPostion == DockPostion.None)
                {
                    var deskResult = DatabaseQuery.FindOne(FilterFactory.Create<Desk>(desk => desk.DeskNumber == accountResult.Config.DefaultDeskNumber));
                    var dockMembers = deskResult.Members.Where(member => member.IsOnDock).ToList();
                    if (dockMembers.Any())
                    {
                        dockMembers.ForEach(f =>
                        {
                            f.OutDock();
                        });

                        _deskRepository.Update(deskResult);
                    }
                    accountResult.Config.ModifyDockPostion(dockPostion);
                }
                else
                {
                    accountResult.Config.ModifyDockPostion(dockPostion);
                }
            }
            else
            {
                throw new BusinessException($"未识别出的码头位置:{newPosition}");
            }
            accountResult.Config.ModifyDefaultDesk(defaultDeskNumber);
            _accountRepository.Update(accountResult);
        }
    }
}