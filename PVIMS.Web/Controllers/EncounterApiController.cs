using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    public class EncounterApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfWork;

        public EncounterApiController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/EncounterApi
        public async Task<IEnumerable<EncounterDTO>> Get()
        {
            return await Task.Run(() => unitOfWork.Repository<Encounter>()
                .Queryable()
                .ToList()
                .Select(e => new EncounterDTO
                {
                    EncounterIdentifier = e.EncounterGuid,
                    PatientId = e.Patient.Id,
                    EncounterId = e.Id,
                    EncounterDate = e.EncounterDate,
                    Notes = e.Notes,
                    EncounterPriority = e.Priority.Id,
                    EncounterType = e.EncounterType.Id,
                    EncounterCreatedDate = e.Created,
                    EncounterUpdatedDate = e.LastUpdated,
                    CreatedBy = e.CreatedBy.FullName,
                    UpdatedBy = e.UpdatedBy != null ? e.UpdatedBy.FullName : null
                }).ToArray());
        }

        [ActionName("lookUpByDate")]
        public async Task<IEnumerable<EncounterDTO>> Get(DateTime id)
        {
            return await Task.Run(() => unitOfWork.Repository<Encounter>()
                .Queryable()
                .Where(k => k.LastUpdated >= id || k.Created >= id)
                .ToList()
                .Select(e => new EncounterDTO
                {
                    EncounterIdentifier = e.EncounterGuid,
                    PatientId = e.Patient.Id,
                    EncounterId = e.Id,
                    EncounterDate = e.EncounterDate,
                    Notes = e.Notes,
                    EncounterPriority = e.Priority.Id,
                    EncounterType = e.EncounterType.Id,
                    EncounterCreatedDate = e.Created,
                    EncounterUpdatedDate = e.LastUpdated,
                    CreatedBy = e.CreatedBy.FullName,
                    UpdatedBy = e.UpdatedBy != null ? e.UpdatedBy.FullName : null
                }).ToArray());
        }

        [ActionName("lookUpTodayEncounters")]
        public async Task<IEnumerable<EncounterDTO>> GetTodayEncounters()
        {
            DateTime today = DateTime.Today;
            return await Task.Run(() => unitOfWork.Repository<Encounter>()
                .Queryable()
                .Where(k => k.EncounterDate == today)
                .ToList()
                .Select(e => new EncounterDTO
                {
                    EncounterIdentifier = e.EncounterGuid,
                    PatientId = e.Patient.Id,
                    EncounterId = e.Id,
                    EncounterDate = e.EncounterDate,
                    Notes = e.Notes,
                    EncounterPriority = e.Priority.Id,
                    EncounterType = e.EncounterType.Id,
                    EncounterCreatedDate = e.Created,
                    EncounterUpdatedDate = e.LastUpdated,
                    CreatedBy = e.CreatedBy.FullName,
                    UpdatedBy = e.UpdatedBy != null ? e.UpdatedBy.FullName : null
                }).ToArray());
        }

        [ActionName("encounterTypes")]
        public async Task<IEnumerable<Object>> GetEncounterTypes()
        {
            return await Task.Run(() => unitOfWork.Repository<EncounterType>()
                .Queryable()
                .ToList()
                .Select(e => new 
                {
                    Id = e.Id,
                    Description = e.Description
                }).ToArray());
        }

        [ActionName("priorities")]
        public async Task<IEnumerable<Object>> GetPriorities()
        {
            return await Task.Run(() => unitOfWork.Repository<Priority>()
                .Queryable()
                .ToList()
                .Select(e => new 
                {
                    Id = e.Id,
                    Description = e.Description
                }).ToArray());
        }

        // POST: api/EncounterApi
        [HttpPost]
        public HttpResponseMessage Post(JObject data)
        {
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = data;

            List<Encounter> synchedEncounters = new List<Encounter>();

            IList<EncounterDTO> encounters = ((JArray)json.items)
                .Select(t => new EncounterDTO
                {
                    EncounterIdentifier = ((dynamic)t).EncounterIdentifier,
                    EncounterDate = ((dynamic)t).EncounterDate != "" ? ((dynamic)t).EncounterDate : null,
                    EncounterId = ((dynamic)t).EncounterId,
                    EncounterPriority = ((dynamic)t).EncounterPriority,
                    EncounterType = ((dynamic)t).EncounterType,
                    CreatedBy = ((dynamic)t).CreatedBy,
                    UpdatedBy = ((dynamic)t).UpdatedBy,
                    PatientId = ((dynamic)t).PatientId,
                    Notes = ((dynamic)t).Notes,
                    EncounterCreatedDate = ((dynamic)t).EncounterCreatedDate != "" ? ((dynamic)t).EncounterCreatedDate : null,
                    EncounterUpdatedDate = ((dynamic)t).EncounterUpdatedDate != "" ? ((dynamic)t).EncounterUpdatedDate : null
                }).ToList();

            foreach (EncounterDTO encounter in encounters)
            {
                var obj = unitOfWork.Repository<Encounter>()
                    .Queryable()
                    .SingleOrDefault(e => e.EncounterGuid == encounter.EncounterIdentifier);

                var encounterType = unitOfWork.Repository<EncounterType>().Queryable().SingleOrDefault(et => et.Id == encounter.EncounterType);
                var priority = unitOfWork.Repository<Priority>().Queryable().SingleOrDefault(p => p.Id == encounter.EncounterPriority);                

                if (obj == null)
                {
                    Patient patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(e => e.Id == encounter.PatientId);
                    //var currentUser = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == encounter.CreatedBy);

                    obj = new Encounter(patient)
                    {
                        EncounterGuid = encounter.EncounterIdentifier,
                       // Id = encounter.EncounterId,
                        EncounterDate = encounter.EncounterDate,
                        EncounterType = encounterType,
                        Priority = priority,
                        ArchivedDate = null,
                        //Created = encounter.EncounterCreatedDate,
                        //CreatedBy = currentUser,
                        Notes = encounter.Notes
                    };

                    unitOfWork.Repository<Encounter>().Save(obj);
                    synchedEncounters.Add(obj);
                }
                else
                {
                    obj.EncounterDate = encounter.EncounterDate;
                    obj.EncounterType = encounterType;
                    obj.Priority = priority;
                    obj.Notes = encounter.Notes;

                    unitOfWork.Repository<Encounter>().Update(obj);
                    synchedEncounters.Add(obj);
                }
            }

            unitOfWork.Complete();

            var insertedObjs = synchedEncounters.Select(e => new EncounterDTO
            {
                EncounterIdentifier = e.EncounterGuid,
                PatientId = e.Patient.Id,
                EncounterId = e.Id,
                EncounterDate = e.EncounterDate,
                Notes = e.Notes,
                EncounterPriority = e.Priority.Id,
                EncounterType = e.EncounterType.Id,
                EncounterCreatedDate = e.Created,
                EncounterUpdatedDate = e.LastUpdated,
                CreatedBy = e.CreatedBy.FullName,
                UpdatedBy = e.UpdatedBy != null ? e.UpdatedBy.FullName : null
            }).ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, insertedObjs);
        }

        // GET: api/EncounterApi/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT: api/EncounterApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EncounterApi/5
        public void Delete(int id)
        {
        }
    }
}
