﻿namespace TranslateWebApplication
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Serialization;

    public class Configuration : IConfiguration
    {
        readonly TranslaterSection Config = ConfigurationManager.GetSection("translater") as TranslaterSection;

        private ImportPackage importPackage;

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

        public ImportPackage GetImportPackage(HttpServerUtilityBase mapper)
        {
            using (var reader = new StreamReader(mapper.MapPath("~/import.xml")))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ImportPackage));
                importPackage = (ImportPackage)deserializer.Deserialize(reader);
            }
            return importPackage;
        }
    }
}