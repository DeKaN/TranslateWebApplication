namespace TranslateWebApplication
{
    using System.Configuration;

    [ConfigurationCollection(typeof(LangElement))]
    public class LangElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LangElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LangElement)element).Locale;
        }
    }
}