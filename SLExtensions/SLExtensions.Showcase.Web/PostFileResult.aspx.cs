namespace SLExtensions.Showcase.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public partial class PostFileResult : System.Web.UI.Page
    {
        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            var items =  (from k in Request.Form.AllKeys
                                    select new KeyValuePair<string,string>(k, (string)Request.Form[k])).ToList();
            if (Request.Files.Count > 0)
            {
                string filename = Request.Files[0].FileName;
                int length = Request.Files[0].ContentLength;

                items.Add(new KeyValuePair<string, string>(
                    " file: " + filename,
                    " content (" + length + ")"));
            }

            Repeater1.DataSource = items;
            Repeater1.DataBind();
        }

        #endregion Methods
    }
}