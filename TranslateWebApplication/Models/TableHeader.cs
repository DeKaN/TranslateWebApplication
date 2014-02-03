namespace TranslateWebApplication.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class TableHeader
    {
        public IEnumerable<ServerData> ServersList { get; set; }
        public IEnumerable<SelectListItem> LanguageList { get; set; } 
    }
}