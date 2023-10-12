using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.UI.Units
{
    public class CustomDataGridColumnHeadersPresenter : DataGridColumnHeadersPresenter
    {
        static CustomDataGridColumnHeadersPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGridColumnHeadersPresenter), new FrameworkPropertyMetadata(typeof(CustomDataGridColumnHeadersPresenter)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomDataGridColumnHeader();
        }
    }
}
