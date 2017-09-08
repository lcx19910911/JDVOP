using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Database.SetInitializer<DbRepository>(null);
            ///预加载
            using (var dbcontext = new DbRepository())
            {
                dbcontext.Database.CreateIfNotExists();
                var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<EdmSchemaError>());
               
            }
        }

        protected void Application_PreSendRequestContent()
        {
        }

        protected void Application_EndRequest()
        {
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            this.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            this.Response.Headers.Add("Access-Control-Allow-Methods", "get, put, post, delete, options");
            this.Response.Headers.Add("Access-Control-Allow-Headers", "authorization, origin, content-type, accept");
        }
    }
}
