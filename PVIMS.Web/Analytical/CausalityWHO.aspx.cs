using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;

namespace PVIMS.Web
{
    public partial class CausalityWHO : MainPageBase
    {
        private int _rid;
        private ReportInstance _reportInstance;

        private int _rmid;
        private Guid _rmguid;
        private ReportInstanceMedication _reportInstanceMedication;

        private PatientClinicalEvent _clinicalEvent;
        private DatasetInstance _instance;

        private enum formContext { SetCausality = 1, IgnoreMedication = 2, View = 3 };
        private formContext _context;

        private enum FormMode { ActiveMode = 1, SpontaneousMode = 2 };
        private FormMode _formMode = FormMode.ActiveMode;

        public IWorkFlowService _workFlowService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "WHO Causality Assessment", SubTitle = "", Icon = "fa fa-dashboard fa-fw", MetaPageId = 0 });

            _context = formContext.View;
            divCausality.Visible = false;

            if (Request.QueryString["rid"] != null)
            {
                _rid = Convert.ToInt32(Request.QueryString["rid"]);
                if (_rid > 0)
                {
                    _reportInstance = UnitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.Id == _rid);

                    if (_reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
                    {
                        Master.MainMenu.SetActive("ActiveReporting");

                        _formMode = FormMode.ActiveMode;
                        _clinicalEvent = UnitOfWork.Repository<PatientClinicalEvent>().Queryable().Include(pce => pce.Patient).SingleOrDefault(pce => pce.PatientClinicalEventGuid == _reportInstance.ContextGuid);

                        HandleActiveInit();
                    }
                    else
                    {
                        Master.MainMenu.SetActive("SpontaneousReporting");

                        _formMode = FormMode.SpontaneousMode;
                        _instance = UnitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(di => di.DatasetInstanceGuid == _reportInstance.ContextGuid);

                        HandleSpontaneousInit();
                    }
                }
                else
                {
                    throw new Exception("rid parameter not passed");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(hidMedication.Value))
            {
                _context = formContext.SetCausality;
            }
            RenderButtons();
        }

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            spnButtons.Controls.Clear();
            spnReturn.Controls.Clear();

            if (_context == formContext.SetCausality)
            {
                btn = new Button();
                btn.ID = "btnSave";
                btn.CssClass = "btn btn-primary";
                btn.Text = "Save";
                btn.Click += btnSave_Click;
                btn.Attributes.Add("formnovalidate", "formnovalidate");
                spnButtons.Controls.Add(btn);

                hyp = new HyperLink()
                {
                    ID = "btnCancel",
                    NavigateUrl = "CausalityNaranjo.aspx?rid=" + _reportInstance.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Cancel"
                };
                spnButtons.Controls.Add(hyp);
            }

            spnButtons.Visible = true;

            if (_formMode == FormMode.ActiveMode)
            {
                hyp = new HyperLink()
                {
                    ID = "btnReturn",
                    NavigateUrl = "/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219",
                    CssClass = "btn btn-default",
                    Text = "Return"
                };
            }
            else
            {
                hyp = new HyperLink()
                {
                    ID = "btnReturn",
                    NavigateUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986",
                    CssClass = "btn btn-default",
                    Text = "Return"
                };
            }
            spnReturn.Controls.Add(hyp);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hidCausality.Value == "" || hidCausality.Value == "Incomplete") { return; };

            _rmid = Convert.ToInt32(hidMedication.Value);
            _reportInstanceMedication = UnitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(rm => rm.Id == _rmid);

            try
            {
                _reportInstanceMedication.WhoCausality = hidCausality.Value;

                UnitOfWork.Repository<ReportInstanceMedication>().Update(_reportInstanceMedication);
                UnitOfWork.Complete();
            }
            catch (DbEntityValidationException ex)
            {
                var err = string.Empty;
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(err);
            }

            _context = formContext.View;

            RenderButtons();

            if (_formMode == FormMode.ActiveMode)
            {
                RenderActiveMeds();
            }
            else
            {
                RenderSpontaneousMeds();
            }

            //divTool.Style["display"] = "none";
            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "WHO Causality set successfully";
            Response.Cookies.Add(cookie);
            Master.ShouldPopUpBeDisplayed();
            divCausality.Visible = false;
        }

        #region "Active Handling"

        private void HandleActiveInit()
        {
            divSelection.Visible = true;

            lblOnsetDate.InnerText = _clinicalEvent.OnsetDate != null ? Convert.ToDateTime(_clinicalEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
            if (_clinicalEvent.SourceTerminologyMedDra != null) { lblSource.InnerText = _clinicalEvent.SourceTerminologyMedDra.DisplayName; };

            if (_reportInstance.TerminologyMedDra != null)
            {
                lblSelection.InnerText = _reportInstance.TerminologyMedDra.DisplayName;

                RenderActiveMeds();

                divDesc.Style["display"] = "block";
            }
            else
            {
                divSetTerm.Visible = true;

                // Give user option of setting terminology
                HyperLink hyp = new HyperLink()
                {
                    Text = "Set Terminology",
                    NavigateUrl = "TerminologyMedDRA.aspx?rid=" + _reportInstance.Id.ToString()
                };
                lblSelection.Controls.Add(hyp);

                divMedications.Style["display"] = "none";
            }
        }

        private void RenderActiveMeds()
        {
            TableRow row;
            TableCell cell;

            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;
            LinkButton lbtn;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Medication")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_1.Rows.Remove(temprow);
            }

            IExtendable patientMedicationExtended;
            var config = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Type of Indication");

            foreach (ReportInstanceMedication med in _reportInstance.Medications)
            {
                var patientMedication = UnitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                patientMedicationExtended = patientMedication;

                row = new TableRow();

                cell = new TableCell();
                cell.Text = med.MedicationIdentifier;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DateStart.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DateEnd != null ? Convert.ToDateTime(patientMedication.DateEnd).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                var typeInd = "";
                if (patientMedicationExtended.GetAttributeValue("Type of Indication") != null)
                {
                    var val = patientMedicationExtended.GetAttributeValue("Type of Indication").ToString();
                    var selection = UnitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(s => s.AttributeKey == config.AttributeKey && s.SelectionKey == val);

                    typeInd = selection.Value;
                }
                cell.Text = typeInd;
                row.Cells.Add(cell);

                var causality = "<span class=\"label label-info\">NOT SET</span>";
                if (med.WhoCausality != null)
                {
                    causality = String.Format("<span class=\"badge txt-color-white bg-color-red\" style=\"padding:5px;width:80px;\"> {0} </span>", med.WhoCausality);
                }

                cell = new TableCell();
                cell.Text = causality;
                row.Cells.Add(cell);

                if (_context != formContext.SetCausality)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };

                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    lbtn = new LinkButton()
                    {
                        ID = "sc" + med.Id.ToString(),
                        Text = "Set Causality"
                    };
                    lbtn.Click += btnSetActiveCausality_Click;
                    li.Controls.Add(lbtn);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    lbtn = new LinkButton()
                    {
                        ID = "im" + med.Id.ToString(),
                        Text = "Ignore Medication"
                    };
                    lbtn.Click += btnIgnoreActiveMedication_Click;
                    li.Controls.Add(lbtn);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);
                    cell.Controls.Add(pnl);

                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = string.Empty;
                    row.Cells.Add(cell);
                }

                dt_1.Rows.Add(row);
            }
            // Hide columns where necessary
            if (_context == formContext.SetCausality)
            {
                dt_1.HideColumn(5); // Action column
            }
            else
            {
                dt_1.ShowColumn(5); // Action column
            }
            if (config == null)
            {
                dt_1.HideColumn(3); // Type of indication column
            }

        }

        protected void btnSetActiveCausality_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            _rmid = Convert.ToInt32(btn.ID.Replace("sc", ""));
            if (_rmid > 0)
            {
                _context = formContext.SetCausality;
                _reportInstanceMedication = UnitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(rm => rm.Id == _rmid);

                divCausality.Visible = true;

                txtMedicine.Value = _reportInstanceMedication.MedicationIdentifier;
                hidMedication.Value = _reportInstanceMedication.Id.ToString();

                RenderActiveMeds();
                RenderButtons();
            }
        }

        protected void btnIgnoreActiveMedication_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            _rmid = Convert.ToInt32(btn.ID.Replace("im", ""));
            if (_rmid > 0)
            {
                _context = formContext.IgnoreMedication;
                _reportInstanceMedication = UnitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(rm => rm.Id == _rmid);

                divCausality.Visible = false;

                _reportInstanceMedication.WhoCausality = "IGNORED";

                UnitOfWork.Repository<ReportInstanceMedication>().Update(_reportInstanceMedication);
                UnitOfWork.Complete();

                RenderActiveMeds();

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Naranjo Causality set successfully";
                Response.Cookies.Add(cookie);
                Master.ShouldPopUpBeDisplayed();
            }
        }

        #endregion

        #region "Spontaneous Handling"

        private void HandleSpontaneousInit()
        {
            divSelection.Visible = false;

            DateTime tempdt;
            var temp = _instance.GetInstanceValue(_instance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction and Treatment").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction start date").DatasetElement);
            if (temp != string.Empty)
            {
                temp = DateTime.TryParse(temp, out tempdt) ? Convert.ToDateTime(temp).ToString("yyyy-MM-dd") : "";
            };
            lblOnsetDate.InnerText = temp;

            if (_reportInstance.TerminologyMedDra != null)
            {
                lblSelection.InnerText = _reportInstance.TerminologyMedDra.DisplayName;

                RenderSpontaneousMeds();

                divDesc.Style["display"] = "block";
            }
            else
            {
                divSetTerm.Visible = true;

                // Give user option of setting terminology
                HyperLink hyp = new HyperLink()
                {
                    Text = "Set Terminology",
                    NavigateUrl = "TerminologyMedDRA.aspx?rid=" + _reportInstance.Id.ToString()
                };
                lblSource.Controls.Add(hyp);

                divMedications.Style["display"] = "none";
            }
        }

        private void RenderSpontaneousMeds()
        {
            TableRow row;
            TableCell cell;

            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;
            LinkButton lbtn;

            DateTime tempdt;

            string[] validNaranjoCriteria = { "Possible", "Probable" };

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Medication")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_1.Rows.Remove(temprow);
            }

            var element = UnitOfWork.Repository<DatasetElement>().Queryable().Include(de1 => de1.DatasetElementSubs).SingleOrDefault(u => u.ElementName == "Product Information");
            var contexts = _instance.GetInstanceSubValuesContext(element);
            foreach (ReportInstanceMedication med in _reportInstance.Medications)
            {
                var drugName = "";
                var drugItemValues = _instance.GetInstanceSubValues(element, med.ReportInstanceMedicationGuid);
                drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                if (drugName != string.Empty)
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = drugName;
                    row.Cells.Add(cell);

                    var startElement = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                    var start = startElement != null ? DateTime.TryParse(startElement.InstanceValue, out tempdt) ? Convert.ToDateTime(startElement.InstanceValue).ToString("yyyy-MM-dd") : "" : "";
                    cell = new TableCell();
                    cell.Text = start;
                    row.Cells.Add(cell);

                    var stopElement = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");
                    var stop = stopElement != null ? DateTime.TryParse(stopElement.InstanceValue, out tempdt) ? Convert.ToDateTime(stopElement.InstanceValue).ToString("yyyy-MM-dd") : "" : "";
                    cell = new TableCell();
                    cell.Text = stop;
                    row.Cells.Add(cell);

                    var indicationElement = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Indication");
                    var indication = indicationElement != null ? DateTime.TryParse(indicationElement.InstanceValue, out tempdt) ? Convert.ToDateTime(indicationElement.InstanceValue).ToString("yyyy-MM-dd") : "" : "";
                    cell = new TableCell();
                    cell.Text = indication;
                    row.Cells.Add(cell);

                    var causality = "<span class=\"label label-info\">NOT SET</span>";
                    if (med.WhoCausality != null)
                    {
                        causality = String.Format("<span class=\"badge txt-color-white bg-color-red\" style=\"padding:5px;width:80px;\"> {0} </span>", med.WhoCausality);
                    }

                    cell = new TableCell();
                    cell.Text = causality;
                    row.Cells.Add(cell);

                    if (_context != formContext.SetCausality)
                    {
                        cell = new TableCell();
                        pnl = new Panel() { CssClass = "btn-group" };

                        btn = new HtmlGenericControl("button");
                        btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                        btn.Attributes.Add("data-toggle", "dropdown");
                        btn.Controls.Add(new Label() { Text = "Action " });
                        btn.Controls.Add(new Label() { CssClass = "caret" });
                        pnl.Controls.Add(btn);

                        ul = new HtmlGenericControl("ul");
                        ul.Attributes.Add("class", "dropdown-menu pull-right");

                        li = new HtmlGenericControl("li");
                        lbtn = new LinkButton()
                        {
                            ID = "sc" + med.ReportInstanceMedicationGuid.ToString(),
                            Text = "Set Causality"
                        };
                        lbtn.Click += btnSetSpontaneousCausality_Click;
                        li.Controls.Add(lbtn);
                        ul.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        lbtn = new LinkButton()
                        {
                            ID = "im" + med.ReportInstanceMedicationGuid.ToString(),
                            Text = "Ignore Medication"
                        };
                        lbtn.Click += btnIgnoreSpontaneousMedication_Click;
                        li.Controls.Add(lbtn);
                        ul.Controls.Add(li);

                        pnl.Controls.Add(ul);
                        cell.Controls.Add(pnl);
                        row.Cells.Add(cell);
                    }
                    else
                    {
                        cell = new TableCell();
                        cell.Text = string.Empty;
                        row.Cells.Add(cell);
                    }

                    dt_1.Rows.Add(row);
                }
            }
            // Hide columns where necessary
            if (_context == formContext.SetCausality)
            {
                dt_1.HideColumn(5); // Action column
            }
            else
            {
                dt_1.ShowColumn(5); // Action column
            }
            dt_1.HideColumn(3); // Type of indication column
        }

        protected void btnSetSpontaneousCausality_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            _rmguid = new Guid(btn.ID.Replace("sc", ""));
            _context = formContext.SetCausality;
            _reportInstanceMedication = UnitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(rm => rm.ReportInstanceMedicationGuid == _rmguid);

            divCausality.Visible = true;

            txtMedicine.Value = _reportInstanceMedication.MedicationIdentifier;
            hidMedication.Value = _reportInstanceMedication.Id.ToString();

            RenderSpontaneousMeds();
            RenderButtons();
        }

        protected void btnIgnoreSpontaneousMedication_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            _rmguid = new Guid(btn.ID.Replace("im", ""));
            _context = formContext.IgnoreMedication;
            _reportInstanceMedication = UnitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(rm => rm.ReportInstanceMedicationGuid == _rmguid);

            divCausality.Visible = false;

            _reportInstanceMedication.NaranjoCausality = "IGNORED";

            UnitOfWork.Repository<ReportInstanceMedication>().Update(_reportInstanceMedication);
            UnitOfWork.Complete();

            RenderSpontaneousMeds();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "Naranjo Causality set successfully";
            Response.Cookies.Add(cookie);
            Master.ShouldPopUpBeDisplayed();
        }

        #endregion

    }
}