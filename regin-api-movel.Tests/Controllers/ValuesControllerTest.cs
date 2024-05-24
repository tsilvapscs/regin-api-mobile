using Microsoft.VisualStudio.TestTools.UnitTesting;
using regin_api_movel.Controllers;
using regin_app_movel.GeracaoXml;
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
            ConsultaProcessoResponse result = controller.GetProcessosPorProtocolo("");

            // Declarar
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Processos.Count());
            Assert.AreEqual(1, result.CodigoMensagem);
        }

    }
}
