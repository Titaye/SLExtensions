namespace SLExtensions.Sharepoint
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    //    Special rendering for the user field values that makes them include the login name, email, SipAddress, and the title when present. This causes a user field to behave as a multi lookup field.
    //The lookup fields used in the expansion are "Name", "EMail", "SipAddress" and "Title". The values are separated by ,#. Any commas in the lookup field name are encoded as ,,.
    //These values occur in the normal field data for each item.
    //<ExpandUserField>FALSE</ExpandUserField> looks like: ows_Author="1;#Admin AdminName"
    //<ExpandUserField>TRUE</ExpandUserField>
    //Looks like: ows_Author="1;#Admin AdminName,#login\name,#emailaddress,#sipaddress,#Admin AdminName "
    [DataContract]
    public class UserInfoExpanded
    {
        #region Properties

        public string EMail
        {
            get; set;
        }

        public string ID
        {
            get; set;
        }

        public string Login
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string SipAddress
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static UserInfoExpanded Parse(string input)
        {
            var data = input.Split(new string[] { ";#", ",#" }, StringSplitOptions.None);
            UserInfoExpanded userInfo = new UserInfoExpanded();
            userInfo.ID = data[0];
            userInfo.Name = data[1].Replace(",,", ",");
            if (data.Length > 2)
            {
                userInfo.Login = data[2].Replace(",,", ",");
                userInfo.EMail = data[3].Replace(",,", ",");
                userInfo.SipAddress = data[4].Replace(",,", ",");
                userInfo.Title = data[5].Replace(",,", ",");
            }
            return userInfo;
        }

        #endregion Methods
    }
}