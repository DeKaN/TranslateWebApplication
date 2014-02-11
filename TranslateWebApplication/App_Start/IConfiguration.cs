namespace TranslateWebApplication
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Linq;

    public interface IConfiguration
    {
        IEnumerable<SelectListItem> GetLanguagesList();

        IEnumerable<SelectListItem> GetServersList();

        string GetInstance();

        XElement GetImportFile(HttpServerUtilityBase mapper);
    }
}
