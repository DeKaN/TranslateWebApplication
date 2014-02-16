namespace TranslateWebApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using TranslateWebApplication.GoblinServiceReference;
    using TranslateWebApplication.Models;

    using Configuration = TranslateWebApplication.Configuration;

    public class HomeController : Controller
    {
        private static IConfiguration configuration;

        private readonly IGoblinService serviceClient;

        private const string Login = "test", Password = "test";

        public HomeController()
            : this(new GoblinServiceClient(), new Configuration())
        {
        }

        public HomeController(IGoblinService goblinService, IConfiguration config)
        {
            configuration = config;
            serviceClient = goblinService;
        }

        private TranslateContext LoadItems(string lang, List<string> ids = null)
        {
            if (configuration.GetImportFile(Server).Element("Lang").Value != lang)
                return null;
            var context = configuration.GetImportFile(Server).Element("Context");
            string contextValue = context.Attribute("Value").Value;
            var items = (from item in context.Elements("Text")
                         let id = item.Attribute("Id").Value
                         where ids == null || ids.Contains(id)
                         select new RowItem { Id = id, TemplateText = item.Value, TemplateLang = lang }).ToList();
            ids = items.Select(item => item.Id).ToList();
            ContextKey key = new ContextKey { Context = contextValue, Ids = ids };
            var searchParams = new TranslateSearchParameters { Context = key, Language = lang };
            var answer = serviceClient.SearchTranslate(Login, Password, configuration.GetInstance(), searchParams);
            if (answer.ErrorCode != GeneralErrorCode.Ok)
                throw new ServiceAccessException(answer.ErrorCode, answer.ErrorDescription);
            var result = new TranslateContext(items) { Value = contextValue };
            if (answer.Result.Content != null)
            {
                foreach (var translatedItem in answer.Result.Content)
                {
                    var rowItem = result[translatedItem.IdInContext];
                    if (rowItem != null)
                    {
                        rowItem.Translate = translatedItem.Translate;
                    }
                }
            }
            return result;
        }

        public ActionResult Index(string language)
        {
            TranslateContext translateData = null;
            var langs = configuration.GetLanguagesList();
            if (language != null)
            {
                var lang = langs.Single(item => item.Value == language);
                if (lang != null) lang.Selected = true;
                try
                {
                    translateData = LoadItems(language);
                }
                catch (ServiceAccessException e)
                {
                    ViewBag.Message = e.ToString();
                    return View();
                }

            }
            return View(new TableWithHeader { ServersList = configuration.GetServersList(), LanguageList = langs, TableData = translateData });
        }


        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByHeader = "X-Requested-With", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Confirm(TranslateContext data)
        {
            if (data.Count == 0)
            {
                ViewBag.Message = "Nothing changed";
                return View();
            }
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
            if (data.Count == 0)
            {
                ViewBag.Message = "Nothing changed";
            }
            else
            {

                string lang = data[0].TemplateLang;
                string contextValue = configuration.GetImportFile(Server).Element("Context").Attribute("Value").Value;
                var keyedTexts = (from item in data
                                  where item.IsChanged
                                  let key = new ContextKey { Context = contextValue, Ids = new List<string> { item.Id } }
                                  select new KeyedText { Key = key, Text = item.NewTranslate }).ToList();
                var package = new KeyedTextPackage { KeyedTexts = keyedTexts };
                var answer = serviceClient.UpdateOrCreateTranslateListByKey(Login, Password, configuration.GetInstance(), package, lang);
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
            }
            return View();
        }
    }
}