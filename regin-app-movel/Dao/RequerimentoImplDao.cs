using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using regin_app_movel.Constante;
using regin_app_movel.GeracaoXml;

namespace regin_app_movel.Dao
{
    /// <summary>
    /// 
    /// </summary>
    public class RequerimentoMySqlImplDao : IRequerimentoDao
    {
        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                        SELECT p.t005_nr_protocolo
                          FROM t005_protocolo p
                         WHERE p.T005_NR_PROTOCOLO = @protocolo
                ");

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

        public TipoProtocolo.Tipos ConsultaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                        SELECT p.t005_nr_protocolo 
                          FROM t005_protocolo p 
                         WHERE p.T005_NR_PROTOCOLO = @protocolo
                ");

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
                        return TipoProtocolo.Tipos.REQUERIMENTO;
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

        public Requerimento ConsultaProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            Requerimento requerimento = null;
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                        SELECT p.t005_NR_PROTOCOLO 
                              ,p.T005_NR_PROTOCOLO_RCPJ 
                              ,p.T005_DT_ENTRADA 
                              ,p.T005_DT_AVERBACAO 
                              ,p.T005_NR_PROTOCOLO_VIABILIDADE 
                              ,p.T005_NR_DBE 
                              ,p.T005_NR_PROTOCOLO_PREFEITURA 
                              ,p.T005_DATA_ASSINATURA 
                              ,p.T005_PROTOCOLO_ORGAO_ORIGEM 
                              ,p.T005_NR_PROTOCOLO_ENQUADRAMENTO 
                              ,IF(p.T005_IN_SITUACAO = 1, 'INVÁLIDO', 'VÁLIDO') AS T005_IN_SITUACAO 
                              ,IF(p.T005_IN_DBE_CARREGADO, 'NÃO', 'SIM') AS T005_IN_DBE_CARREGADO 
                              ,tp.T001_DS_PESSOA AS RAZAO_SOCIAL 
                              ,tpj.T003_NR_CNPJ AS CNPJ_EMPRESA 
                              ,tp1.T001_DS_PESSOA AS NOME_SOLICITANTE 
                              ,tor.T004_UF 
                              ,tpj.A006_CO_NATUREZA_JURIDICA AS CODIGO_NATUREZA_JURIDICA 
                              ,tnj.T009_DS_NATUREZA_JURIDICA AS DESCRICAO_NATUREZA_JURIDICA 
                         FROM t005_protocolo p 
                    LEFT JOIN t001_pessoa tp ON (p.T001_SQ_PESSOA = tp.T001_SQ_PESSOA) 
                    LEFT JOIN t003_pessoa_juridica tpj ON (tp.T001_SQ_PESSOA = tpj.T001_SQ_PESSOA) 
                    LEFT JOIN t009_natureza_juridica tnj ON (tpj.A006_CO_NATUREZA_JURIDICA = tnj.T009_CO_NATUREZA_JURIDICA) 
                    LEFT JOIN r001_vinculo rv ON (tp.T001_SQ_PESSOA = rv.T001_SQ_PESSOA_PAI AND rv.A009_CO_CONDICAO = 500) 
                    LEFT JOIN t001_pessoa tp1 ON (rv.T001_SQ_PESSOA = tp1.T001_SQ_PESSOA) 
                    LEFT JOIN t004_orgao_registro tor ON (p.T004_NR_CNPJ_ORG_REG = tor.T004_NR_CNPJ_ORG_REG) 
                        WHERE p.T005_NR_PROTOCOLO = @protocolo 
                ");

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
                        requerimento = new Requerimento();
                        if (!iDataReader.IsDBNull(0))
                        {
                            requerimento.NumeroProtocolo = iDataReader.GetString(0);
                        }

                        if (!iDataReader.IsDBNull(9))
                        {
                            requerimento.NumeroProtocoloEnquadramento = iDataReader.GetString(9);
                        }

                        if (!iDataReader.IsDBNull(1))
                        {
                            requerimento.NumeroProtocoloOrgaoEstadual = iDataReader.GetString(1);
                        }

                        if (!iDataReader.IsDBNull(6))
                        {
                            requerimento.NumeroProtocoloOrgaoMunicipal = iDataReader.GetString(6);
                        }

                        if (!iDataReader.IsDBNull(8))
                        {
                            requerimento.NumeroProtocoloOrgaoRegistroOutraUf = iDataReader.GetString(8);
                        }

                        if (!iDataReader.IsDBNull(4))
                        {
                            requerimento.NumeroProtocoloViabilidade = iDataReader.GetString(4);
                        }

                        if (!iDataReader.IsDBNull(7))
                        {
                            requerimento.DataAssinatura = iDataReader.GetDateTime(7).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(3))
                        {
                            requerimento.DataAverbacao = iDataReader.GetDateTime(3).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(2))
                        {
                            requerimento.DataEntrada = iDataReader.GetDateTime(2).ToString("dd/MM/yyyy");
                        }

                        if (!iDataReader.IsDBNull(11))
                        {
                            requerimento.DbeCarregado = iDataReader.GetString(11);
                        }

                        if (!iDataReader.IsDBNull(5))
                        {
                            requerimento.NumeroDbe = iDataReader.GetString(5);
                        }

                        if (!iDataReader.IsDBNull(10))
                        {
                            requerimento.Situacao = iDataReader.GetString(10);
                        }

                        if (!iDataReader.IsDBNull(12))
                        {
                            requerimento.RazaoSocial = iDataReader.GetString(12);
                        }

                        if (!iDataReader.IsDBNull(13))
                        {
                            requerimento.Cnpj = iDataReader.GetString(13);
                        }

                        if (!iDataReader.IsDBNull(14))
                        {
                            requerimento.Solicitante = iDataReader.GetString(14);
                        }

                        if (!iDataReader.IsDBNull(15))
                        {
                            requerimento.Uf = iDataReader.GetString(15);
                        }

                        if (!iDataReader.IsDBNull(16))
                        {
                            requerimento.CodigoNaturezaJuridica = iDataReader.GetString(16);
                        }

                        if (!iDataReader.IsDBNull(17))
                        {
                            requerimento.DescricaoNaturezaJuridica = iDataReader.GetString(17);
                        }
                    }
                }

                return requerimento;
            }
            finally
            {
                if (fechaConexao && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }
        }

        public List<ResumoProcesso> ConsultarProcessosVinculoPrincipal(string cpf, string protocolo,
            string dataModificacao, IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                    SELECT (SELECT pj.T003_NR_CNPJ
                            FROM
                              requerimento.t003_pessoa_juridica pj
                            WHERE
                              pj.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCnpj
                         , (SELECT pf.T002_NR_CPF
                            FROM
                              requerimento.t002_pessoa_fisica pf
                            WHERE
                              pf.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCpf
                         , (SELECT ps.T001_DS_PESSOA
                            FROM
                              requerimento.t001_pessoa ps
                            WHERE
                              ps.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaNome
                         , p.T005_DT_CRIACAO DataCriacao
                         , COALESCE((SELECT l.T011_DT_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1),  p.T005_DT_CRIACAO) AS DataAlteracao
                         , (SELECT l.T011_IN_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusCodigo
                         , (SELECT (SELECT tg.TGE_NOMB_DESC
                                    FROM
                                      shared.tab_generica tg
                                    WHERE
                                      (tg.TGE_TIP_TAB = 10
                                      AND tg.TGE_COD_TIP_TAB = cast(l.T011_IN_SITUACAO AS DECIMAL(5, 0))))
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG
                              AND l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                              -- AND l.T011_IN_SITUACAO not in ('0', '6', '11', '12')
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusDescricao
                         , (SELECT a.CO_ATO
                            FROM
                              shared.ato a
                            WHERE
                              a.CO_ATO = p.T005_CO_ATO) co_ato
                         , (SELECT a.NO_ATO
                            FROM
                              shared.ato a
                            WHERE
                              a.CO_ATO = p.T005_CO_ATO) ds_ato
                         , p.T005_NR_PROTOCOLO NumeroProtocolo
                         , p.T005_NR_PROTOCOLO_RCPJ  NumeroProtocoloOrgaoRegistro
                    FROM
                      requerimento.t005_protocolo p
                    INNER JOIN requerimento.r001_vinculo v
                    ON (v.T001_SQ_PESSOA_PAI = p.T001_SQ_PESSOA)
                    INNER JOIN requerimento.t002_pessoa_fisica pf
                    ON (v.T001_SQ_PESSOA = pf.T001_SQ_PESSOA)
                    WHERE
                      pf.T002_NR_CPF = @cpf
                ");

                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query.AppendLine(
                        " AND DATE(COALESCE((select ps.T011_DT_SITUACAO from requerimento.t011_protocolo_status ps where ps.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG and ps.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOL order by ps.T011_DT_SITUACAO desc limit 1), p.T005_DT_AVERBACAO, p.T005_DT_ENVIO_S01, p.T005_DT_ENTRADA, p.T005_DT_CRIACAO)) >= @dataModificacao ");
                }

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(
                        " AND (p.T005_NR_PROTOCOLO = @protocolo OR p.T005_NR_PROTOCOLO_RCPJ = @protocolo OR p.T005_NR_PROTOCOLO_VIABILIDADE = @protocolo)");
                }

                if (!fechaCommand)
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
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosVinculoPrincipal";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosRepresentante(string cpf, string protocolo, string dataModificacao,
            IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                      SELECT (SELECT pj.T003_NR_CNPJ
                                FROM
                                  requerimento.t003_pessoa_juridica pj
                                WHERE
                                  pj.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCnpj
                             , (SELECT pf.T002_NR_CPF
                                FROM
                                  requerimento.t002_pessoa_fisica pf
                                WHERE
                                  pf.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCpf
                             , (SELECT ps.T001_DS_PESSOA
                                FROM
                                  requerimento.t001_pessoa ps
                                WHERE
                                  ps.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaNome
                             , p.T005_DT_CRIACAO DataCriacao
                             , COALESCE((SELECT l.T011_DT_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1),  p.T005_DT_CRIACAO) AS DataAlteracao
                             , (SELECT l.T011_IN_SITUACAO
                                FROM
                                  requerimento.t011_protocolo_status_log l
                                WHERE
                                  l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                                ORDER BY
                                  l.T011_DT_SITUACAO DESC
                                LIMIT
                                  1) StatusCodigo
                             ,  (SELECT (SELECT tg.TGE_NOMB_DESC
                                    FROM
                                      shared.tab_generica tg
                                    WHERE
                                      (tg.TGE_TIP_TAB = 10
                                      AND tg.TGE_COD_TIP_TAB = cast(l.T011_IN_SITUACAO AS DECIMAL(5, 0))))
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG
                              AND l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                              -- AND l.T011_IN_SITUACAO not in ('0', '6', '11', '12')
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusDescricao
                             , (SELECT a.CO_ATO
                                FROM
                                  shared.ato a
                                WHERE
                                  a.CO_ATO = p.T005_CO_ATO) co_ato
                             , (SELECT a.NO_ATO
                                FROM
                                  shared.ato a
                                WHERE
                                  a.CO_ATO = p.T005_CO_ATO) ds_ato
                             , p.T005_NR_PROTOCOLO NumeroProtocolo
                             , p.T005_NR_PROTOCOLO_RCPJ  NumeroProtocoloOrgaoRegistro
                        FROM
                          requerimento.t005_protocolo p
                        INNER JOIN requerimento.r001_vinculo v
                        ON (v.T001_SQ_PESSOA_PAI = p.T001_SQ_PESSOA)
                        INNER JOIN requerimento.r001_vinculo v2
                        ON (v2.T001_SQ_PESSOA_PAI = v.T001_SQ_PESSOA)
                        INNER JOIN requerimento.t002_pessoa_fisica pf
                        ON (v2.T001_SQ_PESSOA = pf.T001_SQ_PESSOA)
                        WHERE
                          pf.T002_NR_CPF = @cpf
                ");
                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query.AppendLine(
                        " AND DATE(COALESCE((select ps.T011_DT_SITUACAO from requerimento.t011_protocolo_status ps where ps.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG and ps.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOL order by ps.T011_DT_SITUACAO desc limit 1), p.T005_DT_AVERBACAO, p.T005_DT_ENVIO_S01, p.T005_DT_ENTRADA, p.T005_DT_CRIACAO)) >= @dataModificacao ");
                }

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(
                        " AND (p.T005_NR_PROTOCOLO = @protocolo OR p.T005_NR_PROTOCOLO_RCPJ = @protocolo OR p.T005_NR_PROTOCOLO_VIABILIDADE = @protocolo)");
                }

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                if (!fechaCommand)
                {
                    iDbCommand.Parameters.Clear();
                }

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
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosRepresentante";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosRepresentanteDoRepresentante(string cpf, string protocolo,
            string dataModificacao, IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                        SELECT (SELECT pj.T003_NR_CNPJ
                                FROM
                                  requerimento.t003_pessoa_juridica pj
                                WHERE
                                  pj.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCnpj
                             , (SELECT pf.T002_NR_CPF
                                FROM
                                  requerimento.t002_pessoa_fisica pf
                                WHERE
                                  pf.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCpf
                             , (SELECT ps.T001_DS_PESSOA
                                FROM
                                  requerimento.t001_pessoa ps
                                WHERE
                                  ps.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaNome
                             , p.T005_DT_CRIACAO DataCriacao
                             , COALESCE((SELECT l.T011_DT_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1),  p.T005_DT_CRIACAO) AS DataAlteracao
                             , (SELECT l.T011_IN_SITUACAO
                                FROM
                                  requerimento.t011_protocolo_status_log l
                                WHERE
                                  l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                                ORDER BY
                                  l.T011_DT_SITUACAO DESC
                                LIMIT
                                  1) StatusCodigo
                             ,  (SELECT (SELECT tg.TGE_NOMB_DESC
                                    FROM
                                      shared.tab_generica tg
                                    WHERE
                                      (tg.TGE_TIP_TAB = 10
                                      AND tg.TGE_COD_TIP_TAB = cast(l.T011_IN_SITUACAO AS DECIMAL(5, 0))))
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG
                              AND l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                              -- AND l.T011_IN_SITUACAO not in ('0', '6', '11', '12')
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusDescricao
                             , (SELECT a.CO_ATO
                                FROM
                                  shared.ato a
                                WHERE
                                  a.CO_ATO = p.T005_CO_ATO) co_ato
                             , (SELECT a.NO_ATO
                                FROM
                                  shared.ato a
                                WHERE
                                  a.CO_ATO = p.T005_CO_ATO) ds_ato
                             , p.T005_NR_PROTOCOLO NumeroProtocolo
                             , p.T005_NR_PROTOCOLO_RCPJ  NumeroProtocoloOrgaoRegistro
                        FROM
                          requerimento.t005_protocolo p
                        INNER JOIN requerimento.r001_vinculo v
                        ON (v.T001_SQ_PESSOA_PAI = p.T001_SQ_PESSOA)
                        INNER JOIN requerimento.r001_vinculo v2
                        ON (v2.T001_SQ_PESSOA_PAI = v.T001_SQ_PESSOA)
                        INNER JOIN requerimento.r001_vinculo v3
                        ON (v3.T001_SQ_PESSOA_PAI = v2.T001_SQ_PESSOA)
                        INNER JOIN requerimento.t002_pessoa_fisica pf
                        ON (v3.T001_SQ_PESSOA = pf.T001_SQ_PESSOA)
                        WHERE
                          pf.T002_NR_CPF = @cpf
                ");
                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query.AppendLine(
                        " AND DATE(COALESCE((select ps.T011_DT_SITUACAO from requerimento.t011_protocolo_status ps where ps.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG and ps.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOL order by ps.T011_DT_SITUACAO desc limit 1), p.T005_DT_AVERBACAO, p.T005_DT_ENVIO_S01, p.T005_DT_ENTRADA, p.T005_DT_CRIACAO)) >= @dataModificacao ");
                }

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(
                        " AND (p.T005_NR_PROTOCOLO = @protocolo OR p.T005_NR_PROTOCOLO_RCPJ = @protocolo OR p.T005_NR_PROTOCOLO_VIABILIDADE = @protocolo)");
                }

                if (!fechaCommand)
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
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosRepresentanteDoRepresentante";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosContador(string cpf, string protocolo, string dataModificacao,
            IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                              SELECT (SELECT pj.T003_NR_CNPJ
                                    FROM
                                      requerimento.t003_pessoa_juridica pj
                                    WHERE
                                      pj.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCnpj
                                 , (SELECT pf.T002_NR_CPF
                                    FROM
                                      requerimento.t002_pessoa_fisica pf
                                    WHERE
                                      pf.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCpf
                                 , (SELECT ps.T001_DS_PESSOA
                                    FROM
                                      requerimento.t001_pessoa ps
                                    WHERE
                                      ps.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaNome
                                 , p.T005_DT_CRIACAO DataCriacao
                                 , COALESCE((SELECT l.T011_DT_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1),  p.T005_DT_CRIACAO) AS DataAlteracao
                                 , (SELECT l.T011_IN_SITUACAO
                                    FROM
                                      requerimento.t011_protocolo_status_log l
                                    WHERE
                                      l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                                    ORDER BY
                                      l.T011_DT_SITUACAO DESC
                                    LIMIT
                                      1) StatusCodigo
                                 ,  (SELECT (SELECT tg.TGE_NOMB_DESC
                                    FROM
                                      shared.tab_generica tg
                                    WHERE
                                      (tg.TGE_TIP_TAB = 10
                                      AND tg.TGE_COD_TIP_TAB = cast(l.T011_IN_SITUACAO AS DECIMAL(5, 0))))
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG
                              AND l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                              -- AND l.T011_IN_SITUACAO not in ('0', '6', '11', '12')
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusDescricao
                                 , (SELECT a.CO_ATO
                                    FROM
                                      shared.ato a
                                    WHERE
                                      a.CO_ATO = p.T005_CO_ATO) co_ato
                                 , (SELECT a.NO_ATO
                                    FROM
                                      shared.ato a
                                    WHERE
                                      a.CO_ATO = p.T005_CO_ATO) ds_ato
                                 , p.T005_NR_PROTOCOLO NumeroProtocolo
                                 , p.T005_NR_PROTOCOLO_RCPJ  NumeroProtocoloOrgaoRegistro
                            FROM
                              requerimento.t005_protocolo p
                            INNER JOIN requerimento.t093_contador c
                            ON (c.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO)
                            WHERE
                              (c.T093_CPF_RESP = @cpf
                              OR c.T093_CPFCNPJ = @cpf)

                ");
                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query.AppendLine(
                        " AND DATE(COALESCE((select ps.T011_DT_SITUACAO from requerimento.t011_protocolo_status ps where ps.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG and ps.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOL order by ps.T011_DT_SITUACAO desc limit 1), p.T005_DT_AVERBACAO, p.T005_DT_ENVIO_S01, p.T005_DT_ENTRADA, p.T005_DT_CRIACAO)) >= @dataModificacao ");
                }

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(
                        " AND (p.T005_NR_PROTOCOLO = @protocolo OR p.T005_NR_PROTOCOLO_RCPJ = @protocolo OR p.T005_NR_PROTOCOLO_VIABILIDADE = @protocolo)");
                }

                if (!fechaCommand)
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
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosContador";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosAssinantes(string cpf, string protocolo, string dataModificacao, IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                            SELECT (SELECT pj.T003_NR_CNPJ
                                        FROM
                                          t003_pessoa_juridica pj
                                        WHERE
                                          pj.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCnpj
                                     , (SELECT pf.T002_NR_CPF
                                        FROM
                                          t002_pessoa_fisica pf
                                        WHERE
                                          pf.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaCpf
                                     , (SELECT ps.T001_DS_PESSOA
                                        FROM
                                          t001_pessoa ps
                                        WHERE
                                          ps.T001_SQ_PESSOA = p.T001_SQ_PESSOA) PessoaNome
                                     , p.T005_DT_CRIACAO DataCriacao
                                     , COALESCE((SELECT l.T011_DT_SITUACAO
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1),  p.T005_DT_CRIACAO) AS DataAlteracao
                                     , (SELECT l.T011_IN_SITUACAO
                                        FROM
                                          requerimento.t011_protocolo_status_log l
                                        WHERE
                                          l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                                        ORDER BY
                                          l.T011_DT_SITUACAO DESC
                                        LIMIT
                                          1) StatusCodigo
                                     ,  (SELECT (SELECT tg.TGE_NOMB_DESC
                                    FROM
                                      shared.tab_generica tg
                                    WHERE
                                      (tg.TGE_TIP_TAB = 10
                                      AND tg.TGE_COD_TIP_TAB = cast(l.T011_IN_SITUACAO AS DECIMAL(5, 0))))
                            FROM
                              requerimento.t011_protocolo_status_log l
                            WHERE
                              l.T004_NR_CNPJ_ORG_REG = p.T004_NR_CNPJ_ORG_REG
                              AND l.T005_NR_PROTOCOLO = p.T005_NR_PROTOCOLO
                              -- AND l.T011_IN_SITUACAO not in ('0', '6', '11', '12')
                            ORDER BY
                              l.T011_DT_SITUACAO DESC
                            LIMIT
                              1) StatusDescricao
                                     , (SELECT a.CO_ATO
                                        FROM
                                          shared.ato a
                                        WHERE
                                          a.CO_ATO = p.T005_CO_ATO)  AtoEventoRfbCodigo
                                     , (SELECT a.NO_ATO
                                        FROM
                                          shared.ato a
                                        WHERE
                                          a.CO_ATO = p.T005_CO_ATO) AtoEventoRfbDescricao
                                     , p.T005_NR_PROTOCOLO NumeroProtocolo
                                     , p.T005_NR_PROTOCOLO_RCPJ  NumeroProtocoloOrgaoRegistro
                                FROM
                                  requerimento.t005_protocolo p
                                INNER JOIN requerimento.t119_assinantes a
                                ON (a.t005_protocolo = p.T005_NR_PROTOCOLO)
                                WHERE
                                  a.t119_cpf =  @cpf
                ");
                if (!string.IsNullOrEmpty(dataModificacao))
                {
                    query.AppendLine(
                        " AND DATE(COALESCE(ps.T011_DT_SITUACAO, p.T005_DT_AVERBACAO, p.T005_DT_ENVIO_S01, p.T005_DT_ENTRADA, p.T005_DT_CRIACAO)) >= @dataModificacao ");
                }

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(
                        " AND (p.T005_NR_PROTOCOLO = @protocolo OR p.T005_NR_PROTOCOLO_RCPJ = @protocolo OR p.T005_NR_PROTOCOLO_VIABILIDADE = @protocolo)");
                }

                if (!fechaCommand)
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
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["AtoEventoRfbCodigo"] != DBNull.Value ? iDataReader["AtoEventoRfbCodigo"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["AtoEventoRfbDescricao"] != DBNull.Value ? iDataReader["AtoEventoRfbDescricao"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosAssinantes";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosRequerimentoServico(string cpf, string protocolo, string dataHoraCriado, IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                            SELECT   rq.t0023_nr_cnpj_empresa PessoaCnpj
                                     , if(rq.t0023_nr_cnpj_empresa IS NULL, (SELECT rqe.t0022_nr_cpf_cnpj
                                                                             FROM
                                                                               requerimento_servico.t0022_requerente rqe
                                                                             WHERE
                                                                               rqe.t0022_sq_requerente = rq.t0022_sq_requerente), NULL) AS PessoaCpf
                                     , rq.t0023_ds_empresa PessoaNome
                                     , rq.t0023_dt_entrada DataCriacao
                                     , rq.t0023_dt_entrada DataAlteracao
                                     , '0' StatusCodigo
                                     , 'CRIADO' StatusDescricao
                                     , (SELECT a.CO_ATO
                                        FROM
                                          shared.ato a
                                        WHERE
                                          rq.t0023_co_ato = a.CO_ATO) AtoEventoRfbCodigo
                                     , (SELECT a.NO_ATO
                                        FROM
                                          shared.ato a
                                        WHERE
                                          rq.t0023_co_ato = a.CO_ATO) AtoEventoRfbDescricao
                                     , rq.t0023_nr_requerimento NumeroProtocolo
                                     , rq.t0023_nro_protocolo_or  NumeroProtocoloOrgaoRegistro
                                FROM
                                  requerimento_servico.t0023_requerimento rq
                                WHERE
                                  rq.t0022_sq_requerente = (SELECT rqent.t0022_sq_requerente
                                                            FROM
                                                              requerimento_servico.t0022_requerente rqent
                                                            WHERE
                                                              rqent.t0022_nr_cpf_cnpj = @cpf)
  
                ");

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(" AND(rq.t0023_nr_requerimento = @protocolo OR rq.t0023_nro_protocolo_or = @protocolo)");
                }

                if (!string.IsNullOrEmpty(dataHoraCriado))
                {
                    query.AppendLine(" AND DATE(rq.t0023_dt_entrada) >= @dataHoraCriado ");
                }

                query.AppendLine(" GROUP BY rq.t0023_nr_requerimento, rq.t0023_nro_protocolo_or");

                if (!fechaCommand)
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

                if (!string.IsNullOrEmpty(dataHoraCriado))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataHoraCriado";
                    parametro.Value = dataHoraCriado;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["AtoEventoRfbCodigo"] != DBNull.Value ? iDataReader["AtoEventoRfbCodigo"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["AtoEventoRfbDescricao"] != DBNull.Value ? iDataReader["AtoEventoRfbDescricao"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;

                        resumoProcesso.origem = OrigemProcesso.REQUERIMENTO;
                        resumoProcesso.metodo = "RequerimentoMySqlImplDao.ConsultarProcessosRequerimentoServico";
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }

        public List<ResumoProcesso> ConsultarProcessosRequerimentoServicoPF(string cpf, string protocolo, string dataHoraCriado, IDbCommand iDbCommand, bool fechaCommand)
        {
            List<ResumoProcesso> resultado = new List<ResumoProcesso>();
            StringBuilder query;
            try
            {
                query = new StringBuilder(@"
                           SELECT rq.t0023_nr_requerimento
                                 , rq.t0023_nr_cnpj_empresa as PessoaCnpj
                                 , if(rq.t0023_nr_cnpj_empresa IS NULL, (SELECT rqe.t0022_nr_cpf_cnpj
                                                                         FROM
                                                                           requerimento_servico.t0022_requerente rqe
                                                                         WHERE
                                                                           rqe.t0022_sq_requerente = rq.t0022_sq_requerente), NULL) AS t0022_nr_cpf_cnpj as PessoaCpf
                                 , rq.t0023_ds_empresa as PessoaNome
                                 , rq.t0023_dt_entrada as DataCriacao
                                 , rq.t0023_dt_entrada as DataAlteracao
                                 , '0' as StatusCodigo
                                 , 'CRIADO' as StatusDescricao
                                 , (SELECT a.CO_ATO
                                    FROM
                                      shared.ato a
                                    WHERE
                                      rq.t0023_co_ato = a.CO_ATO) CO_ATO as AtoEventoRfbCodigo
                                 , (SELECT a.NO_ATO
                                    FROM
                                      shared.ato a
                                    WHERE
                                      rq.t0023_co_ato = a.CO_ATO) NO_ATO as AtoEventoRfbDescricao
                                 , rq.t0023_nr_requerimento as NumeroProtocolo
                                 , rq.t0023_nro_protocolo_or as NumeroProtocoloOrgaoRegistro
                                 , rq.t0023_nr_protocolo_viabilidade as NumeroProtocoloViabilidade
                            FROM
                              requerimento_servico.t0023_requerimento rq
                            WHERE
                              rq.t0023_sq_requerimento IN (SELECT rqpf.t0023_sq_requerimento
                                                           FROM
                                                             requerimento_servico.t0040_requerimento_pessoa_fisica rqpf
                                                           WHERE
                                                             rqpf.t0023_sq_requerimento = rq.t0023_sq_requerimento
                                                             AND rqpf.t0040_nr_cpf = @cpf)
                ");

                if (!string.IsNullOrEmpty(protocolo))
                {
                    query.AppendLine(" AND(rq.t0023_nr_requerimento = @protocolo OR rq.t0023_nro_protocolo_or = @protocolo)");
                }

                if (!string.IsNullOrEmpty(dataHoraCriado))
                {
                    query.AppendLine(" AND DATE(rq.t0023_dt_entrada) >= @dataHoraCriado ");
                }

                query.AppendLine(" GROUP BY rq.t0023_nr_requerimento, rq.t0023_nro_protocolo_or");

                if (!fechaCommand)
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

                if (!string.IsNullOrEmpty(dataHoraCriado))
                {
                    IDbDataParameter parametro = iDbCommand.CreateParameter();
                    parametro.ParameterName = "dataHoraCriado";
                    parametro.Value = dataHoraCriado;
                    iDbCommand.Parameters.Add(parametro);
                }

                using (IDataReader iDataReader = iDbCommand.ExecuteReader())
                {
                    while (iDataReader.Read())
                    {
                        ResumoProcesso resumoProcesso = new ResumoProcesso();
                        resumoProcesso.NumeroProtocolo = iDataReader["NumeroProtocolo"] != DBNull.Value ? iDataReader["NumeroProtocolo"].ToString() : "";
                        resumoProcesso.NumeroProtocoloOrgaoRegistro = iDataReader["NumeroProtocoloOrgaoRegistro"] != DBNull.Value ? iDataReader["NumeroProtocoloOrgaoRegistro"].ToString() : "";
                        resumoProcesso.NumeroProtocoloViabilidade = iDataReader["NumeroProtocoloViabilidade"] != DBNull.Value ? iDataReader["NumeroProtocoloViabilidade"].ToString() : "";
                        resumoProcesso.OrgaoRegistroCnpj = ConfigurationManager.AppSettings["OrgaoRegistroCnpj"].ToString();
                        resumoProcesso.OrgaoRegistroNome = ConfigurationManager.AppSettings["OrgaoRegistroNome"].ToString();
                        resumoProcesso.OrgaoRegistroSigla = ConfigurationManager.AppSettings["OrgaoRegistroSigla"].ToString();
                        resumoProcesso.DataCriacao = iDataReader["DataCriacao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataCriacao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.DataAlteracao = iDataReader["DataAlteracao"] != DBNull.Value ? Convert.ToDateTime(iDataReader["DataAlteracao"].ToString()).ToUniversalTime() : DateTime.MinValue;
                        resumoProcesso.PessoaCnpj = iDataReader["PessoaCnpj"] != DBNull.Value ? iDataReader["PessoaCnpj"].ToString() : "";
                        resumoProcesso.PessoaNome = iDataReader["PessoaNome"] != DBNull.Value ? iDataReader["PessoaNome"].ToString() : "";
                        resumoProcesso.PessoaCpf = iDataReader["PessoaCpf"] != DBNull.Value ? iDataReader["PessoaCpf"].ToString() : "";

                        resumoProcesso.StatusCodigo = iDataReader["StatusCodigo"] != DBNull.Value ? iDataReader["StatusCodigo"].ToString() : "";
                        resumoProcesso.StatusDescricao = iDataReader["StatusDescricao"] != DBNull.Value ? iDataReader["StatusDescricao"].ToString() : "";
                        resumoProcesso.TipoProtocolo = TipoProtocolo.Tipos.REQUERIMENTO;
                        resumoProcesso.AtoEventoRfbCodigo = iDataReader["co_ato"] != DBNull.Value ? iDataReader["co_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbDescricao = iDataReader["ds_ato"] != DBNull.Value ? iDataReader["ds_ato"].ToString() : "";
                        resumoProcesso.AtoEventoRfbTipo = TipoAtoEventoRfb.ATO;
                        resultado.Add(resumoProcesso);
                    }
                }
            }
            finally
            {
                if (fechaCommand && iDbCommand != null)
                {
                    iDbCommand.Dispose();
                }
            }

            return resultado;
        }
    }
}