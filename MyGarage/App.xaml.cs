using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyGarage
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

    private void Application_Startup ( object sender, StartupEventArgs e )
    {
      //Sprachauswahl-Formular anzeigen
      System.Windows.ShutdownMode sm = this.ShutdownMode ;
      this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown ;
      MultiLang.SelectLanguage sl = new MultiLang.SelectLanguage() ;
      sl.LoadSettingsAndShow() ;
      this.ShutdownMode = sm ;
    }

    }
}
