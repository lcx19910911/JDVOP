using Entity;
using JDVOP.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DbRepository : DbContext
    {

        public DbRepository()
           : base("name=DbRepository")
        { 
          //  Database.SetInitializer(new CreateDatabaseIfNotExists<DbRepository>());

            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;//关闭自动跟踪对象的属性变化
            //this.Configuration.ValidateOnSaveEnabled = false; //关闭保存时的实体验证
            //this.Configuration.UseDatabaseNullSemantics = false;//关闭数据库null比较行为


        }

        public override int SaveChanges()
        {
            try
            {
                //var entries = from e in this.ChangeTracker.Entries()
                //              where e.State != EntityState.Unchanged
                //              select e;   //过滤所有修改了的实体，包括：增加 / 修改 / 删除


                //foreach (var entry in entries)
                //{

                //    InitObject(entry);
                //}

                return base.SaveChanges();

            }
            catch (Exception ex)
            {
                //并发冲突数据
                if (ex.GetType() == typeof(DbUpdateConcurrencyException))
                {
                    return -1;
                }
                return 0;
            }

        }

        public DbSet<ProductInfo> ProductInfo { get; set; }

        public DbSet<CategoryInfo> CategoryInfo { get; set; }

        public DbSet<JDOrderDto> JDOrderInfo{ get; set; }

        public DbSet<SMSBatch> SMSBatch { get; set; }
    }

}
