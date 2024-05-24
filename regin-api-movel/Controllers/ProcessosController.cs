using DocumentFormat.OpenXml.Drawing.Diagrams;
using regin_api_movel.Security;
using regin_app_movel.Constante;
using regin_app_movel.GeracaoXml;
using regin_app_movel.Negocio;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace regin_api_movel.Controllers
{
    public class ProcessosController : ApiController
    {
        [InternalBasicAuthenticationAttribute]
        [ApiExplorerSettings(IgnoreApi = true)]
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
                Console.WriteLine(ex.ToString());
                ConsultaProcessoResponse pscs = new ConsultaProcessoResponse();
                pscs.CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                pscs.Mensagem = $"Erro ao consultar o protocolo {protocolo}";
                pscs.DetalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                return pscs;
            }
            //{
            //    codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO,
            //    mensagem = $"Tipo do protocolo encontrado {TipoProtocolo.Tipos.LEGALIZACAO}",
            //    detalheMensagem = $"Protocolo recebido foi {protocolo}"
            //};
        }

        //[InternalBasicAuthenticationAttribute]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("api/processos/pessoas/{cpf}")]
        public ConsultaResumoProcessoResponse GetResumoProcessosPorCpf(string cpf, string protocolo = "", string data = "")
        {
            try
            {
                using (ProtocoloNegocio protocoloNegocio = new ProtocoloNegocio())
                {
                    return new ConsultaResumoProcessoResponse(protocoloNegocio.ConsultarResumoProcessosPorCpf(cpf, protocolo, data));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ConsultaResumoProcessoResponse erro = new ConsultaResumoProcessoResponse();
                erro.CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                erro.Mensagem = $"Erro ao consultar o CPF {cpf}";
                erro.DetalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                return erro;
            }
        }
        /// <summary>
        /// Retorna o processo caso seja vinculado ao cpf informado. Só será retornado processo que possui requerimento eletrônico vinculado.
        /// </summary>
        /// <param name="cpf">CPF do participante vinculado ao processo.</param>
        /// <param name="processo_or">Protocolo do processo no órgão de registro ou número do requerimento eletrônico.</param>
        /// <param name="dataAlteracao">Data/Hora da situação atual do processo.</param>
        /// <returns>O item com o parametros especificados.</returns>
        [BasicAuthenticationAttribute]
        [HttpGet]
        [Route("api/obterprocessos/{cpf}")]
        [SwaggerOperation("obterprocessos")]
        [SwaggerResponse(HttpStatusCode.OK, "Item encontrado.", typeof(ConsultaStatusProcessoResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Item não encontrado")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro", typeof(ErroResponse))]
        [ResponseType(typeof(ConsultaStatusProcessoResponse))]
        public HttpResponseMessage GetProcessosByCPF(string cpf, string processo_or, string dataAlteracao = "")
        {
            try
            {
                using (ProtocoloNegocio protocoloNegocio = new ProtocoloNegocio())
                {
                    var retorno = protocoloNegocio.ConsultarProcessosPorCpf(cpf, processo_or, dataAlteracao);
                    if(retorno.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new ConsultaStatusProcessoResponse(retorno));
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ErroResponse erro = new ErroResponse();
                erro.CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                erro.Mensagem = $"Erro ao consultar o CPF {cpf}";
                erro.DetalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                return Request.CreateResponse(HttpStatusCode.BadRequest, erro);
            }
        }
        
        [InternalBasicAuthenticationAttribute]
        [ApiExplorerSettings(IgnoreApi = true)]
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
                Console.WriteLine(ex.ToString());
                ConsultaTipoProtocoloResponse pscs = new ConsultaTipoProtocoloResponse();
                pscs.CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.ERRO;
                pscs.Mensagem = $"Erro ao consultar o tipo do processo do protocolo {protocolo}";
                pscs.DetalheMensagem = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
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