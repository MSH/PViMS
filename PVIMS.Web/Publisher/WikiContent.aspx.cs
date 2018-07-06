using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class WikiContent : MainPageBase
    {
        private int _id;

        protected void Page_Init(object sender, EventArgs e)
        {
            divArticle1.Visible = false;
            divArticle2.Visible = false;
            divArticle3.Visible = false;
            divArticle4.Visible = false;

            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);

                switch (_id)
                {
                    case 1:
                        divArticle1.Visible = true;
                        break;

                    case 2:
                        divArticle2.Visible = true;
                        break;

                    case 3:
                        divArticle3.Visible = true;
                        break;

                    case 4:
                        divArticle4.Visible = true;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                throw new Exception("id not passed as parameter");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}