using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Services
{
    public class ReportService : IReportService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        public ReportService(IUnitOfWorkInt unitOfWork)
        {
            Check.IsNotNull(unitOfWork, "unitOfWork may not be null");

            _unitOfWork = unitOfWork;
        }

        public ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria)
        {
            string sql = "";

            if (adverseEventCriteria == AdverseEventCriteria.ReportSource)
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                            ORDER BY mpce.SourceTerminologyMedDra asc, Criteria asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpf.Facility AS Criteria,
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
	                            INNER JOIN MetaPatientFacility mpf ON mp.Id = mpf.Patient_Id AND mpf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM MetaPatientFacility impf WHERE impf.Patient_Id = mp.Id)
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY mpce.SourceTerminologyMedDra, mpf.Facility
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpf.Facility asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpm.Medication AS Criteria,
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN MetaPatientMedication mpm ON rim.ReportInstanceMedicationGuid = mpm.PatientMedicationGuid
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, mpm.Medication 
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpm.Medication asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }
            else // MedDRA
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
		                            COUNT(*) AS PatientCount
                            FROM PatientClinicalEvent pce 
	                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
	                            INNER JOIN TerminologyMedDra t ON pce.TerminologyMedDra_Id1 = t.Id
                            WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0
                            GROUP BY t.MedDraTerm, 
			                            CASE WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                            ORDER BY t.MedDraTerm asc, Criteria asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            f.FacilityName AS Criteria,
		                            COUNT(*) AS PatientCount
                            FROM PatientClinicalEvent pce 
	                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
	                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM PatientFacility ipf WHERE ipf.Patient_Id = p.Id)
	                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
	                            INNER JOIN TerminologyMedDra t ON pce.TerminologyMedDra_Id1 = t.Id
                            WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0
                            GROUP BY t.MedDraTerm, f.FacilityName
                            ORDER BY t.MedDraTerm asc, f.FacilityName asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            m.DrugName AS Criteria,
		                            COUNT(*) AS PatientCount
                            FROM PatientClinicalEvent pce 
	                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = pce.PatientClinicalEventGuid
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN PatientMedication pm ON rim.ReportInstanceMedicationGuid = pm.PatientMedicationGuid
	                            INNER JOIN Medication m ON pm.Medication_Id = m.Id
	                            INNER JOIN TerminologyMedDra t ON pce.TerminologyMedDra_Id1 = t.Id
                            WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0 and pm.Archived = 0
                            GROUP BY t.MedDraTerm, m.DrugName
                            ORDER BY t.MedDraTerm asc, m.DrugName asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<AdverseEventList>().ExecuteSql(sql, parameters);
        }

        public ICollection<AdverseEventQuarterlyList> GetAdverseEventQuarterlyItems(DateTime searchFrom, DateTime searchTo)
        {
            string sql = "";

            sql = string.Format(@"
                    SELECT b.PeriodYear, b.PeriodQuarter, b.FacilityName, tm1.MedDraTerm, b.PatientCount
	                    FROM TerminologyMedDra tm1 
		                    LEFT JOIN 
	                    (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], DATEPART(QUARTER, pce.OnsetDate) as [PeriodQuarter],
		                    f.FacilityName,
		                    t5.MedDraTerm AS 'Description', 
			                    COUNT(*) AS PatientCount
	                    FROM PatientClinicalEvent pce 
		                    INNER JOIN Patient p ON pce.Patient_Id = p.Id
		                    INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id
		                    INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
		                    INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
		                    INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
		                    INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
		                    INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
		                    INNER JOIN Facility f on pf.Facility_Id = f.Id
	                    WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0 
	                    GROUP BY DATEPART(YEAR, pce.OnsetDate), DATEPART(QUARTER, pce.OnsetDate), f.FacilityName, t5.MedDraTerm) as b on tm1.MedDraTerm = b.[Description] 
	                    WHERE tm1.MedDraTermType = 'SOC'
	                    ORDER BY tm1.MedDraTerm, b.PeriodYear, b.PeriodQuarter, b.FacilityName
                    ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<AdverseEventQuarterlyList>().ExecuteSql(sql, parameters);
        }

        public ICollection<AdverseEventAnnualList> GetAdverseEventAnnualItems(DateTime searchFrom, DateTime searchTo)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT b.PeriodYear, b.FacilityName, tm1.MedDraTerm, b.PatientCount
	            FROM TerminologyMedDra tm1 
		            LEFT JOIN 
	            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], 
		            f.FacilityName,
		            t5.MedDraTerm AS 'Description', 
			            COUNT(*) AS PatientCount
	            FROM PatientClinicalEvent pce 
		            INNER JOIN Patient p ON pce.Patient_Id = p.Id
		            INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id
		            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
		            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
		            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
		            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
		            INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
		            INNER JOIN Facility f on pf.Facility_Id = f.Id
	            WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0 
	            GROUP BY DATEPART(YEAR, pce.OnsetDate), f.FacilityName, t5.MedDraTerm) as b on tm1.MedDraTerm = b.[Description] 
	            WHERE tm1.MedDraTermType = 'SOC'
	            ORDER BY tm1.MedDraTerm, b.PeriodYear, b.FacilityName
                    ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<AdverseEventAnnualList>().ExecuteSql(sql, parameters);
        }

        public ICollection<CausalityNotSetList> GetCausalityNotSetItems(DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causalityCriteria)
        {
            string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";
            switch (causalityConfig)
            {
                case CausalityConfigType.BothScales:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.NaranjoCausality IS NOT NULL AND rim.WhoCausality IS NOT NULL " : " AND (rim.NaranjoCausality IS NULL OR rim.WhoCausality IS NULL) ";
                    break;

                case CausalityConfigType.WHOScale:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.WhoCausality IS NOT NULL " : " AND rim.WhoCausality IS NULL ";
                    break;

                case CausalityConfigType.NaranjoScale:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.NaranjoCausality IS NOT NULL " : " AND rim.NaranjoCausality IS NULL ";
                    break;
            }

            string sql = string.Format(@"
                SELECT p.Id AS Patient_Id, p.FirstName, p.Surname, t.MedDraTerm AS AdverseEvent, pce.OnsetDate, rim.NaranjoCausality, rim.WhoCausality, rim.MedicationIdentifier 
                    FROM ReportInstance ri
		                INNER JOIN PatientClinicalEvent pce ON ri.ContextGuid = pce.PatientClinicalEventGuid 
                        INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id
		                INNER JOIN Patient p on pce.Patient_Id = p.Id
                        INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                        INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id 
                WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' 
	                AND pce.Archived = 0 AND p.Archived = 0 {2} 
                ORDER BY pce.OnsetDate asc ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), where);

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<CausalityNotSetList>().ExecuteSql(sql, parameters);
        }

        public ICollection<OutstandingVisitList> GetOutstandingVisitItems(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";

            string sql = string.Format(@"
                SELECT p.Id AS Patient_Id, p.FirstName, p.Surname, a.AppointmentDate FROM Patient p 
	                INNER JOIN Appointment a ON p.Id = a.Patient_Id 
                    INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                WHERE a.AppointmentDate < DATEADD(dd, 3, GETDATE())
	                AND a.AppointmentDate BETWEEN '{0}' AND '{1}' {2} 
	                AND a.Cancelled = 0
                    AND p.Archived = 0 and a.Archived = 0
	                AND NOT EXISTS(SELECT Id FROM Encounter ie WHERE ie.Patient_Id = p.Id AND ie.Archived = 0 AND ie.EncounterDate BETWEEN DATEADD(dd, -3, a.AppointmentDate) AND DATEADD(dd, 3, a.AppointmentDate))
                ORDER BY a.AppointmentDate desc ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), where);

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<OutstandingVisitList>().ExecuteSql(sql, parameters);
        }

        public ICollection<PatientOnStudyList> GetPatientOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria)
        {
            string sql = "";
            switch (patientOnStudyCriteria)
            {
                case PatientOnStudyCriteria.HasEncounterinDateRange:
                    sql = string.Format(@"
                        SELECT f.FacilityName, f.Id as FacilityId
	                        ,	(
			                        select count(distinct(ip.Id))
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ie.Archived = 0 and ip.Archived = 0 
			                            and ie.EncounterDate between '{0}' and '{1}'
				                        and ifa.Id = f.Id
		                        ) AS PatientCount
	                        ,	(
			                        select count(distinct(ip.Id))
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
				                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
			                        where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{0}' and '{1}'
				                        and ifa.Id = f.Id
		                        ) AS PatientWithEventCount
                        FROM Facility f
                        ORDER BY f.FacilityName 
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                    break;

                case PatientOnStudyCriteria.PatientRegisteredinFacilityinDateRange:
                    sql = string.Format(@"
                        SELECT f.FacilityName, f.Id as FacilityId
	                        ,	(
			                        select count(distinct(ip.Id))
			                        from Patient ip 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                        where ip.Archived = 0
                                        and ip.Created between '{0}' and '{1}'
				                        and ifa.Id = f.Id
		                        ) AS PatientCount
	                        ,	(
			                        select count(distinct(ip.Id))
			                        from Patient ip
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
				                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
			                        where ip.Archived = 0 and ipce.Archived = 0
                                        and ip.Created between '{0}' and '{1}'
				                        and ifa.Id = f.Id
		                        ) AS PatientWithEventCount
                        FROM Facility f
                        ORDER BY f.FacilityName 
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                    break;

                default:
                    break;
            }

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<PatientOnStudyList>().ExecuteSql(sql, parameters);
        }

        public ICollection<PatientList> GetPatientListOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria, int facilityId)
        {
            string sql = "";
            switch (patientOnStudyCriteria)
            {
                case PatientOnStudyCriteria.HasEncounterinDateRange:
                    sql = string.Format(@"
                        SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS PatientName, s.FacilityName  FROM 
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                        where ie.Archived = 0 and ip.Archived = 0 
                                        and ie.EncounterDate between '{0}' and '{1}'
				                        and ifa.Id = {2}
				                    group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName 
		                        ) AS s
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), facilityId);
                    break;

                case PatientOnStudyCriteria.PatientRegisteredinFacilityinDateRange:
                    sql = string.Format(@"
                        SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS PatientName, s.FacilityName  FROM
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Patient ip 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 
			                            and ip.Created between '{0}' and '{1}'
				                        and ifa.Id = {2}
		                        ) AS s
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), facilityId);
                    break;

                default:
                    break;
            }

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<PatientList>().ExecuteSql(sql, parameters);
        }

        public ICollection<DrugList> GetPatientsByDrugItems()
        {
            string sql = "";

            sql = string.Format(@"
               SELECT m.DrugName, m.Id as MedicationId
	                ,	(
			                select count(distinct(ip.Id))
			                from Patient ip
				                inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
				                inner join Medication im on ipm.Medication_Id = im.Id
			                where im.Id = m.Id and ip.Archived = 0 and ipm.Archived = 0 
		                ) AS PatientCount
                FROM Medication m
                ORDER BY m.DrugName ");

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<DrugList>().ExecuteSql(sql, parameters);
        }

        public ICollection<PatientList> GetPatientListByDrugItems(int medicationId)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS PatientName, s.FacilityName  FROM 
	                    (
			                select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                from Patient ip 
				                inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
				                inner join Medication im on ipm.Medication_Id = im.Id
				                inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                where im.Id = {0} and ip.Archived = 0 and ipm.Archived = 0 
				            group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
		                ) AS s
                ", medicationId);

            SqlParameter[] parameters = new SqlParameter[0];
            return _unitOfWork.Repository<PatientList>().ExecuteSql(sql, parameters);
        }

    }
}
