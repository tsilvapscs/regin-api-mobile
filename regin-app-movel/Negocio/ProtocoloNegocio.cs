using regin_app_mobile.Constante;
using regin_app_mobile.Dao;
using regin_app_mobile.Database;
using regin_app_mobile.GeracaoXml;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace regin_app_mobile.Negocio
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
            return new ConsultaTipoProtocoloResponse() {
                tipoProtocolo = tipo,
                nomeTipoProtocolo = TipoProtocolo.GetNome(tipo),
                codigoMensagem = codigoMensagem
            };
        }

        public ConsultaProcessoResponse PesquisaProtocolo(string protocolo)
        {
            ConsultaProcessoResponse pscs = null;
            Requerimento requerimento = null;
            TipoProtocolo.Tipos tipoProtocolo = TipoProtocolo.Tipos.NAO_ENCONTRADO;
            ConsultaTipoProtocoloResponse consultaTipoProtocoloResponse = PesquisaTipoProtocolo(protocolo);
            if (consultaTipoProtocoloResponse.codigoMensagem == ((int)ConstantesServicoWeb.CodigosRetorno.SUCESSO))
            {
                tipoProtocolo = consultaTipoProtocoloResponse.tipoProtocolo;
            }
            string siglaUf = ConfigurationManager.AppSettings[ConfiguracaoSistema.GetParametroChave(ConfiguracaoSistema.Parametros.SIGLA_UF)];

            List<Processo> processos;
            if (TipoProtocolo.Tipos.REQUERIMENTO.Equals(tipoProtocolo))
            {
                requerimento = requerimentoDao.ConsultaProtocolo(protocolo, conexaoBancoDadosRequerimento.CriaComando(), true);
                Processo processo;
                if (requerimento != null && requerimento.numeroProtocolo != null && requerimento.numeroProtocoloOrgaoEstadual != null && requerimento.numeroProtocoloOrgaoEstadual.Trim().Length > 0)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.numeroProtocoloOrgaoEstadual, conexaoBancoDadosRegin.CriaComando(), true);
                    if (processos != null && processos.Count > 0)
                    {
                        for (int p = 0; p < processos.Count; p++)
                        {
                            requerimento.cargaOrgaoRegistro = "SIM";
                            processos[p].requerimento = requerimento;
                            processos[p].fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                            if (processos[p].processosRelacionados == null)
                            {
                                processos[p].processosRelacionados = new List<ProcessoRelacionado>();
                            }
                            processos[p].processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.uf, requerimento.numeroProtocolo));
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].processoExigencia.Equals("1") && processos[p].processoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].instituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].instituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].instituicoesAnalise.Count; i++)
                            {
                                processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                    }
                    else
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.cargaOrgaoRegistro = "NÃO";
                        processo.requerimento = requerimento;
                        processo.fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.numeroProtocolo = requerimento.numeroProtocolo;
                        processo.pessoa = new Pessoa
                        {
                            cpfCnpj = requerimento.cnpj
                        };
                        if (requerimento.razaoSocial != null && requerimento.razaoSocial.Trim().Length > 0)
                        {
                            processo.pessoa.nome = requerimento.razaoSocial;
                        }
                        else
                        {
                            processo.pessoa.nome = requerimento.solicitante;
                        }
                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                        processos.Add(processo);
                    }
                    pscs = MontaRetornoProtocoloSucesso(processos);
                }
                else if (requerimento != null && requerimento.numeroProtocolo != null && requerimento.numeroProtocoloViabilidade != null && requerimento.numeroProtocoloViabilidade.Trim().Length > 0)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.numeroProtocoloViabilidade, conexaoBancoDadosRegin.CriaComando(), true);
                    if (processos != null && processos.Count > 0)
                    {
                        for (int p = 0; p < processos.Count; p++)
                        {
                            requerimento.cargaOrgaoRegistro = "SIM";
                            processos[p].requerimento = requerimento;
                            processos[p].fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                            if (processos[p].processosRelacionados == null)
                            {
                                processos[p].processosRelacionados = new List<ProcessoRelacionado>();
                            }
                            processos[p].processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.uf, requerimento.numeroProtocolo));
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].processoExigencia.Equals("1") && processos[p].processoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].instituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].instituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].instituicoesAnalise.Count; i++)
                            {
                                processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                    }
                    else
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.cargaOrgaoRegistro = "NÃO";
                        processo.requerimento = requerimento;
                        processo.fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.numeroProtocolo = requerimento.numeroProtocolo;
                        processo.pessoa = new Pessoa
                        {
                            cpfCnpj = requerimento.cnpj
                        };
                        if (requerimento.razaoSocial != null && requerimento.razaoSocial.Trim().Length > 0)
                        {
                            processo.pessoa.nome = requerimento.razaoSocial;
                        }
                        else
                        {
                            processo.pessoa.nome = requerimento.solicitante;
                        }
                        ajustarNumeroprotocolo87(tipoProtocolo, processo);
                        processos.Add(processo);
                    }
                    pscs = MontaRetornoProtocoloSucesso(processos);
                }
                else if (requerimento != null && requerimento.numeroProtocolo != null)
                {
                    processos = reginDao.PesquisaProtocoloRegin(siglaUf, requerimento.numeroProtocolo, conexaoBancoDadosRegin.CriaComando(), true);

                    if (processos == null)
                    {
                        processos = new List<Processo>();
                        processo = new Processo();
                        requerimento.cargaOrgaoRegistro = "NÃO";
                        processo.fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        processo.requerimento = requerimento;
                        processo.pessoa = new Pessoa
                        {
                            cpfCnpj = requerimento.cnpj
                        };
                        if (requerimento.razaoSocial != null && requerimento.razaoSocial.Trim().Length > 0)
                        {
                            processo.pessoa.nome = requerimento.razaoSocial;
                        }
                        else
                        {
                            processo.pessoa.nome = requerimento.solicitante;
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
                                requerimento.cargaOrgaoRegistro = "SIM";

                                processos[p].requerimento = requerimento;
                                processos[p].fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                                if (processos[p].processosRelacionados == null)
                                {
                                    processos[p].processosRelacionados = new List<ProcessoRelacionado>();
                                }
                                processos[p].processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO), requerimento.uf, requerimento.numeroProtocolo));
                                processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                if (processos[p].processoExigencia.Equals("1") && processos[p].processoSequenciaExigencia != null)
                                {
                                    processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                }
                                // processos[p] = reginDao.pesquisaOrgaoRegistroAnalise(processos[p].uf, processos[p], conexaoBancoDadosRegin.criaComando(), true);
                                // processos[p].instituicoesAnalise[0] = reginDao.pesquisaOrgaoRegistroAndamento(protocolo, processos[p].instituicoesAnalise[0], conexaoBancoDadosRegin.criaComando(), true);
                                processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                                for (int i = 0; i < processos[p].instituicoesAnalise.Count; i++)
                                {
                                    processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                }

                                if (processos[p].eventos != null && processos[p].eventos.Count > 0)
                                {
                                    processos[p].tipoOperacao = processos[p].eventos[0].descricao;
                                    processos[p].ato = processos[p].eventos[0].codigo + " - " + processos[p].eventos[0].descricao;
                                }

                                // Ajustes de dados nao encontrados
                                processos[p].tipo = TipoProtocolo.GetNome(TipoProtocolo.Tipos.REQUERIMENTO);


                                processos[p].pessoa.naturezaJuridicaCodigo = processos[p].requerimento.codigoNaturezaJuridica;
                                processos[p].pessoa.naturezaJuridicaNome = processos[p].requerimento.descricaoNaturezaJuridica;
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
                        if (processos[p].requerimento != null && processos[p].requerimento.numeroProtocolo != null && processos[p].requerimento.numeroProtocolo.Trim().Length > 0)
                        {
                            requerimento = requerimentoDao.ConsultaProtocolo(processos[p].requerimento.numeroProtocolo, conexaoBancoDadosRequerimento.CriaComando(), true);
                            if (requerimento != null)
                            {
                                processos[p].requerimento = requerimento;
                                processos[p].requerimento.cargaOrgaoRegistro = "SIM";
                            }
                            else
                            {
                                processos[p].requerimento.situacao = "PROTOCOLO NÃO ENCONTRADO";
                            }
                        }
                        if (TipoProtocolo.Tipos.NAO_ENCONTRADO == tipoProtocolo)
                        {
                            processos[p] = reginDao.PesquisaEventosOrgaoRegistro(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.REGIN);
                        }
                        else
                        {
                            processos[p] = reginDao.PesquisaEventos(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].fonteDados = FontesInformacao.GetNome(FontesInformacao.TipoFonteInformacao.ORGAO_REGISTRO);
                        }


                        if (TipoProtocolo.Tipos.VIABILIDADE.Equals(tipoProtocolo))
                        {
                            processos[p].tipoOperacao = "CONSULTA PRÉVIA";
                            processos[p].ato = "CONSULTA PRÉVIA";
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            for (int i = 0; i < processos[p].instituicoesAnalise.Count; i++)
                            {
                                processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                        }
                        else
                        {
                            if (processos[p].processoExigencia != null && processos[p].processoExigencia.Equals("1") && processos[p].processoSequenciaExigencia != null)
                            {
                                processos[p] = reginDao.PesquisaExigenciasProcesso(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                            if (processos[p].codigoStatus != null && processos[p].codigoStatus.Equals(1))
                            {
                                processos[p].status = "Finalizado no Orgão de Registro - Enviando para REDESIM";
                            }
                            processos[p] = reginDao.PesquisaOrgaoRegistroAnalise(processos[p].uf, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p].instituicoesAnalise[0] = reginDao.PesquisaOrgaoRegistroAndamento(protocolo, processos[p].instituicoesAnalise[0], conexaoBancoDadosRegin.CriaComando(), true);
                            processos[p] = reginDao.PesquisaInstituicoesAnalise(protocolo, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            if (processos[p].instituicoesAnalise == null || processos[p].instituicoesAnalise.Count == 1 && processos[p].protocoloInternoRegin != null)
                            {
                                processos[p] = reginDao.PesquisaInstituicoesAnalise(processos[p].protocoloInternoRegin, processos[p], conexaoBancoDadosRegin.CriaComando(), true);
                            }
                            for (int i = 1; i < processos[p].instituicoesAnalise.Count; i++)
                            {
                                processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(protocolo, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                if (processos[p].instituicoesAnalise[i].andamentos == null || processos[p].instituicoesAnalise[i].andamentos.Count == 0 && processos[p].protocoloInternoRegin != null)
                                {
                                    processos[p].instituicoesAnalise[i] = reginDao.PesquisaInstituicoesAndamento(processos[p].protocoloInternoRegin, processos[p].instituicoesAnalise[i], conexaoBancoDadosRegin.CriaComando(), true);
                                }
                            }
                        }
                        // Ajusta dados da empresa de acordo com os do requerimento.
                        if (processos[p].pessoa != null)
                        {
                            if (requerimento != null)
                            {
                                if (processos[p].pessoa.nome == null || processos[p].pessoa.nome.Trim().Length <= 0)
                                {
                                    processos[p].pessoa.nome = requerimento.razaoSocial;
                                }
                                if (processos[p].pessoa.cpfCnpj == null || processos[p].pessoa.cpfCnpj.Trim().Length <= 0)
                                {
                                    processos[p].pessoa.cpfCnpj = requerimento.cnpj;
                                }
                                if (processos[p].pessoa.naturezaJuridicaCodigo == null || processos[p].pessoa.naturezaJuridicaCodigo.Trim().Length <= 0)
                                {
                                    processos[p].pessoa.naturezaJuridicaCodigo = requerimento.codigoNaturezaJuridica;
                                }
                                if (processos[p].pessoa.naturezaJuridicaNome == null || processos[p].pessoa.naturezaJuridicaNome.Trim().Length <= 0)
                                {
                                    processos[p].pessoa.naturezaJuridicaNome = requerimento.descricaoNaturezaJuridica;
                                }
                                if (processos[p].pessoa.uf == null || processos[p].pessoa.uf.Trim().Length <= 0)
                                {
                                    processos[p].pessoa.uf = requerimento.uf;
                                }
                            }
                        }

                    }
                    foreach(Processo processo in processos)
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

        private static Processo ajustarNumeroprotocolo87(TipoProtocolo.Tipos tipoProtocolo, Processo processo)
        {
            if (processo.juntaComercialProtocolo != null && processo.juntaComercialProtocolo.Trim().Length > 0 && tipoProtocolo.Equals(TipoProtocolo.Tipos.LEGALIZACAO) && processo.numeroProtocolo.StartsWith("87"))
            {
                processo.numeroProtocolo = processo.juntaComercialProtocolo;
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
                codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.NAO_ENCONTRADO,
                mensagem = "Protocolo não encontrado",
                detalheMensagem = "Ao realizar a pesquisa não foi encontrado o protocolo " + protocolo + "."
            };
            return pscs;
        }

        private static ConsultaProcessoResponse MontaRetornoProtocoloSucesso(List<Processo> processos)
        {
            ConsultaProcessoResponse pscs = new ConsultaProcessoResponse
            {
                codigoMensagem = (int)ConstantesServicoWeb.CodigosRetorno.SUCESSO,
                processos = processos
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

    }
}