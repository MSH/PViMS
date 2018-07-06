using Microsoft.Data.OData;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    public class PatientsController : ODataController
    {
        private readonly IUnitOfWorkInt unitOfWork;
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public PatientsController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: odata/PatientOData
        public async Task<IHttpActionResult> Get(ODataQueryOptions<PatientDTO> queryOptions)
        {
            try
            {
                queryOptions.Validate(_validationSettings);

                var patients = await Task.Run(() => unitOfWork.Repository<Patient>()
                    .Queryable()
                    .ToList()
                    .Select(p => new PatientDTO
                    {
                        PatientId = p.Id,
                        PatientUniqueIdentifier = p.PatientGuid,
                        PatientFirstName = p.FirstName,
                        PatientLastName = p.Surname,
                        PatientDateOfBirth = p.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                    }).ToArray());

                return Ok(patients);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: odata/PatientOData(5)
        public async Task<IHttpActionResult> Get([FromODataUri] int key, ODataQueryOptions<PatientDTO> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            // return Ok<PatientDTO>(patientDTO);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PUT: odata/PatientOData(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<PatientDTO> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Put(patientDTO);

            // TODO: Save the patched entity.

            // return Updated(patientDTO);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // POST: odata/PatientOData
        public async Task<IHttpActionResult> Post(PatientDTO patientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Add create logic here.

            // return Created(patientDTO);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PATCH: odata/PatientOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<PatientDTO> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Patch(patientDTO);

            // TODO: Save the patched entity.

            // return Updated(patientDTO);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // DELETE: odata/PatientOData(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            // TODO: Add delete logic here.

            // return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
