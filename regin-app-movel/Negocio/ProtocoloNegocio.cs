using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using regin_app_movel.Constante;
using regin_app_movel.Dao;
using regin_app_movel.Database;
using regin_app_movel.GeracaoXml;

namespace regin_app_movel.Negocio
{
    public class ProtocoloNegocio : IDisposable
    {
        private bool disposed = false;

        private ConfiguracaoSistema.Sgbds tipoSgbdRegin;
        private ConfiguracaoSistema.Sgbds tipoSgbdRequerimento;

        private ConexaoBancoDados conexaoBancoDadosRegin = null;
        private ConexaoBancoDados conexaoBancoDadosRequerimento = null;

        private IReginDao reginDao = null;
        private IRequerimentoDao requerimentoDao = null;

        public ProtocoloNegocio()
        {
            Inicializa();
        }

        private void Inicializa()
        {
            tipoSgbdRegin = ConfiguracaoSistema.GetSgbdPorParametro(ConfiguracaoSistema.Parametros.DB_REGIN_SGBD);
            tipoSgbdRequerimento = ConfiguracaoSistema.GetSgbdPorParametro(ConfiguracaoSistema.Parametros.DB_REQUERIMENTO_SGBD);

            conexaoBancoDadosRegin = ConexaoBancoDados.MontaConexao(tipoSgbdRegin);
            conexaoBancoDadosRequerimento = ConexaoBancoDados.MontaConexao(tipoSgbdRequerimento);

            reginDao = FabricaReginDao.GetFabricaDao(tipoSgbdRegin);
            requerimentoDao = FabricarequerimentoDao.GetFabricaDao(tipoSgbdRequerimento);

            AbreConexoesBancoDados();
        }

        public ConsultaTipoProtocoloResponse PesquisaTipoProtocolo(String protocolo)
        {
            TipoProtocolo.Tipos tipo = requerimentoDao.ConsultaTipoProtocolo(protocolo, conexaoBancoDadosRequerimento.CriaComando(), true);
            if (TipoProtocolo.Tipos.NAO_ENCONTRADO.Equals(tipo))
            {
                tipo = reginDao.PesquisaTipoProtocolo(protocolo, conexaoBancoDadosRegin.CriaComando(), true);
            }

            if (tipo == TipoProtocolo.Tipos.NAO_ENCONTRADO)
            {
                tipo = reginDao.PesquisaTipoProtocoloInterno(protocolo, conexaoBancoDadosRegin.CriaComando(), true);
            }

            int codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO;
            if (tipo == TipoProtocolo.Tipos.NAO_ENCONTRADO)
            {
                codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.NAO_ENCONTRADO;
            }

            return new ConsultaTipoProtocoloResponse()
            {
                TipoProtocolo = tipo,
                NomeTipoProtocolo = TipoProtocolo.GetNome(tipo),
                CodigoMensagem = codigoMensagem
            };
        }

        public ConsultaProcessoResponse PesquisaProtocolo(string protocolo)
        {
            ConsultaProcessoResponse pscs = null;
            Requerimento requerimento = null;
            TipoProtocolo.Tipos tipoProtocolo = TipoProtocolo.Tipos.NAO_ENCONTRADO;
            ConsultaTipoProtocoloResponse consultaTipoProtocoloResponse = PesquisaTipoProtocolo(protocolo);
            if (consultaTipoProtocoloResponse.CodigoMensagem == ((int)ConstantesServicoWeb.CodigosRetorno.SUCESSO))
            {
                tipoProtocolo = consultaTipoProtocoloResponse.TipoProtocolo;
            }

            string siglaUf = ConfigurationManager.AppSettings[ConfiguracaoSistema.GetParametroChave(ConfiguracaoSistema.Parametros.SIGLA_UF)];

            List<Processo> processos;
            if (TipoProtocolo.Tipos.REQUERIMENTO.Equals(tipoProtocolo))
            {
                requerimento = requerimentoDao.ConsultaProtocolo(protocolo, conexaoBancoDadosRequerimento.CriaComando(), true);
                Processo processo;
                if (requerimento != null && requerimento.NumeroProtocolo != null && requerimento.NumeroProtocoloOrgaoEstadual != null && requerimento.NumeroProtocoloOrgaoEstadual.Trim().Length > 0)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.NumeroProtocoloOrgaoEstadual, conexaoBancoDadosRegin.CriaComando(), true);
                    if (processos != null && processos.Count > 0)
                    {
                        for (int p = 0; p < processos.Count; p++)
                        {
                            requerimento.CargaOrgaoRegistro = "SIM";
                            processos[p].Requerimento = requerimento;
                            processos[p].FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                            if (processos[p].ProcessosRelacionados == null)
                            {
                                processos[p].ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processos[p].ProcessosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.Uf, requerimento.NumeroProtocolo));
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].ProcessoExigencia.Equals("1") && processos[p].ProcessoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }

                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].Uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].InstituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].InstituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].InstituicoesAnalise.Count; i++)
                            {
                                processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                    }
                    else
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.CargaOrgaoRegistro = "NÃO";
                        processo.Requerimento = requerimento;
                        processo.FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.NumeroProtocolo = requerimento.NumeroProtocolo;
                        processo.Pessoa = new Pessoa
                        {
                            CpfCnpj = requerimento.Cnpj
                        };
                        if (requerimento.RazaoSocial != null && requerimento.RazaoSocial.Trim().Length > 0)
                        {
                            processo.Pessoa.Nome = requerimento.RazaoSocial;
                        }
                        else
                        {
                            processo.Pessoa.Nome = requerimento.Solicitante;
                        }

                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                        processos.Add(processo);
                    }

                    pscs = MontaRetornoProtocoloSucesso(processos);
                }
                else if (requerimento != null && requerimento.NumeroProtocolo != null && requerimento.NumeroProtocoloViabilidade != null && requerimento.NumeroProtocoloViabilidade.Trim().Length > 0)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.NumeroProtocoloViabilidade, conexaoBancoDadosRegin.CriaComando(), true);
                    if (processos != null && processos.Count > 0)
                    {
                        for (int p = 0; p < processos.Count; p++)
                        {
                            requerimento.CargaOrgaoRegistro = "SIM";
                            processos[p].Requerimento = requerimento;
                            processos[p].FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                            if (processos[p].ProcessosRelacionados == null)
                            {
                                processos[p].ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processos[p].ProcessosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.Uf, requerimento.NumeroProtocolo));
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].ProcessoExigencia.Equals("1") && processos[p].ProcessoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }

                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].Uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].InstituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].InstituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].InstituicoesAnalise.Count; i++)
                            {
                                processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                    }
                    else
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.CargaOrgaoRegistro = "NÃO";
                        processo.Requerimento = requerimento;
                        processo.FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.NumeroProtocolo = requerimento.NumeroProtocolo;
                        processo.Pessoa = new Pessoa
                        {
                            CpfCnpj = requerimento.Cnpj
                        };
                        if (requerimento.RazaoSocial != null && requerimento.RazaoSocial.Trim().Length > 0)
                        {
                            processo.Pessoa.Nome = requerimento.RazaoSocial;
                        }
                        else
                        {
                            processo.Pessoa.Nome = requerimento.Solicitante;
                        }

                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                        processos.Add(processo);
                    }

                    pscs = MontaRetornoProtocoloSucesso(processos);
                }
                else if (requerimento != null && requerimento.NumeroProtocolo != null)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.NumeroProtocolo, conexaoBancoDadosRegin.CriaComando(), true);

                    if (processos == null)
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.CargaOrgaoRegistro = "NÃO";
                        processo.FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.Requerimento = requerimento;
                        processo.Pessoa = new Pessoa
                        {
                            CpfCnpj = requerimento.Cnpj
                        };
                        if (requerimento.RazaoSocial != null && requerimento.RazaoSocial.Trim().Length > 0)
                        {
                            processo.Pessoa.Nome = requerimento.RazaoSocial;
                        }
                        else
                        {
                            processo.Pessoa.Nome = requerimento.Solicitante;
                        }

                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                        processos.Add(processo);
                        pscs = MontaRetornoProtocoloSucesso(processos);
                    }
                    else
                    {
                        if (processos != null && processos.Count > 0)
                        {
                            for (int p = 0; p < processos.Count; p++)
                            {
                                requerimento.CargaOrgaoRegistro = "SIM";

                                processos[p].Requerimento = requerimento;
                                processos[p].FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                                if (processos[p].ProcessosRelacionados == null)
                                {
                                    processos[p].ProcessosRelacionados = new List<ProcessoRelacionado>();
                                }

                                processos[p].ProcessosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.Uf, requerimento.NumeroProtocolo));
                                processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                if (processos[p].ProcessoExigencia.Equals("1") && processos[p].ProcessoSequenciaExigencia != null)
                                {
                                    processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                }

                                // processos[p] = reginDao.pesquisaOrgaoRegistroAnalise(processos[p].uf, processos[p], conexaoBancoDadosRegin.criaComando(), true);
                                // processos[p].instituicoesAnalise[0] = reginDao.pesquisaOrgaoRegistroAndamento(protocolo, processos[p].instituicoesAnalise[0], conexaoBancoDadosRegin.criaComando(), true);
                                processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                for (int i = 0; i < processos[p].InstituicoesAnalise.Count; i++)
                                {
                                    processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                }

                                if (processos[p].Eventos != null && processos[p].Eventos.Count > 0)
                                {
                                    processos[p].TipoOperacao = processos[p].Eventos[0].Descricao;
                                    processos[p].Ato = processos[p].Eventos[0].Codigo + " - " + processos[p].Eventos[0].Descricao;
                                }

                                // Ajustes de dados nao encontrados
                                processos[p].Tipo = TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO);


                                processos[p].Pessoa.NaturezaJuridicaCodigo = processos[p].Requerimento.CodigoNaturezaJuridica;
                                processos[p].Pessoa.NaturezaJuridicaNome = processos[p].Requerimento.DescricaoNaturezaJuridica;
                                processos[p] = ajustarNumeroprotocolo87(tipoProtocolo, processos[p]);
                            }

                            pscs = MontaRetornoProtocoloSucesso(processos);
                        }
                    }
                }
                else
                {
                    pscs = MontaRetornoProtocoloNaoEncontrado(protocolo);
                }
            }
            else
            {
                if (TipoProtocolo.Tipos.NAO_ENCONTRADO == tipoProtocolo)
                {
                    processos = reginDao.PesquisaProtocoloOrgaoRegistro(siglaUf, protocolo, conexaoBancoDadosRegin.CriaComando(), true);
                }
                else
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, protocolo, conexaoBancoDadosRegin.CriaComando(), true);
                }

                if (processos != null && processos.Count > 0)
                {
                    for (int p = 0; p < processos.Count; p++)
                    {
                        if (processos[p].Requerimento != null && processos[p].Requerimento.NumeroProtocolo != null && processos[p].Requerimento.NumeroProtocolo.Trim().Length > 0)
                        {
                            requerimento = requerimentoDao.ConsultaProtocolo(processos[p].Requerimento.NumeroProtocolo, conexaoBancoDadosRequerimento.CriaComando(), true);
                            if (requerimento != null)
                            {
                                processos[p].Requerimento = requerimento;
                                processos[p].Requerimento.CargaOrgaoRegistro = "SIM";
                            }
                            else
                            {
                                processos[p].Requerimento.Situacao = "PROTOCOLO NÃO ENCONTRADO";
                            }
                        }

                        if (TipoProtocolo.Tipos.NAO_ENCONTRADO == tipoProtocolo)
                        {
                            processos[p] = reginDao.PesquisaEventosOrgaoRegistro(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        }
                        else
                        {
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].FonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.ORGAO_REGISTRO);
                        }


                        if (TipoProtocolo.Tipos.VIABILIDADE.Equals(tipoProtocolo))
                        {
                            processos[p].TipoOperacao = "CONSULTA PRÉVIA";
                            processos[p].Ato = "CONSULTA PRÉVIA";
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].InstituicoesAnalise.Count; i++)
                            {
                                processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                        else
                        {
                            if (processos[p].ProcessoExigencia != null && processos[p].ProcessoExigencia.Equals("1") && processos[p].ProcessoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }

                            if (processos[p].CodigoStatus != null && processos[p].CodigoStatus.Equals(1))
                            {
                                processos[p].Status = "Finalizado no Orgão de Registro - Enviando para REDESIM";
                            }

                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].Uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].InstituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].InstituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].InstituicoesAnalise == null || processos[p].InstituicoesAnalise.Count == 1 && processos[p].ProtocoloInternoRegin != null)
                            {
                                processos[p] = reginDao.PesquisaInstituicoesAnalise(processos[p].ProtocoloInternoRegin, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }

                            for (int i = 1; i < processos[p].InstituicoesAnalise.Count; i++)
                            {
                                processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                if (processos[p].InstituicoesAnalise[i].Andamentos == null || processos[p].InstituicoesAnalise[i].Andamentos.Count == 0 && processos[p].ProtocoloInternoRegin != null)
                                {
                                    processos[p].InstituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(processos[p].ProtocoloInternoRegin, processos[p].InstituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                }
                            }
                        }

                        // Ajusta dados da empresa de acordo com os do requerimento.
                        if (processos[p].Pessoa != null)
                        {
                            if (requerimento != null)
                            {
                                if (processos[p].Pessoa.Nome == null || processos[p].Pessoa.Nome.Trim().Length <= 0)
                                {
                                    processos[p].Pessoa.Nome = requerimento.RazaoSocial;
                                }

                                if (processos[p].Pessoa.CpfCnpj == null || processos[p].Pessoa.CpfCnpj.Trim().Length <= 0)
                                {
                                    processos[p].Pessoa.CpfCnpj = requerimento.Cnpj;
                                }

                                if (processos[p].Pessoa.NaturezaJuridicaCodigo == null || processos[p].Pessoa.NaturezaJuridicaCodigo.Trim().Length <= 0)
                                {
                                    processos[p].Pessoa.NaturezaJuridicaCodigo = requerimento.CodigoNaturezaJuridica;
                                }

                                if (processos[p].Pessoa.NaturezaJuridicaNome == null || processos[p].Pessoa.NaturezaJuridicaNome.Trim().Length <= 0)
                                {
                                    processos[p].Pessoa.NaturezaJuridicaNome = requerimento.DescricaoNaturezaJuridica;
                                }

                                if (processos[p].Pessoa.Uf == null || processos[p].Pessoa.Uf.Trim().Length <= 0)
                                {
                                    processos[p].Pessoa.Uf = requerimento.Uf;
                                }
                            }
                        }
                    }

                    foreach (Processo processo in processos)
                    {
                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                    }

                    pscs = MontaRetornoProtocoloSucesso(processos);
                }
                else
                {
                    pscs = MontaRetornoProtocoloNaoEncontrado(protocolo);
                }
            }

            return pscs;
        }

        public List<ResumoProcesso> ConsultarResumoProcessosPorCpf(string cpf, string protocolo, string data)
        {
            string urlApiStatusProcess = ConfigurationManager.AppSettings["WsControleRequerimento"].ToString();
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Iniciando consulta de processos.");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Consultas serão filtradas por:");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] CPF => {cpf}");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Protocolo => {protocolo}");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Data => {data}");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Serviço consulta status => {urlApiStatusProcess}");
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();

            WsControleRequerimento.WsControleRequerimento req = new WsControleRequerimento.WsControleRequerimento
            {
                Url = urlApiStatusProcess
            };

            // Requerimento

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosVinculoPrincipal");
            requerimentoDao.ConsultarProcessosVinculoPrincipal(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processos => processos.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosVinculoPrincipal");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosVinculoPrincipal. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosRepresentante");
            requerimentoDao.ConsultarProcessosRepresentante(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosRepresentante");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosRepresentante. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosRepresentanteDoRepresentante");
            requerimentoDao.ConsultarProcessosRepresentanteDoRepresentante(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosRepresentanteDoRepresentante");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosRepresentanteDoRepresentante. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosContador");
            requerimentoDao.ConsultarProcessosContador(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosContador");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosContador. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosAssinantes");
            requerimentoDao.ConsultarProcessosAssinantes(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosAssinantes");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosAssinantes. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (Requerimento) ConsultarProcessosRequerimentoServico");
            requerimentoDao.ConsultarProcessosRequerimentoServico(cpf, protocolo, data, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (Requerimento) ConsultarProcessosRequerimentoServico");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (Requerimento) ConsultarProcessosRequerimentoServico. Qtd. Registros: {resultado.Count}");

            // REGIN

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (REGIN) ConsultarViabilidadePorSolicitante");
            reginDao.ConsultarViabilidadePorSolicitante(cpf, protocolo, data, conexaoBancoDadosRegin.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (REGIN) ConsultarViabilidadePorSolicitante");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (REGIN) ConsultarViabilidadePorSolicitante. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (REGIN) ConsultarViabilidadePorSocios");
            reginDao.ConsultarViabilidadePorSocios(cpf, protocolo, data, conexaoBancoDadosRegin.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (REGIN) ConsultarViabilidadePorSocios");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (REGIN) ConsultarViabilidadePorSocios. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (REGIN) ConsultarProtocoloPorSocios");
            reginDao.ConsultarProtocoloPorSocios(cpf, protocolo, data, conexaoBancoDadosRegin.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (REGIN) ConsultarProtocoloPorSocios");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (REGIN) ConsultarProtocoloPorSocios. Qtd. Registros: {resultado.Count}");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Inicio - (REGIN) ConsultarProtocoloPorRepresentantes");
            reginDao.ConsultarProtocoloPorRepresentantes(cpf, protocolo, data, conexaoBancoDadosRegin.CriaComando(), true)
                .Where(processo => filtrarProcessos(processo))
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo => resultado.Add(processo));
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Termino - (REGIN) ConsultarProtocoloPorRepresentantes");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] (REGIN) ConsultarProtocoloPorRepresentantes. Qtd. Registros: {resultado.Count}");

            return resultado.Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .Select(processo =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Consultando detalhe do status do processo {processo.NumeroProtocolo}");
                    DataSet ds = req.GetStatusProcesso(processo.NumeroProtocolo);
                    DataTable dtProtocolo = ds.Tables[0];
                    if (dtProtocolo.Rows.Count > 0)
                    {
                        processo.StatusDetalhe = dtProtocolo.Rows[0]["Descricao"].ToString();
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Para o processo {processo.NumeroProtocolo} foi encontrado o detalhe {processo.StatusDetalhe}");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(processo.NumeroProtocoloOrgaoRegistro))
                        {
                            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Consultando detalhe do status do processo {processo.NumeroProtocoloOrgaoRegistro}");
                            DataSet dsOr = req.GetStatusProcesso(processo.NumeroProtocoloOrgaoRegistro);
                            DataTable dtProtocoloOR = dsOr.Tables[0];
                            if (dtProtocoloOR.Rows.Count > 0)
                            {
                                processo.StatusDetalhe = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Para o processo {processo.NumeroProtocoloOrgaoRegistro} foi encontrado o detalhe {processo.StatusDetalhe}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] Para o processo {processo.NumeroProtocoloOrgaoRegistro} não foi encontrado detalhe do status");
                            }
                        }
                    }
                    return processo;
                })
                .ToList();
        }
        public List<StatusProcesso> ConsultarProcessosPorCpf(string cpf, string processo_or, string dataSituacao)
        {

            List<StatusProcesso> resultado = new List<StatusProcesso>();
            if (ConfigurationManager.AppSettings["WsControleRequerimento"] != null)
            {
                WsControleRequerimento.WsControleRequerimento req = new WsControleRequerimento.WsControleRequerimento();
                req.Url = ConfigurationManager.AppSettings["WsControleRequerimento"].ToString();

                try
                {


                    // Requerimento
                    requerimentoDao.ConsultarProcessosVinculoPrincipal(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processos => processos.First())
                .ToList()
                .ForEach(processo =>
                {
                    StatusProcesso sp = new StatusProcesso();
                    sp.NumeroProtocolo = processo.NumeroProtocolo;
                    sp.RazaoSocial = processo.PessoaNome;
                    sp.Cnpj = processo.PessoaCnpj;
                    sp.DataCriacao = processo.DataCriacao;
                    sp.DataAlteracao = processo.DataAlteracao;
                    sp.Ato = processo.AtoEventoRfbDescricao;
                    //Verifica se tem Requerimento
                    DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                    DataTable dtProtocoloOR = ds.Tables[0];
                    if (dtProtocoloOR.Rows.Count > 0)
                    {
                        sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                    }
                    else
                    {
                        sp.StatusDescricao = processo.StatusDescricao;
                    }
                    resultado.Add(sp);
                });

                    requerimentoDao.ConsultarProcessosRepresentante(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo =>
                {
                    StatusProcesso sp = new StatusProcesso();
                    sp.NumeroProtocolo = processo.NumeroProtocolo;
                    sp.RazaoSocial = processo.PessoaNome;
                    sp.Cnpj = processo.PessoaCnpj;
                    sp.DataCriacao = processo.DataCriacao;
                    sp.DataAlteracao = processo.DataAlteracao;
                    sp.Ato = processo.AtoEventoRfbDescricao;
                    //Verifica se tem Requerimento
                    DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                    DataTable dtProtocoloOR = ds.Tables[0];
                    if (dtProtocoloOR.Rows.Count > 0)
                    {
                        sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                    }
                    else
                    {
                        sp.StatusDescricao = processo.StatusDescricao;
                    }
                    resultado.Add(sp);
                });

                    requerimentoDao.ConsultarProcessosRepresentanteDoRepresentante(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                        .Distinct()
                        .OrderByDescending(processo => processo.DataAlteracao)
                        .GroupBy(processo => processo.NumeroProtocolo)
                        .Select(processo => processo.First())
                        .ToList()
                        .ForEach(processo =>
                        {
                            StatusProcesso sp = new StatusProcesso();
                            sp.NumeroProtocolo = processo.NumeroProtocolo;
                            sp.RazaoSocial = processo.PessoaNome;
                            sp.Cnpj = processo.PessoaCnpj;
                            sp.DataCriacao = processo.DataCriacao;
                            sp.DataAlteracao = processo.DataAlteracao;
                            sp.Ato = processo.AtoEventoRfbDescricao;
                            //Verifica se tem Requerimento
                            DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                            DataTable dtProtocoloOR = ds.Tables[0];
                            if (dtProtocoloOR.Rows.Count > 0)
                            {
                                sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                            }
                            else
                            {
                                sp.StatusDescricao = processo.StatusDescricao;
                            }
                            resultado.Add(sp);
                        });

                    requerimentoDao.ConsultarProcessosContador(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo =>
                {
                    StatusProcesso sp = new StatusProcesso();
                    sp.NumeroProtocolo = processo.NumeroProtocolo;
                    sp.RazaoSocial = processo.PessoaNome;
                    sp.Cnpj = processo.PessoaCnpj;
                    sp.DataCriacao = processo.DataCriacao;
                    sp.DataAlteracao = processo.DataAlteracao;
                    sp.Ato = processo.AtoEventoRfbDescricao;
                    //Verifica se tem Requerimento
                    DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                    DataTable dtProtocoloOR = ds.Tables[0];
                    if (dtProtocoloOR.Rows.Count > 0)
                    {
                        sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                    }
                    else
                    {
                        sp.StatusDescricao = processo.StatusDescricao;
                    }
                    resultado.Add(sp);
                });

                    requerimentoDao.ConsultarProcessosAssinantes(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo =>
                {
                    StatusProcesso sp = new StatusProcesso();
                    sp.NumeroProtocolo = processo.NumeroProtocolo;
                    sp.RazaoSocial = processo.PessoaNome;
                    sp.Cnpj = processo.PessoaCnpj;
                    sp.DataCriacao = processo.DataCriacao;
                    sp.DataAlteracao = processo.DataAlteracao;
                    sp.Ato = processo.AtoEventoRfbDescricao;
                    //Verifica se tem Requerimento
                    DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                    DataTable dtProtocoloOR = ds.Tables[0];
                    if (dtProtocoloOR.Rows.Count > 0)
                    {
                        sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                    }
                    else
                    {
                        sp.StatusDescricao = processo.StatusDescricao;
                    }
                    resultado.Add(sp);
                });

                    requerimentoDao.ConsultarProcessosRequerimentoServico(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
                .ForEach(processo =>
                {
                    StatusProcesso sp = new StatusProcesso();
                    sp.NumeroProtocolo = processo.NumeroProtocolo;
                    sp.RazaoSocial = processo.PessoaNome;
                    sp.Cnpj = processo.PessoaCnpj;
                    sp.DataCriacao = processo.DataCriacao;
                    sp.DataAlteracao = processo.DataAlteracao;
                    sp.Ato = processo.AtoEventoRfbDescricao;
                    //Verifica se tem Requerimento
                    DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                    DataTable dtProtocoloOR = ds.Tables[0];
                    if (dtProtocoloOR.Rows.Count > 0)
                    {
                        sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                    }
                    else
                    {
                        sp.StatusDescricao = processo.StatusDescricao;
                    }
                    resultado.Add(sp);
                });

                    requerimentoDao.ConsultarProcessosRequerimentoServicoPF(cpf, processo_or, dataSituacao, conexaoBancoDadosRequerimento.CriaComando(), true)
                .Distinct()
                .OrderByDescending(processo => processo.DataAlteracao)
                .GroupBy(processo => processo.NumeroProtocolo)
                .Select(processo => processo.First())
                .ToList()
               .ForEach(processo =>
               {
                   StatusProcesso sp = new StatusProcesso();
                   sp.NumeroProtocolo = processo.NumeroProtocolo;
                   sp.RazaoSocial = processo.PessoaNome;
                   sp.Cnpj = processo.PessoaCnpj;
                   sp.DataCriacao = processo.DataCriacao;
                   sp.DataAlteracao = processo.DataAlteracao;
                   sp.Ato = processo.AtoEventoRfbDescricao;
                   //Verifica se tem Requerimento
                   DataSet ds = req.GetStatusProcesso(sp.NumeroProtocolo);
                   DataTable dtProtocoloOR = ds.Tables[0];
                   if (dtProtocoloOR.Rows.Count > 0)
                   {
                       sp.StatusDescricao = dtProtocoloOR.Rows[0]["Descricao"].ToString();
                   }
                   else
                   {
                       sp.StatusDescricao = processo.StatusDescricao;
                   }
                   resultado.Add(sp);
               });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return resultado;
        }
        private static Processo ajustarNumeroprotocolo87(TipoProtocolo.Tipos tipoProtocolo, Processo processo)
        {
            if (processo.JuntaComercialProtocolo != null && processo.JuntaComercialProtocolo.Trim().Length > 0 && tipoProtocolo.Equals(TipoProtocolo.Tipos.LEGALIZACAO) && processo.NumeroProtocolo.StartsWith("87"))
            {
                processo.NumeroProtocolo = processo.JuntaComercialProtocolo;
            }

            return processo;
        }

        public ConsultaProcessoResponse PesquisaDadosProtocolo(string protocolo)
        {
            return PesquisaProtocolo(protocolo);
        }

        public Processo TesteExigencia(string protocolo)
        {
            Processo processo = new Processo();
            reginDao.PesquisaExigenciasProcesso(protocolo, processo, conexaoBancoDadosRegin.CriaComando(), true);
            return processo;
        }

        private static ConsultaProcessoResponse MontaRetornoProtocoloNaoEncontrado(string protocolo)
        {
            ConsultaProcessoResponse pscs = new ConsultaProcessoResponse
            {
                CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.NAO_ENCONTRADO,
                Mensagem = "Protocolo não encontrado",
                DetalheMensagem = "Ao realizar a pesquisa não foi encontrado o protocolo " + protocolo + "."
            };
            return pscs;
        }

        private static ConsultaProcessoResponse MontaRetornoProtocoloSucesso(List<Processo> processos)
        {
            ConsultaProcessoResponse pscs = new ConsultaProcessoResponse
            {
                CodigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO,
                Processos = processos
            };
            return pscs;
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    FechaConexoesBancoDados();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        ~ProtocoloNegocio()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        private void AbreConexoesBancoDados()
        {
            if (conexaoBancoDadosRegin != null)
            {
                conexaoBancoDadosRegin.AbreConexao();
            }

            if (conexaoBancoDadosRequerimento != null)
            {
                conexaoBancoDadosRequerimento.AbreConexao();
            }
        }

        private void FechaConexoesBancoDados()
        {
            if (conexaoBancoDadosRegin != null)
            {
                conexaoBancoDadosRegin.FecharConexao();
            }

            if (conexaoBancoDadosRequerimento != null)
            {
                conexaoBancoDadosRequerimento.FecharConexao();
            }
        }

        private bool filtrarProcessos(ResumoProcesso processo)
        {
            if (processo == null)
            {
                return false;
            }
            //return !string.IsNullOrEmpty(processo.NumeroProtocolo) || (!string.IsNullOrEmpty(processo.PessoaCnpj) && !string.IsNullOrEmpty(processo.PessoaCpf)) || !string.IsNullOrEmpty(processo.StatusCodigo) || !string.IsNullOrEmpty(processo.StatusDescricao);
            return true;
        }
    }
}