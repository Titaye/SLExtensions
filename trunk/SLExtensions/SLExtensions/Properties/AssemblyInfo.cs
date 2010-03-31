#region Header

// <copyright file="AssemblyInfo.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>

#endregion Header

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Resources;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SLExtensions")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SLExtensions")]
[assembly: AssemblyCopyright("Copyright ©  2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("SLExtensions.Test")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("62a418ef-2ba1-4c57-9025-cd145da0fa18")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: CLSCompliant(false)]
[assembly: XmlnsPrefix("http://www.slextensions.net/2009/data", "sld")]
[assembly: XmlnsDefinition("http://www.slextensions.net/2009/data", "SLExtensions.Data")]
[assembly: XmlnsPrefix("http://www.slextensions.net/2009/input", "sli")]
[assembly: XmlnsDefinition("http://www.slextensions.net/2009/input", "SLExtensions.Input")]
[assembly: XmlnsPrefix("http://www.slextensions.net/2009/globalization", "slg")]
[assembly: XmlnsDefinition("http://www.slextensions.net/2009/globalization", "SLExtensions.Globalization")]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
