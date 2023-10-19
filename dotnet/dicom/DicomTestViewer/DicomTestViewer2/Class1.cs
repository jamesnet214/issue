using Dicom.Imaging;
using Dicom;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Windows.Media.Imaging;

namespace DimLibrary
{
    public enum DicomFileType
    {
        SingleImage,
        MultiFrameImage,
        DataOnly
    }

    public class DicomData
    {
        public string PatientName { get; set; }
        public string PatientID { get; set; }
        public string PatientBirthDate { get; set; }
        public string PatientSex { get; set; }
        public string StudyID { get; set; }
        public string StudyDate { get; set; }
        public string StudyTime { get; set; }
        public string SeriesNumber { get; set; }
        public string SeriesDate { get; set; }
        public string Modality { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerModelName { get; set; }
        public List<BitmapSource> WpfImages { get; set; }
        public string TotalFrames { get; internal set; }
        public DicomFileType FileType { get; set; }
    }

    public class DicomReader2
    {
        public List<DicomData> ExtractDicomData(string directory)
        {
            List<DicomData> dataList = new List<DicomData>();

            foreach (string file in Directory.GetFiles(directory, "*.dcm", SearchOption.AllDirectories))
            {
                DicomFile dicomFile = DicomFile.Open(file);

                DicomImage dcmImg = new DicomImage(dicomFile.Dataset);

                var _file = dicomFile;

                string fileName = file;
                string filePath = file.Substring(0, (file.Length - fileName.Length));
                string saveFilePath = string.Format("{0}/{1}.png", filePath, fileName);
                System.Drawing.Bitmap dcmBitmap = dcmImg.RenderImage().AsClonedBitmap();

            }

            return dataList;
        }


        private string GetValueOrDefault(DicomDataset dataset, DicomTag tag, string defaultValue = null)
        {
            if (dataset.Contains(tag) && dataset.GetValues<string>(tag).Length > 0)
            {
                return dataset.GetValue<string>(tag, 0);
            }
            return defaultValue;
        }

        //public BitmapImage ConvertToBitmapImage(Image<Bgra32> imageSharpImage)
        //{
        //    using var memoryStream = new MemoryStream();
        //    imageSharpImage.Save(memoryStream, new PngEncoder());
        //    memoryStream.Position = 0;

        //    var bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.StreamSource = memoryStream;
        //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmapImage.EndInit();
        //    bitmapImage.Freeze();

        //    return bitmapImage;
        //}
    }
}
