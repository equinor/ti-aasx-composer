namespace S
{
    public class Payload
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] PayloadData { get; set; }

        public Payload(IFormFile file) {
            FileName = file.FileName;
            ContentType = file.ContentType;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                PayloadData = ms.ToArray();
            }
        }
    }
    
}
