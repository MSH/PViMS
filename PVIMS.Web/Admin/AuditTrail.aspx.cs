using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class AuditTrail : MainPageBase
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            txtDateFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            txtDateTo.Value = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Menu.SetActive("AuditTrail");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bool err = false;
            DateTime dttemp;

            lblDateFrom.Attributes.Remove("class");
            lblDateFrom.Attributes.Add("class", "input");
            lblDateTo.Attributes.Remove("class");
            lblDateTo.Attributes.Add("class", "input");

            DateTime datefrom = DateTime.MinValue;
            DateTime dateto = DateTime.MinValue;

            // Validation
            if (!String.IsNullOrWhiteSpace(txtDateFrom.Value))
            {
                if (DateTime.TryParse(txtDateFrom.Value, out dttemp))
                {
                    dttemp = Convert.ToDateTime(txtDateFrom.Value);
                    datefrom = dttemp;
                    if (dttemp > DateTime.Today.AddDays(1))
                    {
                        lblDateFrom.Attributes.Remove("class");
                        lblDateFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date from should be before current date";
                        lblDateFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblDateFrom.Attributes.Remove("class");
                        lblDateFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date from cannot be so far in the past";
                        lblDateFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblDateFrom.Attributes.Remove("class");
                    lblDateFrom.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Date from has an invalid date format";
                    lblDateFrom.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            else
            {
                lblDateFrom.Attributes.Remove("class");
                lblDateFrom.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Date from must be selected";
                lblDateFrom.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (!String.IsNullOrWhiteSpace(txtDateTo.Value))
            {
                if (DateTime.TryParse(txtDateTo.Value, out dttemp))
                {
                    dttemp = Convert.ToDateTime(txtDateTo.Value);
                    dateto = dttemp;
                    if (dttemp > DateTime.Today.AddDays(1))
                    {
                        lblDateTo.Attributes.Remove("class");
                        lblDateTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date to should be before current date";
                        lblDateTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblDateTo.Attributes.Remove("class");
                        lblDateTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date to cannot be so far in the past";
                        lblDateTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < datefrom)
                    {
                        lblDateTo.Attributes.Remove("class");
                        lblDateTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date to must be after date from";
                        lblDateTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblDateTo.Attributes.Remove("class");
                    lblDateTo.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Date to has an invalid date format";
                    lblDateTo.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            else
            {
                lblDateTo.Attributes.Remove("class");
                lblDateTo.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Date to must be selected";
                lblDateTo.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err) { return; };

            IEnumerable<AuditLog> auditList = null;
            auditList = UnitOfWork.Repository<AuditLog>().Queryable().Include("User").OrderBy(a => a.ActionDate);

            switch (ddlAuditType.SelectedValue)
            {
                case "Subscriber Access":
                    auditList = auditList.Where(au => au.AuditType == AuditType.InvalidSubscriberAccess || au.AuditType == AuditType.ValidSubscriberAccess);
                    break;

                case "Subscriber Post":
                    auditList = auditList.Where(au => au.AuditType == AuditType.InValidSubscriberPost || au.AuditType == AuditType.ValidSubscriberPost);
                    break;

                case "MedDRA Import":
                    auditList = auditList.Where(au => au.AuditType == AuditType.InValidMedDRAImport || au.AuditType == AuditType.ValidMedDRAImport);
                    break;

                case "User Logins":
                    auditList = auditList.Where(au => au.AuditType == AuditType.UserLogin);
                    break;

                default:
                    break;
            }

            auditList = auditList.Where(au => au.ActionDate >= datefrom && au.ActionDate <= dateto);
            auditList = auditList.OrderByDescending(au => au.ActionDate);

            TableRow row;
            TableCell cell;
            HyperLink hyp;

            // Loop through and render table
            foreach (AuditLog au in auditList)
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = au.AuditType.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = au.Details;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = String.Format("On {0} by {1}", au.ActionDate.ToString("yyyy-MM-dd hh:mm"), au.User != null ? au.User.UserName : "Interop.Context");
                row.Cells.Add(cell);

                if(!String.IsNullOrEmpty(au.Log))
                {
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/FileDownload/DownloadAuditLog?auId=" + au.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Download Log"
                    };
                    cell = new TableCell();
                    cell.Controls.Add(hyp);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_basic.Rows.Add(row);
            }
            switch (ddlAuditType.SelectedValue)
            {
                case "Subscriber Access":
                    dt_basic.ShowColumn(3);
                    break;

                case "Subscriber Post":
                    dt_basic.ShowColumn(3);
                    break;

                case "MedDRA Import":
                    dt_basic.ShowColumn(3);
                    break;

                case "User Logins":
                    dt_basic.HideColumn(3);
                    break;

                default:
                    break;
            }
        }
    }
}