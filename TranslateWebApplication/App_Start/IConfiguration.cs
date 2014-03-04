namespace TranslateWebApplication
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    public interface IConfiguration
    {
        IEnumerable<SelectListItem> GetLanguagesList();

        IEnumerable<SelectListItem> GetServersList();

        string GetInstance();

        ImportPackage GetImportPackage(HttpServerUtilityBase mapper);
    }
}
