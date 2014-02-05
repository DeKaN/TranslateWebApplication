namespace TranslateWebApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Xml.Linq;

    using TranslateWebApplication.GoblinServiceReference;
    using TranslateWebApplication.Models;

    public class HomeController : Controller
    {
        private static TranslaterSection Config;

        private readonly IGoblinService serviceClient;

        private const string Login = "test", Password = "test";

        private XElement importFile;

        private XElement GetImportFile()
        {
            // TODO: Fix possible NPE in Server
            return this.importFile ?? (this.importFile = XElement.Load(this.Server.MapPath("~/import.xml")));
        }

        private readonly string instance;

        public HomeController()
            : this(new GoblinServiceClient(), ConfigurationManager.GetSection("translater") as TranslaterSection)
        {
        }

        public HomeController(IGoblinService goblinService, TranslaterSection configSection)
        {
            Config = configSection;
            instance = ConfigurationManager.AppSettings["instance"];
            serviceClient = goblinService;
        }

        private TranslateContext LoadItems(string lang, List<string> ids = null)
        {
            if (GetImportFile().Element("Lang").Value != lang)
                return null;
            var context = GetImportFile().Element("Context");
            string contextValue = context.Attribute("Value").Value;
            var items = (from item in context.Elements("Text")
                         let id = item.Attribute("Id").Value
                         where ids == null || ids.Contains(id)
                         select new RowItem { Id = id, TemplateText = item.Value, TemplateLang = lang }).ToList();
            ids = items.Select(item => item.Id).ToList();
            ContextKey key = new ContextKey { Context = contextValue, Ids = ids };
            var searchParams = new TranslateSearchParameters { Context = key, Language = lang };
            var answer = serviceClient.SearchTranslate(Login, Password, instance, searchParams);
            if (answer.ErrorCode != GeneralErrorCode.Ok)
                throw new Exception(answer.ErrorDescription);
            var result = new TranslateContext(items) { Value = contextValue };
            foreach (var translatedItem in answer.Result.Content)
            {
                var rowItem = result[translatedItem.IdInContext];
                if (rowItem != null)
                {
                    rowItem.Translate = translatedItem.Translate;
                }
            }
            return result;
        }

        public ActionResult Index(string language)
        {
            var servers = from ServerElement server in Config.Servers
                          select new ServerData { Name = server.Key, Url = server.Url };
            var langs = from LangElement lang in Config.Languages
                        select new SelectListItem { Text = lang.Text, Value = lang.Locale, Selected = language != null && lang.Locale == language };
            var translateData = language != null ? LoadItems(language) : null;
            return View(new TableWithHeader { ServersList = servers, LanguageList = langs, TableData = translateData });
        }


        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByHeader = "X-Requested-With", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Confirm(TranslateContext data)
        {
            var ids = data.Where(item => !string.IsNullOrWhiteSpace(item.NewTranslate)).Select(item => item.Id).ToList();
            string lang = data[0].TemplateLang;
            var translateContext = LoadItems(lang, ids);
            ModelState.Clear();
            foreach (var rowItem in translateContext)
            {
                rowItem.IsChanged = true;
                rowItem.NewTranslate = data[rowItem.Id].NewTranslate;
            }
            translateContext.IsReadOnly = true;
            return View(translateContext);
        }

        [HttpPost]
        public ActionResult Save(TranslateContext data)
        {
            string lang = data[0].TemplateLang;
            string contextValue = GetImportFile().Element("Context").Attribute("Value").Value;
            var keyedTexts = (from item in data
                              where item.IsChanged
                              let key = new ContextKey { Context = contextValue, Ids = new List<string> { item.Id } }
                              select new KeyedText { Key = key, Text = item.NewTranslate }).ToList();
            var package = new KeyedTextPackage { KeyedTexts = keyedTexts };
            var answer = serviceClient.UpdateOrCreateTranslateListByKey(Login, Password, instance, package, lang);
            StringBuilder sb = new StringBuilder("Status: ");
            sb.Append(answer.ErrorCode).Append(".\n");
            if (answer.ErrorCode != GeneralErrorCode.Ok)
            {
                sb.Append(answer.ErrorDescription);
            }
            else
            {
                var result = answer.Result;
                sb.Append("Added: ")
                    .Append(result.AddedCount)
                    .Append(". Edited: ")
                    .Append(result.EditedCount)
                    .Append(".");
            }
            ViewBag.Message = sb.ToString();
            return View();
        }
    }
}