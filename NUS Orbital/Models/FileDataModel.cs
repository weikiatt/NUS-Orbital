namespace NUS_Orbital.Models
{
    public class FileDataModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }

        public FileDataModel(string fileName, string contentType, byte[] fileData) 
        { 
            this.FileName = fileName;
            this.ContentType = contentType;
            this.FileData = fileData;
        }
    }
}
