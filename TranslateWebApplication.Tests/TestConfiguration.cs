namespace TranslateWebApplication.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    class TestConfiguration : IConfiguration
    {
        private const string ImportStub = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + "<ImportPackage><Lang>en-us</Lang>" 
            + "<Context Value=\"opentao:interface:exception:message\">" 
            + "<Text Id=\"ID_1\">Account with login '{login}' is banned!</Text>" 
            + "<Text Id=\"ID_2\">Account {personalAccountId} not found.</Text>" 
            + "</Context></ImportPackage>";

        public IEnumerable<SelectListItem> GetLanguagesList()
        {
            return new List<SelectListItem>
                   {
                       new SelectListItem { Text = "English", Value = "en-us" }
                   };
        }

        public IEnumerable<SelectListItem> GetServersList()
        {
            return new List<SelectListItem>
                   {
                       new SelectListItem { Text = "dev [test]", Value = "dev" }
                   };
        }

        public string GetInstance()
        {
            return "opentao";
        }

        public ImportPackage GetImportPackage(HttpServerUtilityBase mapper)
        {
            TextReader reader = null;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ImportPackage));
                reader = new StringReader(ImportStub);
                ImportPackage result = (ImportPackage)deserializer.Deserialize(reader);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
