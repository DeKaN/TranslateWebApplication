namespace TranslateWebApplication
{
    using System.Configuration;

    public class TranslaterSection : ConfigurationSection
    {
        [ConfigurationProperty("languages")]
        public LangElementCollection Languages
        {
            get { return (LangElementCollection)this["languages"]; }
            set { this["languages"] = value; }
        }

        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers
        {
            get { return (ServerElementCollection)this["servers"]; }
            set { this["servers"] = value; }
        }
    }
}