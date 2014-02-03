namespace TranslateWebApplication
{
    using System.Configuration;

    public class LangElement : ConfigurationElement
    {
        [ConfigurationProperty("locale", IsKey = true, IsRequired = true)]
        public string Locale
        {
            get
            {
                return (string)this["locale"];
            }
            set
            {
                this["locale"] = value;
            }
        }

        [ConfigurationProperty("text", IsRequired = true)]
        public string Text
        {
            get
            {
                return (string)this["text"];
            }
            set
            {
                this["text"] = value;
            }
        }
    }
}