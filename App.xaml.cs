using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YAASS.Engine.Contract.FrontEndInterface;

namespace YAASS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void DisplayUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string dumpString = $"Unhandled exception occurred: {e.Exception}"
                    + Environment.NewLine + $"stack trace: {e.Exception.StackTrace}";
                ASS.Instance.GetInstanceLogger().Log(dumpString, Engine.Contract.DataModel.AssLogLevel.Error);
                MessageBox.Show(dumpString,
                    "Exception Occurred", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Was not able to display and log exception due to exception: {ex}");
            }
        }
    }
}
