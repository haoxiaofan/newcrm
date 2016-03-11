﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NewCRM.Domain.DomainModel;
using NewCRM.Domain.Repositories;
using NewCRM.Domain.UnitWork;
using NewCRM.Infrastructure.CommonTools.CustemException;
using NewCRM.Infrastructure.CommonTools.CustomHelper;
using NewCRM.Infrastructure.Repositories.UnitOfWorkProvide;

namespace NewCRM.Infrastructure.Repositories.RepositoryProvide
{
    /// <summary>
    ///     EntityFramework仓储操作基类
    /// </summary>
    /// <typeparam name="TEntity">动态实体类型</typeparam>
    /// <typeparam name="TKey">实体主键类型</typeparam>
    public abstract class EfRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        #region 属性

        private IUnitOfWork _unitOfWork;
        /// <summary>
        ///  获取工作单元上下文
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork = new EfUnitOfWorkContext(); }
        }

        /// <summary>
        ///     获取 EntityFramework的数据仓储上下文
        /// </summary>
        protected UnitOfWorkContextBase EfContext
        {
            get
            {
                var unitOfWork = (UnitOfWork as UnitOfWorkContextBase);
                if (unitOfWork == null)
                {
                    throw new RepositoryException("无法获取当前工作单元的实例");
                }
                return unitOfWork;
            }
        }

        /// <summary>
        ///  获取 当前实体的查询数据集
        /// </summary>
        public virtual IQueryable<TEntity> Entities
        {
            get { return EfContext.Set<TEntity, TKey>(); }
        }


        #endregion

        #region 公共方法

        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Add(TEntity entity, bool isSave = true)
        {
            Parameter.Vaildate(entity);
            EfContext.RegisterNew<TEntity, TKey>(entity);
            if (isSave)
            {
                EfContext.Commit();
            }
            else
            {
                throw new RepositoryException("数据添加失败");
            }
        }

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Add(IEnumerable<TEntity> entities, bool isSave = true)
        {
            Parameter.Vaildate(entities);
            EfContext.RegisterNew<TEntity, TKey>(entities);
            if (isSave)
            {
                EfContext.Commit();
            }
            else
            {
                throw new RepositoryException("数据添加失败");
            }
        }

        /// <summary>
        ///     删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Remove(TKey id, bool isSave = true)
        {
            Parameter.Vaildate(id);
            TEntity entity = EfContext.Set<TEntity, TKey>().Find(id);
            if (entity != null)
            {
                Remove(entity, isSave);
            }
            else
            {
                throw new RepositoryException("数据移除失败");
            }
        }

        /// <summary>
        ///     删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Remove(TEntity entity, bool isSave = true)
        {
            Parameter.Vaildate(entity);
            EfContext.RegisterDeleted<TEntity, TKey>(entity);
            if (isSave)
            {
                EfContext.Commit();
            }
            else
            {
                throw new RepositoryException("数据移除失败");
            }
        }

        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Remove(IEnumerable<TEntity> entities, bool isSave = true)
        {
            Parameter.Vaildate(entities);
            EfContext.RegisterDeleted<TEntity, TKey>(entities);
            if (isSave)
            {
                EfContext.Commit();
            }
            else
            {
                throw new RepositoryException("数据添加失败");
            }
        }

        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Remove(Expression<Func<TEntity, bool>> predicate, bool isSave = true)
        {
            Parameter.Vaildate(predicate);
            IList<TEntity> entities = EfContext.Set<TEntity, TKey>().Where(predicate).ToList();
            if (entities.Any())
            {
                Remove(entities, isSave);
            }
            else
            {
                throw new RepositoryException("数据移除失败");
            }
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        public virtual void Update(TEntity entity, bool isSave = true)
        {
            Parameter.Vaildate(entity);
            EfContext.RegisterModified<TEntity, TKey>(entity);
            if (isSave)
            {
                EfContext.Commit();
            }
            else
            {
                throw new RepositoryException("数据更新失败");
            }
        }

        /// <summary>
        /// 使用附带新值的实体信息更新指定实体属性的值
        /// </summary>
        /// <param name="propertyExpression">属性表达式</param>
        /// <param name="isSave">是否执行保存</param>
        /// <param name="entity">附带新值的实体信息，必须包含主键</param>
        public void Update(Expression<Func<TEntity, object>> propertyExpression, TEntity entity, bool isSave = true)
        {
            throw new NotSupportedException("上下文公用，不支持按需更新功能。");
            //PublicHelper.VaildateArgument(propertyExpression, "propertyExpression");
            //PublicHelper.VaildateArgument(entity, "entity");
            //EfContext.RegisterModified<TEntity, TKey>(propertyExpression, entity);
            //if (isSave)
            //{
            //    var dbSet = EfContext.Set<TEntity, TKey>();
            //    dbSet.Local.Clear();
            //    var entry = EfContext.DbContext.Entry(entity);
            //    return EfContext.Commit(false);
            //}
            //return 0;
        }
        #endregion
    }
}