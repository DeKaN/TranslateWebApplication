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
            var fullPath = new FileInfo(Configpath).FullName;
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = fullPath };
            configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            controller = new HomeController(serviceMock.Object, configuration.GetSection("translater") as TranslaterSection);
        }

        [TestMethod]
        public void Index()
        {
            
            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = (TableWithHeader)result.Model;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.ServersList);
            Assert.AreNotEqual(model.ServersList.Count(), 0);
            Assert.IsNotNull(model.LanguageList);
            Assert.AreNotEqual(model.LanguageList.Count(), 0);
            Assert.IsNull(model.TableData);
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
