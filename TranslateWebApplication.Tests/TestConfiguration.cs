namespace TranslateWebApplication.Tests
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    class TestConfiguration : IConfiguration
    {
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
    }
}
