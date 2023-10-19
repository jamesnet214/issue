using System.IO;
using System.Collections.Generic;
using System.Drawing;
using FellowOakDicom.Imaging;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Windows.Media.Imaging;
using FellowOakDicom;
using FellowOakDicom.Imaging.NativeCodec;
using System.Windows.Data;
using System.Xml.Linq;
using System.Reflection;
using FellowOakDicom.Imaging.Codec;

namespace DimLibrary2
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
        public List<BitmapImage?> WpfImages { get; set; }
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
               FellowOakDicom.DicomFile dicomFile = FellowOakDicom.DicomFile.Open(file);

                FellowOakDicom.Imaging.DicomImage dcmImg = new FellowOakDicom.Imaging.DicomImage(dicomFile.Dataset);
                var _file = dicomFile;



                DicomData data = new DicomData
                {
                    PatientName = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.PatientName),
                    PatientID = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.PatientID),
                    PatientBirthDate = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.PatientBirthDate),
                    PatientSex = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.PatientSex),
                    StudyID = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.StudyID),
                    StudyDate = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.StudyDate),
                    StudyTime = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.StudyTime),
                    SeriesNumber = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.SeriesNumber),
                    SeriesDate = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.SeriesDate),
                    Modality = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom. DicomTag.Modality),
                    Manufacturer = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.Manufacturer),
                    ManufacturerModelName = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.ManufacturerModelName),
                    WpfImages = new()
                };


                bool hasPixelData = dicomFile.Dataset.Contains(FellowOakDicom.DicomTag.PixelData);
                if (hasPixelData)
                {
                    //FellowOakDicom.DicomImage dicomImage = new FellowOakDicom.DicomImage(dicomFile.Dataset);
                    DicomImage dicomImage = new DicomImage(dicomFile.Dataset);

                    string totalFramesStr = GetValueOrDefault(dicomFile.Dataset, FellowOakDicom.DicomTag.NumberOfFrames, "1");
                    if (!int.TryParse(totalFramesStr, out int totalFrames))
                    {
                        totalFrames = 1;  // 기본값 설정
                    }

                    data.TotalFrames = totalFramesStr;

                    if (totalFrames > 1)
                    {
                        data.FileType = DicomFileType.MultiFrameImage;
                    }
                    else
                    {
                        data.FileType = DicomFileType.SingleImage;
                    }

                    for (int i = 0; i < totalFrames; i++)
                    {
                        string fileName = file;
                        string filePath = file.Substring(0, (file.Length - fileName.Length));
                        string saveFilePath = string.Format("{0}/{1}.png", filePath, fileName);
                        FellowOakDicom.Imaging.IImage image = dicomImage.RenderImage(i);
                        var bitmap = image.AsSharpImage();
                        //var imageSharpImage = ConvertToBitmapImage(bitmap);
                        //data.WpfImages.Add(imageSharpImage);
                    }
                }
                else
                {
                    data.FileType = DicomFileType.DataOnly;
                }

                dataList.Add(data);
            }

            return dataList;
        }


        public BitmapImage ConvertToBitmapImage(Image<Bgra32> imageSharpImage)
        {
            using var memoryStream = new MemoryStream();
            imageSharpImage.Save(memoryStream, new PngEncoder());
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
        private string GetValueOrDefault(FellowOakDicom.DicomDataset dataset, FellowOakDicom.DicomTag tag, string defaultValue = null)
        {
            if (dataset.Contains(tag) && dataset.GetValues<string>(tag).Length > 0)
            {
                return dataset.GetValue<string>(tag, 0);
            }
            return defaultValue;
        }
        public byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png); // PNG 형식을 사용했지만 원하는 형식으로 변경 가능
                return memory.ToArray();
            }
        }
        public Bitmap ConvertBytesToBitmap(byte[] imageData)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream(imageData))
                {
                    return new Bitmap(memory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  // 예외 메시지 출력
                throw;  // 예외를 다시 발생시키거나 적절한 처리를 수행하세요.
            }
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
