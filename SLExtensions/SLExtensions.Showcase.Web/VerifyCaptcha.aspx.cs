namespace SLExtensions.Showcase.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public partial class VerifyCaptcha : System.Web.UI.Page
    {
        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod != "POST")
                return;

            string privateKey = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            WebClient wc = new WebClient();
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("challenge", Request["challenge"]);
            parameters.Add("response", Request["response"]);
            parameters.Add("privatekey", privateKey);
            parameters.Add("remoteip", Request.UserHostAddress);
            byte[] result = wc.UploadValues("http://api-verify.recaptcha.net/verify", parameters);

            Response.Write(System.Text.Encoding.Default.GetString(result));
            Response.End();
        }

        #endregion Methods
    }
}