using Microsoft.VisualStudio.TestTools.UnitTesting;
using regin_api_movel.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace regin_api_movel.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Organizar
            ProcessosController controller = new ProcessosController();

            // Agir
            IEnumerable<string> result = controller.Get();

            // Declarar
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("value1", result.ElementAt(0));
            Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Organizar
            ProcessosController controller = new ProcessosController();

            // Agir
            string result = controller.Get(5);

            // Declarar
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Organizar
            ProcessosController controller = new ProcessosController();

            // Agir
            controller.Post("value");

            // Declarar
        }

        [TestMethod]
        public void Put()
        {
            // Organizar
            ProcessosController controller = new ProcessosController();

            // Agir
            controller.Put(5, "value");

            // Declarar
        }

        [TestMethod]
        public void Delete()
        {
            // Organizar
            ProcessosController controller = new ProcessosController();

            // Agir
            controller.Delete(5);

            // Declarar
        }
    }
}
