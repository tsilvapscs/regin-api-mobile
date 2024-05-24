using System;
using System.Collections.Generic;
using System.ComponentModel;
using regin_app_movel.Constante;

namespace regin_app_movel.GeracaoXml
{
    public enum TipoAtoEventoRfb
    {
        ATO,
        EVENTO_RFB
    }

    public enum OrigemProcesso
    {
        REGIN,
        REQUERIMENTO
    }

    public class ResumoProcesso
    {
        public string OrgaoRegistroCnpj;
        public string OrgaoRegistroNome;
        public string OrgaoRegistroSigla;
        public string NumeroProtocolo;
        public string PessoaCnpj;
        public string PessoaCpf;
        public string PessoaNome;
        public string SolicitanteNome;
        public string StatusCodigo;
        public string StatusDescricao;
        public string StatusDetalhe;
        public DateTime DataCriacao;
        public DateTime DataAlteracao;
        public TipoAtoEventoRfb AtoEventoRfbTipo;
        public string AtoEventoRfbCodigo;
        public string AtoEventoRfbDescricao;
        public string NumeroProtocoloOrgaoRegistro;
        public string NumeroProtocoloViabilidade;
        public TipoProtocolo.Tipos TipoProtocolo;
        public OrigemProcesso origem;
        public string metodo;
    }
    public class StatusProcesso
    {
        /// <summary>
        /// Número do Protocolo do órgão de registro
        /// </summary>
        public string NumeroProtocolo;
        /// <summary>
        /// CNPJ da pessoa jurídica/empresa
        /// </summary>
        public string Cnpj;
        /// <summary>
        /// Razão Social da pessoa jurídica/empresa
        /// </summary>
        public string RazaoSocial;
        /// <summary>
        /// Descrição do status atual do processo no órgão de registro
        /// </summary>
        public string StatusDescricao;
        /// <summary>
        /// Data de criação do requerimento
        /// </summary>
        public DateTime DataCriacao;
        /// <summary>
        /// Data do status atual do processo no órgão de registro
        /// </summary>
        public DateTime DataAlteracao;
        /// <summary>
        /// Descrição do Ato do processo
        /// </summary>
        public string Ato;
    }
    public class ConsultaProcessosCpfResponse
    {
        public List<ResumoProcesso> Processos;
    }

    public class ConsultaProcessoResponse
    {
        public List<Processo> Processos = new List<Processo>();
        public int? CodigoMensagem;
        public string Mensagem;
        public string DetalheMensagem;

        public ConsultaProcessoResponse()
        {
        }

        public ConsultaProcessoResponse(List<Processo> processos)
        {
            this.Processos = processos;
        }
    }

    public class ConsultaResumoProcessoResponse
    {
        public List<ResumoProcesso> Processos = new List<ResumoProcesso>();
        public int? CodigoMensagem;
        public string Mensagem;
        public string DetalheMensagem;

        public ConsultaResumoProcessoResponse()
        {
        }

        public ConsultaResumoProcessoResponse(List<ResumoProcesso> processos)
        {
            this.Processos = processos;
        }
    }
    public class ConsultaStatusProcessoResponse
    {
        public List<StatusProcesso> Processos = new List<StatusProcesso>();
        public int? CodigoMensagem;
        public string Mensagem;
        public string DetalheMensagem;

        public ConsultaStatusProcessoResponse()
        {
        }

        public ConsultaStatusProcessoResponse(List<StatusProcesso> processos)
        {
            this.Processos = processos;
        }
    }
    public class ErroResponse
    {
        public int? CodigoMensagem;
        public string Mensagem;
        public string DetalheMensagem;        
    }

    public class ConsultaTipoProtocoloResponse
    {
        public TipoProtocolo.Tipos TipoProtocolo;
        public string NomeTipoProtocolo;
        public int? CodigoMensagem;
        public string Mensagem;
        public string DetalheMensagem;

        public ConsultaTipoProtocoloResponse()
        {
        }

        public ConsultaTipoProtocoloResponse(TipoProtocolo.Tipos tipoProtocolo, string nomeTipoProtocolo)
        {
            this.TipoProtocolo = tipoProtocolo;
            this.NomeTipoProtocolo = nomeTipoProtocolo;
        }
    }

    public class Processo
    {
        public string Uf;
        public string OrgaoRegistroCnpj;
        public string OrgaoRegistroNome;
        public string NumeroProtocolo;
        public string Tipo;
        public string Status;
        public string TipoOperacao;
        public string DataInicioProcesso;
        public string DataAtualizacaoProcesso;
        public string DataCancelamentoProcesso;
        public Requerimento Requerimento = new Requerimento();
        public string NumeroDbe;
        public string PrefeituraAlvara;
        public string PrefeituraIss;
        public string PrefeituraIptu;
        public string BombeiroLicenca;
        public string VigilanciaLicenca;
        public string InscricaoEstadual;
        public string JuntaComercialProtocolo;
        public string MotivoCancelamentoProcesso;
        public string ProcessoExigencia;
        public string ProcessoSequenciaExigencia;
        public string FonteDados;
        public string Ato;
        public Pessoa Pessoa = new Pessoa();
        public List<Evento> Eventos = new List<Evento>();
        public List<InstituicaoAnalise> InstituicoesAnalise = new List<InstituicaoAnalise>();
        public List<ProcessoRelacionado> ProcessosRelacionados = new List<ProcessoRelacionado>();
        public List<Exigencia> Exigencias = new List<Exigencia>();
        public string CodigoStatus;
        public string ProtocoloInternoRegin;
    }

    public class ProcessoRelacionado
    {
        public string Tipo;
        public string Uf;
        public string NumeroProtocolo;

        public ProcessoRelacionado(string tipo, string uf, string numeroProtocolo)
        {
            this.Tipo = tipo;
            this.Uf = uf;
            this.NumeroProtocolo = numeroProtocolo;
        }
    }

    public class Pessoa
    {
        public string CpfCnpj;
        public string Nire;
        public string Nome;
        public string NaturezaJuridicaCodigo;
        public string NaturezaJuridicaNome;
        public string Uf;
        public string Municipio;

        public Pessoa()
        {
        }

        public Pessoa(string cpfCnpj, string nome, string naturezaJuridicaCodigo, string naturezaJuridicaNome, string uf, string municipio)
        {
            this.CpfCnpj = cpfCnpj;
            this.Nome = nome;
            this.NaturezaJuridicaCodigo = naturezaJuridicaCodigo;
            this.NaturezaJuridicaNome = naturezaJuridicaNome;
            this.Uf = uf;
            this.Municipio = municipio;
        }
    }

    public class Requerimento
    {
        public string NumeroProtocolo;
        public string NumeroDbe;
        public string NumeroProtocoloViabilidade;
        public string NumeroProtocoloOrgaoEstadual;
        public string NumeroProtocoloOrgaoMunicipal;
        public string NumeroProtocoloEnquadramento;
        public string NumeroProtocoloOrgaoRegistroOutraUf;
        public string Situacao;
        public string DbeCarregado;
        public string CargaOrgaoRegistro;
        public string DataEntrada;
        public string DataAverbacao;
        public string DataAssinatura;
        public string RazaoSocial;
        public string Cnpj;
        public string Solicitante;
        public string Uf;
        public string CodigoNaturezaJuridica;
        public string DescricaoNaturezaJuridica;

        public Requerimento()
        {
        }

        public Requerimento(string numeroProtocolo, string numeroDbe, string numeroProtocoloViabilidade, string numeroProtocoloOrgaoEstadual, string numeroProtocoloOrgaoMunicipal, string numeroProtocoloEnquadramento, string numeroProtocoloOrgaoRegistroOutraUf, string situacao, string dbeCarregado, string dataEntrada, string dataAverbacao, string dataAssinatura, string razaoSocial, string cnpj, string solicitante, string uf, string codigoNaturezaJuridica, string descricaoNaturezaJuridica)
        {
            this.NumeroProtocolo = numeroProtocolo;
            this.NumeroDbe = numeroDbe;
            this.NumeroProtocoloViabilidade = numeroProtocoloViabilidade;
            this.NumeroProtocoloOrgaoEstadual = numeroProtocoloOrgaoEstadual;
            this.NumeroProtocoloOrgaoMunicipal = numeroProtocoloOrgaoMunicipal;
            this.NumeroProtocoloEnquadramento = numeroProtocoloEnquadramento;
            this.NumeroProtocoloOrgaoRegistroOutraUf = numeroProtocoloOrgaoRegistroOutraUf;
            this.Situacao = situacao;
            this.DbeCarregado = dbeCarregado;
            this.DataEntrada = dataEntrada;
            this.DataAverbacao = dataAverbacao;
            this.DataAssinatura = dataAssinatura;
            this.RazaoSocial = razaoSocial;
            this.Cnpj = cnpj;
            this.Solicitante = solicitante;
            this.Uf = uf;
            this.CodigoNaturezaJuridica = codigoNaturezaJuridica;
            this.DescricaoNaturezaJuridica = descricaoNaturezaJuridica;
        }
    }

    public class Evento
    {
        public string Codigo;
        public string Descricao;

        public Evento()
        {
        }

        public Evento(string codigo, string descricao)
        {
            this.Codigo = codigo;
            this.Descricao = descricao;
        }
    }

    public class InstituicaoAnalise
    {
        public string Cnpj;
        public string Nome;
        public string Sigla;
        public string Status;
        public string Data;
        public List<Andamento> Andamentos = new List<Andamento>();

        public InstituicaoAnalise()
        {
        }

        public InstituicaoAnalise(string cnpj, string nome, List<Andamento> andamentos)
        {
            this.Cnpj = cnpj;
            this.Nome = nome;
            this.Andamentos = andamentos;
        }
    }

    public class Andamento
    {
        public string Codigo;
        public string Area;
        public string Data;
        public string Status;
        public string Descricao;
        public string IdFuncionarioAndamento;
        public string IdFuncionarioAnalise;
        public string NomeFuncionarioAndamento;
        public string NomeFuncionarioAnalise;
        public List<Exigencia> Exigencias = new List<Exigencia>();

        public Andamento()
        {
        }

        public Andamento(string codigo, string area, string data, string status, string descricao, List<Exigencia> exigencias)
        {
            this.Codigo = codigo;
            this.Area = area;
            this.Data = data;
            this.Status = status;
            this.Descricao = descricao;
            this.Exigencias = exigencias;
        }
    }

    public class Exigencia
    {
        public string Codigo;
        public string Nome;
        public string Valor;
        public string Descricao;
        public string Data;
        public List<ExigenciaOutro> ExigenciasOutros = new List<ExigenciaOutro>();


        public Exigencia()
        {
        }

        public Exigencia(string codigo, string nome, string valor, string descricao, string data, List<ExigenciaOutro> exigenciasOutros)
        {
            this.Codigo = codigo;
            this.Nome = nome;
            this.Valor = valor;
            this.Descricao = descricao;
            this.Data = data;
            this.ExigenciasOutros = exigenciasOutros;
        }
    }

    public class ExigenciaOutro
    {
        public string Sequencia;
        public string Descricao;
        public string Fundamentacao;

        public ExigenciaOutro()
        {
        }

        public ExigenciaOutro(string sequencia, string descricao, string fundamentacao)
        {
            this.Sequencia = sequencia;
            this.Descricao = descricao;
            this.Fundamentacao = fundamentacao;
        }
    }
}