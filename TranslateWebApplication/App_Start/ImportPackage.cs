namespace TranslateWebApplication
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ImportPackage
    {
        public string Lang { get; set; }
        public ImportPackageContext Context { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ImportPackageContext
    {
        [System.Xml.Serialization.XmlElementAttribute("Text")]
        public ImportPackageContextText[] Text { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ImportPackageContextText
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }
}