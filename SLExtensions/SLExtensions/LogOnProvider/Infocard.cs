namespace SLExtensions.LogOnProvider
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class Infocard
    {
        #region Fields

        public const string FieldsCountry = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country";
        public const string FieldsDateOfBirth = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth";
        public const string FieldsEmail = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public const string FieldsGender = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender";

        /* claims */
        public const string FieldsGivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string FieldsHomePhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/homephone";
        public const string FieldsLocality = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/locality";
        public const string FieldsMobilePhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone";
        public const string FieldsOtherPhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/otherphone";
        public const string FieldsPostalCode = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/postalcode";
        public const string FieldsPPID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier";
        public const string FieldsStateOrProvince = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/stateorprovince";
        public const string FieldsStreetAddress = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/streetaddress";
        public const string FieldsSurname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string FieldsWebpage = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage";
        public const string SAML10 = "urn:oasis:names:tc:SAML:1.0:assertion";
        public const string SAML11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";

        private const string detectInfoCardJs = @"var _idfxAreCardsSupported;
          function idfxAreCardsSupported() {

        if ('' + _idfxAreCardsSupported != 'undefined')
        {
          return _idfxAreCardsSupported;
        }

        var IEVer = -1;
        if (navigator.appName == 'Microsoft Internet Explorer')
        {
        if (new RegExp('MSIE ([0-9]{1,}[\.0-9]{0,})').exec(navigator.userAgent) != null)
        {
            IEVer = parseFloat( RegExp.$1 );
        }
        }

        if (IEVer >= 6 ) {
          var embed = document.createElement('object');
          embed.setAttribute('type', 'application/x-informationcard');

          if ( ''+embed.issuerPolicy != 'undefined' ) {

        _idfxAreCardsSupported = true;
        return _idfxAreCardsSupported;
          }

          _idfxAreCardsSupported = false;
          return _idfxAreCardsSupported;
        }

        if (IEVer < 0 && navigator.mimeTypes && navigator.mimeTypes.length) {

        x = navigator.mimeTypes['application/x-informationcard'];
          if (x && x.enabledPlugin) {
        _idfxAreCardsSupported = true;
        return _idfxAreCardsSupported;
          }

        try {
        var event = document.createEvent('Events');
        event.initEvent('IdentitySelectorAvailable', true, true);
        top.dispatchEvent(event);
        if (top.IdentitySelectorAvailable == true) {
          _idfxAreCardsSupported = true;
          return _idfxAreCardsSupported;
        }
          } catch (ex) { }

        }
        _idfxAreCardsSupported = false;
        return _idfxAreCardsSupported;
          };
        idfxAreCardsSupported();
        ";

        private static readonly Regex regExIE = new Regex(@"MSIE ([0-9]{1,}[\.0-9]{0,})");

        private static bool? areCardSupported = null;

        #endregion Fields

        #region Constructors

        public Infocard()
        {
            this.TokenType = SAML11;
            RequiredClaims = new List<string>();
            OptionalClaims = new List<string>();
            PrivacyUrl = new Uri("http://www.ucaya.com/openidpolicy.html");
            PrivacyVersion = "1.0";
            RequiredClaims.Add(FieldsPPID);
        }

        #endregion Constructors

        #region Events

        public event EventHandler AuthenticationFailed;

        #endregion Events

        #region Properties

        public static bool IsInfocardSupported
        {
            get
            {
                if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                {
                    return true;
                }

                if (areCardSupported == null)
                {
                    var detect = HtmlPage.Window.Eval(detectInfoCardJs);
                    areCardSupported = (detect as bool?).GetValueOrDefault();
                }
                return areCardSupported.GetValueOrDefault();
            }
        }

        public string Issuer
        {
            get;
            set;
        }

        public IList<string> OptionalClaims
        {
            get;
            private set;
        }

        public Uri PrivacyUrl
        {
            get;
            set;
        }

        public string PrivacyVersion
        {
            get;
            set;
        }

        public IList<string> RequiredClaims
        {
            get;
            private set;
        }

        public string TokenType
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public HtmlElement GetInformationCard()
        {
            HtmlElement obj = HtmlPage.Document.CreateElement("object");

            StringBuilder sb = new StringBuilder();
            // add all parameters first
            foreach (var item in RequiredClaims)
            {
                sb.Append(item);
                sb.Append(" ");
            }

            addObjectParameter(obj, "requiredClaims", sb.ToString());

            if (!string.IsNullOrEmpty(TokenType))
                addObjectParameter(obj, "tokenType", TokenType);

            if (OptionalClaims.Count > 0)
            {
                sb = new StringBuilder();
                // add all parameters first
                foreach (var item in OptionalClaims)
                {
                    sb.Append(item);
                    sb.Append(" ");
                }

                addObjectParameter(obj, "optionalClaims", sb.ToString());
            }

            if (!string.IsNullOrEmpty(this.PrivacyVersion) && PrivacyUrl != null)
            {
                addObjectParameter(obj, "privacyUrl", PrivacyUrl.ToString());
                addObjectParameter(obj, "privacyVersion", PrivacyVersion);
            }

            if (!string.IsNullOrEmpty(Issuer))
                addObjectParameter(obj, "Issuer", Issuer);

            obj.SetAttribute("type", "application/x-informationcard");
            HtmlPage.Document.Body.AppendChild(obj);
            return obj;
        }

        public string GetToken()
        {
            var xmltkn = GetInformationCard();

            return xmltkn.GetProperty("value") as string ?? "";
        }

        protected virtual void OnAuthenticationFailed()
        {
            if (AuthenticationFailed != null)
            {
                AuthenticationFailed(this, EventArgs.Empty);
            }
        }

        private static void addObjectParameter(HtmlElement element, string name, string value)
        {
            var p = HtmlPage.Document.CreateElement("param");
            p.SetAttribute("name", name);
            p.SetAttribute("value", value);
            element.AppendChild(p);
        }

        #endregion Methods
    }
}