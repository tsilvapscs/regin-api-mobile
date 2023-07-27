using regin_app_mobile.Constante;
using regin_app_mobile.GeracaoXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace regin_app_mobile.Dao
{
    /// <summary>
    /// Implementacao da interface ReginGeo para consultas no banco da junta comercial que tenha SGBD Oracle
    /// </summary>
    public class ReginOracleImplDao : IReginDao
    {
        public ReginOracleImplDao()
        {

        }

        public bool FiltrarAndamentos(IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {

                query.Append("SELECT COUNT(*) AS QTD ");
                query.Append("  FROM swm_filtro_andamento s ");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                iDataReader = iDbCommand.ExecuteReader();

                if (iDataReader.Read())
                {
                    IDataRecord iDataRecord = (IDataRecord)iDataReader;
                    int qtdRegistros = iDataRecord.GetInt32(0);
                    if (qtdRegistros > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {

                query.Append("SELECT p.pro_protocolo ");
                query.Append("  FROM psc_protocolo p ");
                query.Append(" WHERE p.pro_protocolo = :protocolo");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();

                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;

                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                if (iDataReader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT p.pro_protocolo ");
                query.Append("     , DECODE(p.pro_tip_operacao, 1, 3, 2, 3, 3, 5, 8, 8, 7, 7, -1) AS pro_tip_operacao ");
                query.Append("  FROM psc_protocolo p ");
                query.Append(" WHERE p.pro_protocolo = :protocolo");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                if (iDataReader.Read())
                {
                    IDataRecord iDataRecord = (IDataRecord)iDataReader;
                    return TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16((decimal)iDataRecord[1]));
                }
                else
                {
                    return TipoProtocolo.Tipos.NAO_ENCONTRADO;
                }
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<Processo> PesquisaProtocoloRegin(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                List<Processo> processos = new List<Processo>();

                query.Append("SELECT PPR.PRO_PROTOCOLO AS PROTOCOLO "); // 0
                query.Append("      ,pkg_util.GetGenerica(10, PPR.PRO_STATUS) AS PROTOCOLO_STATUS "); // 1
                query.Append("      ,COALESCE(PPR.PRO_FEC_INC, sysdate) AS PROTOCOLO_DATA_INICIO "); // 2
                query.Append("      ,COALESCE(TIN.TIN_TMU_TUF_UF, '" + siglaUf + "') AS ORGAO_REGISTRO_UF "); // 3
                query.Append("      ,TMU.TMU_NOM_MUN AS PESSOA_MUNICIPIO "); // 4
                query.Append("      ,pkg_util.GetGenerica(9, PPR.PRO_TIP_OPERACAO) AS PROTOCOLO_TIPO_OPERACAO "); // 5
                query.Append("      ,DECODE(PPR.pro_tip_operacao, 1, 3, 2, 3, 3, 5, 8, 8, 7, 7, -1) AS PROTOCOLO_TIPO "); // 6
                query.Append("      ,PPR.PRO_DATA_CANCELAMENTO AS PROTOCOLO_CANCELAMENTO_DATA "); // 7
                query.Append("      ,to_char(PPR.PRO_MOTIVO) AS PROTOCOLO_CANCELAMENTO_MOTIVO "); // 8
                query.Append("      ,COALESCE(PPR.PRO_FEC_ATUALIZACAO, PPR.PRO_FEC_INC, sysdate) AS PROTOCOLO_DATA_ATUALIZACAO "); // 9
                query.Append("      ,pkg_util.GetGenerica(PPR.PRO_TGE_TGACAO, PPR.PRO_TGE_VGACAO) AS PROTOCOLO_ACAO "); // 10
                query.Append("      ,PPR.PRO_VPV_COD_PROTOCOLO AS PROTOCOLO_VIABILIDADE "); // 11
                query.Append("      ,PPR.PRO_NR_DBE AS PROTOCOLO_DBE "); // 12
                query.Append("      ,PPR.PRO_NR_REQUERIMENTO AS PROTOCOLO_REQUERIMENTO "); // 13
                query.Append("      ,TIN.TIN_CNPJ AS PROTOCOLO_ORGAO_REGISTRO_CNPJ "); // 14
                query.Append("      ,TIN.TIN_NOM_INST AS PROTOCOLO_ORGAO_REGISTRO_NOME "); // 15
                query.Append("      ,COALESCE(PIP.PIP_CNPJ, pj.nr_cgc, '-') AS PESSOA_CNPJ "); // 16
                query.Append("      ,PIP.PIP_NIRE AS PESSOA_NIRE "); // 17
                query.Append("      ,PIP.PIP_ALVARA_PM AS PROTOCOLO_PREFEITURA_ALVARA "); // 18
                query.Append("      ,PIP.PIP_RUC AS PROTOCOLO_NUMERO_INSCRICAO_RUC "); // 19
                query.Append("      ,COALESCE(PIP.PIP_NOME_RAZAO_SOCIAL, '-') AS PESSOA_RAZAO_SOCIAL "); // 20
                query.Append("      ,PIP.PIP_ISS AS PROTOCOLO_PREFEITURA_ISS "); // 21
                query.Append("      ,PIP.PIP_IPTU AS PROTOCOLO_PREFEITURA_IPTU "); // 22
                query.Append("      ,PIP.PIP_ALVARA_BOMBEIRO AS PROTOCOLO_BOMBEIRO_LICENCA "); // 23
                query.Append("      ,PIP.PIP_ALVARA_VIGILANCIA AS PROTOCOLO_VIGILANCIA_LICENCA "); // 24
                query.Append("      ,PIP.PIP_PROTOCOLO_JUNTA AS PROTOCOLO_JUNTA_COMERCIAL "); // 25
                query.Append("      ,PPR.PRO_DATA_INICIO_ATIV AS PROTOCOLO_DT_INI_ATIVIDADE "); // 26
                query.Append("      ,pkg_consulta_movel.consultaexigencia(PPR.PRO_PROTOCOLO) AS SEQUENCIA_ANDAMENTO_EXIGENCIA "); // 27
                query.Append("      ,COALESCE(PPR.PRO_STATUS, 0) AS CODIGO_STATUS"); // 28
                query.Append("      ,COALESCE(to_char(RCP.RCO_TNC_COD_NATUR), REPLACE(p.co_natureza_juridica, '-', '')) AS CODIGO_NATUREZA_JURIDICA "); // 29
                query.Append("      ,UPPER(pkg_util.GetGenerica(900, COALESCE(to_char(RCP.RCO_TNC_COD_NATUR), REPLACE(p.co_natureza_juridica, '-', '')))) AS DESCRICAO_NATUREZA_JURIDICA "); // 30
                query.Append("      ,COALESCE(UPPER((SELECT a1.no_ato FROM solicitacao s1 INNER JOIN ATO a1 ON (s1.co_ato = a1.co_ato) WHERE s1.nr_protocolo = PPR.PRO_PROTOCOLO AND s1.sq_solicitacao = 1)), '-') AS PROTOCOLO_ATO "); // 31
                query.Append("      ,RGE.RGE_RUC AS INSCRICAO_ESTADUAL "); // 32
                query.Append("  FROM PSC_PROTOCOLO PPR ");
                query.Append("  LEFT JOIN PSC_IDENT_PROTOCOLO PIP ON (PPR.PRO_PROTOCOLO = PIP.PIP_PRO_PROTOCOLO) ");
                query.Append("  LEFT JOIN TAB_MUNIC TMU ON (PPR.PRO_TMU_TUF_UF = TMU.TMU_TUF_UF AND PPR.PRO_TMU_COD_MUN = TMU.TMU_COD_MUN) ");
                query.Append("  LEFT JOIN TAB_INSTITUICAO TIN ON (pkg_consulta_movel.LOOKUP_CNPJ_JUNTA_COMERCIAL() = TIN.TIN_CNPJ) ");
                query.Append("  LEFT JOIN RUC_COMP RCP ON (RCP.RCO_RGE_PRA_PROTOCOLO = PPR.PRO_PROTOCOLO) ");
                query.Append("  LEFT JOIN processo p ON (p.nr_protocolo = PPR.PRO_PROTOCOLO) ");
                query.Append("  LEFT JOIN processo_pessoa pp ON (pp.co_junta_comercial = p.co_junta_comercial AND pp.co_sequencial = p.co_sequencial AND pp.nr_protocolo = p.nr_protocolo) ");
                query.Append("  LEFT JOIN pessoa pe ON (pe.sq_pessoa = pp.sq_pessoa AND pe.co_junta_comercial = pp.co_junta_comercial AND pe.co_sequencial = pp.co_sequencial) ");
                query.Append("  LEFT JOIN pessoa_juridica pj ON (pj.sq_pessoa = pe.sq_pessoa AND pj.co_junta_comercial = pe.co_junta_comercial AND pj.co_sequencial = pe.co_sequencial) ");
                query.Append("  LEFT JOIN RUC_GENERAL RGE ON (PPR.PRO_PROTOCOLO = RGE.RGE_PRA_PROTOCOLO) ");
                query.Append(" WHERE PPR.PRO_PROTOCOLO = :protocolo");
                query.Append("   AND PPR.PRO_TIP_OPERACAO <> 4");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;

                    Processo processo = new Processo();

                    if (!iDataRecord.IsDBNull(0))
                    {
                        processo.numeroProtocolo = iDataRecord.GetString(0);
                    }
                    if (!iDataRecord.IsDBNull(6))
                    {
                        processo.tipo = TipoProtocolo.GetNome(TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16(iDataRecord.GetDecimal(6))));
                    }
                    if (!iDataRecord.IsDBNull(25))
                    {
                        processo.juntaComercialProtocolo = iDataRecord.GetString(25);
                    }
                    if (!iDataRecord.IsDBNull(12))
                    {
                        processo.numeroDbe = iDataRecord.GetString(12);
                    }
                    if (!iDataRecord.IsDBNull(14))
                    {
                        processo.orgaoRegistroCnpj = iDataRecord.GetString(14);
                    }
                    if (!iDataRecord.IsDBNull(15))
                    {
                        processo.orgaoRegistroNome = iDataRecord.GetString(15);
                    }
                    if (!iDataRecord.IsDBNull(18))
                    {
                        processo.prefeituraAlvara = iDataRecord.GetString(18);
                    }
                    if (!iDataRecord.IsDBNull(22))
                    {
                        processo.prefeituraIptu = iDataRecord.GetString(22);
                    }
                    if (!iDataRecord.IsDBNull(21))
                    {
                        processo.prefeituraIss = iDataRecord.GetString(21);
                    }
                    if (!iDataRecord.IsDBNull(1))
                    {
                        processo.status = iDataRecord.GetString(1);
                    }
                    if (!iDataRecord.IsDBNull(5))
                    {
                        processo.tipoOperacao = iDataRecord.GetString(5);
                    }
                    if (!iDataRecord.IsDBNull(3))
                    {
                        processo.uf = iDataRecord.GetString(3);
                    }
                    if (!iDataRecord.IsDBNull(24))
                    {
                        processo.vigilanciaLicenca = iDataRecord.GetString(24);
                    }
                    if (!iDataRecord.IsDBNull(23))
                    {
                        processo.bombeiroLicenca = iDataRecord.GetString(23);
                    }
                    if (!iDataRecord.IsDBNull(13))
                    {
                        processo.requerimento.numeroProtocolo = iDataRecord.GetString(13);
                    }
                    if (!iDataRecord.IsDBNull(2))
                    {
                        processo.dataInicioProcesso = iDataRecord.GetDateTime(2).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(9))
                    {
                        processo.dataAtualizacaoProcesso = iDataRecord.GetDateTime(9).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(7))
                    {
                        processo.dataCancelamentoProcesso = iDataRecord.GetDateTime(7).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(8))
                    {
                        processo.motivoCancelamentoProcesso = iDataRecord.GetString(8);
                    }
                    if (!iDataRecord.IsDBNull(27))
                    {
                        processo.processoSequenciaExigencia = iDataRecord.GetString(27);
                    }
                    if (!iDataRecord.IsDBNull(31))
                    {
                        processo.ato = iDataRecord.GetString(31);
                    }
                    if (processo.processoSequenciaExigencia != null)
                    {
                        processo.processoExigencia = "1";
                    }
                    else
                    {
                        processo.processoExigencia = "0";
                    }
                    if (!iDataRecord.IsDBNull(32))
                    {
                        processo.inscricaoEstadual = iDataRecord.GetString(32);
                    }
                    if (!iDataRecord.IsDBNull(16))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.cpfCnpj = iDataRecord.GetString(16);
                    }
                    if (!iDataRecord.IsDBNull(17))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.nire = iDataRecord.GetString(17);
                    }
                    if (!iDataRecord.IsDBNull(20))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.nome = iDataRecord.GetString(20);
                    }
                    if (!iDataRecord.IsDBNull(4))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.municipio = iDataRecord.GetString(4);
                    }
                    if (!iDataRecord.IsDBNull(3))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.uf = iDataRecord.GetString(3);
                    }
                    if (!iDataRecord.IsDBNull(29))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.naturezaJuridicaCodigo = iDataRecord.GetString(29);
                    }
                    if (!iDataRecord.IsDBNull(30))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.naturezaJuridicaNome = iDataRecord.GetString(30);
                    }
                    if (!iDataRecord.IsDBNull(11))
                    {
                        if (processo.processosRelacionados == null)
                        {
                            processo.processosRelacionados = new List<ProcessoRelacionado>();
                        }
                        processo.processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.VIABILIDADE), processo.uf, iDataRecord.GetString(11)));
                    }
                    if (!iDataRecord.IsDBNull(12))
                    {
                        if (processo.processosRelacionados == null)
                        {
                            processo.processosRelacionados = new List<ProcessoRelacionado>();
                        }
                        processo.processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.DBE), processo.uf, iDataRecord.GetString(12)));
                    }
                    if (!iDataRecord.IsDBNull(28))
                    {
                        processo.codigoStatus = iDataRecord.GetInt32(28).ToString();
                    }
                    processos.Add(processo);
                }
                return processos;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<Processo> PesquisaProtocoloOrgaoRegistro(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                List<Processo> processos = new List<Processo>();

                query.Append("SELECT p.nr_protocolo AS PROTOCOLO "); // 0
                query.Append("      ,UPPER(DECODE(p.in_status_processo, 1, 'Em tramitação', 2, 'Em Exigência', 3, 'Deferido', 4, 'Indeferido', 5, 'Desistência', 6, 'Suspenso por ordem judicial', 7, 'Em condições de aporvação', 8, 'Recebido pelo arquivo', 9, 'Autenticado', 'Status não identificado')) AS PROTOCOLO_STATUS "); // 1
                query.Append("      ,(SELECT a.dt_andamento FROM andamento a WHERE a.co_junta_comercial = p.co_junta_comercial AND a.nr_protocolo = p.nr_protocolo AND a.sq_andamento = 1) AS PROTOCOLO_DATA_INICIO "); // 2
                query.Append("      ,COALESCE(u.si_uf, '" + siglaUf + "') AS ORGAO_REGISTRO_UF "); // 3
                query.Append("      ,m.no_municipio AS PESSOA_MUNICIPIO "); // 4
                query.Append("      ,COALESCE(UPPER((SELECT a1.no_ato FROM solicitacao s1 INNER JOIN ATO a1 ON (s1.co_ato = a1.co_ato) WHERE s1.nr_protocolo = p.nr_protocolo AND s1.sq_solicitacao = 1)), '-') AS PROTOCOLO_TIPO_OPERACAO "); // 5
                query.Append("      ,3 AS PROTOCOLO_TIPO "); // 6
                query.Append("      ,null AS PROTOCOLO_CANCELAMENTO_DATA "); // 7
                query.Append("      ,null AS PROTOCOLO_CANCELAMENTO_MOTIVO "); // 8
                query.Append("      ,COALESCE((SELECT MAX(a.dt_andamento) FROM andamento a WHERE a.co_junta_comercial = p.co_junta_comercial AND a.co_sequencial = p.co_sequencial AND a.nr_protocolo = p.nr_protocolo AND CONCAT(to_char(a.si_secao_origem), to_char(a.si_secao_destino)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) GROUP BY a.nr_protocolo), sysdate) AS PROTOCOLO_DATA_ATUALIZACAO "); // 9
                query.Append("      ,'N/A' AS PROTOCOLO_ACAO "); // 10
                query.Append("      ,null AS PROTOCOLO_VIABILIDADE "); // 11
                query.Append("      ,null AS PROTOCOLO_DBE "); // 12
                query.Append("      ,null AS PROTOCOLO_REQUERIMENTO "); // 13
                query.Append("      ,(SELECT un.nr_cgc FROM unidade un WHERE un.co_sequencial = '000') AS PROTOCOLO_ORGAO_REGISTRO_CNPJ "); // 14
                query.Append("      ,(SELECT un.no_unidade FROM unidade un WHERE un.co_sequencial = '000') AS PROTOCOLO_ORGAO_REGISTRO_NOME "); // 15
                query.Append("      ,pj.nr_cgc AS PESSOA_CNPJ "); // 16
                query.Append("      ,pj.nr_nire AS PESSOA_NIRE "); // 17
                query.Append("      ,pj.nr_inscricao_municipal AS PROTOCOLO_PREFEITURA_ALVARA "); // 18
                query.Append("      ,null AS PROTOCOLO_NUMERO_INSCRICAO_RUC "); // 19
                query.Append("      ,COALESCE(pe.no_pessoa, p.no_empresarial, '-') AS PESSOA_RAZAO_SOCIAL "); // 20
                query.Append("      ,null AS PROTOCOLO_PREFEITURA_ISS "); // 21
                query.Append("      ,null AS PROTOCOLO_PREFEITURA_IPTU "); // 22
                query.Append("      ,null AS PROTOCOLO_BOMBEIRO_LICENCA "); // 23
                query.Append("      ,null AS PROTOCOLO_VIGILANCIA_LICENCA "); // 24
                query.Append("      ,null AS PROTOCOLO_JUNTA_COMERCIAL "); // 25
                query.Append("      ,pj.dt_inicio_atividade AS PROTOCOLO_DT_INI_ATIVIDADE "); // 26
                query.Append("      ,pkg_consulta_movel.consultaexigencia(p.nr_protocolo) AS SEQUENCIA_ANDAMENTO_EXIGENCIA "); // 27
                query.Append("      ,nj.co_natureza_juridica AS CODIGO_NATURZA_JURIDICA "); // 28
                query.Append("      ,nj.no_natureza_juridica AS DESCRICAO_NATURZA_JURIDICA "); // 29
                query.Append("FROM PROCESSO p ");
                query.Append("LEFT JOIN uf u ON (u.co_uf = p.co_junta_comercial) ");
                query.Append("LEFT JOIN processo_pessoa pp ON (pp.co_junta_comercial = p.co_junta_comercial AND pp.co_sequencial = p.co_sequencial AND pp.nr_protocolo = p.nr_protocolo) ");
                query.Append("LEFT JOIN pessoa pe ON (pe.sq_pessoa = pp.sq_pessoa AND pe.co_junta_comercial = pp.co_junta_comercial AND pe.co_sequencial = pp.co_sequencial) ");
                query.Append("LEFT JOIN pessoa_juridica pj ON (pj.sq_pessoa = pe.sq_pessoa AND pj.co_junta_comercial = pe.co_junta_comercial AND pj.co_sequencial = pe.co_sequencial) ");
                query.Append("LEFT JOIN municipio m ON (pe.co_municipio = m.co_municipio) ");
                query.Append("LEFT JOIN natureza_juridica nj ON (p.co_natureza_juridica = nj.co_natureza_juridica) ");
                query.Append(" WHERE p.nr_protocolo = :protocolo");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;

                    Processo processo = new Processo();

                    if (!iDataRecord.IsDBNull(0))
                    {
                        processo.numeroProtocolo = iDataRecord.GetString(0);
                    }
                    if (!iDataRecord.IsDBNull(6))
                    {
                        processo.tipo = TipoProtocolo.GetNome(TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16(iDataRecord.GetDecimal(6))));
                    }
                    if (!iDataRecord.IsDBNull(25))
                    {
                        processo.juntaComercialProtocolo = iDataRecord.GetString(25);
                    }
                    if (!iDataRecord.IsDBNull(12))
                    {
                        processo.numeroDbe = iDataRecord.GetString(12);
                    }
                    if (!iDataRecord.IsDBNull(14))
                    {
                        processo.orgaoRegistroCnpj = iDataRecord.GetString(14);
                    }
                    if (!iDataRecord.IsDBNull(15))
                    {
                        processo.orgaoRegistroNome = iDataRecord.GetString(15);
                    }
                    if (!iDataRecord.IsDBNull(18))
                    {
                        processo.prefeituraAlvara = iDataRecord.GetString(18);
                    }
                    if (!iDataRecord.IsDBNull(22))
                    {
                        processo.prefeituraIptu = iDataRecord.GetString(22);
                    }
                    if (!iDataRecord.IsDBNull(21))
                    {
                        processo.prefeituraIss = iDataRecord.GetString(21);
                    }
                    if (!iDataRecord.IsDBNull(1))
                    {
                        processo.status = iDataRecord.GetString(1);
                    }
                    if (!iDataRecord.IsDBNull(5))
                    {
                        processo.tipoOperacao = iDataRecord.GetString(5);
                        processo.ato = iDataRecord.GetString(5);
                    }
                    if (!iDataRecord.IsDBNull(3))
                    {
                        processo.uf = iDataRecord.GetString(3);
                    }
                    if (!iDataRecord.IsDBNull(24))
                    {
                        processo.vigilanciaLicenca = iDataRecord.GetString(24);
                    }
                    if (!iDataRecord.IsDBNull(23))
                    {
                        processo.bombeiroLicenca = iDataRecord.GetString(23);
                    }
                    if (!iDataRecord.IsDBNull(13))
                    {
                        processo.requerimento.numeroProtocolo = iDataRecord.GetString(13);
                    }
                    if (!iDataRecord.IsDBNull(2))
                    {
                        processo.dataInicioProcesso = iDataRecord.GetDateTime(2).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(9))
                    {
                        processo.dataAtualizacaoProcesso = iDataRecord.GetDateTime(9).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(7))
                    {
                        processo.dataCancelamentoProcesso = iDataRecord.GetDateTime(7).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(8))
                    {
                        processo.motivoCancelamentoProcesso = iDataRecord.GetString(8);
                    }
                    if (!iDataRecord.IsDBNull(27))
                    {
                        processo.processoSequenciaExigencia = iDataRecord.GetString(27);
                    }
                    if (processo.processoSequenciaExigencia != null)
                    {
                        processo.processoExigencia = "1";
                    }
                    else
                    {
                        processo.processoExigencia = "0";
                    }
                    if (!iDataRecord.IsDBNull(16))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.cpfCnpj = iDataRecord.GetString(16);
                    }
                    if (!iDataRecord.IsDBNull(17))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.nire = iDataRecord.GetString(17);
                    }
                    if (!iDataRecord.IsDBNull(20))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.nome = iDataRecord.GetString(20);
                    }
                    if (!iDataRecord.IsDBNull(4))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.municipio = iDataRecord.GetString(4);
                    }
                    if (!iDataRecord.IsDBNull(3))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.uf = iDataRecord.GetString(3);
                    }
                    if (!iDataRecord.IsDBNull(28))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.naturezaJuridicaCodigo = iDataRecord.GetString(28);
                    }
                    if (!iDataRecord.IsDBNull(29))
                    {
                        if (processo.pessoa == null)
                        {
                            processo.pessoa = new Pessoa();
                        }
                        processo.pessoa.naturezaJuridicaNome = iDataRecord.GetString(29);
                    }
                    if (!iDataRecord.IsDBNull(11))
                    {
                        if (processo.processosRelacionados == null)
                        {
                            processo.processosRelacionados = new List<ProcessoRelacionado>();
                        }
                        processo.processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.VIABILIDADE), processo.uf, iDataRecord.GetString(11)));
                    }
                    if (!iDataRecord.IsDBNull(12))
                    {
                        if (processo.processosRelacionados == null)
                        {
                            processo.processosRelacionados = new List<ProcessoRelacionado>();
                        }
                        processo.processosRelacionados.Add(new ProcessoRelacionado(TipoProtocolo.GetNome(TipoProtocolo.Tipos.DBE), processo.uf, iDataRecord.GetString(12)));
                    }
                    processos.Add(processo);
                }
                return processos;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaEventos(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT e.MER_COD_EVENTO AS EVENTO_CODIGO "); // 0
                query.Append("      ,e.MER_DSC_EVENTO AS EVENTO_DESCRICAO "); // 1
                query.Append("  FROM PSC_PROT_EVENTO_RFB p ");
                query.Append("  INNER JOIN MAC_EVENTOS_RFB e ON (p.PEV_COD_EVENTO = e.MER_COD_EVENTO) ");
                query.Append(" WHERE p.PEV_PRO_PROTOCOLO = :protocolo");
                query.Append(" ORDER BY p.PEV_PRO_PROTOCOLO ASC, e.MER_COD_EVENTO ASC");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1))
                    {
                        processo.eventos.Add(new Evento(iDataRecord.GetDecimal(0).ToString(), iDataRecord.GetString(1)));
                    }
                }
                return processo;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaEventosOrgaoRegistro(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT e.co_evento AS EVENTO_CODIGO "); // 0
                query.Append("      ,e.no_evento AS EVENTO_DESCRICAO "); // 1
                query.Append("  FROM PROCESSO p ");
                query.Append("  INNER JOIN SOLICITACAO s ON (s.nr_protocolo = p.nr_protocolo) ");
                query.Append("  INNER JOIN SOLICITACAO_EVENTO se ON (s.nr_protocolo = se.nr_protocolo AND s.sq_solicitacao = se.sq_solicitacao) ");
                query.Append("  INNER JOIN ATO a ON (s.co_ato = a.co_ato) ");
                query.Append("  INNER JOIN EVENTO e ON (se.co_evento = e.co_evento) ");
                query.Append(" WHERE p.nr_protocolo = :protocolo ");
                query.Append(" GROUP BY e.co_evento, e.no_evento ");
                query.Append(" ORDER BY e.co_evento ASC");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1))
                    {
                        processo.eventos.Add(new Evento(iDataRecord.GetString(0), iDataRecord.GetString(1)));
                    }
                }
                return processo;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaOrgaoRegistroAnalise(string uf, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            Int16 status;
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT i.tin_cnpj AS INSTITUICAO_CNPJ "); // 0
                query.Append("      ,i.tin_nom_inst AS INSTITUICAO_NOME "); // 1
                query.Append("      ,(SELECT p.in_status_processo FROM processo p WHERE p.nr_protocolo = '" + processo.numeroProtocolo + "') "); // 2
                query.Append("      ,(SELECT MAX(a1.dt_andamento) FROM ANDAMENTO a1 WHERE a1.nr_protocolo = '" + processo.numeroProtocolo + "' AND CONCAT(to_char(a1.si_secao_origem), to_char(a1.si_secao_destino)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) GROUP BY a1.nr_protocolo) "); // 3
                query.Append("  FROM TAB_INSTITUICAO i ");
                query.Append(" WHERE i.TIN_TIP_INSTITUICAO = 5 ");
                query.Append("   AND i.TIN_TMU_TUF_UF = :uf ");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "uf";
                iDbDataParameter.Value = uf;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1))
                    {
                        InstituicaoAnalise instituicaoAnalise = new InstituicaoAnalise
                        {
                            cnpj = iDataRecord.GetString(0),
                            nome = iDataRecord.GetString(1)
                        };
                        if (!iDataRecord.IsDBNull(2))
                        {
                            status = Int16.Parse(iDataRecord.GetString(2));
                        }
                        else
                        {
                            status = -1;
                        }
                        if (!iDataRecord.IsDBNull(3))
                        {
                            instituicaoAnalise.data = iDataRecord.GetDateTime(3).ToString("dd/MM/yyyy");
                        }

                        if (status.Equals(1) || status.Equals(2) || status.Equals(3) || status.Equals(6) || status.Equals(7) || status.Equals(8))
                        {
                            instituicaoAnalise.data = null;
                        }

                        instituicaoAnalise.andamentos = new List<Andamento>();
                        processo.instituicoesAnalise.Add(instituicaoAnalise);
                    }
                }
                return processo;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public InstituicaoAnalise PesquisaOrgaoRegistroAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            int sequencia = 1;
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT a.SQ_ANDAMENTO AS ANDAMENTO_SEQUENCIA "); // 0
                query.Append("      ,to_char(sOrigem.NO_SECAO) AS ANDAMENTO_ORIGEM "); // 1
                query.Append("      ,to_char(sDestino.NO_SECAO) AS ANDAMENTO_DESTINO "); // 2
                query.Append("      ,to_char(d.NO_DESPACHO) AS ANDAMENTO_DESPACHO "); // 3
                query.Append("      ,a.DT_ANDAMENTO AS ANDAMENTO_DATA "); // 4
                query.Append("      ,to_char(f.NO_FUNCIONARIO) AS ANDA_FUNCIONARIO "); // 5
                query.Append("      ,to_char(fAnalista.NO_FUNCIONARIO) AS ANDA_FUNCIONARIO_ANALISTA "); // 6
                query.Append("      ,to_char(f.SQ_FUNCIONARIO) AS SQ_ANDA_FUNCIONARIO "); // 7
                query.Append("      ,to_char(fAnalista.SQ_FUNCIONARIO) AS SQ_ANDA_FUNCIONARIO_ANALISTA "); // 8
                query.Append("  FROM ANDAMENTO a ");
                query.Append("  INNER JOIN DESPACHO d ON (a.CO_DESPACHO = d.CO_DESPACHO) ");
                query.Append("  INNER JOIN ESTRUTURA_ORGANIZACIONAL sOrigem ON (a.CO_JUNTA_COMERCIAL = sOrigem.CO_JUNTA_COMERCIAL AND a.CO_SEQUENCIAL = sOrigem.CO_SEQUENCIAL AND a.SI_SECAO_ORIGEM = sOrigem.SI_SECAO) ");
                query.Append("  INNER JOIN ESTRUTURA_ORGANIZACIONAL sDestino ON (a.CO_JUNTA_COMERCIAL = sDestino.CO_JUNTA_COMERCIAL AND a.CO_SEQUENCIAL = sDestino.CO_SEQUENCIAL AND a.SI_SECAO_DESTINO = sDestino.SI_SECAO) ");
                query.Append("  LEFT JOIN FUNCIONARIO f ON (a.SQ_FUNCIONARIO = f.SQ_FUNCIONARIO) ");
                query.Append("  LEFT JOIN FUNCIONARIO fAnalista ON (a.SQ_FUNCIONARIO_ANALISTA = fAnalista.SQ_FUNCIONARIO) ");
                query.Append(" WHERE a.NR_PROTOCOLO = :protocolo");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append(" AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
                }
                query.Append("  ORDER BY a.SQ_ANDAMENTO ASC");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1) && !iDataRecord.IsDBNull(2) && !iDataRecord.IsDBNull(3) && !iDataRecord.IsDBNull(4))
                    {
                        string area = iDataRecord.GetString(1).Trim() + " para " + iDataRecord.GetString(2).Trim();

                        Andamento andamento = new Andamento(sequencia.ToString(), area, iDataRecord.GetDateTime(4).ToString("dd/MM/yyyy"), iDataRecord.GetString(3), null, new List<Exigencia>());

                        if (!iDataRecord.IsDBNull(5))
                        {
                            andamento.nomeFuncionarioAndamento = iDataRecord.GetString(5);
                        }

                        if (!iDataRecord.IsDBNull(6))
                        {
                            andamento.nomeFuncionarioAnalise = iDataRecord.GetString(6);
                        }

                        if (!iDataRecord.IsDBNull(7))
                        {
                            andamento.idFuncionarioAndamento = iDataRecord.GetString(7);
                        }

                        if (!iDataRecord.IsDBNull(8))
                        {
                            andamento.idFuncionarioAnalise = iDataRecord.GetString(8);
                        }

                        andamento = PesquisaExigenciasAndamento(protocolo, andamento, iDbCommand, false);

                        instituicaoAnalise.andamentos.Add(andamento);
                    }
                    sequencia++;
                }
                return instituicaoAnalise;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaInstituicoesAnalise(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            IDataRecord iDataRecord;
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT i.TIN_CNPJ AS INSTITUICAO_CNPJ "); // 0
                query.Append("      ,i.TIN_NOM_INST AS INSTITUICAO_NOME "); // 1
                query.Append("      ,ps.PPE_DATE_FIM_PROCESSO AS INSTITUICAO_FINALIZACAO_DATA "); // 2
                query.Append("  FROM PSC_PROTOCOLO_STATUS ps ");
                query.Append("  INNER JOIN TAB_INSTITUICAO i ON (i.TIN_CNPJ = ps.PPE_TIG_TIN_CNPJ) ");
                query.Append(" WHERE ps.PPE_PRO_PROTOCOLO = :protocolo");
                query.Append(" UNION ");
                query.Append("SELECT DISTINCT a.PPI_TIG_TIN_CNPJ AS INSTITUICAO_CNPJ ");
                query.Append("      ,i.TIN_NOM_INST AS INSTITUICAO_NOME ");
                query.Append("      ,ps.PPE_DATE_FIM_PROCESSO AS INSTITUICAO_FINALIZACAO_DATA ");
                query.Append("  FROM PSC_PROTOCOLO_INSTITUICAO a ");
                query.Append("  INNER JOIN TAB_INSTITUICAO i ON (a.PPI_TIG_TIN_CNPJ = i.TIN_CNPJ) ");
                query.Append("  LEFT JOIN PSC_PROTOCOLO_STATUS ps ON (i.TIN_CNPJ = ps.PPE_TIG_TIN_CNPJ) ");
                query.Append("  WHERE a.PPI_PRO_PROTOCOLO = :protocolo");
                query.Append("    AND a.PPI_TIG_TIN_CNPJ NOT IN ");
                query.Append("               (SELECT i.TIN_CNPJ ");
                query.Append("                  FROM PSC_PROTOCOLO_STATUS ps ");
                query.Append("                  INNER JOIN TAB_INSTITUICAO i ON (i.TIN_CNPJ = ps.PPE_TIG_TIN_CNPJ) ");
                query.Append("                  WHERE ps.PPE_PRO_PROTOCOLO = :protocolo)");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1))
                    {
                        processo.instituicoesAnalise.Add(new InstituicaoAnalise(iDataRecord.GetString(0), iDataRecord.GetString(1), new List<Andamento>()));
                    }
                }
                return processo;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public InstituicaoAnalise PesquisaInstituicoesAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            int sequencia = 1;
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT ppi.PPI_PRO_PROTOCOLO AS PROTOCOLO "); // 0
                query.Append("      ,ppi.PPI_FEC_ENVIO AS ENVIO_DATA "); // 1
                query.Append("      ,ppi.PPI_FEC_RESPUESTA AS RESPOSTA_DATA "); // 2
                query.Append("      ,pkg_util.GetGenerica(ppi.PPI_TIG_TGE_TIP_TAB, ppi.PPI_TIG_TGE_COD_TIP_TAB) RESPOSTA_AREA "); // 3
                query.Append("      ,pkg_util.GetGenerica(12, ppi.PPI_STATUS_PESQUISA) RESPOSTA_STATUS "); // 4
                query.Append("      ,ppi.PPI_DESC_PESQUISA AS RESPOSTA_DESCRICAO "); // 5
                query.Append("      ,ppi.PPI_TIR_CPF_RESP AS RESPOSTA_RESPONSAVEL_CPF "); // 6
                query.Append("      ,tir.TIR_NOM_RESP AS RESPOSTA_RESPONSAVEL_NOME "); // 7
                query.Append("  FROM PSC_PROTOCOLO_INSTITUICAO ppi ");
                query.Append("  LEFT JOIN TAB_INST_RESPONSAVEL tir ON (ppi.PPI_TIR_CPF_RESP = tir.TIR_CPF_RESP) ");
                query.Append("   WHERE ppi.PPI_PRO_PROTOCOLO = :protocolo");
                query.Append("     AND ppi.PPI_TIG_TIN_CNPJ = :cnpjInstituicao");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "cnpjInstituicao";
                iDbDataParameter.Value = instituicaoAnalise.cnpj;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    string dataRetorno = null;
                    string descricao = null;
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(3) && !iDataRecord.IsDBNull(4))
                    {
                        if (!iDataRecord.IsDBNull(2))
                        {
                            dataRetorno = iDataRecord.GetDateTime(2).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            if (!iDataRecord.IsDBNull(1))
                            {
                                dataRetorno = iDataRecord.GetDateTime(1).ToString("dd/MM/yyyy");
                            }
                        }
                        if (!iDataRecord.IsDBNull(5))
                        {
                            descricao = iDataRecord.GetString(5);
                        }

                        instituicaoAnalise.andamentos.Add(new Andamento(sequencia.ToString(), iDataRecord.GetString(3), dataRetorno, iDataRecord.GetString(4), descricao, null));
                    }
                    sequencia++;
                }
                return instituicaoAnalise;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaExigenciasProcesso(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT e.CO_EXIGENCIA AS EXIGENCIA_CODIGO "); // 0
                query.Append("      ,to_char(e.NO_EXIGENCIA) AS EXIGENCIA_NOME "); // 1
                query.Append("      ,to_char(pe.VL_EXIGENCIA) AS EXIGENCIA_VALOR "); // 2
                query.Append("      ,to_char(pe.DS_PREENCHE_CAMPO) AS EXIGENCIA_DESCRICAO "); // 3
                query.Append("      ,a.DT_ANDAMENTO AS EXIGENCIA_DATA "); // 4
                query.Append("  FROM EXIGENCIA_DE_PROCESSO pe ");
                query.Append("  LEFT JOIN ANDAMENTO a ON (pe.NR_PROTOCOLO = a.NR_PROTOCOLO AND pe.SQ_ANDAMENTO = a.SQ_ANDAMENTO) ");
                query.Append("  LEFT JOIN EXIGENCIA e ON (pe.CO_EXIGENCIA = e.CO_EXIGENCIA) ");
                query.Append(" WHERE pe.NR_PROTOCOLO = :protocolo");
                query.Append("   AND pe.SQ_ANDAMENTO = :sequenciaAndamento");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append("   AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
                }
                query.Append(" ORDER BY a.DT_ANDAMENTO ASC");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "sequenciaAndamento";
                iDbDataParameter.Value = processo.processoSequenciaExigencia;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    string valor = null;
                    string descricao = null;
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1) && !iDataRecord.IsDBNull(4))
                    {
                        if (!iDataRecord.IsDBNull(2))
                        {
                            valor = iDataRecord.GetString(2);
                        }
                        if (!iDataRecord.IsDBNull(3))
                        {
                            descricao = iDataRecord.GetString(3);
                        }

                        Exigencia exigencia = new Exigencia(iDataRecord.GetString(0), iDataRecord.GetString(1), valor, descricao, iDataRecord.GetDateTime(4).ToString("dd/MM/yyyy"), new List<ExigenciaOutro>());

                        if (processo.processoSequenciaExigencia != null && ("3.3".Equals(exigencia.codigo) || (exigencia.descricao != null && exigencia.descricao.Contains("Outras"))))
                        {
                            exigencia = PesquisaExigenciasOutrosProcesso(protocolo, processo.processoSequenciaExigencia, exigencia, iDbCommand, false);
                        }

                        processo.exigencias.Add(exigencia);
                    }
                }
                return processo;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Andamento PesquisaExigenciasAndamento(string protocolo, Andamento andamento, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT e.CO_EXIGENCIA "); // 0
                query.Append("      ,e.NO_EXIGENCIA "); // 1
                query.Append("      ,pe.VL_EXIGENCIA "); // 2
                query.Append("      ,pe.DS_PREENCHE_CAMPO "); // 3
                query.Append("      ,a.DT_ANDAMENTO "); // 4
                query.Append("  FROM EXIGENCIA_DE_PROCESSO pe ");
                query.Append("  LEFT JOIN ANDAMENTO a ON (pe.NR_PROTOCOLO = a.NR_PROTOCOLO AND pe.SQ_ANDAMENTO = a.SQ_ANDAMENTO) ");
                query.Append("  LEFT JOIN EXIGENCIA e ON (pe.CO_EXIGENCIA = e.CO_EXIGENCIA) ");
                query.Append(" WHERE pe.NR_PROTOCOLO = :protocolo ");
                query.Append("   AND pe.SQ_ANDAMENTO = :sequenciaAndamento ");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append("   AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
                }
                query.Append(" ORDER BY a.DT_ANDAMENTO ASC");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "sequenciaAndamento";
                iDbDataParameter.Value = Convert.ToInt16(andamento.codigo);
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    string valor = null;
                    string descricao = null;
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1) && !iDataRecord.IsDBNull(4))
                    {
                        if (!iDataRecord.IsDBNull(2))
                        {
                            valor = iDataRecord.GetString(2);
                        }
                        if (!iDataRecord.IsDBNull(3))
                        {
                            descricao = iDataRecord.GetString(3);
                        }

                        Exigencia exigencia = new Exigencia(iDataRecord.GetString(0), iDataRecord.GetString(1), valor, descricao, iDataRecord.GetDateTime(4).ToString("dd/MM/yyyy"), new List<ExigenciaOutro>());

                        if (andamento.codigo != null && ("3.3".Equals(exigencia.codigo) || (exigencia.descricao != null && exigencia.descricao.Contains("Outras"))))
                        {
                            exigencia = PesquisaExigenciasOutrosProcesso(protocolo, andamento.codigo, exigencia, iDbCommand, false);
                        }

                        andamento.exigencias.Add(exigencia);
                    }
                }
                return andamento;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Exigencia PesquisaExigenciasOutrosProcesso(string protocolo, string sequenciaExigencia, Exigencia exigencia, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            IDataRecord iDataRecord;
            try
            {
                query.Append("SELECT oe.SQ_EXIGENCIA "); // 0
                query.Append("      ,oe.DS_EXIGENCIA "); // 1
                query.Append("      ,oe.DS_FUNDAMENTACAO "); // 2
                query.Append("  FROM OUTRA_EXIGENCIA oe ");
                query.Append(" WHERE oe.NR_PROTOCOLO = :protocolo ");
                query.Append("   AND oe.SQ_ANDAMENTO = :sequenciaAndamento ");

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDbDataParameter = iDbCommand.CreateParameter();
                iDbDataParameter.ParameterName = "sequenciaAndamento";
                iDbDataParameter.Value = sequenciaExigencia;
                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                while (iDataReader.Read())
                {
                    iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0) && !iDataRecord.IsDBNull(1) && !iDataRecord.IsDBNull(2))
                    {
                        exigencia.exigenciasOutros.Add(new ExigenciaOutro(iDataRecord.GetInt32(0).ToString(), iDataRecord.GetString(1), iDataRecord.GetString(2)));
                    }
                }
                return exigencia;
            }
            finally
            {
                if (iDataReader != null)
                {
                    iDataReader.Close();
                }
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }
    }


    /// <summary>
    /// Implementacao da interface ReginGeo para consultas no banco da junta comercial que tenha SGBD Oracle
    /// </summary>
    public class ReginSqlServerImplDao : IReginDao
    {
        public ReginSqlServerImplDao()
        {

        }

        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            return false;
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            return TipoProtocolo.Tipos.NAO_ENCONTRADO;
        }


        public List<Processo> PesquisaProtocoloRegin(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public List<Processo> PesquisaProtocoloOrgaoRegistro(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaEventos(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaEventosOrgaoRegistro(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaOrgaoRegistroAnalise(string uf, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public InstituicaoAnalise PesquisaOrgaoRegistroAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaInstituicoesAnalise(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public InstituicaoAnalise PesquisaInstituicoesAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaExigenciasProcesso(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }
        public Andamento PesquisaExigenciasAndamento(string protocolo, Andamento andamento, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Exigencia PesquisaExigenciasOutrosProcesso(string protocolo, string sequenciaExigencia, Exigencia exigencia, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }
    }
}
