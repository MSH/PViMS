using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VPS.Common.Repositories;

namespace PVIMS.Web.ActionFilters
{
    public class MvcUnitOfWorkAttribute : ActionFilterAttribute
    {
        private IUnitOfWorkInt unitOfWork;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            unitOfWork = DependencyResolver.Current.GetService<IUnitOfWorkInt>();

            if (unitOfWork != null) unitOfWork.Start();

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (unitOfWork != null)
            {
                unitOfWork.Complete();
            }

            base.OnActionExecuted(filterContext);
        }
    }
}