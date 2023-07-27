using regin_app_mobile.Constante;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace regin_app_mobile.GeracaoXml
{
    //[Serializable]
    //[XmlRootAttribute("PSCS", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class ConsultaProcessoResponse
    {
        //[XmlArrayAttribute("PROCESSOS", IsNullable = true, ElementName = "PROCESSOS")]
        //[XmlArrayItem("PROCESSO", IsNullable = true)]
        public List<Processo> processos = new List<Processo>();
        //[XmlElementAttribute("CODIGO_MENSAGEM", IsNullable = true)]
        public int? codigoMensagem;
        //[XmlElementAttribute("MENSAGEM", IsNullable = true)]
        public string mensagem;
        //[XmlElementAttribute("DETALHE_MENSAGEM", IsNullable = true)]
        public string detalheMensagem;
        public ConsultaProcessoResponse() { }

        public ConsultaProcessoResponse(List<Processo> processos)
        {
            this.processos = processos;
        }

        //private static string toXml(ConsultaProcessoResponse pscs)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(ConsultaProcessoResponse));
        //    using (StringWriter writer = new StringWriter())
        //    {
        //        serializer.Serialize(writer, pscs);
        //        return writer.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
        //    }
        //}

        //private static ConsultaProcessoResponse fromXml(string xmlPscs)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(ConsultaProcessoResponse));

        //    serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
        //    serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

        //    using (TextReader reader = new StringReader(xmlPscs))
        //    {
        //        return (ConsultaProcessoResponse)serializer.Deserialize(reader);
        //    }
        //}

        //private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        //{
        //    Console.WriteLine("Unknown Node: " + e.Name + "\t" + e.Text);
        //}

        //private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        //{
        //    System.Xml.XmlAttribute attr = e.Attr;
        //    Console.WriteLine("Unknown attribute " + attr.Name + " = '" + attr.Value + "'");
        //}


        //public static string serializar(ConstantesServicoWeb.FormatoComunicacao formato, ConsultaProcessoResponse pscs)
        //{
        //    if (ConstantesServicoWeb.FormatoComunicacao.XML.Equals(formato))
        //    {
        //        return toXml(pscs);
        //    }
        //    else if (ConstantesServicoWeb.FormatoComunicacao.JSON.Equals(formato))
        //    {
        //        throw new NotImplementedException();
        //    }
        //    return null;
        //}

        //public static ConsultaProcessoResponse deserializar(ConstantesServicoWeb.FormatoComunicacao formato, string pscsStr)
        //{
        //    if (ConstantesServicoWeb.FormatoComunicacao.XML.Equals(formato))
        //    {
        //        return fromXml(pscsStr);
        //    }
        //    else if (ConstantesServicoWeb.FormatoComunicacao.JSON.Equals(formato))
        //    {
        //        throw new NotImplementedException();
        //    }
        //    return null;
        //}

    }

    public class ConsultaTipoProtocoloResponse
    {
        public TipoProtocolo.Tipos tipoProtocolo;
        public string nomeTipoProtocolo;
        public int? codigoMensagem;
        //[XmlElementAttribute("MENSAGEM", IsNullable = true)]
        public string mensagem;
        //[XmlElementAttribute("DETALHE_MENSAGEM", IsNullable = true)]
        public string detalheMensagem;
        public ConsultaTipoProtocoloResponse() { }

        public ConsultaTipoProtocoloResponse(TipoProtocolo.Tipos tipoProtocolo, string nomeTipoProtocolo)
        {
            this.tipoProtocolo = tipoProtocolo;
            this.nomeTipoProtocolo = nomeTipoProtocolo;
        }
    }

    //[Serializable]
    //[XmlRootAttribute("PROCESSO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Processo
    {
        //[XmlElementAttribute("UF", IsNullable = true)]
        public string uf = null;
        //[XmlElementAttribute("ORGAO_REGISTRO_CNPJ", IsNullable = true)]
        public string orgaoRegistroCnpj = null;
        //[XmlElementAttribute("ORGAO_REGISTRO_NOME", IsNullable = true)]
        public string orgaoRegistroNome = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO", IsNullable = true)]
        public string numeroProtocolo = null;
        //[XmlElementAttribute("TIPO", IsNullable = true)]
        public string tipo = null;
        //[XmlElementAttribute("STATUS", IsNullable = true)]
        public string status = null;
        //[XmlElementAttribute("TIPO_OPERACAO", IsNullable = true)]
        public string tipoOperacao = null;
        //[XmlElementAttribute("DATA_INICIO_PROCESSO", IsNullable = true)]
        public string dataInicioProcesso = null;
        //[XmlElementAttribute("DATA_ATUALIZACAO_PROCESSO", IsNullable = true)]
        public string dataAtualizacaoProcesso = null;
        //[XmlElementAttribute("DATA_CANCELAMENTO_PROCESSO", IsNullable = true)]
        public string dataCancelamentoProcesso = null;
        //[XmlElementAttribute("REQUERIMENTO", IsNullable = true)]
        public Requerimento requerimento = new Requerimento();
        //[XmlElementAttribute("NUMERO_DBE", IsNullable = true)]
        public string numeroDbe = null;
        //[XmlElementAttribute("PREFEITURA_ALVARA", IsNullable = true)]
        public string prefeituraAlvara = null;
        //[XmlElementAttribute("PREFEITURA_ISS", IsNullable = true)]
        public string prefeituraIss = null;
        //[XmlElementAttribute("PREFEITURA_IPTU", IsNullable = true)]
        public string prefeituraIptu = null;
        //[XmlElementAttribute("BOMBEIRO_LICENCA", IsNullable = true)]
        public string bombeiroLicenca = null;
        //[XmlElementAttribute("VIGILANCIA_LICENCA", IsNullable = true)]
        public string vigilanciaLicenca = null;
        //[XmlElementAttribute("INSCRICAO_ESTADUAL", IsNullable = true)]
        public string inscricaoEstadual = null;
        //[XmlElementAttribute("JUNTA_COMERCIAL_PROTOCOLO", IsNullable = true)]
        public string juntaComercialProtocolo = null;
        //[XmlElementAttribute("MOTIVO_CANCELAMENTO_PROCESSO", IsNullable = true)]
        public string motivoCancelamentoProcesso = null;
        //[XmlElementAttribute("PROCESSO_EXIGENCIA", IsNullable = true)]
        public string processoExigencia = null;
        //[XmlElementAttribute("PROCESSO_SEQ_EXIGENCIA", IsNullable = true)]
        public string processoSequenciaExigencia = null;
        //[XmlElementAttribute("FONTE_DADOS", IsNullable = true)]
        public string fonteDados = null;
        //[XmlElementAttribute("ATO", IsNullable = true)]
        public string ato = null;
        //[XmlElementAttribute("PESSOA", IsNullable = true)]
        public Pessoa pessoa = new Pessoa();
        //[XmlArrayAttribute("EVENTOS", IsNullable = true, ElementName = "EVENTOS")]
        //[XmlArrayItem("EVENTO", IsNullable = true)]
        public List<Evento> eventos = new List<Evento>();
        //[XmlArrayAttribute("INSTITUICOES_ANALISE", IsNullable = true, ElementName = "INSTITUICOES_ANALISE")]
        //[XmlArrayItem("INSTITUICAO_ANALISE", IsNullable = true)]
        public List<InstituicaoAnalise> instituicoesAnalise = new List<InstituicaoAnalise>();
        //[XmlArrayAttribute("PROCESSOS_RELACIONADOS", IsNullable = true, ElementName = "PROCESSOS_RELACIONADOS")]
        //[XmlArrayItem("PROCESSO_RELACIONADO", IsNullable = true)]
        public List<ProcessoRelacionado> processosRelacionados = new List<ProcessoRelacionado>();
        //[XmlArrayAttribute("EXIGENCIAS", IsNullable = true, ElementName = "EXIGENCIAS")]
        //[XmlArrayItem("EXIGENCIA", IsNullable = true)]
        public List<Exigencia> exigencias = new List<Exigencia>();
        //[XmlIgnore]
        public string codigoStatus;

        public Processo() { }

        public Processo(string uf, string orgaoRegistroCnpj, string orgaoRegistroNome, string numeroProtocolo, string tipoOperacao, string status, string data, string numeroDbe, Pessoa pessoa, List<Evento> eventos, string tipo, List<Exigencia> exigencias, string processoExigencia, string processoSequenciaExigencia, string fonteDados, string codigoStatus, string ato, string inscricaoEstadual)
        {
            this.uf = uf;
            this.orgaoRegistroCnpj = orgaoRegistroCnpj;
            this.orgaoRegistroNome = orgaoRegistroNome;
            this.numeroProtocolo = numeroProtocolo;
            this.tipoOperacao = tipoOperacao;
            this.pessoa = pessoa;
            this.eventos = eventos;
            this.tipo = tipo;
            this.status = status;
            this.numeroDbe = numeroDbe;
            this.exigencias = exigencias;
            this.dataInicioProcesso = data;
            this.processoExigencia = processoExigencia;
            this.processoSequenciaExigencia = processoSequenciaExigencia;
            this.fonteDados = fonteDados;
            this.ato = ato;
            this.inscricaoEstadual = inscricaoEstadual;
            this.codigoStatus = codigoStatus;
        }

    }

    //[Serializable]
    //[XmlRootAttribute("PROCESSO_RELACIONADO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class ProcessoRelacionado
    {
        //[XmlElementAttribute("TIPO", IsNullable = true)]
        public string tipo;
        //[XmlElementAttribute("UF", IsNullable = true)]
        public string uf;
        //[XmlElementAttribute("NUMERO_PROTOCOLO", IsNullable = true)]
        public string numeroProtocolo;

        public ProcessoRelacionado() { }

        public ProcessoRelacionado(string tipo, string uf, string numeroProtocolo)
        {
            this.tipo = tipo;
            this.uf = uf;
            this.numeroProtocolo = numeroProtocolo;
        }
    }

    //[Serializable]
    //[XmlRootAttribute("PESSOA", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Pessoa
    {
        //[XmlElementAttribute("CPF_CNPJ", IsNullable = true)]
        public string cpfCnpj = null;
        //[XmlElementAttribute("NIRE", IsNullable = true)]
        public string nire = null;
        //[XmlElementAttribute("RAZAO_SOCIAL", IsNullable = true)]
        public string nome = null;
        //[XmlElementAttribute("NATUREZA_JURIDICA_CODIGO", IsNullable = true)]
        public string naturezaJuridicaCodigo = null;
        //[XmlElementAttribute("NATUREZA_JURIDICA_NOME", IsNullable = true)]
        public string naturezaJuridicaNome = null;
        //[XmlElementAttribute("UF", IsNullable = true)]
        public string uf = null;
        //[XmlElementAttribute("MUNICIPIO", IsNullable = true)]
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

    //[Serializable]
    //[XmlRootAttribute("REQUERIMENTO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Requerimento
    {
        //[XmlElementAttribute("NUMERO_PROTOCOLO", IsNullable = true)]
        public string numeroProtocolo = null;
        //[XmlElementAttribute("NUMERO_DBE", IsNullable = true)]
        public string numeroDbe = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO_VIABILIDADE", IsNullable = true)]
        public string numeroProtocoloViabilidade = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO_ORGAO_ESTADUAL", IsNullable = true)]
        public string numeroProtocoloOrgaoEstadual = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO_ORGAO_MUNICIPAL", IsNullable = true)]
        public string numeroProtocoloOrgaoMunicipal = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO_ENQUADRAMENTO", IsNullable = true)]
        public string numeroProtocoloEnquadramento = null;
        //[XmlElementAttribute("NUMERO_PROTOCOLO_ORGAO_REGISTRO_OUTRA_UF", IsNullable = true)]
        public string numeroProtocoloOrgaoRegistroOutraUf = null;
        //[XmlElementAttribute("SITUACAO", IsNullable = true)]
        public string situacao = null;
        //[XmlElementAttribute("DBE_CARREGADO", IsNullable = true)]
        public string dbeCarregado = null;
        //[XmlElementAttribute("CARGA_ORGAO_REGISTRO", IsNullable = true)]
        public string cargaOrgaoRegistro = null;
        //[XmlElementAttribute("DATA_ENTRADA", IsNullable = true)]
        public string dataEntrada = null;
        //[XmlElementAttribute("DATA_AVERBACAO", IsNullable = true)]
        public string dataAverbacao = null;
        //[XmlElementAttribute("DATA_ASSINATURA", IsNullable = true)]
        public string dataAssinatura = null;
        //[XmlElementAttribute("RAZAO_SOCIAL", IsNullable = true)]
        public string razaoSocial = null;
        //[XmlElementAttribute("CNPJ", IsNullable = true)]
        public string cnpj = null;
        //[XmlElementAttribute("SOLICITANTE", IsNullable = true)]
        public string solicitante = null;
        //[XmlElementAttribute("UF", IsNullable = true)]
        public string uf = null;
        //[XmlElementAttribute("CODIGO_NATURZA_JURIDICA", IsNullable = true)]
        public string codigoNaturezaJuridica = null;
        //[XmlElementAttribute("DESCRICAO_NATURZA_JURIDICA", IsNullable = true)]
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

    //[Serializable]
    //[XmlRootAttribute("EVENTO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Evento
    {
        //[XmlElementAttribute("CODIGO", IsNullable = true)]
        public string codigo = null;
        //[XmlElementAttribute("DESCRICAO", IsNullable = true)]
        public string descricao = null;

        public Evento() { }

        public Evento(string codigo, string descricao)
        {
            this.codigo = codigo;
            this.descricao = descricao;
        }
    }

    //[Serializable]
    //[XmlRootAttribute("INSTITUICAO_ANALISE", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class InstituicaoAnalise
    {
        //[XmlElementAttribute("CNPJ", IsNullable = true)]
        public string cnpj = null;
        //[XmlElementAttribute("NOME", IsNullable = true)]
        public string nome = null;
        //[XmlElementAttribute("SIGLA", IsNullable = true)]
        public string sigla = null;
        //[XmlElementAttribute("STATUS", IsNullable = true)]
        public string status = null;
        //[XmlElementAttribute("DATA_FINAL", IsNullable = true)]
        public string data = null;
        //[XmlArrayAttribute("ANDAMENTOS", IsNullable = true, ElementName = "ANDAMENTOS")]
        //[XmlArrayItem("ANDAMENTO", IsNullable = true)]
        public List<Andamento> andamentos = new List<Andamento>();

        public InstituicaoAnalise() { }

        public InstituicaoAnalise(string cnpj, string nome, List<Andamento> andamentos)
        {
            this.cnpj = cnpj;
            this.nome = nome;
            this.andamentos = andamentos;
        }
    }

    //[Serializable]
    //[XmlRootAttribute("ANDAMENTO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Andamento
    {
        //[XmlElementAttribute("CODIGO", IsNullable = true)]
        public string codigo = null;
        //[XmlElementAttribute("AREA", IsNullable = true)]
        public string area = null;
        //[XmlElementAttribute("DATA", IsNullable = true)]
        public string data;
        //[XmlElementAttribute("STATUS", IsNullable = true)]
        public string status = null;
        //[XmlElementAttribute("DESCRICAO", IsNullable = true)]
        public string descricao = null;
        //[XmlElementAttribute("ID_FUNCIONARIO_ANDAMENTO", IsNullable = true)]
        public string idFuncionarioAndamento = null;
        //[XmlElementAttribute("ID_FUNCIONARIO_ANALISE", IsNullable = true)]
        public string idFuncionarioAnalise = null;
        //[XmlElementAttribute("NOME_FUNCIONARIO_ANDAMENTO", IsNullable = true)]
        public string nomeFuncionarioAndamento = null;
        //[XmlElementAttribute("NOME_FUNCIONARIO_ANALISE", IsNullable = true)]
        public string nomeFuncionarioAnalise = null;
        //[XmlArrayAttribute("EXIGENCIAS", IsNullable = true, ElementName = "EXIGENCIAS")]
        //[XmlArrayItem("EXIGENCIA", IsNullable = true)]
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

    //[Serializable]
    //[XmlRootAttribute("EXIGENCIA", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class Exigencia
    {
        //[XmlElementAttribute("CODIGO", IsNullable = true)]
        public string codigo = null;
        //[XmlElementAttribute("NOME", IsNullable = true)]
        public string nome = null;
        //[XmlElementAttribute("VALOR", IsNullable = true)]
        public string valor = null;
        //[XmlElementAttribute("DESCRICAO", IsNullable = true)]
        public string descricao = null;
        //[XmlElementAttribute("DATA", IsNullable = true)]
        public string data;
        //[XmlArrayAttribute("EXIGENCIAS_OUTROS", IsNullable = true, ElementName = "EXIGENCIAS_OUTROS")]
        //[XmlArrayItem("EXIGENCIA_OUTRO", IsNullable = true)]
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

    //[Serializable]
    //[XmlRootAttribute("EXIGENCIA_OUTRO", Namespace = "http://www.pscs.com.br", IsNullable = true)]
    public class ExigenciaOutro
    {
        //[XmlElementAttribute("SEQUENCIA", IsNullable = true)]
        public string sequencia = null;
        //[XmlElementAttribute("DESCRICAO", IsNullable = true)]
        public string descricao = null;
        //[XmlElementAttribute("FUNDAMENTACAO", IsNullable = true)]
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