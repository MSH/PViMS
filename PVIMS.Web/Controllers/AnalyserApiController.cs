using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;
using PVIMS.Web.HttpResults;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    public class AnalyserApiController : ApiController
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        public AnalyserApiController(IUnitOfWorkInt unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        [WebApiUnitOfWork]
        public IHttpActionResult GetCohortDetails(int id)
        {
            var cohortGroup = _unitOfWork.Repository<CohortGroup>()
                .Queryable()
                .Include(cg => cg.CohortGroupEnrolments)
                .Include(cg => cg.Condition)
                .Single(cg => cg.Id == id);

            StringBuilder sb = new StringBuilder();

            sb.Append("<ul>");
            sb.AppendFormat("<li>Primary Condition Group: <b>{0}</b></li>", cohortGroup.Condition != null ? cohortGroup.Condition.Description : "Not defined");
            sb.AppendFormat("<li>Date Range: <b>{0} - {1}</b></li>", cohortGroup.StartDate.ToString("yyyy-MM-dd"), cohortGroup.FinishDate != null ? Convert.ToDateTime(cohortGroup.FinishDate).ToString("yyyy-MM-dd") : "No End Date");
            sb.AppendFormat("<li>Number Patients: <b>{0}</b></li>", cohortGroup.CohortGroupEnrolments.Where(cg => !cg.Archived && !cg.Patient.Archived).Count().ToString());
            sb.Append("</ul>");

            return Ok<string>(sb.ToString());
        }
    }
}
