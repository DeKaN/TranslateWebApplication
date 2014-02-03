namespace TranslateWebApplication
{
    using System.Configuration;

    [ConfigurationCollection(typeof(ServerElement))]
    public class ServerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).Key;
        }
    }
}