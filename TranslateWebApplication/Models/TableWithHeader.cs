namespace TranslateWebApplication.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class TableWithHeader
    {
        public IEnumerable<ServerData> ServersList { get; set; }
        public IEnumerable<SelectListItem> LanguageList { get; set; }

        public TranslateContext TableData { get; set; }
    }
}