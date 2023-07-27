using regin_app_mobile.Constante;
using regin_app_mobile.GeracaoXml;
using System;
using System.Data;
using System.Text;

namespace regin_app_mobile.Dao
{
    /// <summary>
    /// 
    /// </summary>
    public class RequerimentoMySQLImplDao : IRequerimentoDao
    {
        public RequerimentoMySQLImplDao()
        {

        }

        public bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {
                query.AppendLine("SELECT p.t005_nr_protocolo ");
                query.AppendLine("  FROM t005_protocolo p ");
                query.AppendLine(" WHERE p.T005_NR_PROTOCOLO = @protocolo ");

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
            catch (Exception)
            {
                throw;
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

        public TipoProtocolo.Tipos ConsultaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {
                query.AppendLine("SELECT p.t005_nr_protocolo ");
                query.AppendLine("  FROM t005_protocolo p ");
                query.AppendLine(" WHERE p.T005_NR_PROTOCOLO = @protocolo ");

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();

                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;


                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                if (iDataReader.Read())
                {
                    return TipoProtocolo.Tipos.REQUERIMENTO;
                }
                else
                {
                    return TipoProtocolo.Tipos.NAO_ENCONTRADO;
                }
            }
            catch (Exception)
            {
                throw;
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

        public Requerimento ConsultaProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao)
        {
            Requerimento requerimento = null;
            IDataReader iDataReader = null;
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT p.t005_NR_PROTOCOLO "); // 0
                query.Append("      ,p.T005_NR_PROTOCOLO_RCPJ "); // 1
                query.Append("      ,p.T005_DT_ENTRADA "); // 2
                query.Append("      ,p.T005_DT_AVERBACAO "); // 3
                query.Append("      ,p.T005_NR_PROTOCOLO_VIABILIDADE "); // 4
                query.Append("      ,p.T005_NR_DBE "); // 5
                query.Append("      ,p.T005_NR_PROTOCOLO_PREFEITURA "); // 6
                query.Append("      ,p.T005_DATA_ASSINATURA "); // 7
                query.Append("      ,p.T005_PROTOCOLO_ORGAO_ORIGEM "); // 8
                query.Append("      ,p.T005_NR_PROTOCOLO_ENQUADRAMENTO "); // 9
                query.Append("      ,IF(p.T005_IN_SITUACAO = 1, 'INVÁLIDO', 'VÁLIDO') AS T005_IN_SITUACAO "); // 10
                query.Append("      ,IF(p.T005_IN_DBE_CARREGADO, 'NÃO', 'SIM') AS T005_IN_DBE_CARREGADO "); // 11
                // query.Append("      ,IF(p.T005_IN_PROTOCOLO = 1, 'SIM', 'NÃO') AS T005_IN_PROTOCOLO "); // 12
                query.Append("      ,tp.T001_DS_PESSOA AS RAZAO_SOCIAL "); // 12
                query.Append("      ,tpj.T003_NR_CNPJ AS CNPJ_EMPRESA "); // 13
                query.Append("      ,tp1.T001_DS_PESSOA AS NOME_SOLICITANTE "); // 14
                query.Append("      ,tor.T004_UF "); // 15
                query.Append("      ,tpj.A006_CO_NATUREZA_JURIDICA AS CODIGO_NATUREZA_JURIDICA "); // 16
                query.Append("      ,tnj.T009_DS_NATUREZA_JURIDICA AS DESCRICAO_NATUREZA_JURIDICA "); // 17
                query.Append("  FROM t005_protocolo p ");
                query.Append("  LEFT JOIN t001_pessoa tp ON (p.T001_SQ_PESSOA = tp.T001_SQ_PESSOA) ");
                query.Append("  LEFT JOIN t003_pessoa_juridica tpj ON (tp.T001_SQ_PESSOA = tpj.T001_SQ_PESSOA) ");
                query.Append("  LEFT JOIN t009_natureza_juridica tnj ON (tpj.A006_CO_NATUREZA_JURIDICA = tnj.T009_CO_NATUREZA_JURIDICA) ");
                query.Append("  LEFT JOIN r001_vinculo rv ON (tp.T001_SQ_PESSOA = rv.T001_SQ_PESSOA_PAI AND rv.A009_CO_CONDICAO = 500) ");
                query.Append("  LEFT JOIN t001_pessoa tp1 ON (rv.T001_SQ_PESSOA = tp1.T001_SQ_PESSOA) ");
                query.Append("  LEFT JOIN t004_orgao_registro tor ON (p.T004_NR_CNPJ_ORG_REG = tor.T004_NR_CNPJ_ORG_REG) ");
                query.Append(" WHERE p.T005_NR_PROTOCOLO = @protocolo ");

                iDbCommand.CommandType = CommandType.Text;
                iDbCommand.CommandText = query.ToString();

                IDbDataParameter iDbDataParameter = iDbCommand.CreateParameter();

                iDbDataParameter.ParameterName = "protocolo";
                iDbDataParameter.Value = protocolo;

                iDbCommand.Parameters.Add(iDbDataParameter);

                iDataReader = iDbCommand.ExecuteReader();

                if (iDataReader.Read())
                {
                    requerimento = new Requerimento();
                    IDataRecord iDataRecord = (IDataRecord)iDataReader;
                    if (!iDataRecord.IsDBNull(0))
                    {
                        requerimento.numeroProtocolo = iDataRecord.GetString(0);
                    }
                    if (!iDataRecord.IsDBNull(9))
                    {
                        requerimento.numeroProtocoloEnquadramento = iDataRecord.GetString(9);
                    }
                    if (!iDataRecord.IsDBNull(1))
                    {
                        requerimento.numeroProtocoloOrgaoEstadual = iDataRecord.GetString(1);
                    }
                    if (!iDataRecord.IsDBNull(6))
                    {
                        requerimento.numeroProtocoloOrgaoMunicipal = iDataRecord.GetString(6);
                    }
                    if (!iDataRecord.IsDBNull(8))
                    {
                        requerimento.numeroProtocoloOrgaoRegistroOutraUf = iDataRecord.GetString(8);
                    }
                    if (!iDataRecord.IsDBNull(4))
                    {
                        requerimento.numeroProtocoloViabilidade = iDataRecord.GetString(4);
                    }
                    if (!iDataRecord.IsDBNull(7))
                    {
                        requerimento.dataAssinatura = iDataRecord.GetDateTime(7).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(3))
                    {
                        requerimento.dataAverbacao = iDataRecord.GetDateTime(3).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(2))
                    {
                        requerimento.dataEntrada = iDataRecord.GetDateTime(2).ToString("dd/MM/yyyy");
                    }
                    if (!iDataRecord.IsDBNull(11))
                    {
                        requerimento.dbeCarregado = iDataRecord.GetString(11);
                    }
                    if (!iDataRecord.IsDBNull(5))
                    {
                        requerimento.numeroDbe = iDataRecord.GetString(5);
                    }
                    if (!iDataRecord.IsDBNull(10))
                    {
                        requerimento.situacao = iDataRecord.GetString(10);
                    }
                    if (!iDataRecord.IsDBNull(12))
                    {
                        requerimento.razaoSocial = iDataRecord.GetString(12);
                    }
                    if (!iDataRecord.IsDBNull(13))
                    {
                        requerimento.cnpj = iDataRecord.GetString(13);
                    }
                    if (!iDataRecord.IsDBNull(14))
                    {
                        requerimento.solicitante = iDataRecord.GetString(14);
                    }
                    if (!iDataRecord.IsDBNull(15))
                    {
                        requerimento.uf = iDataRecord.GetString(15);
                    }
                    if (!iDataRecord.IsDBNull(16))
                    {
                        requerimento.codigoNaturezaJuridica = iDataRecord.GetString(16);
                    }
                    if (!iDataRecord.IsDBNull(17))
                    {
                        requerimento.descricaoNaturezaJuridica = iDataRecord.GetString(17);
                    }
                }
                return requerimento;
            }
            catch (Exception)
            {
                throw;
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
}
