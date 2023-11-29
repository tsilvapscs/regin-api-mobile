using regin_app_mobile.Constante;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace regin_app_mobile.GeracaoXml
{
    public class ConsultaProcessoResponse
    {
        public List<Processo> processos = new List<Processo>();
        public int? codigoMensagem;
        public string mensagem;
        public string detalheMensagem;
        public ConsultaProcessoResponse() { }

        public ConsultaProcessoResponse(List<Processo> processos)
        {
            this.processos = processos;
        }










    }

    public class ConsultaTipoProtocoloResponse
    {
        public TipoProtocolo.Tipos tipoProtocolo;
        public string nomeTipoProtocolo;
        public int? codigoMensagem;
        public string mensagem;
        public string detalheMensagem;
        public ConsultaTipoProtocoloResponse() { }

        public ConsultaTipoProtocoloResponse(TipoProtocolo.Tipos tipoProtocolo, string nomeTipoProtocolo)
        {
            this.tipoProtocolo = tipoProtocolo;
            this.nomeTipoProtocolo = nomeTipoProtocolo;
        }
    }

    public class Processo
    {
        public string uf = null;
        public string orgaoRegistroCnpj = null;
        public string orgaoRegistroNome = null;
        public string numeroProtocolo = null;
        public string tipo = null;
        public string status = null;
        public string tipoOperacao = null;
        public string dataInicioProcesso = null;
        public string dataAtualizacaoProcesso = null;
        public string dataCancelamentoProcesso = null;
        public Requerimento requerimento = new Requerimento();
        public string numeroDbe = null;
        public string prefeituraAlvara = null;
        public string prefeituraIss = null;
        public string prefeituraIptu = null;
        public string bombeiroLicenca = null;
        public string vigilanciaLicenca = null;
        public string inscricaoEstadual = null;
        public string juntaComercialProtocolo = null;
        public string motivoCancelamentoProcesso = null;
        public string processoExigencia = null;
        public string processoSequenciaExigencia = null;
        public string fonteDados = null;
        public string ato = null;
        public Pessoa pessoa = new Pessoa();
        public List<Evento> eventos = new List<Evento>();
        public List<InstituicaoAnalise> instituicoesAnalise = new List<InstituicaoAnalise>();
        public List<ProcessoRelacionado> processosRelacionados = new List<ProcessoRelacionado>();
        public List<Exigencia> exigencias = new List<Exigencia>();
        public string codigoStatus;
        public string protocoloInternoRegin;

        public Processo() { }

    }

    public class ProcessoRelacionado
    {
        public string tipo;
        public string uf;
        public string numeroProtocolo;

        public ProcessoRelacionado() { }

        public ProcessoRelacionado(string tipo, string uf, string numeroProtocolo)
        {
            this.tipo = tipo;
            this.uf = uf;
            this.numeroProtocolo = numeroProtocolo;
        }
    }

    public class Pessoa
    {
        public string cpfCnpj = null;
        public string nire = null;
        public string nome = null;
        public string naturezaJuridicaCodigo = null;
        public string naturezaJuridicaNome = null;
        public string uf = null;
        public string municipio = null;

        public Pessoa() { }

        public Pessoa(string cpfCnpj, string nome, string naturezaJuridicaCodigo, string naturezaJuridicaNome, string uf, string municipio)
        {
            this.cpfCnpj = cpfCnpj;
            this.nome = nome;
            this.naturezaJuridicaCodigo = naturezaJuridicaCodigo;
            this.naturezaJuridicaNome = naturezaJuridicaNome;
            this.uf = uf;
            this.municipio = municipio;
        }
    }

    public class Requerimento
    {
        public string numeroProtocolo = null;
        public string numeroDbe = null;
        public string numeroProtocoloViabilidade = null;
        public string numeroProtocoloOrgaoEstadual = null;
        public string numeroProtocoloOrgaoMunicipal = null;
        public string numeroProtocoloEnquadramento = null;
        public string numeroProtocoloOrgaoRegistroOutraUf = null;
        public string situacao = null;
        public string dbeCarregado = null;
        public string cargaOrgaoRegistro = null;
        public string dataEntrada = null;
        public string dataAverbacao = null;
        public string dataAssinatura = null;
        public string razaoSocial = null;
        public string cnpj = null;
        public string solicitante = null;
        public string uf = null;
        public string codigoNaturezaJuridica = null;
        public string descricaoNaturezaJuridica = null;

        public Requerimento() { }

        public Requerimento(string numeroProtocolo, string numeroDbe, string numeroProtocoloViabilidade, string numeroProtocoloOrgaoEstadual, string numeroProtocoloOrgaoMunicipal, string numeroProtocoloEnquadramento, string numeroProtocoloOrgaoRegistroOutraUf, string situacao, string dbeCarregado, string dataEntrada, string dataAverbacao, string dataAssinatura, string razaoSocial, string cnpj, string solicitante, string uf, string codigoNaturezaJuridica, string descricaoNaturezaJuridica)
        {
            this.numeroProtocolo = numeroProtocolo;
            this.numeroDbe = numeroDbe;
            this.numeroProtocoloViabilidade = numeroProtocoloViabilidade;
            this.numeroProtocoloOrgaoEstadual = numeroProtocoloOrgaoEstadual;
            this.numeroProtocoloOrgaoMunicipal = numeroProtocoloOrgaoMunicipal;
            this.numeroProtocoloEnquadramento = numeroProtocoloEnquadramento;
            this.numeroProtocoloOrgaoRegistroOutraUf = numeroProtocoloOrgaoRegistroOutraUf;
            this.situacao = situacao;
            this.dbeCarregado = dbeCarregado;
            this.dataEntrada = dataEntrada;
            this.dataAverbacao = dataAverbacao;
            this.dataAssinatura = dataAssinatura;
            this.razaoSocial = razaoSocial;
            this.cnpj = cnpj;
            this.solicitante = solicitante;
            this.uf = uf;
            this.codigoNaturezaJuridica = codigoNaturezaJuridica;
            this.descricaoNaturezaJuridica = descricaoNaturezaJuridica;
        }
    }

    public class Evento
    {
        public string codigo = null;
        public string descricao = null;

        public Evento() { }

        public Evento(string codigo, string descricao)
        {
            this.codigo = codigo;
            this.descricao = descricao;
        }
    }

    public class InstituicaoAnalise
    {
        public string cnpj = null;
        public string nome = null;
        public string sigla = null;
        public string status = null;
        public string data = null;
        public List<Andamento> andamentos = new List<Andamento>();

        public InstituicaoAnalise() { }

        public InstituicaoAnalise(string cnpj, string nome, List<Andamento> andamentos)
        {
            this.cnpj = cnpj;
            this.nome = nome;
            this.andamentos = andamentos;
        }
    }

    public class Andamento
    {
        public string codigo = null;
        public string area = null;
        public string data;
        public string status = null;
        public string descricao = null;
        public string idFuncionarioAndamento = null;
        public string idFuncionarioAnalise = null;
        public string nomeFuncionarioAndamento = null;
        public string nomeFuncionarioAnalise = null;
        public List<Exigencia> exigencias = new List<Exigencia>();

        public Andamento() { }

        public Andamento(string codigo, string area, string data, string status, string descricao, List<Exigencia> exigencias)
        {
            this.codigo = codigo;
            this.area = area;
            this.data = data;
            this.status = status;
            this.descricao = descricao;
            this.exigencias = exigencias;
        }
    }

    public class Exigencia
    {
        public string codigo = null;
        public string nome = null;
        public string valor = null;
        public string descricao = null;
        public string data;
        public List<ExigenciaOutro> exigenciasOutros = new List<ExigenciaOutro>();


        public Exigencia() { }

        public Exigencia(string codigo, string nome, string valor, string descricao, string data, List<ExigenciaOutro> exigenciasOutros)
        {
            this.codigo = codigo;
            this.nome = nome;
            this.valor = valor;
            this.descricao = descricao;
            this.data = data;
            this.exigenciasOutros = exigenciasOutros;
        }
    }

    public class ExigenciaOutro
    {
        public string sequencia = null;
        public string descricao = null;
        public string fundamentacao = null;

        public ExigenciaOutro() { }

        public ExigenciaOutro(string sequencia, string descricao, string fundamentacao)
        {
            this.sequencia = sequencia;
            this.descricao = descricao;
            this.fundamentacao = fundamentacao;
        }
    }

}