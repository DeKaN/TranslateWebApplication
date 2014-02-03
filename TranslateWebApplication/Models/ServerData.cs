namespace TranslateWebApplication.Models
{
    public class ServerData
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Url);
        }
    }
}