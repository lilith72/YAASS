using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JustinsASS
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
            //MessageBox.Show($"Unhandled exception occurred: {e.Exception.Message}"
            //    + Environment.NewLine + $"stack trace: {e.Exception.StackTrace}",
            //    "Exception Occurred", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
