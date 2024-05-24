using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using regin_app_movel.Constante;
using regin_app_movel.GeracaoXml;

namespace regin_app_movel.Dao
{
    /// <summary>
    /// Implementacao da interface ReginGeo para consultas no banco da junta comercial que tenha SGBD Oracle
    /// </summary>
    public class ReginOracleImplDao : IReginDao
    {
        private bool FiltrarAndamentos(IDbCommand iDbCommand, bool fechaConexao)
        {
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    if (iDataReader.Read())
                    {
                        int qtdRegistros = iDataReader.GetInt32(0);
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
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    if (iDataReader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT p.pro_protocolo ");
                query.Append(
                    "     , DECODE(p.pro_tip_operacao, 1, 3, 2, 3, 3, 5, 8, 8, 7, 7, -1) AS pro_tip_operacao ");
                query.Append("  FROM psc_protocolo p ");
                query.Append(" WHERE p.pro_protocolo = :protocolo ");

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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    if (iDataReader.Read())
                    {
                        return TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16((decimal)iDataReader[1]));
                    }
                    else
                    {
                        return TipoProtocolo.Tipos.NAO_ENCONTRADO;
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocoloInterno(string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT p.pro_protocolo ");
                query.Append(
                    "     , DECODE(p.pro_tip_operacao, 1, 3, 2, 3, 3, 5, 8, 8, 7, 7, -1) AS pro_tip_operacao ");
                query.Append("  FROM psc_protocolo p ");
                query.Append(
                    " WHERE p.pro_protocolo = :protocolo OR p.pro_protocolo = (select pip.pip_pro_protocolo from psc_ident_protocolo pip WHERE pip.pip_protocolo_junta = :protocolo)");

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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    if (iDataReader.Read())
                    {
                        return TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16((decimal)iDataReader[1]));
                    }
                    else
                    {
                        return TipoProtocolo.Tipos.NAO_ENCONTRADO;
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<Processo> PesquisaProtocoloRegin(string siglaUf, string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                List<Processo> processos = new List<Processo>();

                query.Append("SELECT PPR.PRO_PROTOCOLO AS PROTOCOLO "); // 0
                query.Append("      ,pkg_util.GetGenerica(10, PPR.PRO_STATUS) AS PROTOCOLO_STATUS "); // 1
                query.Append("      ,COALESCE(PPR.PRO_FEC_INC, sysdate) AS PROTOCOLO_DATA_INICIO "); // 2
                query.Append("      ,COALESCE(TIN.TIN_TMU_TUF_UF, '" + siglaUf + "') AS ORGAO_REGISTRO_UF "); // 3
                query.Append("      ,TMU.TMU_NOM_MUN AS PESSOA_MUNICIPIO "); // 4
                query.Append("      ,pkg_util.GetGenerica(9, PPR.PRO_TIP_OPERACAO) AS PROTOCOLO_TIPO_OPERACAO "); // 5
                query.Append(
                    "      ,DECODE(PPR.pro_tip_operacao, 1, 3, 2, 3, 3, 5, 8, 8, 7, 7, -1) AS PROTOCOLO_TIPO "); // 6
                query.Append("      ,PPR.PRO_DATA_CANCELAMENTO AS PROTOCOLO_CANCELAMENTO_DATA "); // 7
                query.Append("      ,to_char(PPR.PRO_MOTIVO) AS PROTOCOLO_CANCELAMENTO_MOTIVO "); // 8
                query.Append(
                    "      ,COALESCE(PPR.PRO_FEC_ATUALIZACAO, PPR.PRO_FEC_INC, sysdate) AS PROTOCOLO_DATA_ATUALIZACAO "); // 9
                query.Append(
                    "      ,pkg_util.GetGenerica(PPR.PRO_TGE_TGACAO, PPR.PRO_TGE_VGACAO) AS PROTOCOLO_ACAO "); // 10
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
                query.Append(
                    "      ,pkg_consulta_movel.consultaexigencia(PPR.PRO_PROTOCOLO) AS SEQUENCIA_ANDAMENTO_EXIGENCIA "); // 27
                query.Append("      ,COALESCE(PPR.PRO_STATUS, 0) AS CODIGO_STATUS"); // 28
                query.Append(
                    "      ,COALESCE(to_char(RCP.RCO_TNC_COD_NATUR), REPLACE(p.co_natureza_juridica, '-', '')) AS CODIGO_NATUREZA_JURIDICA "); // 29
                query.Append(
                    "      ,UPPER(pkg_util.GetGenerica(900, COALESCE(to_char(RCP.RCO_TNC_COD_NATUR), REPLACE(p.co_natureza_juridica, '-', '')))) AS DESCRICAO_NATUREZA_JURIDICA "); // 30
                query.Append(
                    "      ,COALESCE(UPPER((SELECT a1.no_ato FROM solicitacao s1 INNER JOIN ATO a1 ON (s1.co_ato = a1.co_ato) WHERE s1.nr_protocolo = PPR.PRO_PROTOCOLO AND s1.sq_solicitacao = 1)), '-') AS PROTOCOLO_ATO "); // 31
                query.Append("      ,RGE.RGE_RUC AS INSCRICAO_ESTADUAL ");
                query.Append("  FROM PSC_PROTOCOLO PPR ");
                query.Append("  LEFT JOIN PSC_IDENT_PROTOCOLO PIP ON (PPR.PRO_PROTOCOLO = PIP.PIP_PRO_PROTOCOLO) ");
                query.Append(
                    "  LEFT JOIN TAB_MUNIC TMU ON (PPR.PRO_TMU_TUF_UF = TMU.TMU_TUF_UF AND PPR.PRO_TMU_COD_MUN = TMU.TMU_COD_MUN) ");
                query.Append(
                    "  LEFT JOIN TAB_INSTITUICAO TIN ON (pkg_consulta_movel.LOOKUP_CNPJ_JUNTA_COMERCIAL() = TIN.TIN_CNPJ) ");
                query.Append("  LEFT JOIN RUC_COMP RCP ON (RCP.RCO_RGE_PRA_PROTOCOLO = PPR.PRO_PROTOCOLO) ");
                query.Append("  LEFT JOIN processo p ON (p.nr_protocolo = PPR.PRO_PROTOCOLO) ");
                query.Append(
                    "  LEFT JOIN processo_pessoa pp ON (pp.co_junta_comercial = p.co_junta_comercial AND pp.co_sequencial = p.co_sequencial AND pp.nr_protocolo = p.nr_protocolo) ");
                query.Append(
                    "  LEFT JOIN pessoa pe ON (pe.sq_pessoa = pp.sq_pessoa AND pe.co_junta_comercial = pp.co_junta_comercial AND pe.co_sequencial = pp.co_sequencial) ");
                query.Append(
                    "  LEFT JOIN pessoa_juridica pj ON (pj.sq_pessoa = pe.sq_pessoa AND pj.co_junta_comercial = pe.co_junta_comercial AND pj.co_sequencial = pe.co_sequencial) ");
                query.Append("  LEFT JOIN RUC_GENERAL RGE ON (PPR.PRO_PROTOCOLO = RGE.RGE_PRA_PROTOCOLO) ");
                query.Append(" WHERE (PPR.PRO_PROTOCOLO = :protocolo OR PIP.PIP_PROTOCOLO_JUNTA = :protocolo)");
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        Processo processo = new Processo();

                        if (!iDataReader.IsDBNull(0))
                        {
                            processo.NumeroProtocolo = iDataReader.GetString(0);
                            processo.ProtocoloInternoRegin = iDataReader.GetString(0);
                        }

                        if (!iDataReader.IsDBNull(6))
                        {
                            processo.Tipo =
                                TipoProtocolo.GetNome(
                                    TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16(iDataReader.GetDecimal(6))));
                        }

                        if (!iDataReader.IsDBNull(25))
                        {
                            processo.JuntaComercialProtocolo = iDataReader.GetString(25);
                        }

                        if (!iDataReader.IsDBNull(12))
                        {
                            processo.NumeroDbe = iDataReader.GetString(12);
                        }

                        if (!iDataReader.IsDBNull(14))
                        {
                            processo.OrgaoRegistroCnpj = iDataReader.GetString(14);
                        }

                        if (!iDataReader.IsDBNull(15))
                        {
                            processo.OrgaoRegistroNome = iDataReader.GetString(15);
                        }

                        if (!iDataReader.IsDBNull(18))
                        {
                            processo.PrefeituraAlvara = iDataReader.GetString(18);
                        }

                        if (!iDataReader.IsDBNull(22))
                        {
                            processo.PrefeituraIptu = iDataReader.GetString(22);
                        }

                        if (!iDataReader.IsDBNull(21))
                        {
                            processo.PrefeituraIss = iDataReader.GetString(21);
                        }

                        if (!iDataReader.IsDBNull(1))
                        {
                            processo.Status = iDataReader.GetString(1);
                        }

                        if (!iDataReader.IsDBNull(5))
                        {
                            processo.TipoOperacao = iDataReader.GetString(5);
                        }

                        if (!iDataReader.IsDBNull(3))
                        {
                            processo.Uf = iDataReader.GetString(3);
                        }

                        if (!iDataReader.IsDBNull(24))
                        {
                            processo.VigilanciaLicenca = iDataReader.GetString(24);
                        }

                        if (!iDataReader.IsDBNull(23))
                        {
                            processo.BombeiroLicenca = iDataReader.GetString(23);
                        }

                        if (!iDataReader.IsDBNull(13))
                        {
                            processo.Requerimento.NumeroProtocolo = iDataReader.GetString(13);
                        }

                        if (!iDataReader.IsDBNull(2))
                        {
                            processo.DataInicioProcesso = iDataReader.GetDateTime(2).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(9))
                        {
                            processo.DataAtualizacaoProcesso = iDataReader.GetDateTime(9).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(7))
                        {
                            processo.DataCancelamentoProcesso = iDataReader.GetDateTime(7).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(8))
                        {
                            processo.MotivoCancelamentoProcesso = iDataReader.GetString(8);
                        }

                        if (!iDataReader.IsDBNull(27))
                        {
                            processo.ProcessoSequenciaExigencia = iDataReader.GetString(27);
                        }

                        if (!iDataReader.IsDBNull(31))
                        {
                            processo.Ato = iDataReader.GetString(31);
                        }

                        processo.ProcessoExigencia = "0";
                        if (processo.ProcessoSequenciaExigencia != null)
                        {
                            processo.ProcessoExigencia = "1";
                        }

                        if (!iDataReader.IsDBNull(32))
                        {
                            processo.InscricaoEstadual = iDataReader.GetString(32);
                        }

                        if (!iDataReader.IsDBNull(16))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.CpfCnpj = iDataReader.GetString(16);
                        }

                        if (!iDataReader.IsDBNull(17))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Nire = iDataReader.GetString(17);
                        }

                        if (!iDataReader.IsDBNull(20))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Nome = iDataReader.GetString(20);
                        }

                        if (!iDataReader.IsDBNull(4))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Municipio = iDataReader.GetString(4);
                        }

                        if (!iDataReader.IsDBNull(3))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Uf = iDataReader.GetString(3);
                        }

                        if (!iDataReader.IsDBNull(29))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.NaturezaJuridicaCodigo = iDataReader.GetString(29);
                        }

                        if (!iDataReader.IsDBNull(30))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.NaturezaJuridicaNome = iDataReader.GetString(30);
                        }

                        if (!iDataReader.IsDBNull(11))
                        {
                            if (processo.ProcessosRelacionados == null)
                            {
                                processo.ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processo.ProcessosRelacionados.Add(new ProcessoRelacionado(
                                TipoProtocolo.GetNome(TipoProtocolo.Tipos.VIABILIDADE), processo.Uf,
                                iDataReader.GetString(11)));
                        }

                        if (!iDataReader.IsDBNull(12))
                        {
                            if (processo.ProcessosRelacionados == null)
                            {
                                processo.ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processo.ProcessosRelacionados.Add(new ProcessoRelacionado(
                                TipoProtocolo.GetNome(TipoProtocolo.Tipos.DBE), processo.Uf, iDataReader.GetString(12)));
                        }

                        if (!iDataReader.IsDBNull(28))
                        {
                            processo.CodigoStatus = iDataReader.GetInt32(28).ToString();
                        }

                        processos.Add(processo);
                    }
                }

                return processos;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<Processo> PesquisaProtocoloOrgaoRegistro(string siglaUf, string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                List<Processo> processos = new List<Processo>();

                query.Append("SELECT p.nr_protocolo AS PROTOCOLO "); // 0
                query.Append(
                    "      ,UPPER(DECODE(p.in_status_processo, 1, 'Em tramitação', 2, 'Em Exigência', 3, 'Deferido', 4, 'Indeferido', 5, 'Desistência', 6, 'Suspenso por ordem judicial', 7, 'Em condições de aporvação', 8, 'Recebido pelo arquivo', 9, 'Autenticado', 'Status não identificado')) AS PROTOCOLO_STATUS "); // 1
                query.Append(
                    "      ,(SELECT a.dt_andamento FROM andamento a WHERE a.co_junta_comercial = p.co_junta_comercial AND a.nr_protocolo = p.nr_protocolo AND a.sq_andamento = 1) AS PROTOCOLO_DATA_INICIO "); // 2
                query.Append("      ,COALESCE(u.si_uf, '" + siglaUf + "') AS ORGAO_REGISTRO_UF "); // 3
                query.Append("      ,m.no_municipio AS PESSOA_MUNICIPIO "); // 4
                query.Append(
                    "      ,COALESCE(UPPER((SELECT a1.no_ato FROM solicitacao s1 INNER JOIN ATO a1 ON (s1.co_ato = a1.co_ato) WHERE s1.nr_protocolo = p.nr_protocolo AND s1.sq_solicitacao = 1)), '-') AS PROTOCOLO_TIPO_OPERACAO "); // 5
                query.Append("      ,3 AS PROTOCOLO_TIPO "); // 6
                query.Append("      ,null AS PROTOCOLO_CANCELAMENTO_DATA "); // 7
                query.Append("      ,null AS PROTOCOLO_CANCELAMENTO_MOTIVO "); // 8
                query.Append(
                    "      ,COALESCE((SELECT MAX(a.dt_andamento) FROM andamento a WHERE a.co_junta_comercial = p.co_junta_comercial AND a.co_sequencial = p.co_sequencial AND a.nr_protocolo = p.nr_protocolo AND CONCAT(to_char(a.si_secao_origem), to_char(a.si_secao_destino)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) GROUP BY a.nr_protocolo), sysdate) AS PROTOCOLO_DATA_ATUALIZACAO "); // 9
                query.Append("      ,'N/A' AS PROTOCOLO_ACAO "); // 10
                query.Append("      ,null AS PROTOCOLO_VIABILIDADE "); // 11
                query.Append("      ,null AS PROTOCOLO_DBE "); // 12
                query.Append("      ,null AS PROTOCOLO_REQUERIMENTO "); // 13
                query.Append(
                    "      ,(SELECT un.nr_cgc FROM unidade un WHERE un.co_sequencial = '000') AS PROTOCOLO_ORGAO_REGISTRO_CNPJ "); // 14
                query.Append(
                    "      ,(SELECT un.no_unidade FROM unidade un WHERE un.co_sequencial = '000') AS PROTOCOLO_ORGAO_REGISTRO_NOME "); // 15
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
                query.Append(
                    "      ,pkg_consulta_movel.consultaexigencia(p.nr_protocolo) AS SEQUENCIA_ANDAMENTO_EXIGENCIA "); // 27
                query.Append("      ,nj.co_natureza_juridica AS CODIGO_NATURZA_JURIDICA "); // 28
                query.Append("      ,nj.no_natureza_juridica AS DESCRICAO_NATURZA_JURIDICA "); // 29
                query.Append("FROM PROCESSO p ");
                query.Append("LEFT JOIN uf u ON (u.co_uf = p.co_junta_comercial) ");
                query.Append(
                    "LEFT JOIN processo_pessoa pp ON (pp.co_junta_comercial = p.co_junta_comercial AND pp.co_sequencial = p.co_sequencial AND pp.nr_protocolo = p.nr_protocolo) ");
                query.Append(
                    "LEFT JOIN pessoa pe ON (pe.sq_pessoa = pp.sq_pessoa AND pe.co_junta_comercial = pp.co_junta_comercial AND pe.co_sequencial = pp.co_sequencial) ");
                query.Append(
                    "LEFT JOIN pessoa_juridica pj ON (pj.sq_pessoa = pe.sq_pessoa AND pj.co_junta_comercial = pe.co_junta_comercial AND pj.co_sequencial = pe.co_sequencial) ");
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        Processo processo = new Processo();

                        if (!iDataReader.IsDBNull(0))
                        {
                            processo.NumeroProtocolo = iDataReader.GetString(0);
                        }

                        if (!iDataReader.IsDBNull(6))
                        {
                            processo.Tipo =
                                TipoProtocolo.GetNome(
                                    TipoProtocolo.GetTipoPorCodigo(Decimal.ToInt16(iDataReader.GetDecimal(6))));
                        }

                        if (!iDataReader.IsDBNull(25))
                        {
                            processo.JuntaComercialProtocolo = iDataReader.GetString(25);
                        }

                        if (!iDataReader.IsDBNull(12))
                        {
                            processo.NumeroDbe = iDataReader.GetString(12);
                        }

                        if (!iDataReader.IsDBNull(14))
                        {
                            processo.OrgaoRegistroCnpj = iDataReader.GetString(14);
                        }

                        if (!iDataReader.IsDBNull(15))
                        {
                            processo.OrgaoRegistroNome = iDataReader.GetString(15);
                        }

                        if (!iDataReader.IsDBNull(18))
                        {
                            processo.PrefeituraAlvara = iDataReader.GetString(18);
                        }

                        if (!iDataReader.IsDBNull(22))
                        {
                            processo.PrefeituraIptu = iDataReader.GetString(22);
                        }

                        if (!iDataReader.IsDBNull(21))
                        {
                            processo.PrefeituraIss = iDataReader.GetString(21);
                        }

                        if (!iDataReader.IsDBNull(1))
                        {
                            processo.Status = iDataReader.GetString(1);
                        }

                        if (!iDataReader.IsDBNull(5))
                        {
                            processo.TipoOperacao = iDataReader.GetString(5);
                            processo.Ato = iDataReader.GetString(5);
                        }

                        if (!iDataReader.IsDBNull(3))
                        {
                            processo.Uf = iDataReader.GetString(3);
                        }

                        if (!iDataReader.IsDBNull(24))
                        {
                            processo.VigilanciaLicenca = iDataReader.GetString(24);
                        }

                        if (!iDataReader.IsDBNull(23))
                        {
                            processo.BombeiroLicenca = iDataReader.GetString(23);
                        }

                        if (!iDataReader.IsDBNull(13))
                        {
                            processo.Requerimento.NumeroProtocolo = iDataReader.GetString(13);
                        }

                        if (!iDataReader.IsDBNull(2))
                        {
                            processo.DataInicioProcesso = iDataReader.GetDateTime(2).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(9))
                        {
                            processo.DataAtualizacaoProcesso = iDataReader.GetDateTime(9).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(7))
                        {
                            processo.DataCancelamentoProcesso = iDataReader.GetDateTime(7).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(8))
                        {
                            processo.MotivoCancelamentoProcesso = iDataReader.GetString(8);
                        }

                        if (!iDataReader.IsDBNull(27))
                        {
                            processo.ProcessoSequenciaExigencia = iDataReader.GetString(27);
                        }

                        processo.ProcessoExigencia = "0";
                        if (processo.ProcessoSequenciaExigencia != null)
                        {
                            processo.ProcessoExigencia = "1";
                        }

                        if (!iDataReader.IsDBNull(16))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.CpfCnpj = iDataReader.GetString(16);
                        }

                        if (!iDataReader.IsDBNull(17))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Nire = iDataReader.GetString(17);
                        }

                        if (!iDataReader.IsDBNull(20))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Nome = iDataReader.GetString(20);
                        }

                        if (!iDataReader.IsDBNull(4))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Municipio = iDataReader.GetString(4);
                        }

                        if (!iDataReader.IsDBNull(3))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.Uf = iDataReader.GetString(3);
                        }

                        if (!iDataReader.IsDBNull(28))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.NaturezaJuridicaCodigo = iDataReader.GetString(28);
                        }

                        if (!iDataReader.IsDBNull(29))
                        {
                            if (processo.Pessoa == null)
                            {
                                processo.Pessoa = new Pessoa();
                            }

                            processo.Pessoa.NaturezaJuridicaNome = iDataReader.GetString(29);
                        }

                        if (!iDataReader.IsDBNull(11))
                        {
                            if (processo.ProcessosRelacionados == null)
                            {
                                processo.ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processo.ProcessosRelacionados.Add(new ProcessoRelacionado(
                                TipoProtocolo.GetNome(TipoProtocolo.Tipos.VIABILIDADE), processo.Uf,
                                iDataReader.GetString(11)));
                        }

                        if (!iDataReader.IsDBNull(12))
                        {
                            if (processo.ProcessosRelacionados == null)
                            {
                                processo.ProcessosRelacionados = new List<ProcessoRelacionado>();
                            }

                            processo.ProcessosRelacionados.Add(new ProcessoRelacionado(
                                TipoProtocolo.GetNome(TipoProtocolo.Tipos.DBE), processo.Uf, iDataReader.GetString(12)));
                        }

                        processos.Add(processo);
                    }
                }

                return processos;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaEventos(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1))
                        {
                            processo.Eventos.Add(new Evento(iDataReader.GetDecimal(0).ToString(CultureInfo.CurrentCulture),
                                iDataReader.GetString(1)));
                        }
                    }
                }

                return processo;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaEventosOrgaoRegistro(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT e.co_evento AS EVENTO_CODIGO "); // 0
                query.Append("      ,e.no_evento AS EVENTO_DESCRICAO "); // 1
                query.Append("  FROM PROCESSO p ");
                query.Append("  INNER JOIN SOLICITACAO s ON (s.nr_protocolo = p.nr_protocolo) ");
                query.Append(
                    "  INNER JOIN SOLICITACAO_EVENTO se ON (s.nr_protocolo = se.nr_protocolo AND s.sq_solicitacao = se.sq_solicitacao) ");
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1))
                        {
                            processo.Eventos.Add(new Evento(iDataReader.GetString(0), iDataReader.GetString(1)));
                        }
                    }
                }

                return processo;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaOrgaoRegistroAnalise(string uf, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT i.tin_cnpj AS INSTITUICAO_CNPJ "); // 0
                query.Append("      ,i.tin_nom_inst AS INSTITUICAO_NOME "); // 1
                query.Append("      ,(SELECT p.in_status_processo FROM processo p WHERE p.nr_protocolo = '" +
                             processo.NumeroProtocolo + "') "); // 2
                query.Append("      ,(SELECT MAX(a1.dt_andamento) FROM ANDAMENTO a1 WHERE a1.nr_protocolo = '" +
                             processo.NumeroProtocolo +
                             "' AND CONCAT(to_char(a1.si_secao_origem), to_char(a1.si_secao_destino)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) GROUP BY a1.nr_protocolo) "); // 3
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1))
                        {
                            InstituicaoAnalise instituicaoAnalise = new InstituicaoAnalise
                            {
                                Cnpj = iDataReader.GetString(0),
                                Nome = iDataReader.GetString(1)
                            };
                            Int16 status;
                            if (!iDataReader.IsDBNull(2))
                            {
                                status = Int16.Parse(iDataReader.GetString(2));
                            }
                            else
                            {
                                status = -1;
                            }

                            if (!iDataReader.IsDBNull(3))
                            {
                                instituicaoAnalise.Data = iDataReader.GetDateTime(3).ToString("dd/MM/yyyy");
                            }

                            if (status.Equals(1) || status.Equals(2) || status.Equals(3) || status.Equals(6) ||
                                status.Equals(7) || status.Equals(8))
                            {
                                instituicaoAnalise.Data = null;
                            }

                            instituicaoAnalise.Andamentos = new List<Andamento>();
                            processo.InstituicoesAnalise.Add(instituicaoAnalise);
                        }
                    }
                }

                return processo;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public InstituicaoAnalise PesquisaOrgaoRegistroAndamento(string protocolo,
            InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            int sequencia = 1;
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
                query.Append(
                    "  INNER JOIN ESTRUTURA_ORGANIZACIONAL sOrigem ON (a.CO_JUNTA_COMERCIAL = sOrigem.CO_JUNTA_COMERCIAL AND a.CO_SEQUENCIAL = sOrigem.CO_SEQUENCIAL AND a.SI_SECAO_ORIGEM = sOrigem.SI_SECAO) ");
                query.Append(
                    "  INNER JOIN ESTRUTURA_ORGANIZACIONAL sDestino ON (a.CO_JUNTA_COMERCIAL = sDestino.CO_JUNTA_COMERCIAL AND a.CO_SEQUENCIAL = sDestino.CO_SEQUENCIAL AND a.SI_SECAO_DESTINO = sDestino.SI_SECAO) ");
                query.Append("  LEFT JOIN FUNCIONARIO f ON (a.SQ_FUNCIONARIO = f.SQ_FUNCIONARIO) ");
                query.Append(
                    "  LEFT JOIN FUNCIONARIO fAnalista ON (a.SQ_FUNCIONARIO_ANALISTA = fAnalista.SQ_FUNCIONARIO) ");
                query.Append(" WHERE a.NR_PROTOCOLO = :protocolo");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append(
                        " AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1) && !iDataReader.IsDBNull(2) &&
                            !iDataReader.IsDBNull(3) && !iDataReader.IsDBNull(4))
                        {
                            string area = iDataReader.GetString(1).Trim() + " para " + iDataReader.GetString(2).Trim();

                            Andamento andamento = new Andamento(sequencia.ToString(), area,
                                iDataReader.GetDateTime(4).ToString("dd/MM/yyyy"), iDataReader.GetString(3), null,
                                new List<Exigencia>());

                            if (!iDataReader.IsDBNull(5))
                            {
                                andamento.NomeFuncionarioAndamento = iDataReader.GetString(5);
                            }

                            if (!iDataReader.IsDBNull(6))
                            {
                                andamento.NomeFuncionarioAnalise = iDataReader.GetString(6);
                            }

                            if (!iDataReader.IsDBNull(7))
                            {
                                andamento.IdFuncionarioAndamento = iDataReader.GetString(7);
                            }

                            if (!iDataReader.IsDBNull(8))
                            {
                                andamento.IdFuncionarioAnalise = iDataReader.GetString(8);
                            }

                            andamento = PesquisaExigenciasAndamento(protocolo, andamento, iDbCommand, false);

                            instituicaoAnalise.Andamentos.Add(andamento);
                        }

                        sequencia++;
                    }
                }

                return instituicaoAnalise;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaInstituicoesAnalise(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1))
                        {
                            processo.InstituicoesAnalise.Add(new InstituicaoAnalise(iDataReader.GetString(0),
                                iDataReader.GetString(1), new List<Andamento>()));
                        }
                    }
                }

                return processo;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public InstituicaoAnalise PesquisaInstituicoesAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise,
            IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            int sequencia = 1;
            try
            {
                query.Append("SELECT ppi.PPI_PRO_PROTOCOLO AS PROTOCOLO "); // 0
                query.Append("      ,ppi.PPI_FEC_ENVIO AS ENVIO_DATA "); // 1
                query.Append("      ,ppi.PPI_FEC_RESPUESTA AS RESPOSTA_DATA "); // 2
                query.Append(
                    "      ,pkg_util.GetGenerica(ppi.PPI_TIG_TGE_TIP_TAB, ppi.PPI_TIG_TGE_COD_TIP_TAB) RESPOSTA_AREA "); // 3
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
                iDbDataParameter.Value = instituicaoAnalise.Cnpj;
                iDbCommand.Parameters.Add(iDbDataParameter);

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        string dataRetorno = null;
                        string descricao = null;
                        if (!iDataReader.IsDBNull(3) && !iDataReader.IsDBNull(4))
                        {
                            if (!iDataReader.IsDBNull(2))
                            {
                                dataRetorno = iDataReader.GetDateTime(2).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                if (!iDataReader.IsDBNull(1))
                                {
                                    dataRetorno = iDataReader.GetDateTime(1).ToString("dd/MM/yyyy");
                                }
                            }

                            if (!iDataReader.IsDBNull(5))
                            {
                                descricao = iDataReader.GetString(5);
                            }

                            instituicaoAnalise.Andamentos.Add(new Andamento(sequencia.ToString(), iDataReader.GetString(3),
                                dataRetorno, iDataReader.GetString(4), descricao, null));
                        }

                        sequencia++;
                    }
                }

                return instituicaoAnalise;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Processo PesquisaExigenciasProcesso(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT e.CO_EXIGENCIA AS EXIGENCIA_CODIGO "); // 0
                query.Append("      ,to_char(e.NO_EXIGENCIA) AS EXIGENCIA_NOME "); // 1
                query.Append("      ,to_char(pe.VL_EXIGENCIA) AS EXIGENCIA_VALOR "); // 2
                query.Append("      ,to_char(pe.DS_PREENCHE_CAMPO) AS EXIGENCIA_DESCRICAO "); // 3
                query.Append("      ,a.DT_ANDAMENTO AS EXIGENCIA_DATA "); // 4
                query.Append("  FROM EXIGENCIA_DE_PROCESSO pe ");
                query.Append(
                    "  LEFT JOIN ANDAMENTO a ON (pe.NR_PROTOCOLO = a.NR_PROTOCOLO AND pe.SQ_ANDAMENTO = a.SQ_ANDAMENTO) ");
                query.Append("  LEFT JOIN EXIGENCIA e ON (pe.CO_EXIGENCIA = e.CO_EXIGENCIA) ");
                query.Append(" WHERE pe.NR_PROTOCOLO = :protocolo");
                query.Append("   AND pe.SQ_ANDAMENTO = :sequenciaAndamento");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append(
                        "   AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
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
                iDbDataParameter.Value = processo.ProcessoSequenciaExigencia;
                iDbCommand.Parameters.Add(iDbDataParameter);

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        string valor = null;
                        string descricao = null;
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1) && !iDataReader.IsDBNull(4))
                        {
                            if (!iDataReader.IsDBNull(2))
                            {
                                valor = iDataReader.GetString(2);
                            }

                            if (!iDataReader.IsDBNull(3))
                            {
                                descricao = iDataReader.GetString(3);
                            }

                            Exigencia exigencia = new Exigencia(iDataReader.GetString(0), iDataReader.GetString(1), valor,
                                descricao, iDataReader.GetDateTime(4).ToString("dd/MM/yyyy"), new List<ExigenciaOutro>());

                            if (processo.ProcessoSequenciaExigencia != null && ("3.3".Equals(exigencia.Codigo) ||
                                                                                (exigencia.Descricao != null &&
                                                                                 exigencia.Descricao.Contains("Outras"))))
                            {
                                exigencia = PesquisaExigenciasOutrosProcesso(protocolo, processo.ProcessoSequenciaExigencia,
                                    exigencia, iDbCommand, false);
                            }

                            processo.Exigencias.Add(exigencia);
                        }
                    }
                }

                return processo;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Andamento PesquisaExigenciasAndamento(string protocolo, Andamento andamento, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT e.CO_EXIGENCIA "); // 0
                query.Append("      ,e.NO_EXIGENCIA "); // 1
                query.Append("      ,pe.VL_EXIGENCIA "); // 2
                query.Append("      ,pe.DS_PREENCHE_CAMPO "); // 3
                query.Append("      ,a.DT_ANDAMENTO "); // 4
                query.Append("  FROM EXIGENCIA_DE_PROCESSO pe ");
                query.Append(
                    "  LEFT JOIN ANDAMENTO a ON (pe.NR_PROTOCOLO = a.NR_PROTOCOLO AND pe.SQ_ANDAMENTO = a.SQ_ANDAMENTO) ");
                query.Append("  LEFT JOIN EXIGENCIA e ON (pe.CO_EXIGENCIA = e.CO_EXIGENCIA) ");
                query.Append(" WHERE pe.NR_PROTOCOLO = :protocolo ");
                query.Append("   AND pe.SQ_ANDAMENTO = :sequenciaAndamento ");
                if (FiltrarAndamentos(iDbCommand, false))
                {
                    query.Append(
                        "   AND CONCAT(to_char(a.SI_SECAO_ORIGEM), to_char(a.SI_SECAO_DESTINO)) IN (SELECT CONCAT(t.sfa_secao_origem, t.sfa_secao_destino) FROM SWM_FILTRO_ANDAMENTO t) ");
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
                iDbDataParameter.Value = Convert.ToInt16(andamento.Codigo);
                iDbCommand.Parameters.Add(iDbDataParameter);

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        string valor = null;
                        string descricao = null;
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1) && !iDataReader.IsDBNull(4))
                        {
                            if (!iDataReader.IsDBNull(2))
                            {
                                valor = iDataReader.GetString(2);
                            }

                            if (!iDataReader.IsDBNull(3))
                            {
                                descricao = iDataReader.GetString(3);
                            }

                            Exigencia exigencia = new Exigencia(iDataReader.GetString(0), iDataReader.GetString(1), valor,
                                descricao, iDataReader.GetDateTime(4).ToString("dd/MM/yyyy"), new List<ExigenciaOutro>());

                            if (andamento.Codigo != null && ("3.3".Equals(exigencia.Codigo) ||
                                                             (exigencia.Descricao != null &&
                                                              exigencia.Descricao.Contains("Outras"))))
                            {
                                exigencia = PesquisaExigenciasOutrosProcesso(protocolo, andamento.Codigo, exigencia,
                                    iDbCommand, false);
                            }

                            andamento.Exigencias.Add(exigencia);
                        }
                    }
                }

                return andamento;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public Exigencia PesquisaExigenciasOutrosProcesso(string protocolo, string sequenciaExigencia,
            Exigencia exigencia, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query = new StringBuilder();
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

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        if (!iDataReader.IsDBNull(0) && !iDataReader.IsDBNull(1) && !iDataReader.IsDBNull(2))
                        {
                            exigencia.ExigenciasOutros.Add(new ExigenciaOutro(iDataReader.GetInt32(0).ToString(),
                                iDataReader.GetString(1), iDataReader.GetString(2)));
                        }
                    }
                }

                return exigencia;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<ResumoProcesso> ConsultarViabilidadePorSolicitante(string cpf, string protocolo, string dataModificacao,
            IDbCommand iDbCommand, bool fechaConexao)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            try
            {
                string query = @"
                                SELECT DISTINCT
                                    p.PRO_PROTOCOLO NumeroProtocolo,                                    
                                    (SELECT pi.pip_cnpj
                                       FROM PSC_IDENT_PROTOCOLO pi
                                      WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO
                                        AND rownum = 1) PessoaCnpj,
                                    (SELECT pi.pip_nome_razao_social
                                       FROM PSC_IDENT_PROTOCOLO pi
                                      WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO
                                       AND rownum = 1) PessoaNome,
                                    v.vpv_vsv_cpf_cnpj_solic cpfCnpjSolicitante,
                                    v.vpv_nom_solic nomeSolicitante,
                                    p.PRO_FEC_INC DataCriacao,
                                    p.PRO_FEC_ATUALIZACAO DataAlteracao,
                                    p.PRO_STATUS StatusCodigo,
                                    (SELECT g.TGE_NOMB_DESC
                                       FROM TAB_GENERICA g
                                      WHERE g.TGE_TIP_TAB = 10
                                        AND p.PRO_STATUS = g.TGE_COD_TIP_TAB
                                        AND rownum = 1) StatusDescricao,
                                    pj.pev_cod_evento co_ato,
                                    (SELECT a.mer_dsc_evento
                                       FROM mac_eventos_rfb a
                                      WHERE pj.pev_pro_protocolo = p.PRO_PROTOCOLO
                                        AND a.mer_cod_evento = pj.pev_cod_evento
                                        AND rownum = 1) ds_ato,
                                    p.PRO_TIP_OPERACAO tp_operacao
                                FROM VIA_PROTOCOLO_VIAB v
                          INNER JOIN PSC_PROTOCOLO p ON p.PRO_PROTOCOLO = v.VPV_COD_PROTOCOLO 
                           LEFT JOIN psc_prot_evento_rfb pj ON pj.pev_pro_protocolo =  v.VPV_COD_PROTOCOLO                                 
                               WHERE v.VPV_VSV_CPF_CNPJ_SOLIC = :cpf
                ";

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query += " AND p.NR_PROTOCOLO = :protocolo";
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query += " AND COALESCE(p.PRO_FEC_ATUALIZACAO, p.PRO_FEC_INC) = :dataModificacao";
                }

                //query += " ORDER BY p.PRO_PROTOCOLO";

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter parametroCpf = iDbCommand.CreateParameter();
                parametroCpf.ParameterName = "cpf";
                parametroCpf.Value = cpf;
                iDbCommand.Parameters.Add(parametroCpf);

                if (!string.IsNullOrEmpty(protocolo))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "protocolo";
                    parametro.Value = protocolo;
                    iDbCommand.Parameters.Add(parametro);
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataModificacao";
                    parametro.Value = dataModificacao;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["cpfCnpjSolicitante"] != DBNull.Value ? iDataReader["cpfCnpjSolicitante"].ToString() : "";
                        resumoProcesso.SolicitanteNome = iDataReader["nomeSolicitante"] != DBNull.Value ? iDataReader["nomeSolicitante"].ToString() : "";
                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                      
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.TipoProtocolo = TipoProtocolo.GetTipoPorCodigo(iDataReader["tp_operacao"] != DBNull.Value ? Int32.Parse(iDataReader["tp_operacao"].ToString()) : 3);
                        resumoProcesso.origem = OrigemProcesso.REGIN;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarViabilidadePorSolicitante";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }
        public List<ResumoProcesso> ConsultarViabilidadePorSocios(string cpf, string protocolo, string dataModificacao,
            IDbCommand iDbCommand, bool fechaConexao)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();

            try
            {
                string query = @"    SELECT distinct
                                            p.PRO_PROTOCOLO NumeroProtocolo,                                           
                                            (SELECT pi.pip_cnpj
                                               FROM PSC_IDENT_PROTOCOLO pi
                                              WHERE pi.pip_pro_protocolo =  p.PRO_PROTOCOLO) PessoaCnpj,
                                            (SELECT pi.pip_nome_razao_social
                                               FROM PSC_IDENT_PROTOCOLO pi
                                              WHERE pi.pip_pro_protocolo =  p.PRO_PROTOCOLO) PessoaNome,
                                            v.vpv_vsv_cpf_cnpj_solic cpfCnpjSolicitante,
                                            v.vpv_nom_solic nomeSolicitante,
                                            p.PRO_FEC_INC DataCriacao,
                                            p.PRO_FEC_ATUALIZACAO DataAlteracao,
                                            p.PRO_STATUS StatusCodigo,
                                            (SELECT g.TGE_NOMB_DESC
                                               FROM TAB_GENERICA g
                                              WHERE g.TGE_TIP_TAB = 10
                                                AND p.PRO_STATUS = g.TGE_COD_TIP_TAB) StatusDescricao,
                                            pj.pev_cod_evento co_ato,
                                            (SELECT a.mer_dsc_evento
                                               FROM mac_eventos_rfb a
                                              WHERE pj.pev_pro_protocolo = p.PRO_PROTOCOLO
                                                AND a.mer_cod_evento = pj.pev_cod_evento
                                                AND rownum = 1) ds_ato,
                                            p.PRO_TIP_OPERACAO tp_operacao
                                       FROM VIA_PROTOCOLO_VIAB v
                                 INNER JOIN PSC_PROTOCOLO p ON p.PRO_PROTOCOLO = v.VPV_COD_PROTOCOLO 
                                 INNER JOIN VIA_PROT_SOCIOS ps ON ps.vpv_cod_protocolo = p.PRO_PROTOCOLO
                                  LEFT JOIN psc_prot_evento_rfb pj ON pj.pev_pro_protocolo =  p.PRO_PROTOCOLO                                   
                                      WHERE ps.VPS_CPF_CNPJ_SOCIO = :cpf
                ";


                if (!string.IsNullOrEmpty(protocolo))
                {
                    query += " AND p.NR_PROTOCOLO = :protocolo ";
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query += " AND COALESCE(p.PRO_FEC_ATUALIZACAO, p.PRO_FEC_INC) = :dataModificacao";
                }

                //query += " ORDER BY p.PRO_PROTOCOLO";

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter parametroCpf = iDbCommand.CreateParameter();
                parametroCpf.ParameterName = "cpf";
                parametroCpf.Value = cpf;
                iDbCommand.Parameters.Add(parametroCpf);

                if (!string.IsNullOrEmpty(protocolo))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "protocolo";
                    parametro.Value = protocolo;
                    iDbCommand.Parameters.Add(parametro);
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataModificacao";
                    parametro.Value = dataModificacao;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["cpfCnpjSolicitante"] != DBNull.Value ? iDataReader["cpfCnpjSolicitante"].ToString() : "";
                        resumoProcesso.SolicitanteNome = iDataReader["nomeSolicitante"] != DBNull.Value ? iDataReader["nomeSolicitante"].ToString() : "";
                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.TipoProtocolo = TipoProtocolo.GetTipoPorCodigo(iDataReader["tp_operacao"] != DBNull.Value ? Int32.Parse(iDataReader["tp_operacao"].ToString()) : 3);
                        resumoProcesso.origem = OrigemProcesso.REGIN;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarViabilidadePorSocios";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }
        public List<ResumoProcesso> ConsultarProtocoloPorSocios(string cpf, string protocolo, string dataModificacao,
           IDbCommand iDbCommand, bool fechaConexao)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();

            try
            {
                string query = @"
                                    SELECT p.PRO_PROTOCOLO NumeroProtocolo,
                                           (SELECT pi.pip_cnpj
                                              FROM PSC_IDENT_PROTOCOLO pi
                                             WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO) PessoaCnpj,
                                           (SELECT pi.pip_nome_razao_social
                                              FROM PSC_IDENT_PROTOCOLO pi
                                             WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO) PessoaNome,
                                           p.PRO_FEC_INC DataCriacao,
                                           p.PRO_FEC_ATUALIZACAO DataAlteracao,
                                           p.PRO_STATUS StatusCodigo,
                                           (SELECT g.TGE_NOMB_DESC
                                              FROM TAB_GENERICA g
                                             WHERE g.TGE_TIP_TAB = 10
                                               AND p.PRO_STATUS = g.TGE_COD_TIP_TAB) StatusDescricao,
                                           (SELECT pj.co_ato
                                              FROM solicitacao pj
                                             WHERE pj.nr_protocolo =  p.PRO_PROTOCOLO) co_ato,
                                           (SELECT a.no_ato
                                              FROM ato a, solicitacao pj
                                             WHERE pj.nr_protocolo =  p.PRO_PROTOCOLO AND a.co_ato = pj.co_ato) ds_ato,
                                           p.PRO_TIP_OPERACAO tp_operacao
                                      FROM PSC_PROTOCOLO p
                                INNER JOIN RUC_RELAT_PROF rp ON (rp.RRP_RGE_PRA_PROTOCOLO = p.PRO_PROTOCOLO)                                   
                                     WHERE rp.rrp_cgc_cpf_secd = :cpf
                ";


                if (!string.IsNullOrEmpty(protocolo))
                {
                    query += " AND p.NR_PROTOCOLO = :protocolo ";
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query += " AND COALESCE(p.PRO_FEC_ATUALIZACAO, p.PRO_FEC_INC) = :dataModificacao";
                }

                //query += " ORDER BY p.PRO_PROTOCOLO";

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter parametroCpf = iDbCommand.CreateParameter();
                parametroCpf.ParameterName = "cpf";
                parametroCpf.Value = cpf;
                iDbCommand.Parameters.Add(parametroCpf);

                if (!string.IsNullOrEmpty(protocolo))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "protocolo";
                    parametro.Value = protocolo;
                    iDbCommand.Parameters.Add(parametro);
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataModificacao";
                    parametro.Value = dataModificacao;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.LEGALIZACAO;
                        resumoProcesso.origem = OrigemProcesso.REGIN;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProtocoloPorSocios";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }
        public List<ResumoProcesso> ConsultarProtocoloPorRepresentantes(string cpf, string protocolo, string dataModificacao,
            IDbCommand iDbCommand, bool fechaConexao)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
             new StringBuilder();
            try
            {
                string query = @"
                                    SELECT p.PRO_PROTOCOLO NumeroProtocolo,
                                           (SELECT pi.pip_cnpj
                                              FROM PSC_IDENT_PROTOCOLO pi
                                             WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO) PessoaCnpj,
                                           (SELECT pi.pip_nome_razao_social
                                             FROM PSC_IDENT_PROTOCOLO pi
                                            WHERE pi.pip_pro_protocolo = p.PRO_PROTOCOLO) PessoaNome,
                                           p.PRO_FEC_INC DataCriacao,
                                           p.PRO_FEC_ATUALIZACAO DataAlteracao,
                                           p.PRO_STATUS StatusCodigo,
                                           (SELECT g.TGE_NOMB_DESC
                                              FROM TAB_GENERICA g
                                             WHERE g.TGE_TIP_TAB = 10
                                               AND p.PRO_STATUS = g.TGE_COD_TIP_TAB) StatusDescricao,
                                           (SELECT pj.co_ato
                                              FROM solicitacao pj
                                             WHERE pj.nr_protocolo = p.PRO_PROTOCOLO) co_ato,
                                           (SELECT a.no_ato
                                              FROM ato a, solicitacao pj
                                             WHERE pj.nr_protocolo = p.PRO_PROTOCOLO
                                               AND a.co_ato = pj.co_ato) ds_ato,
                                           p.PRO_TIP_OPERACAO tp_operacao
                                      FROM PSC_PROTOCOLO p
                                 LEFT JOIN RUC_RELAT_PROF rp ON (rp.RRP_RGE_PRA_PROTOCOLO = p.PRO_PROTOCOLO)
                                 LEFT JOIN RUC_REPRESENTANTES r ON (r.RSR_PRA_PROTOCOLO = p.PRO_PROTOCOLO)                                           
                                     WHERE (rp.RRP_CGC_CPF_SECD = :cpf OR r.RSR_CGC_CPF_SECD = :cpf)
                ";

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query += " AND p.NR_PROTOCOLO = :protocolo ";
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query += " AND COALESCE(p.PRO_FEC_ATUALIZACAO, p.PRO_FEC_INC) = :dataModificacao";
                }

                 //query += " ORDER BY p.PRO_PROTOCOLO";

                if (!fechaConexao)
                {
                    iDbCommand.Parameters.Clear();
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter parametroCpf = iDbCommand.CreateParameter();
                parametroCpf.ParameterName = "cpf";
                parametroCpf.Value = cpf;
                iDbCommand.Parameters.Add(parametroCpf);

                if (!string.IsNullOrEmpty(protocolo))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "protocolo";
                    parametro.Value = protocolo;
                    iDbCommand.Parameters.Add(parametro);
                }

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataModificacao";
                    parametro.Value = dataModificacao;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.LEGALIZACAO;
                        resumoProcesso.origem = OrigemProcesso.REGIN;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProtocoloPorRepresentantes";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }
    }


    /// <summary>
    /// Implementacao da interface ReginGeo para consultas no banco da junta comercial que tenha SGBD Oracle
    /// </summary>
    public class ReginSqlServerImplDao : IReginDao
    {
        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            return false;
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            return TipoProtocolo.Tipos.NAO_ENCONTRADO;
        }

        public TipoProtocolo.Tipos PesquisaTipoProtocoloInterno(string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            return TipoProtocolo.Tipos.NAO_ENCONTRADO;
        }

        public List<Processo> PesquisaProtocoloRegin(string siglaUf, string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public List<Processo> PesquisaProtocoloOrgaoRegistro(string siglaUf, string protocolo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaEventos(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaEventosOrgaoRegistro(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaOrgaoRegistroAnalise(string uf, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public InstituicaoAnalise PesquisaOrgaoRegistroAndamento(string protocolo,
            InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaInstituicoesAnalise(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public InstituicaoAnalise PesquisaInstituicoesAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise,
            IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Processo PesquisaExigenciasProcesso(string protocolo, Processo processo, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Andamento PesquisaExigenciasAndamento(string protocolo, Andamento andamento, IDbCommand iDbCommand,
            bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public List<ResumoProcesso> ConsultarViabilidadePorSolicitante(string cpf, string protocolo, string dataModificacao, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }
        public List<ResumoProcesso> ConsultarProtocoloPorSocios(string cpf, string protocolo, string dataModificacao, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }
        public List<ResumoProcesso> ConsultarViabilidadePorSocios(string cpf, string protocolo, string dataModificacao, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public List<ResumoProcesso> ConsultarProtocoloPorRepresentantes(string cpf, string protocolo, string dataModificacao, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }

        public Exigencia PesquisaExigenciasOutrosProcesso(string protocolo, string sequenciaExigencia,
            Exigencia exigencia, IDbCommand iDbCommand, bool fechaConexao)
        {
            throw new NotImplementedException();
        }
    }
}