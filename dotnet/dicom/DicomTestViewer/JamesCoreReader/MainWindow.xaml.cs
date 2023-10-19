using DimLibrary2;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JamesCoreReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new DicomSetupBuilder()
       .RegisterServices(s =>
            s.AddFellowOakDicom()
              .AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>()
              .AddImageManager<ImageSharpImageManager>())
       .SkipValidation()
       .Build();

            DicomReader2 d = new();
            d.ExtractDicomData("Data/studylist/");
        }
    }
}