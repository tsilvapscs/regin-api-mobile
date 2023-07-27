using regin_app_mobile.Constante;
using regin_app_mobile.GeracaoXml;
using regin_app_mobile.Negocio;
using System;
using System.Web.Http;

namespace regin_api_movel.Controllers
{
    public class ProcessosController : ApiController
    {
        [HttpGet]
        [Route("api/processos/{protocolo}")]
        public ConsultaProcessoResponse GetProcessosPorProtocolo(string protocolo)
        {
            try
            {
                using (ProtocoloNegocio protocoloNegocio = new ProtocoloNegocio())
                {
                    return protocoloNegocio.PesquisaDadosProtocolo(protocolo);
                }
            }
            catch (Exception ex)
            {
                ConsultaProcessoResponse pscs = new ConsultaProcessoResponse();
                pscs.codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                pscs.mensagem = $"Erro ao consultar o protocolo {protocolo}";
                pscs.detalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                return pscs;
            }
            //{
            //    codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO,
            //    mensagem = $"Tipo do protocolo encontrado {TipoProtocolo.Tipos.LEGALIZACAO}",
            //    detalheMensagem = $"Protocolo recebido foi {protocolo}"
            //};
        }

        [HttpGet]
        // GET api/values
        [Route("api/processos/{protocolo}/tipo")]
        public ConsultaTipoProtocoloResponse GetTipoByProtocolo(string protocolo)
        {
            try
            {
                using (ProtocoloNegocio protocoloNegocio = new ProtocoloNegocio())
                {
                    return protocoloNegocio.PesquisaTipoProtocolo(protocolo);
                }
            }
            catch (Exception ex)
            {
                ConsultaTipoProtocoloResponse pscs = new ConsultaTipoProtocoloResponse();
                pscs.codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                pscs.mensagem = $"Erro ao consultar o tipo do processo do protocolo {protocolo}";
                pscs.detalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                return pscs;
            }
            //return new ConsultaTipoProtocoloResponse()
            //{
            //    tipoProtocolo = TipoProtocolo.Tipos.LEGALIZACAO,
            //    nomeTipoProtocolo = TipoProtocolo.GetNome(TipoProtocolo.Tipos.LEGALIZACAO),
            //    codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO,
            //    mensagem = $"Consulta feita com sucesso referente ao protocolo {protocolo}"
            //};
        }


    }

}
