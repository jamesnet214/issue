using Dicom.Imaging;
using Dicom;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

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
        public List<byte[]> WpfImages { get; set; }
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
                    WpfImages = new List<byte[]>()
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
                        string fileName = file;
                        string filePath = file.Substring(0, (file.Length - fileName.Length));
                        string saveFilePath = string.Format("{0}/{1}.png", filePath, fileName);
                        Bitmap dcmBitmap = dcmImg.RenderImage().AsClonedBitmap();

                        //JPG 저장.
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.ReadWrite))
                            {
                                dcmBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                byte[] byteBitmap = ms.ToArray();
                                fs.Write(byteBitmap, 0, byteBitmap.Length);
                            }
                        }

                        var bytes = ConvertBitmapToByteArray(dcmBitmap);
                        data.WpfImages.Add(bytes);
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


        private string GetValueOrDefault(DicomDataset dataset, DicomTag tag, string defaultValue = null)
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
