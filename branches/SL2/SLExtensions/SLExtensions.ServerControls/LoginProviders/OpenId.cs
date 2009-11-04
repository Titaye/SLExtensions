using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtremeSwank.Authentication.OpenID;
using ExtremeSwank.Authentication.OpenID.Plugins.Extensions;

namespace SLExtensions.ServerControls.LoginProviders
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:OpenId runat=server RequiredFields='' OptionalFields='fullname,email,nickname,dob,gender,postcode,country,language,timezone'></{0}:OpenId>")]
    public class OpenId : WebControl, IPostBackEventHandler
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
       
//            <input id="openIdPostback" type="hidden" Value="<%= GetPostBackEventReference() %>"/>
//<input id="openIdLogin" type="hidden" />
            Page.ClientScript.RegisterHiddenField("openIdLogin", null);

            ScriptManager.RegisterClientScriptBlock(this, typeof(OpenId), "test", "function openidpb(){" + GetPostBackEventReference() + "}", true);


            //Trace.TraceFinished += new TraceContextEventHandler(Trace_TraceFinished);
            if (!Page.IsPostBack)
            {
                OpenIDConsumer openid = new OpenIDConsumer();
                switch (openid.RequestedMode)
                {
                    case RequestedMode.IdResolution:
                        openid.Identity = this.Identity;
                        SetAuthMode(openid);
                        if (openid.Validate())
                        {
                            _UserObject = openid.RetrieveUser();

                            OnValidateSuccess(e);
                            OnResponseProcessed(e);
                        }
                        else
                        {
                            //FormPanel.Visible = true;
                            //StatusPanel.Visible = false;
                            //LLabel.Text = TranslateError(openid.GetError());
                            OnValidateFail(e);
                            OnResponseProcessed(e);
                        }
                        break;
                    case RequestedMode.CancelledByUser:
                        //FormPanel.Visible = true;
                        //StatusPanel.Visible = false;
                        //LLabel.Text = TranslateError(Errors.RequestCancelled);
                        OnRemoteCancel(e);
                        OnResponseProcessed(e);
                        break;
                    case RequestedMode.None:
                        if (UserObject != null)
                        {
                            //FormPanel.Visible = false;
                            //StatusPanel.Visible = true;
                        }
                        break;
                }
            }
        }

        public string GetPostBackEventReference()
        {
            return Page.ClientScript.GetPostBackEventReference(this, string.Empty);
        }

        /// <summary>
        /// Event fires upon successful validation received from 
        /// Identity Provider.
        /// </summary>
        public event EventHandler ValidateSuccess;

        /// <summary>
        /// Fires when successful validation is received from
        /// Identity Provider.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnValidateSuccess(EventArgs e)
        {
            if (ValidateSuccess != null)
            {
                ValidateSuccess(this, e);
            }
        }

        /// <summary>
        /// Event fires when unsuccessful validation is received frmo
        /// Identity Provider
        /// </summary>
        public event EventHandler ValidateFail;

        /// <summary>
        /// Fires when unsuccessful validation is received from
        /// Identity Provider
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnValidateFail(EventArgs e)
        {
            if (ValidateFail != null)
            {
                ValidateFail(this, e);
            }
        }
        /// <summary>
        /// Event fires when user cancels request at Identity Provider
        /// and is redirected back to this application.
        /// </summary>
        public event EventHandler RemoteCancel;

        /// <summary>
        /// Fires when user cancels request at Identity Provider
        /// and is redirected back to this application.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRemoteCancel(EventArgs e)
        {
            if (RemoteCancel != null)
            {
                RemoteCancel(this, e);
            }
        }

        /// <summary>
        /// Event fires after user has submitted login form,
        /// but before performing authentication-related functions.
        /// </summary>
        public event EventHandler Login;

        /// <summary>
        /// Fires after user has submitted login form, but
        /// before performing authentication-related functions.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLogin(EventArgs e)
        {
            if (Login != null)
            {
                Login(this, e);
            }
        }

        /// <summary>
        /// Event fires when a OpenID response is received and processing has completed.
        /// </summary>
        public event EventHandler ResponseProcessed;

        protected virtual void OnResponseProcessed(EventArgs e)
        {
            if (ResponseProcessed != null)
            {
                ResponseProcessed(this, e);
            }
        }

        /// <summary>
        /// Event fires after user has used the "log out" function.
        /// </summary>
        public event EventHandler Logout;

        /// <summary>
        /// Fires after user has used the "log out" function.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLogout(EventArgs e)
        {
            if (Logout != null)
            {
                Logout(this, e);
            }
        }

        /// <summary>
        /// Sets the authentication mode to be used. 
        /// Supports either "stateful" or "stateless".
        /// Defaults to "stateful".
        /// </summary>
        public AuthenticationMode AuthMode
        {
            get
            {
                if (ViewState["AuthMode"] == null) { return AuthenticationMode.Stateful; }
                return (AuthenticationMode)ViewState["AuthMode"];
            }
            set
            {
                ViewState["AuthMode"] = value;
            }
        }

        /// <summary>
        /// The URL of the OpenID Provider to authenticate against.  This will result in the user being prompted for the desired
        /// OpenID by the OpenID Provider.
        /// </summary>
        /// <remarks>
        /// Setting this property to a non-blank value will result in the Identity being ignored.
        /// </remarks>
        public string OpenIDServerURL
        {
            get
            {
                if (ViewState["UseDirectedIdentity"] == null) { return null; }
                return (string)ViewState["UseDirectedIdentity"];
            }
            set
            {
                ViewState["UseDirectedIdentity"] = value;
            }
        }

        /// <summary>
        /// From Simple Registration Extension. Comma-delimited list of Simple Registration
        /// fields that the Identity Provider should require the user to provide.
        /// </summary>
        public string RequiredFields
        {
            get { return (string)ViewState["RequiredFields"]; }
            set { ViewState["RequiredFields"] = value; }
        }

        /// <summary>
        /// From Simple Registration Extension. Comma-delimited list of Simple Registration
        /// fields that the Identity Provider can optionally ask the user to provide.
        /// </summary>
        public string OptionalFields
        {
            get { return (string)ViewState["OptionalFields"]; }
            set { ViewState["OptionalFields"] = value; }
        }

        /// <summary>
        /// From Simple Registration Extension. URL of this site's privacy policy to send
        /// to the Identity Provider.
        /// </summary>
        public string PolicyURL
        {
            get { return (string)ViewState["Identity"]; }
            set { ViewState["Identity"] = value; }
        }

        /// <summary>
        /// Optional. Base URL of this site. Sets the scope of the authentication request. 
        /// </summary>
        public string Realm
        {
            get { return (string)ViewState["TrustRoot"]; }
            set { ViewState["TrustRoot"] = value; }
        }

        /// <summary>
        /// OpenID identitier.
        /// </summary>
        private string Identity
        {
            get { return (string)Page.Session["OpenID_Identity"]; }
            set { Page.Session["OpenID_Identity"] = value; }
        }

        private OpenIDUser _UserObject
        {
            get { return (OpenIDUser)Page.Session["UserObject"]; }
            set { Page.Session["UserObject"] = value; }
        }

        /// <summary>
        /// OpenIDUser object that represents the authenticated user and all
        /// information received from the Identity Provider.
        /// </summary>
        public OpenIDUser UserObject
        {
            get { return _UserObject; }
        }
        private void SetAuthMode(OpenIDConsumer openid)
        {
            openid.AuthMode = AuthMode;
        }

        /// <summary>
        /// User has clicked the login button. Sets up a new OpenIDConsumer
        /// object and begins the authentication sequence. 
        /// Fires the OnLogin event. 
        /// </summary>
        /// <param name="sender">Object invoking this method.</param>
        /// <param name="e">EventArgs related to this request.</param>
        protected void Button_Click(object sender, EventArgs e)
        {
            //OpenIDConsumer openid = new OpenIDConsumer();
            //SetAuthMode(openid);
            //SimpleRegistration sr = new SimpleRegistration(openid);
            //if (!String.IsNullOrEmpty(this.RequiredFields)) { sr.RequiredFields = this.RequiredFields; }
            //if (!String.IsNullOrEmpty(this.OptionalFields)) { sr.OptionalFields = this.OptionalFields; }
            //if (!String.IsNullOrEmpty(this.PolicyURL)) { sr.PolicyURL = this.PolicyURL; }

            //if (String.IsNullOrEmpty(OpenIDServerURL))
            //{
            //    openid.Identity = openid_url.Text;
            //    this.Identity = openid.Identity;
            //}
            //else
            //{
            //    openid.UseDirectedIdentity = true;
            //    openid.OpenIDServer = OpenIDServerURL;
            //}

            //OnLogin(e);
            //openid.BeginAuth();

            //if (openid.IsError())
            //{
            //    LLabel.Text = TranslateError(openid.GetError());
            //}
            //else
            //{
            //    LLabel.Text = "";
            //}
        }
        /// <summary>
        /// User has clicked the "log out" button. Removes all information
        /// about the user from the Session state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LogOutButton_Click(object sender, EventArgs e)
        {
            //OnLogout(e);
            //HttpContext.Current.Session.Clear();
            //FormPanel.Visible = true;
            //StatusPanel.Visible = false;
        }
        
    

        /// <summary>
        /// Translate errors received from Consumer to user-visible text.
        /// </summary>
        /// <param name="error">Value from Errors enumeration</param>
        /// <returns>A user-visible string matching the error</returns>
        public string TranslateError(Errors error)
        {
            switch (error)
            {
                case Errors.NoErrors:
                    return null;
                case Errors.HttpError:
                    return "Connection to OpenID Provider failed. Please try again later.";
                case Errors.NoIdSpecified:
                    return "Please specify an OpenID.";
                case Errors.NoServersFound:
                    return "Unable to locate OpenID Provider. Double-check your OpenID.";
                case Errors.NoStatelessImmediate:
                    return "Immediate-mode does not support Stateless authentication.";
                case Errors.RequestRefused:
                    return "OpenID Provider refused authentication request.";
                case Errors.SessionTimeout:
                    return "Session timed out. Please try again.";
                case Errors.RequestCancelled:
                    return "Login request cancelled.";
                default:
                    return null;
            }
        }

        void Trace_TraceFinished(object sender, TraceContextEventArgs e)
        {
            //List<string> list = new List<string>();
            //list.Add("Timestamp: " + DateTime.Now);
            //foreach (TraceContextRecord tcr in e.TraceRecords)
            //{
            //    if (tcr.Category == "openid")
            //    {
            //        list.Add(tcr.Message);
            //    }
            //}
            //list.Add("--------------------");
            //string alltext = String.Join("\n", list.ToArray()) + "\n";
            //File.AppendAllText(Server.MapPath("~/openid.log"), alltext);
            //alltext = null;
            //list = null;
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            OpenIDConsumer openid = new OpenIDConsumer();
            SetAuthMode(openid);
            SimpleRegistration sr = new SimpleRegistration(openid);
            if (!String.IsNullOrEmpty(this.RequiredFields)) { sr.RequiredFields = this.RequiredFields; }
            if (!String.IsNullOrEmpty(this.OptionalFields)) { sr.OptionalFields = this.OptionalFields; }
            if (!String.IsNullOrEmpty(this.PolicyURL)) { sr.PolicyURL = this.PolicyURL; }

            if (String.IsNullOrEmpty(OpenIDServerURL))
            {
                openid.Identity = Page.Request["openIdLogin"];
                this.Identity = openid.Identity;
            }
            else
            {
                openid.UseDirectedIdentity = true;
                openid.OpenIDServer = OpenIDServerURL;
            }

            //OnLogin(e);
            openid.BeginAuth();

            //if (openid.IsError())
            //{
            //    LLabel.Text = TranslateError(openid.GetError());
            //}
            //else
            //{
            //    LLabel.Text = "";
            //}
        }

        #endregion
    }
}