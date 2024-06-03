// Decompiled with JetBrains decompiler
// Type: DemoAdminLTE.Resources.Views.ChartViews.Titles
// Assembly: DemoAdminLTE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 771896B4-5B09-4A8A-B2F6-33E1EA9470A2
// Assembly location: C:\Users\thanh\Downloads\webserver_backup\hst6.win.npnlab.com_103.28.39.47\rapido.npnlab.com\bin\DemoAdminLTE.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DemoAdminLTE.Resources.Views.ChartViews
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    public class Titles
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Titles()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ResourceManager ResourceManager
        {
            get
            {
                if (Titles.resourceMan == null)
                    Titles.resourceMan = new ResourceManager("DemoAdminLTE.Resources.Views.ChartViews.Titles", typeof(Titles).Assembly);
                return Titles.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture
        {
            get => Titles.resourceCulture;
            set => Titles.resourceCulture = value;
        }

        public static string From => Titles.ResourceManager.GetString(nameof(From), Titles.resourceCulture);

        public static string To => Titles.ResourceManager.GetString(nameof(To), Titles.resourceCulture);
    }
}
