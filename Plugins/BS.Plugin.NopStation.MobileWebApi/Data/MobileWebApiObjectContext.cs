using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Data;
using Nop.Data.Extensions;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public class MobileWebApiObjectContext : DbContext, IDbContext
    {
        public MobileWebApiObjectContext(DbContextOptions<MobileWebApiObjectContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mobile Web API Configuration
            modelBuilder.ApplyConfiguration(new DeviceMap());
            modelBuilder.ApplyConfiguration(new BS_FeaturedProductsMap());
            modelBuilder.ApplyConfiguration(new BS_ContentManagementTemplateMap());
            modelBuilder.ApplyConfiguration(new BS_ContentManagementMap());
            modelBuilder.ApplyConfiguration(new BS_CategoryIconsMap());
            modelBuilder.ApplyConfiguration(new BS_SliderMap());

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Generate a script to create all tables for the current model
        /// </summary>
        /// <returns>A SQL script</returns>
        public virtual string GenerateCreateScript()
        {
            return this.Database.GenerateCreateScript();
        }

        /// <summary>
        /// Install
        /// </summary>
        public void Install()
        {
            this.ExecuteSqlScript(this.GenerateCreateScript());
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            //drop the table
            this.DropPluginTable("BS_WebApi_Device");
            this.DropPluginTable("BS_FeaturedProducts");
            this.DropPluginTable("BS_ContentManagement");
            this.DropPluginTable("BS_ContentManagementTemplate");
            this.DropPluginTable("BS_CategoryIcons");
            this.DropPluginTable("BS_Slider");
        }


        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public virtual void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new NotImplementedException();

            //if (entity == null)
            //    throw new ArgumentNullException("entity");

            //((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            using (var transaction = this.Database.BeginTransaction())
            {
                var result = this.Database.ExecuteSqlCommand(sql, parameters);
                transaction.Commit();

                return result;
            }
        }
    }
}
