<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SLExtensionsSite.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SLExtensions</title>
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <style type="text/css">
        html, body
        {
            color: White;
            font-size: 100%;
            font-family: Arial;
        }
        html, body, form
        {
            height: 100%;
            width: 100%;
            overflow: auto;
            font-family: Arial;
        }
        form
        {
            font-size: 0.9em;
        }
        body
        {
            padding: 0;
            margin: 0;
            background-color: #002339;
        }
        #top
        {
            position: absolute;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 700px;
            background-color: black;
            background-image: url('Assets/bkgGradient.jpg');
            background-repeat: repeat-x;
            background-position: left bottom;
        }
        #logo
        {
            position: absolute;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 213px;
            background-image: url('Assets/logo.jpg');
            background-repeat: no-repeat;
            background-position: center top;
        }
        #footer
        {
            position: absolute;
            top: 700px;
            width: 100%;
            font-size: 0.9em;
            left: 0px;
        }
        #descriptionHolder
        {
            background-image: url('Assets/bkgGradientTransp.png');
            background-repeat: no-repeat;
            width: 506px;
            height: 450px;
            float: left;
            padding: 10px;
        }
        #descriptionHolder h1
        {
            color: #0091E4;
            font-weight: normal;
            margin: 0;
        }
        #descriptionHolder h2, .title
        {
            color: #0091E4;
            font-weight: normal;
            margin: 0;
        }
        #description
        {
            padding-top: 10px;
            display: block;
            width: 490px;
        }
        #actions
        {
            padding-top: 10px;
            float: right;
            background-image: url('Assets/bkgGradientTranspSmall.png');
            background-repeat: no-repeat;
            width: 282px;
            height: 450px;
        }
        #footer ul
        {
            position: relative;
            margin-left: auto;
            margin-right: auto;
            width: 828px;
            margin-top: 20px;
            list-style-type: none;
        }
        #footer ul li h1
        {
            color: #02548C;
            font-weight: normal;
            font-size: 1.4em;
            margin: 0;
            margin-left: 30px;
        }
        #footer ul li
        {
            float: left;
            width: 25%;
        }
        #footer ul li p
        {
            color: #9A9997;
            padding-left: 10px;
            background-image: url(Assets/li.jpg);
            background-repeat: no-repeat;
            background-position: left top;
            margin-left: 20px;
            margin-right: 20px;
        }
        #mainContent
        {
            margin-top: 149px;
            height: 500px;
        }
        #mainContentCenter
        {
            position: relative;
            margin-left: auto;
            margin-right: auto;
            width: 828px;
        }
        .info
        {
            color: #8DBAD9;
            font-size: 0.7em;
            display: block;
            text-decoration: none;
        }
        #featureSections
        {
            list-style-type: none;
            padding: 0;
            margin: 0;
            
        }
        
        #featureSections li
        {
            float: left;
            margin: 0;
            margin-left:10px;
        }
        
        <%--#featureControls
        {
        	width: 200px;
        }
        
        featureLibrary
        {
        	width: 150px;
        }
        
        .featurePlayers
        {
        	width: 150px;
        }--%>
        
        .features
        {
            list-style-type: none;
            padding: 0;
            margin: 0;
        }
        .features li
        {
            clear: both;
            margin: 0;
            padding: 0;
        }
        .btnHover
        {
            width: 296px;
            height: 96px;
            display: block;
            padding: 0;
            padding-top: 10px;
            margin: 0;
        }
        .btnHover:hover
        {
            background-repeat: no-repeat;
            background-image: url('Assets/btnHover.png');
        }
        #silverlightControlHost
        {
            width: 282px;
            height: 96px;
        }
        .btn
        {
            width: 200px;
            height: 96px;
            display: block;
            background-repeat: no-repeat;
            padding-left: 60px;
            text-decoration: none;
            margin-left: 10px;
            margin-right: 10px;
            cursor: pointer;
        }
        .news
        {
            background-image: url('Assets/silverlight.png');
            padding-left: 80px;
        }
        .download
        {
            background-image: url('Assets/iconDownload.png');
        }
        .showcase
        {
            background-image: url('Assets/iconShowcase.png');
        }
        .addcontrib
        {
            background-image: url('Assets/iconAdd.png');
        }
        .title
        {
            text-decoration: none;
            font-size: large;
        }
        #footerSeparator
        {
            position: relative;
            height: 10px;
            width: 100%;
        }
        #footerSepLeft
        {
            position: relative;
            float: left;
            background: #0171BB;
            width: 50%;
            height: 100%;
        }
        #footerSepRight
        {
            position: relative;
            float: right;
            background: black;
            width: 50%;
            height: 100%;
        }
        #footerBkg
        {
            position: absolute;
            margin-left: -512px;
            left: 50%;
            width: 1024px;
            height: 100%;
            background-image: url('Assets/horzGradient.jpg');
            background-repeat: repeat-y;
        }
        .downloadslext
        {
            display: block;
            position: relative;
            bottom: 10px;
            left: 40px;
            width: 268px;
            height: 40px;
            background-image: url('Assets/download.png');
            text-decoration: none;
            color: White;
            background-repeat: no-repeat;
            text-align: center;
            padding-top: 10px;
        }
        .downloadslext span
        {
            position: relative;
            font-size: 1.2em;
            width: 228px;
            margin-left: 10px;
        }
        .downloadslext:hover
        {
            background-image: url('Assets/downloadHover.png');
        }
        .footerlink
        {
            text-decoration: none;
            color: #9A9997;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

    <script type="text/javascript">

        function downloadProgress(sender, eventArgs) {
            //sender.findName("tb").Text = "" + Math.round(eventArgs.progress * 100) + " %";
            var grd = sender.findName("opGradient");
            var stop1 = grd.GradientStops.GetItem(1);
            var stop2 = grd.GradientStops.GetItem(2);

            var val1 = eventArgs.progress;
            var val2 = val1 - 0.05;
            stop1.Offset = val1;
            stop2.Offset = val2 < 0 ? 0 : val2;
        }
    </script>

    <script type="text/javascript">
        var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
        document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
    </script>

    <script type="text/javascript">
        var pageTracker = _gat._getTracker("UA-6095100-1");
        pageTracker._trackPageview();
    </script>

    <div id="top">
    </div>
    <div id="logo">
    </div>
    <div id="footer">
        <div id="footerSeparator">
            <div id="footerSepLeft">
            </div>
            <div id="footerSepRight">
            </div>
            <div id="footerBkg">
            </div>
        </div>
        <ul>
            <li>
                <h1>
                    <asp:Label ID="about" runat="server" Text="About"></asp:Label>
                </h1>
                <p>
                    <asp:Label ID="aboutContent" runat="server" Text="Free control library released on MS-PL license"></asp:Label>
                </p>
            </li>
            <li>
                <h1>
                    <asp:Label ID="lastAdditions" runat="server" Text="Last additions"></asp:Label>
                </h1>
                <p>
                    <asp:Label ID="Label6" runat="server" Text="Html editor"></asp:Label></p>
                <p>
                    <asp:Label ID="Label5" runat="server" Text="Bootstraper"></asp:Label></p>
                <p>
                    <asp:Label ID="Label3" runat="server" Text="Binding comparer"></asp:Label></p>
                <p>
                    <asp:Label ID="lastAdditionsContent" runat="server" Text="Captcha control"></asp:Label></p>
                <p>
                    <asp:Label ID="Label1" runat="server" Text="RTW compatibility"></asp:Label>
                </p>
            </li>
            <li>
                <h1>
                    <asp:Label ID="contributors" runat="server" Text="Contributors"></asp:Label>
                </h1>
                <p>
                    <asp:Label ID="contributorsContent" runat="server" Text="Thierry Bouquain"></asp:Label></p>
                <p>
                    <asp:Label ID="Label2" runat="server" Text="Pierre Lagarde"></asp:Label></p>
                <p>
                    <asp:Label ID="Label4" runat="server" Text="Simon Ferquel"></asp:Label>
                </p>
            </li>
            <li>
                <h1>
                    <asp:Label ID="Links" runat="server" Text="Links"></asp:Label>
                </h1>
                <p>
                    <asp:HyperLink ID="link1" runat="server" CssClass="footerlink" Text="Codeplex project"
                        NavigateUrl="http://www.codeplex.com/SLExtensions" /></p>
                <p>
                    <asp:HyperLink ID="link2" runat="server" CssClass="footerlink" Text="Discussions"
                        NavigateUrl="http://www.codeplex.com/SLExtensions/Thread/List.aspx" /></p>
                <p>
                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="footerlink" Text="SLExtensions at Ucaya Blog (in French) "
                        NavigateUrl="http://www.ucaya.com/blog/SearchView.aspx?q=slextensions" /></p>
            </li>
        </ul>
    </div>
    <div id="mainContentCenter">
        <div id="mainContent">
            <div id="descriptionHolder">
                <h1>
                    SLExtensions Alpha 2</h1>
                <asp:Label ID="releaseDate" CssClass="info" runat="server" Text="mardi 28 Octobre 2008"></asp:Label>
                <div id="description">
                    <p>
                        The aim of Silverlight Extensions is to group usefull controls and silverlight best
                        practises in one place.</p>
                    <p>
                        SLExtensions is already used in several in production projects like <a href="http://info.francetv.fr">
                            http://info.francetv.fr</a> or <a href="http://www.nouvelle-renault-megane.com">http://www.nouvelle-renault-megane.com</a>.</p>
                    <p>
                        All contributions are welcome. You can send us your piece of code or become a permanent
                        contributor.</p>
                </div>
                <ul id="featureSections">
                    <li id="featureControls">
                        <h2>
                            Controls</h2>
                        <ul class="features">
                            <li>Treeview</li>
                            <li>Captcha</li>
                            <li>Virtualized stack panel</li>
                            <li>Dockpanel</li>
                            <li>Flow layout</li>
                            <li>Viewbox</li>
                            <li>GoogleMap</li>
                            <li>Virtual earth</li>
                            <li>Change tracker</li>
                            <li>Binding comparer</li>
                            <li>Bootstrap</li>
                            <li>HTML Editor</li>
                        </ul>
                    </li>
                    <li id="featureLibrary">
                        <h2>
                            Library</h2>
                        <ul class="features">
                            <li>CommandService</li>
                            <li>Deepzoom helpers</li>
                            <li>Mouse wheel listener</li>
                            <li>Various comparers</li>
                            <li>Google analytics</li>
                            <li>Browser history</li>
                        </ul>
                    </li>
                    <li id="featurePlayers">
                        <h2>
                            Players</h2>
                        <ul class="features">
                            <li>Deepzoom</li>
                            <li>Video</li>
                            <li>Photo</li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div id="actions">
                <div class="btn news">
                    <asp:Label ID="news" runat="server" Text="Latest source are now compatible with Silverlight 2 RTW."></asp:Label>
                </div>
                <div class="btnHover">
                    <a class="btn download" href="http://www.codeplex.com/SLExtensions/Release/ProjectReleases.aspx#ReleaseFiles">
                        <asp:Label ID="downloadSLExtensions" CssClass="title" runat="server" Text="Download SLExtensions"></asp:Label>
                        <asp:Label ID="downloadSLExtensionsInfo" CssClass="info" runat="server" Text="Silverlight 2 compatible"></asp:Label>
                    </a>
                </div>
                <div class="btnHover">
                    <a class="btn showcase" href="showcase.aspx">
                        <asp:Label ID="showcase" CssClass="title" runat="server" Text="Showcase"></asp:Label>
                        <asp:Label ID="showcaseInfo" CssClass="info" runat="server" Text="Live samples from SLExtensions library."></asp:Label>
                    </a>
                </div>
                <%-- <div id="silverlightControlHost">
                    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                        width="100%" height="100%">
                        <param name="source" value="ClientBin/SLExtensionsSite.xap" />
                        <param name="background" value="transparent" />
                        <param name="windowless" value="true" />
                        <param name="minRuntimeVersion" value="2.0.31005.0" />
                        <param name="autoUpgrade" value="true" />
                        <param name="splashscreensource" value="ClientBin/splash.xaml" />
                        <param name="onSourceDownloadProgressChanged" value="downloadProgress" />
                    </object>
                    
                    <iframe style='visibility: hidden; height: 0; width: 0; border: 0px'></iframe>
                </div>--%>
                <div class="btnHover">
                    <a class="btn addcontrib" href="mailto:thierry.bouquain@ucaya.com?subject=SLextensions%20Contribution">
                        <asp:Label ID="addContrib" CssClass="title" runat="server" Text="Send a contribution"></asp:Label>
                        <asp:Label ID="addContribInfo" CssClass="info" runat="server" Text="Email us and become a contributor. All controls are welcome."></asp:Label>
                    </a>
                </div>
            </div>
        </div>
        <a class="downloadslext" href="http://www.codeplex.com/SLExtensions/Release/ProjectReleases.aspx#ReleaseFiles">
            <asp:Label ID="downloadSlExt" runat="server" Text="Download SLExtensions"></asp:Label>
        </a>
    </div>
    </form>
</body>
</html>
