using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VPS.Common.Repositories;
using System.Net.Http;

namespace PVIMS.Web
{
    public class WebApiUnitOfWorkAttribute : ActionFilterAttribute
    {
        private IUnitOfWorkInt unitOfWork;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            unitOfWork = actionContext.Request.GetDependencyScope().GetService(typeof(IUnitOfWorkInt)) as IUnitOfWorkInt;

            if (unitOfWork != null) unitOfWork.Start();

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (unitOfWork != null)
            {
                unitOfWork.Complete();
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}