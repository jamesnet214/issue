using Jamesnet.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.UI.Views;

namespace WpfApp1
{
    internal class App : JamesApplication
    {
        protected override Window CreateShell()
        {
            return new CustomWindow();
        }
    }
}
