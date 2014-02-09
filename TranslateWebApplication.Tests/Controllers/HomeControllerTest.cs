using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranslateWebApplication;
using TranslateWebApplication.Controllers;

namespace TranslateWebApplication.Tests.Controllers
{
    using System.Configuration;
    using System.IO;
    using System.Web;
    using System.Web.Routing;

    using Moq;

    using TranslateWebApplication.GoblinServiceReference;
    using TranslateWebApplication.Models;

    [TestClass]
    public class HomeControllerTest
    {
        private const string Configpath = "../../Web.config";

        private Configuration configuration;

        private Mock<IGoblinService> serviceMock;

        private HomeController controller;

        [TestInitialize]
        public void Init()
        {
            serviceMock = new Mock<IGoblinService>();
            var fileInfo = new FileInfo(Configpath);
            string dirPath = fileInfo.DirectoryName;
            string configFullPath = fileInfo.FullName;
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFullPath };
            configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            controller = new HomeController(serviceMock.Object, configuration.GetSection("translater") as TranslaterSection);
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(m => m.MapPath("~/import.xml")).Returns(Path.Combine(dirPath, "import.xml"));
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(p => p.Server).Returns(serverMock.Object);
            controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);
        }

        private void TestInit(ViewResult result)
        {
            Assert.IsNotNull(result);
            var model = (TableWithHeader)result.Model;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.ServersList);
            Assert.AreNotEqual(model.ServersList.Count(), 0);
            Assert.IsNotNull(model.LanguageList);
            Assert.AreNotEqual(model.LanguageList.Count(), 0);
        }

        [TestMethod]
        public void Index()
        {
            string lang = null;

            ViewResult result = controller.Index(lang) as ViewResult;

            TestInit(result);
            var model = (TableWithHeader)result.Model;
            Assert.IsNull(model.TableData);
        }

        [TestMethod]
        public void IndexWithLang()
        {
            const string Lang = "en-us";
            const int TranslatesCount = 4;

            serviceMock.Setup(m => m.SearchTranslate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TranslateSearchParameters>()))
                .Returns((string l, string p, string i, TranslateSearchParameters t) => new TranslatedItemListAnswer
                {
                    ErrorCode = GeneralErrorCode.Ok,
                    Result =
                        new DataListOfTranslatedItemwFXowe_P_S
                        {
                            Content = this.GenerateItems(t, TranslatesCount)
                        }
                });
            
            ViewResult result = controller.Index(Lang) as ViewResult;

            TestInit(result);
            var model = (TableWithHeader)result.Model;
            Assert.IsNotNull(model.TableData);
            Assert.AreEqual(model.TableData.Count(), TranslatesCount);
        }

        private List<TranslatedItem> GenerateItems(TranslateSearchParameters searchParameters, int count)
        {
            var items = new List<TranslatedItem>();
            string lang = searchParameters.Language;
            var ids = searchParameters.Context.Ids;
            if (count < ids.Count)
                count = ids.Count;
            for (int i = 0; i < count; i++)
            {
                items.Add(new TranslatedItem
                {
                    Context = searchParameters.Context.Context,
                    Language = lang,
                    Translate = "Translate for " + ids[i],
                    IdInContext = ids[i]
                });
            }
            return items;
        }

        [TestMethod]
        public void Confirm()
        {
            const int ChangedCount = 2;
            List<RowItem> items = new List<RowItem>();
            items.Add(new RowItem
            {
                Id = "AccountIsBanned",
                Translate = "Translate",
                NewTranslate = "New translate",
                TemplateLang = "en-us",
            });
            items.Add(new RowItem
            {
                Id = "AccountWithId",
                Translate = "Translate",
                NewTranslate = "New translate",
                TemplateLang = "en-us",
            });
            var translateContext = new TranslateContext(items) { Value = "context:value" };

            serviceMock.Setup(m => m.SearchTranslate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TranslateSearchParameters>()))
                .Returns((string l, string p, string i, TranslateSearchParameters t) => new TranslatedItemListAnswer
                {
                    ErrorCode = GeneralErrorCode.Ok,
                    Result =
                        new DataListOfTranslatedItemwFXowe_P_S
                        {
                            Content = this.GenerateItems(t, ChangedCount)
                        }
                });

            ViewResult result = controller.Confirm(translateContext) as ViewResult;

            Assert.IsNotNull(result);
            var model = (TranslateContext)result.Model;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.IsReadOnly, true);
            Assert.AreEqual(model.Count(), ChangedCount);
        }

        [TestMethod]
        public void Save()
        {
            const int Added = 1, Edited = 0;
            string message = string.Format("Status: {0}.\nAdded: {1}. Edited: {2}.", GeneralErrorCode.Ok, Added, Edited);
            var translateContext = new TranslateContext(new List<RowItem> { new RowItem() });

            serviceMock.Setup(m => m.UpdateOrCreateTranslateListByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyedTextPackage>(), It.IsAny<string>()))
                .Returns(new ChangeSourceInfoAnswer { ErrorCode = GeneralErrorCode.Ok, Result = new ChangeSourceInfo { AddedCount = Added, EditedCount = Edited } });

            ViewResult result = controller.Save(translateContext) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewBag.Message, message);
        }

        [TestMethod]
        public void SaveNothing()
        {
            const string Message = "Nothing changed";

            ViewResult result = controller.Save(new TranslateContext()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewBag.Message, Message);
        }

        /*[TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }*/
    }
}
