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
