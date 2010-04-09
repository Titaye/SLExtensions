namespace SLMedia.Core
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    #region Enumerations

    public enum TypeCommand
    {
        Pub,
        Link,
        Log,
        Custom
    }

    #endregion Enumerations

    public class ScriptCommandItem
    {
        #region Properties

        public String Command
        {
            get; set;
        }

        public double Duration
        {
            get; set;
        }

        public double Time
        {
            get; set;
        }

        public TypeCommand Type
        {
            get; set;
        }

        public bool Visible
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        internal static ScriptCommandItem ParseXML(XElement i)
        {
            ScriptCommandItem c = new ScriptCommandItem();

            switch (i.Attribute("Type").Value)
            {
                case "Pub": c.Type = TypeCommand.Pub;
                    break;
                case "Link": c.Type = TypeCommand.Link;
                    break;
                case "Log": c.Type = TypeCommand.Log;
                    break;

                default: c.Type = TypeCommand.Custom;
                    break;
            }

            c.Time = TimeSpan.Parse(i.Attribute("Time").Value).TotalSeconds;
            string[] commands = i.Attribute("Command").Value.Split('|');
            c.Command = commands[0];
            if (commands.Length == 2)
            {
                c.Duration = TimeSpan.Parse(commands[1]).TotalSeconds;
            }
            c.Visible = false;
            return c;
        }

        #endregion Methods
    }
}