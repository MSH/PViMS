using System.Linq;
using System.Web.Http;

using PVIMS.Core.Entities;

using VPS.Common.Repositories;

namespace PVIMS.Web.Controllers
{
     public class CohortApiController : ApiController
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        public CohortApiController(IUnitOfWorkInt unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetCohortGroups()
        {
            var cohortGroups = _unitOfWork.Repository<CohortGroup>()
                .Queryable()
                .OrderBy(cg => cg.CohortName)
                .Select(cg => new { Id = cg.Id, CohortName = cg.CohortName });

            return Ok(cohortGroups.ToArray());
        }
    }
}