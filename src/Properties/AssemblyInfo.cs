using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("LPE")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2022")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: ComVisible(false)]
[assembly: InternalsVisibleTo("lpe.tests")]
[assembly: AssemblyFileVersion("1.22.0218.0647")]
[assembly: AssemblyInformationalVersion("1.22.0218")]
[assembly: AssemblyVersion("1.22.0218.0647")]