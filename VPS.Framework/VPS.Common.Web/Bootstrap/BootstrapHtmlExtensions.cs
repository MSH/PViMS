using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace VPS.Common.Web.Bootstrap
{
    public static class BootstrapHtmlExtensions
    {
	    public static MvcHtmlString DefaultButtonLink(this HtmlHelper helper, string linkText, string actionName, string controllerName,
		    object routeValues)
	    {
		    return helper.ActionLink(linkText, actionName, controllerName, routeValues, new {@class="btn btn-default"});
	    }

		
	    public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
	    {
		    return helper.LabelFor(expression, new {@class="control-label"});
	    }

	    public static MvcHtmlString FormControlTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper,
		    Expression<Func<TModel, TValue>> expression)
	    {
		    return helper.TextBoxFor(expression, new {@class = "form-control"});
	    }

		public static MvcHtmlString FormControlDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper,
		   Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList)
		{
			return helper.DropDownListFor(expression, selectList, new { @class = "form-control" });
		}

	    public static MvcHtmlString PrimaryButton(this HtmlHelper helper, string text)
	    {
		    return new MvcHtmlString(string.Format(@"<button class=""btn btn-primary"" type=""submit"">{0}</button>", text));
	    }
    }
}
