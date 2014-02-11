namespace TranslateWebApplication
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Linq;

    public class Configuration : IConfiguration
    {
        readonly TranslaterSection Config = ConfigurationManager.GetSection("translater") as TranslaterSection;
        private XElement importFile;

        public IEnumerable<SelectListItem> GetLanguagesList()
        {
            return from LangElement lang in Config.Languages
                        select new SelectListItem { Text = lang.Text, Value = lang.Locale };
        }

        public IEnumerable<SelectListItem> GetServersList()
        {
            return from ServerElement server in Config.Servers
                select
                    new SelectListItem { Text = string.Format("{0} [{1}]", server.Key, server.Url), Value = server.Key };
        }

        public string GetInstance()
        {
            return ConfigurationManager.AppSettings["instance"];
        }

        public XElement GetImportFile(HttpServerUtilityBase mapper)
        {
            return this.importFile ?? (this.importFile = XElement.Load(mapper.MapPath("~/import.xml")));
        }
    }
}