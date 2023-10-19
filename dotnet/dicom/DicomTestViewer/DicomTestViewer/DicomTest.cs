using FellowOakDicom;
using FellowOakDicom.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace DicomTestViewer
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
        public List<BitmapImage> WpfImages { get; set; }
        public string TotalFrames { get; internal set; }
        public DicomFileType FileType { get; set; }
    }

    public class DicomReader
    {
        public List<DicomData> ExtractDicomData(string directory)
        {
            List<DicomData> dataList = new List<DicomData>();

            foreach (var filePath in Directory.GetFiles(directory, "*.dcm", SearchOption.AllDirectories))
            {

                new DicomSetupBuilder()
                    .RegisterServices(s => s.AddFellowOakDicom().AddImageManager<ImageSharpImageManager>())
                    .Build();


                DicomFile dicomFile = DicomFile.Open(filePath);

                try
                {
                    DicomData data = new DicomData
                    {
                        PatientName = GetValueOrDefault(dicomFile.Dataset, DicomTag.PatientName),
                        PatientID = GetValueOrDefault(dicomFile.Dataset, DicomTag.PatientID),
                        PatientBirthDate = GetValueOrDefault(dicomFile.Dataset, DicomTag.PatientBirthDate),
                        PatientSex = GetValueOrDefault(dicomFile.Dataset, DicomTag.PatientSex),
                        StudyID = GetValueOrDefault(dicomFile.Dataset, DicomTag.StudyID),
                        StudyDate = GetValueOrDefault(dicomFile.Dataset, DicomTag.StudyDate),
                        StudyTime = GetValueOrDefault(dicomFile.Dataset, DicomTag.StudyTime),
                        SeriesNumber = GetValueOrDefault(dicomFile.Dataset, DicomTag.SeriesNumber),
                        SeriesDate = GetValueOrDefault(dicomFile.Dataset, DicomTag.SeriesDate),
                        Modality = GetValueOrDefault(dicomFile.Dataset, DicomTag.Modality),
                        Manufacturer = GetValueOrDefault(dicomFile.Dataset, DicomTag.Manufacturer),
                        ManufacturerModelName = GetValueOrDefault(dicomFile.Dataset, DicomTag.ManufacturerModelName),
                        WpfImages = new List<BitmapImage>()
                    };

                    bool hasPixelData = dicomFile.Dataset.Contains(DicomTag.PixelData);
                    if (hasPixelData)
                    {
                        DicomImage dicomImage = new DicomImage(dicomFile.Dataset);

                        string totalFramesStr = GetValueOrDefault(dicomFile.Dataset, DicomTag.NumberOfFrames, "1");
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
                            FellowOakDicom.Imaging.IImage image = dicomImage.RenderImage(i);
                            var bitmap = image.AsSharpImage();
                            var imageSharpImage = ConvertToBitmapImage(bitmap);
                            data.WpfImages.Add(imageSharpImage);
                        }
                    }
                    else
                    {
                        data.FileType = DicomFileType.DataOnly;
                    }

                    dataList.Add(data);
                }
                catch (Exception ex)
                {
                    // 오류 처리 코드를 추가하십시오. 예를 들면:
                    // Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
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
    }
}
