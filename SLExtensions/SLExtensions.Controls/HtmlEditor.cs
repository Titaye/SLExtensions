// <copyright file="HtmlEditor.cs" company="UCAYA">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Romuald Boulanger</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using SLExtensions;
    using SLExtensions.Controls.Animation;

    /// <summary>
    /// Html Editor Control
    /// </summary>
    [TemplatePart(Name = HtmlEditor.ElementRootName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = HtmlEditor.ElementPanelCommandContainerName, Type = typeof(Panel))]
    [TemplatePart(Name = HtmlEditor.ElementIframePlaceName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = HtmlEditor.ElementButtonBoldName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonItalicName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonUnderlineName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonIndentName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonOutdentName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonAlignLeftName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonAlignCenterName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonAlignRightName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonAlignJustifyName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonOrderedListName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonBulletedListName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonHorizontalRuleName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonSubscriptName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonSuperscriptName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonHyperLinkName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonImageName, Type = typeof(Button))]
    [TemplatePart(Name = HtmlEditor.ElementButtonForeColorName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = HtmlEditor.HtmlAnimationIframePlaceName, Type = typeof(HtmlAnimation))]
    [TemplatePart(Name = HtmlEditor.ElementForeColorPalettedName, Type = typeof(Panel))]
    [TemplatePart(Name = HtmlEditor.ElementButtonBackgroundColorName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = HtmlEditor.ElementBackgroundColorPalettedName, Type = typeof(Panel))]
    [TemplateVisualState(Name = HtmlEditor.StateMenuCollapsedName, GroupName = HtmlEditor.GroupStateMenuStatesName)]
    [TemplateVisualState(Name = HtmlEditor.StateMenuForeColorExpandedName, GroupName = HtmlEditor.GroupStateMenuStatesName)]
    [TemplateVisualState(Name = HtmlEditor.StateMenuBackgroundColorExpandedName, GroupName = HtmlEditor.GroupStateMenuStatesName)]
    public class HtmlEditor : Control, IDisposable
    {
        #region Fields

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(HtmlEditor), new PropertyMetadata(_TextChangedCallback));

        internal const string ElementBackgroundColorPalettedName = "BackgroundColorPalette";
        internal const string ElementButtonAlignCenterName = "ButtonAlignCenterElement";
        internal const string ElementButtonAlignJustifyName = "ButtonAlignJustifyElement";
        internal const string ElementButtonAlignLeftName = "ButtonAlignLeftElement";
        internal const string ElementButtonAlignRightName = "ButtonAlignRightElement";
        internal const string ElementButtonBackgroundColorName = "ButtonBackgroundColorElement";
        internal const string ElementButtonBoldName = "ButtonBoldElement";
        internal const string ElementButtonBulletedListName = "ButtonBulletedListElement";
        internal const string ElementButtonForeColorName = "ButtonForeColorElement";
        internal const string ElementButtonHorizontalRuleName = "ButtonHorizontalRuleElement";
        internal const string ElementButtonHyperLinkName = "ButtonHyperLinkElement";
        internal const string ElementButtonImageName = "ButtonImageElement";
        internal const string ElementButtonIndentName = "ButtonIndentElement";
        internal const string ElementButtonItalicName = "ButtonItalicElement";
        internal const string ElementButtonOrderedListName = "ButtonOrderedListElement";
        internal const string ElementButtonOutdentName = "ButtonOutdentElement";
        internal const string ElementButtonSubscriptName = "ButtonSubscriptElement";
        internal const string ElementButtonSuperscriptName = "ButtonSuperscriptElement";
        internal const string ElementButtonUnderlineName = "ButtonUnderlineElement";
        internal const string ElementForeColorPalettedName = "ForeColorPalette";
        internal const string ElementIframePlaceName = "IframePlaceElement";
        internal const string ElementPanelCommandContainerName = "PanelCommandContainer";
        internal const string ElementRootName = "RootElement";
        internal const string GroupStateMenuStatesName = "MenuStates";
        internal const string HtmlAnimationIframePlaceName = "HtmlAnimationIframePlace";
        internal const string StateMenuBackgroundColorExpandedName = "BackgroundColor Expanded";
        internal const string StateMenuCollapsedName = "Collapsed";
        internal const string StateMenuForeColorExpandedName = "ForeColor Expanded";

        internal Panel ElementBackgroundColorPalette;
        internal Button ElementButtonAlignCenter;
        internal Button ElementButtonAlignJustify;
        internal Button ElementButtonAlignLeft;
        internal Button ElementButtonAlignRight;
        internal ToggleButton ElementButtonBackgroundColor;
        internal Button ElementButtonBold;
        internal Button ElementButtonBulletedList;
        internal ToggleButton ElementButtonForeColor;
        internal Button ElementButtonHorizontalRule;
        internal Button ElementButtonHyperLink;
        internal Button ElementButtonImage;
        internal Button ElementButtonIndent;
        internal Button ElementButtonItalic;
        internal Button ElementButtonOrderedList;
        internal Button ElementButtonOutdent;
        internal Button ElementButtonSubscript;
        internal Button ElementButtonSuperscript;
        internal Button ElementButtonUnderline;
        internal Panel ElementForeColorPalette;
        internal FrameworkElement ElementIframePlace;
        internal Panel ElementPanelCommandContainer;
        internal FrameworkElement ElementRoot;
        internal HtmlAnimation HtmlAnimationIframePlace;

        protected const string ScriptVariableIframeDocument = "iframeDocument";
        protected const string ScriptVariableIframeDocumentFocusable = "iframeDocumentFocusable";

        private HtmlEditorMenuItem _currentExpandedMenuItem = HtmlEditorMenuItem.None;
        HtmlElement _htmlAnchorFocusElement = null;
        HtmlElement _htmlDivContainerElement = null;
        HtmlDocument _htmlIframeDocument = null;
        HtmlElement _htmlIframeEditorElement = null;
        HtmlWindow _htmlIframeWindow = null;
        bool _iframeHasFocus = false;
        bool _iframeInitialized = false;
        private string _initialText = string.Empty;
        bool _isDisposed = false;
        Guid _uniqueID;

        //private string text;
        //public string Text
        //{
        //    get
        //    {
        //        if (_iframeInitialized)
        //            return _htmlIframeDocument.Body.GetProperty("innerHTML") as string;
        //        else
        //            return text;
        //    }
        //    set
        //    {
        //        if (_iframeInitialized)
        //            _htmlIframeDocument.Body.SetProperty("innerHTML", value);
        //        else
        //            text = value;
        //    }
        //}
        private bool isReadOnly;
        DispatcherTimer timerInitialize = null;

        #endregion Fields

        #region Constructors

        public HtmlEditor()
        {
            DefaultStyleKey = typeof(HtmlEditor);

            this.LayoutUpdated += new EventHandler(HtmlEditor_LayoutUpdated);

            _uniqueID = Guid.NewGuid();

            this.MouseEnter += new MouseEventHandler(HtmlEditor_MouseEnter);

            _iframeInitialized = false;
            _isDisposed = false;
        }

        #endregion Constructors

        #region Enumerations

        public enum HtmlEditorMenuItem
        {
            None = 0
            ,
            ForeColor
                ,
            BackgroundColor
                ,
            FontSize
                , FontFamily
        }

        #endregion Enumerations

        #region Properties

        public string HtmlAnchorFocusID
        {
            get
            {
                return string.Format("{0}_{1}", _uniqueID, "af");
            }
        }

        public HtmlElement HtmlContainer
        {
            get
            {
                return !string.IsNullOrEmpty(this.HtmlContainerID) ? HtmlPage.Document.GetElementById(this.HtmlContainerID) : HtmlPage.Document.Body;
            }
        }

        public string HtmlContainerID
        {
            get;
            set;
        }

        public string HtmlDivContainerID
        {
            get
            {
                return string.Format("{0}_{1}", _uniqueID, "hdc");
            }
        }

        public string HtmlIframeID
        {
            get
            {
                return string.Format("{0}_{1}", _uniqueID, "hi");
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                if (!_iframeInitialized)
                    isReadOnly = value;
                else
                    throw new Exception("This value can not be changed once the iframe initialized.");

                //TODO: bug...
                //if (_iframeInitialized)
                //    SetHtmlDesignMode(!isReadOnly);
            }
        }

        public string Text
        {
            //get { return (string)GetValue(TextProperty); }
            get
            {
                if (_iframeInitialized)
                    return _htmlIframeDocument.Body.GetProperty("innerHTML") as string;
                else
                    return this._initialText;
            }
            set { SetValue(TextProperty, value); }
        }

        private bool IsBrowserIE
        {
            get
            {
                return (HtmlPage.BrowserInformation.Name == "Microsoft Internet Explorer");
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            if (HtmlAnimationIframePlace != null)
            {
                HtmlAnimationIframePlace.Pairs.Clear();

                HtmlAnimationIframePlace.ControlOwner = null;
            }

            RemoveHtml();

            _isDisposed = true;
        }

        public void HideHtml()
        {
            SetHtmlVisibility(false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ElementRoot = GetTemplateChild(ElementRootName) as FrameworkElement;

            ElementPanelCommandContainer = GetTemplateChild(ElementPanelCommandContainerName) as Panel;

            ElementIframePlace = GetTemplateChild(ElementIframePlaceName) as FrameworkElement;

            ElementButtonBold = GetTemplateChild(ElementButtonBoldName) as Button;
            ElementButtonItalic = GetTemplateChild(ElementButtonItalicName) as Button;
            ElementButtonUnderline = GetTemplateChild(ElementButtonUnderlineName) as Button;

            ElementButtonIndent = GetTemplateChild(ElementButtonIndentName) as Button;
            ElementButtonOutdent = GetTemplateChild(ElementButtonOutdentName) as Button;

            ElementButtonAlignLeft = GetTemplateChild(ElementButtonAlignLeftName) as Button;
            ElementButtonAlignCenter = GetTemplateChild(ElementButtonAlignCenterName) as Button;
            ElementButtonAlignRight = GetTemplateChild(ElementButtonAlignRightName) as Button;
            ElementButtonAlignJustify = GetTemplateChild(ElementButtonAlignJustifyName) as Button;

            ElementButtonOrderedList = GetTemplateChild(ElementButtonOrderedListName) as Button;
            ElementButtonBulletedList = GetTemplateChild(ElementButtonBulletedListName) as Button;

            ElementButtonHorizontalRule = GetTemplateChild(ElementButtonHorizontalRuleName) as Button;

            ElementButtonSubscript = GetTemplateChild(ElementButtonSubscriptName) as Button;
            ElementButtonSuperscript = GetTemplateChild(ElementButtonSuperscriptName) as Button;

            ElementButtonHyperLink = GetTemplateChild(ElementButtonHyperLinkName) as Button;
            ElementButtonImage = GetTemplateChild(ElementButtonImageName) as Button;

            ElementButtonForeColor = GetTemplateChild(ElementButtonForeColorName) as ToggleButton;

            ElementForeColorPalette = GetTemplateChild(ElementForeColorPalettedName) as Panel;

            ElementButtonBackgroundColor = GetTemplateChild(ElementButtonBackgroundColorName) as ToggleButton;

            ElementBackgroundColorPalette = GetTemplateChild(ElementBackgroundColorPalettedName) as Panel;

            ElementIframePlace.SizeChanged += new SizeChangedEventHandler(ElementIframePlace_SizeChanged);

            if (ElementButtonBold != null)
                ElementButtonBold.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonItalic != null)
                ElementButtonItalic.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonUnderline != null)
                ElementButtonUnderline.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonIndent != null)
                ElementButtonIndent.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonOutdent != null)
                ElementButtonOutdent.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonAlignLeft != null)
                ElementButtonAlignLeft.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonAlignCenter != null)
                ElementButtonAlignCenter.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonAlignRight != null)
                ElementButtonAlignRight.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonAlignJustify != null)
                ElementButtonAlignJustify.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonOrderedList != null)
                ElementButtonOrderedList.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonBulletedList != null)
                ElementButtonBulletedList.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonHorizontalRule != null)
                ElementButtonHorizontalRule.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonSubscript != null)
                ElementButtonSubscript.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonSuperscript != null)
                ElementButtonSuperscript.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonHyperLink != null)
                ElementButtonHyperLink.Click += new RoutedEventHandler(ElementButtonCommand_Click);
            if (ElementButtonImage != null)
                ElementButtonImage.Click += new RoutedEventHandler(ElementButtonCommand_Click);

            if (ElementButtonForeColor != null)
                ElementButtonForeColor.Click += new RoutedEventHandler(ElementButtonForeColor_Click);

            if (ElementForeColorPalette != null)
            {
                foreach (var item in ElementForeColorPalette.Children)
                {
                    Button button = item as Button;

                    if (item != null)
                    {
                        button.Click += new RoutedEventHandler(ButtonColorForeColor_Click);
                    }
                }
            }

            if (ElementButtonBackgroundColor != null)
                ElementButtonBackgroundColor.Click += new RoutedEventHandler(ElementButtonBackgroundColor_Click);

            if (ElementBackgroundColorPalette != null)
            {
                foreach (var item in ElementBackgroundColorPalette.Children)
                {
                    Button button = item as Button;

                    if (item != null)
                    {
                        button.Click += new RoutedEventHandler(ButtonColorBackgroundColor_Click);
                    }
                }
            }

            HtmlAnimationIframePlace = ElementRoot.Resources[HtmlEditor.HtmlAnimationIframePlaceName] as HtmlAnimation;

            if (ElementPanelCommandContainer != null && IsReadOnly)
            {
                ElementPanelCommandContainer.Visibility = Visibility.Collapsed;
            }

            InitializeHtml();
        }

        public void ShowHtml()
        {
            SetHtmlVisibility(true);
        }

        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Handle the Expansion states

            switch (_currentExpandedMenuItem)
            {
                case HtmlEditorMenuItem.None:
                    VisualStateManager.GoToState(this, StateMenuCollapsedName, useTransitions);
                    break;
                case HtmlEditorMenuItem.ForeColor:
                    VisualStateManager.GoToState(this, StateMenuForeColorExpandedName, useTransitions);
                    break;
                case HtmlEditorMenuItem.BackgroundColor:
                    VisualStateManager.GoToState(this, StateMenuBackgroundColorExpandedName, useTransitions);
                    break;
                case HtmlEditorMenuItem.FontSize:
                    break;
                case HtmlEditorMenuItem.FontFamily:
                    break;
                default:
                    break;
            }
        }

        protected virtual void ToggleMenu(HtmlEditorMenuItem item)
        {
            if (_currentExpandedMenuItem == item)
                _currentExpandedMenuItem = HtmlEditorMenuItem.None;
            else
                _currentExpandedMenuItem = item;

            ToggleButton button = ElementButtonForeColor;

            if (button != null)
                button.IsChecked = _currentExpandedMenuItem == HtmlEditorMenuItem.ForeColor;

            button = ElementButtonBackgroundColor;

            if (button != null)
                button.IsChecked = _currentExpandedMenuItem == HtmlEditorMenuItem.BackgroundColor;

            UpdateVisualState(true);
        }

        private static void _TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor sender = (HtmlEditor)d;

            if (sender._iframeInitialized)
            {
                sender._htmlIframeDocument.Body.SetProperty("innerHTML", (string)e.NewValue);
            }
            else
            {
                sender._initialText = (string)e.NewValue;
            }
        }

        private void ButtonColorBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                string color = (button.Tag is string) ? button.Tag.ToString() : string.Empty;

                ToggleMenu(HtmlEditorMenuItem.None);

                DoBackgroundColor(color);
            }
        }

        private void ButtonColorForeColor_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                string color = (button.Tag is string) ? button.Tag.ToString() : string.Empty;

                ToggleMenu(HtmlEditorMenuItem.None);

                DoForeColor(color);
            }
        }

        private void DoBackgroundColor(string color)
        {
            string commandName = this.IsBrowserIE ? "backcolor" : "hilitecolor";
            if (!string.IsNullOrEmpty(color))
                ExecuteCommand(commandName, color);
        }

        private void DoBold()
        {
            ExecuteCommand("bold", null);
        }

        private void DoBreak()
        {
            ExecuteCommand("justifyleft", null);
        }

        private void DoBulletedList()
        {
            ExecuteCommand("insertunorderedlist", null);
        }

        private void DoCenter()
        {
            ExecuteCommand("justifycenter", null);
        }

        private void DoFont(string fontName)
        {
            if (!string.IsNullOrEmpty(fontName))
                ExecuteCommand("fontname", fontName);
        }

        private void DoForeColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
                ExecuteCommand("forecolor", color);
        }

        private void DoFormat(string formatType)
        {
            if (!string.IsNullOrEmpty(formatType))
                ExecuteCommand("formatblock", string.Format("<{0}>", formatType));
        }

        private void DoIndent()
        {
            ExecuteCommand("indent", null);
        }

        private void DoItalic()
        {
            ExecuteCommand("italic", null);
        }

        private void DoJustify()
        {
            ExecuteCommand("justifyfull", null);
        }

        private void DoLeft()
        {
            ExecuteCommand("justifyleft", null);
        }

        private void DoOrderedList()
        {
            ExecuteCommand("insertorderedlist", null);
        }

        private void DoOutdent()
        {
            ExecuteCommand("outdent", null);
        }

        private void DoRight()
        {
            ExecuteCommand("justifyright", null);
        }

        private void DoRule()
        {
            ExecuteCommand("inserthorizontalrule", null);
        }

        private void DoSize(string fontSize)
        {
            if (!string.IsNullOrEmpty(fontSize))
                ExecuteCommand("fontsize", fontSize);
        }

        private void DoSubscript()
        {
            ExecuteCommand("subscript", null);
        }

        private void DoSuperscript()
        {
            ExecuteCommand("superscript", null);
        }

        private void DoUnderline()
        {
            ExecuteCommand("underline", null);
        }

        private void ElementButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu(HtmlEditorMenuItem.BackgroundColor);
        }

        private void ElementButtonCommand_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null && !this.IsReadOnly)
            {
                switch (button.Name)
                {
                    case HtmlEditor.ElementButtonBoldName:
                        DoBold();
                        break;
                    case HtmlEditor.ElementButtonItalicName:
                        DoItalic();
                        break;
                    case HtmlEditor.ElementButtonUnderlineName:
                        DoUnderline();
                        break;
                    case HtmlEditor.ElementButtonIndentName:
                        DoIndent();
                        break;
                    case HtmlEditor.ElementButtonOutdentName:
                        DoOutdent();
                        break;
                    case HtmlEditor.ElementButtonAlignLeftName:
                        DoLeft();
                        break;
                    case HtmlEditor.ElementButtonAlignCenterName:
                        DoCenter();
                        break;
                    case HtmlEditor.ElementButtonAlignRightName:
                        DoRight();
                        break;
                    case HtmlEditor.ElementButtonAlignJustifyName:
                        DoJustify();
                        break;
                    case HtmlEditor.ElementButtonBulletedListName:
                        DoBulletedList();
                        break;
                    case HtmlEditor.ElementButtonOrderedListName:
                        DoOrderedList();
                        break;
                    case HtmlEditor.ElementButtonHorizontalRuleName:
                        DoRule();
                        break;
                    case HtmlEditor.ElementButtonSubscriptName:
                        DoSubscript();
                        break;
                    case HtmlEditor.ElementButtonSuperscriptName:
                        DoSuperscript();
                        break;
                    default:
                        break;
                }
            }
        }

        private void ElementButtonForeColor_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu(HtmlEditorMenuItem.ForeColor);
        }

        private void ElementIframePlace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ScratchHtml();
        }

        private void ExecuteCommand(string commandName, string commandValue)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetIframeDocumentScript());
            sb.Append(GetIframeDocumentFocusableScript());
            sb.AppendFormat("{0}.focus();", ScriptVariableIframeDocumentFocusable);
            sb.AppendFormat("{0}.execCommand('{1}', false, {2});"
                        , ScriptVariableIframeDocument
                        , commandName
                        , string.IsNullOrEmpty(commandValue) ? "null" : string.Format("'{0}'", commandValue));

            System.Windows.Browser.HtmlPage.Window.Eval(sb.ToString());
        }

        private string GetIframeDocumentFocusableScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("var {0} =", ScriptVariableIframeDocumentFocusable);
            if (this.IsBrowserIE)
            {
                sb.AppendFormat("{0};", ScriptVariableIframeDocument);
            }
            else
            {
                sb.AppendFormat("document.getElementById('{0}').contentWindow;", this.HtmlIframeID);
            }

            return sb.ToString();
        }

        private string GetIframeDocumentScript()
        {
            return string.Format("var {1} = document.getElementById('{0}').contentDocument ? document.getElementById('{0}').contentDocument : document.frames['{0}'].document;", this.HtmlIframeID, ScriptVariableIframeDocument);
        }

        private string GetInitialIframeOuterHTML()
        {
            string outerHTML =
                "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                "<head>" +
                "<title>Design Editor Frame</title>" +
                "<style type='text/css'>" +
                "body { font-family: arial; font-size: 0.8em; background: transparent;}" +
                "p { margin: 0px; padding: 0px;}" +
                //"body {" + bgColor + "; " + fgColor + ";}" +
                "</style>" +
                //((this.get_designModeCss().length > 0) ? "<link rel='stylesheet' href='" + this.get_designModeCss() + "' type='text/css' />" : "") +
                "</head>" +
                "<body>" +
                "</body>" +
                "</html>";

            return outerHTML;
        }

        private void HandleHtmlBlur(object sender, HtmlEventArgs args)
        {
            _iframeHasFocus = false;
        }

        private void HandleHtmlFocus(object sender, HtmlEventArgs args)
        {
            _iframeHasFocus = true;
        }

        private void HandleHtmlKeyDown(object sender, HtmlEventArgs args)
        {
            if (!this.IsReadOnly)
            {
                // Ctrl + B or Ctrl + G
                if (args.CtrlKey && (args.CharacterCode == 66 || args.CharacterCode == 71))
                {
                    args.PreventDefault();
                    args.StopPropagation();

                    DoBold();
                }
                // Ctrl + I
                else if (args.CtrlKey && args.CharacterCode == 73)
                {
                    args.PreventDefault();
                    args.StopPropagation();

                    DoItalic();
                }
                // Ctrl + U
                else if (args.CtrlKey && args.CharacterCode == 85)
                {
                    args.PreventDefault();
                    args.StopPropagation();

                    DoUnderline();
                }
                // ENTER
                else if (args.KeyCode == 13)
                {
                    DoBreak();
                }
                // TAB
                else if (args.KeyCode == 9)
                {
                    args.PreventDefault();
                    args.StopPropagation();

                    if (args.ShiftKey)
                    {
                        DoOutdent();
                    }
                    else
                    {
                        DoIndent();
                    }
                }
            }
        }

        void HtmlEditor_LayoutUpdated(object sender, EventArgs e)
        {
            bool isRooted = this.IsInVisualTree();

            if (!isRooted)
                Dispose();
            else if (_isDisposed)
            {
                _isDisposed = false;
                InitializeHtml();
            }

            //Debug.DebugWrite("HtmlEditor_LayoutUpdated: isRooted = " + isRooted.ToString()
            //                    + " ID = " + this._uniqueID.ToString()
            //                    + " _htmlDivContainerElement = " + (_htmlDivContainerElement != null ? "OK" : "null")
            //                    + " _htmlInitialized = " + _htmlInitialized.ToString()
            //                    + " _iframeInitialized = " + _iframeInitialized.ToString()
            //                    + " _isDisposed = " + _isDisposed.ToString()
            //                    );
        }

        private void HtmlEditor_MouseEnter(object sender, MouseEventArgs e)
        {
            //IntializeHandlersHtml();
        }

        private void InitializeHtml()
        {
            // Create Container Div Element
            _htmlDivContainerElement = System.Windows.Browser.HtmlPage.Document.CreateElement("div");
            _htmlDivContainerElement.Id = this.HtmlDivContainerID;

            _htmlDivContainerElement.SetStyleAttribute("position", "absolute");

            // Create A Element
            _htmlAnchorFocusElement = System.Windows.Browser.HtmlPage.Document.CreateElement("a");
            _htmlAnchorFocusElement.SetAttribute("href", "#");
            _htmlAnchorFocusElement.SetStyleAttribute("position", "absolute");
            _htmlAnchorFocusElement.SetStyleAttribute("left", "0px");
            _htmlAnchorFocusElement.SetStyleAttribute("top", "0px");
            _htmlAnchorFocusElement.SetStyleAttribute("width", "0px");
            _htmlAnchorFocusElement.SetStyleAttribute("height", "0px");

            _htmlAnchorFocusElement.Id = this.HtmlAnchorFocusID;

            _htmlDivContainerElement.AppendChild(_htmlAnchorFocusElement);

            // Create Iframe Element
            _htmlIframeEditorElement = System.Windows.Browser.HtmlPage.Document.CreateElement("iframe");

            _htmlIframeEditorElement.Id = this.HtmlIframeID;

            _htmlIframeEditorElement.SetAttribute("frameBorder", "0");
            _htmlIframeEditorElement.SetAttribute("allowTransparency", "true");
            _htmlIframeEditorElement.SetStyleAttribute("position", "relative");
            _htmlIframeEditorElement.SetStyleAttribute("width", "100%");
            _htmlIframeEditorElement.SetStyleAttribute("height", "100%");

            _htmlDivContainerElement.AppendChild(_htmlIframeEditorElement);

            //System.Windows.Browser.HtmlPage.Document.Body.AppendChild(_htmlDivContainerElement);
            HtmlContainer.AppendChild(_htmlDivContainerElement);

            string initialText = !this.IsBrowserIE ? "<br/>" : "";

            if (!string.IsNullOrEmpty(this.Text))
                initialText = this.Text;

            StringBuilder sb = new StringBuilder();

            sb.Append("setTimeout(function(){");

            sb.Append(GetIframeDocumentScript());

            if (this.IsBrowserIE)
                sb.AppendFormat("{0}.designMode = '{1}';", ScriptVariableIframeDocument, isReadOnly ? "Off" : "On");

            sb.AppendFormat("{0}.open('text/html', 'replace');", ScriptVariableIframeDocument);
            sb.AppendFormat("{0}.write(\"{1}\");", ScriptVariableIframeDocument, this.GetInitialIframeOuterHTML());
            sb.AppendFormat("{0}.close();", ScriptVariableIframeDocument);

            //sb.Append("setTimeout(function(){");

            if (!this.IsBrowserIE)
                sb.AppendFormat("{0}.designMode = '{1}';", ScriptVariableIframeDocument, isReadOnly ? "Off" : "On");

            StringBuilder sbInit = new StringBuilder(initialText);
            sbInit.Replace("\"", "\\\"");
            sbInit.Replace("\r", "");
            sbInit.Replace("\n", "");

            sb.AppendFormat("{0}.body.innerHTML = \"{1}\";", ScriptVariableIframeDocument, sbInit.ToString());

            //sb.Append("}, 100);");

            //sb.AppendFormat("document.getElementById('{0}').contentWindow.IsReady = true;", this.HtmlIframeID);

            sb.Append("var w;");
            sb.AppendFormat("if({0}.parentWindow)", ScriptVariableIframeDocument);
            sb.AppendFormat("   w = {0}.parentWindow;", ScriptVariableIframeDocument);
            sb.AppendFormat("else if({0}.defaultView)", ScriptVariableIframeDocument);
            sb.AppendFormat("   w = {0}.defaultView;", ScriptVariableIframeDocument);
            sb.AppendFormat("else ", ScriptVariableIframeDocument);
            sb.AppendFormat("   w = {0}.parentWindow;", ScriptVariableIframeDocument);
            sb.Append("w.IsReady = true;");

            sb.Append("}, 100);");

            System.Windows.Browser.HtmlPage.Window.Eval(sb.ToString());

            timerInitialize = new DispatcherTimer();

            timerInitialize.Interval = TimeSpan.FromMilliseconds(100);
            timerInitialize.Tick += delegate
            {
                if (!_iframeInitialized)
                {
                    timerInitialize.Stop();

                    if (_htmlIframeEditorElement == null)
                    {
                        Dispose();
                        return;
                    }

                    IntializeHandlersHtml();

                    if (!_iframeInitialized)
                        timerInitialize.Start();
                }
            };
            timerInitialize.Start();

            // _htmlInitialized = true;
            //ScratchHtml();

            if (HtmlAnimationIframePlace != null)
            {
                HtmlAnimationPair hap = new HtmlAnimationPair();
                hap.HtmlElementName = this.HtmlDivContainerID;
                //hap.HtmlContainerName = this.HtmlContainerID;
                hap.SLElement = ElementIframePlace;

                HtmlAnimationIframePlace.Pairs.Add(hap);

                HtmlAnimationIframePlace.ControlOwner = this;
            }

            Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
        }

        private void IntializeHandlersHtml()
        {
            if (!_iframeInitialized)
            {
                if (_htmlIframeWindow == null && _htmlIframeEditorElement != null)
                    _htmlIframeWindow = _htmlIframeEditorElement.GetProperty("contentWindow") as HtmlWindow;

                if (_htmlIframeWindow == null
                    || _htmlIframeWindow.GetProperty("IsReady") == null)
                    return;

                if (_htmlIframeDocument == null)
                    _htmlIframeDocument = _htmlIframeWindow.GetProperty("document") as HtmlDocument;

                _htmlIframeDocument.AttachEvent("keydown", HandleHtmlKeyDown);

                if (this.IsBrowserIE)
                {
                    _htmlIframeWindow.AttachEvent("focus", HandleHtmlFocus);
                    _htmlIframeWindow.AttachEvent("blur", HandleHtmlBlur);
                }
                else
                {
                    _htmlIframeDocument.AttachEvent("focus", HandleHtmlFocus);
                    _htmlIframeDocument.AttachEvent("blur", HandleHtmlBlur);
                }

                _iframeInitialized = true;
            }
        }

        private void ReloadHtml()
        {
            if (HtmlAnimationIframePlace != null)
            {
                HtmlAnimationIframePlace.Pairs.Clear();

                HtmlAnimationIframePlace.ControlOwner = null;
            }

            RemoveHtml();
            InitializeHtml();
        }

        private void RemoveHandlersHtml()
        {
            if (_iframeInitialized)
            {
                _htmlIframeDocument.DetachEvent("keypress", HandleHtmlKeyDown);

                if (this.IsBrowserIE)
                {
                    _htmlIframeWindow.DetachEvent("focus", HandleHtmlFocus);
                    _htmlIframeWindow.DetachEvent("blur", HandleHtmlBlur);
                }
                else
                {
                    _htmlIframeDocument.DetachEvent("focus", HandleHtmlFocus);
                    _htmlIframeDocument.DetachEvent("blur", HandleHtmlBlur);
                }
                _iframeInitialized = false;
            }
        }

        private void RemoveHtml()
        {
            RemoveHandlersHtml();

            Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);

            //System.Windows.Browser.HtmlPage.Document.Body.RemoveChild(_htmlDivContainerElement);
            if (_htmlDivContainerElement != null)
                HtmlContainer.RemoveChild(_htmlDivContainerElement);

            _htmlAnchorFocusElement = null;
            _htmlIframeDocument = null;
            _htmlIframeWindow = null;
            _htmlIframeEditorElement = null;
            _htmlDivContainerElement = null;
        }

        void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point p = e.GetPosition(this);

                bool overlap = (p.X > 0 && p.X < this.ActualWidth) && (p.Y > 0 && p.Y < this.ActualHeight);

                if (!overlap && _iframeHasFocus)
                {
                    SetHtmlFocus(false);
                }
            }
            catch
            {
            }
        }

        // Remplacé par HtmlAnimation
        private void ScratchHtml()
        {
            GeneralTransform transform = ElementIframePlace.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point newLocation = transform.Transform(new Point());
            Size newSize = new Size(ElementIframePlace.ActualWidth, ElementIframePlace.ActualHeight);

            _htmlDivContainerElement.SetStyleAttribute("left", ((int)newLocation.X).ToString() + "px");
            _htmlDivContainerElement.SetStyleAttribute("top", ((int)newLocation.Y).ToString() + "px");
            _htmlDivContainerElement.SetStyleAttribute("width", ((int)newSize.Width).ToString() + "px");
            _htmlDivContainerElement.SetStyleAttribute("height", ((int)newSize.Height).ToString() + "px");
        }

        private void SetHtmlDesignMode(bool enable)
        {
            //((System.Windows.Browser.HtmlDocument)_htmlIframeEditorElement.GetProperty("contentDocument")).SetProperty("designMode", "On");

            StringBuilder sb = new StringBuilder();

            sb.Append(GetIframeDocumentScript());
            sb.AppendFormat("{0}.designMode = '{1}';", ScriptVariableIframeDocument, enable ? "On" : "Off");

            if (this.IsBrowserIE)
            {
                //TODO:
            }

            System.Windows.Browser.HtmlPage.Window.Eval(sb.ToString());
        }

        private void SetHtmlFocus(bool value)
        {
            StringBuilder sb = new StringBuilder();

            if (value)
            {
                sb.Append(GetIframeDocumentScript());
                sb.Append(GetIframeDocumentFocusableScript());
                sb.AppendFormat("{0}.focus();", ScriptVariableIframeDocumentFocusable);
            }
            else
            {
                sb.AppendFormat("document.getElementById('{0}').focus();", this.HtmlAnchorFocusID);
            }

            System.Windows.Browser.HtmlPage.Window.Eval(sb.ToString());
        }

        private void SetHtmlVisibility(bool value)
        {
            _htmlDivContainerElement.SetStyleAttribute("display", value ? "block" : "none");
        }

        #endregion Methods
    }
}