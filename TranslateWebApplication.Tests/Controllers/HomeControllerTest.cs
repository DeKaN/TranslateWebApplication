namespace TranslateWebApplication.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using TranslateWebApplication.Controllers;
    using TranslateWebApplication.GoblinServiceReference;
    using TranslateWebApplication.Models;

    using TestConfiguration = TranslateWebApplication.Tests.TestConfiguration;

    [TestClass]
    public class HomeControllerTest
    {
        private IConfiguration configuration;

        private Mock<IGoblinService> serviceMock;

        private HomeController controller;

        [TestInitialize]
        public void Init()
        {
            serviceMock = new Mock<IGoblinService>();
            configuration = new TestConfiguration();
            controller = new HomeController(serviceMock.Object, configuration);
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

            serviceMock.Setup(m => m.SearchTranslate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TranslateSearchParameters>()))
                .Returns((string l, string p, string i, TranslateSearchParameters t) => new TranslatedItemListAnswer
                {
                    ErrorCode = GeneralErrorCode.Ok,
                    Result =
                        new DataListOfTranslatedItemwFXowe_P_S
                        {
                            Content = this.GenerateItems(t)
                        }
                });

            ViewResult result = controller.Index(Lang) as ViewResult;

            TestInit(result);
            var model = (TableWithHeader)result.Model;
            Assert.IsNotNull(model.TableData);
        }

        private List<TranslatedItem> GenerateItems(TranslateSearchParameters searchParameters)
        {
            return searchParameters.Context.Ids.Select(t => new TranslatedItem
                                                            {
                                                                Context = searchParameters.Context.Context,
                                                                Language = searchParameters.Language,
                                                                Translate = "Translate for " + t,
                                                                IdInContext = t
                                                            }).ToList();
        }

        [TestMethod]
        public void Confirm()
        {
            const int ChangedCount = 2;
            const string Lang = "en-us";
            var context = configuration.GetImportFile(null).Element("Context");
            var items = (from item in context.Elements("Text")
                         select new RowItem { Id = item.Attribute("Id").Value, NewTranslate = "New translate", TemplateLang = Lang }).ToList();

            var translateContext = new TranslateContext(items) { Value = context.Attribute("Value").Value };

            serviceMock.Setup(m => m.SearchTranslate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TranslateSearchParameters>()))
                .Returns((string l, string p, string i, TranslateSearchParameters t) => new TranslatedItemListAnswer
                {
                    ErrorCode = GeneralErrorCode.Ok,
                    Result =
                        new DataListOfTranslatedItemwFXowe_P_S
                        {
                            Content = this.GenerateItems(t)
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
