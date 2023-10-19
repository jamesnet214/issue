using DimLibrary2;
using System.Windows;

namespace DicomTestViewer3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DicomReader2 dicom2 = new DicomReader2();
            var data2 = dicom2.ExtractDicomData("Data/studylist/");
        }
    }
}
