using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfApp1.UI.Units
{
    internal class CustomDataGridColumnHeader : DataGridColumnHeader
    {
        static CustomDataGridColumnHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGridColumnHeader), new FrameworkPropertyMetadata(typeof(CustomDataGridColumnHeader)));
        }
    }
}
