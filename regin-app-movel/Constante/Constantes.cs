using System;
using System.Configuration;

namespace regin_app_movel.Constante
{

    /// <summary>
    /// Constante de parametros de configuracao do Servico Web
    /// </summary>
    public class ConfiguracaoSistema
    {
        /// <summary>
        /// Constante de chave de parametros que devem existir no Web.xml
        /// </summary>
        public enum Parametros
        {
            /// <summary>
            /// Chave: db.regin.sgbd
            /// </summary>
            DB_REGIN_SGBD,
            /// <summary>
            /// Chave: db.requerimento.sgbd
            /// </summary>
            DB_REQUERIMENTO_SGBD,
            /// <summary>
            /// Chave: db.connection.string.oracle
            /// </summary>
            DB_CONNECTION_STRING_ORACLE,
            /// <summary>
            /// Chave: db.connection.string.mysql
            /// </summary>
            DB_CONNECTION_STRING_MYSQL,
            /// <summary>
            /// Chave: db.connection.string.sqlserver
            /// </summary>
            DB_CONNECTION_STRING_SQLSERVER,
            /// <summary>
            /// Chave: sigla.uf
            /// </summary>
            SIGLA_UF
        }

        public enum Sgbds
        {
            /// <summary>
            /// Valor: ORACLE
            /// </summary>
            ORACLE,
            /// <summary>
            /// Valor: SQLSERVER
            /// </summary>
            SQL_SERVER,
            /// <summary>
            /// Valor: MYSQL
            /// </summary>
            MYSQL,
            /// <summary>
            /// Valor: NULL
            /// Esse parametro deve ser utilizado quando o valor do parametro "db.regin.sgbd" ou "db.requerimento.sgbd" não for valido.
            /// Os valores validos sao: ORACLE, SQLSERVER, MYSQL
            /// </summary>
            NULL
        }

        /// <summary>
        /// Valor do SGBD como deve ser encontrado no valor do parametro "db.junta.sgbd"
        /// </summary>
        /// <param name="sgbds">Constante de SGBDs validos para o Servico Web</param>
        /// <returns>Uma String que representa o valor do parametro "db.junta.sgbd"</returns>
        public static String GetSgbdNome(Sgbds sgbds)
        {
            if (sgbds.Equals(Sgbds.ORACLE))
            {
                return "ORACLE";
            }
            else if (sgbds.Equals(Sgbds.SQL_SERVER))
            {
                return "SQLSERVER";
            }
            else if (sgbds.Equals(Sgbds.MYSQL))
            {
                return "MYSQL";
            }
            return null;
        }

        /// <summary>
        /// Chave do parametro da string de conexao baseado no SGBD atualmente configurado
        /// </summary>
        /// <param name="sgbd">Constante de SGBDs validos para o Servico Web</param>
        /// <returns>Chave do parametro que contem a string de conexao com o SGBD atualmente configurado</returns>
        public static String GetParametroChaveSgbd(Sgbds sgbd)
        {
            if (sgbd.Equals(Sgbds.ORACLE))
            {
                return GetParametroChave(Parametros.DB_CONNECTION_STRING_ORACLE);
            }
            else if (sgbd.Equals(Sgbds.SQL_SERVER))
            {
                return GetParametroChave(Parametros.DB_CONNECTION_STRING_SQLSERVER);
            }
            else if (sgbd.Equals(Sgbds.MYSQL))
            {
                return GetParametroChave(Parametros.DB_CONNECTION_STRING_MYSQL);
            }
            return null;
        }

        /// <summary>
        /// Constante do SGBD baseado no valor contido no parametro com a chave "db.junta.sgbd"
        /// </summary>
        /// <param name="nome">Nome do SGBD. Os valores validos sao: ORACLE, SQLSERVER, MYSQL</param>
        /// <returns>A constante que representa o SGBD no sistema</returns>
        public static Sgbds GetSgbd(String nome)
        {
            if (GetSgbdNome(Sgbds.ORACLE).Equals(nome))
            {
                return Sgbds.ORACLE;
            }
            else if (GetSgbdNome(Sgbds.SQL_SERVER).Equals(nome))
            {
                return Sgbds.SQL_SERVER;
            }
            else if (GetSgbdNome(Sgbds.MYSQL).Equals(nome))
            {
                return Sgbds.MYSQL;
            }
            return Sgbds.NULL;
        }

        /// <summary>
        /// Chave do parametro de configuracao do Servico Web
        /// </summary>
        /// <param name="parametro">Constante de parametros do Servico Web</param>
        /// <returns>A chave do parametro informado</returns>
        public static String GetParametroChave(Parametros parametro)
        {
            if (parametro.Equals(Parametros.DB_REGIN_SGBD))
            {
                return "db.regin.sgbd";
            }
            else if (parametro.Equals(Parametros.DB_REQUERIMENTO_SGBD))
            {
                return "db.requerimento.sgbd";
            }
            else if (parametro.Equals(Parametros.DB_CONNECTION_STRING_MYSQL))
            {
                return "mysql";
            }
            else if (parametro.Equals(Parametros.DB_CONNECTION_STRING_ORACLE))
            {
                return "oracle";
            }
            else if (parametro.Equals(Parametros.DB_CONNECTION_STRING_SQLSERVER))
            {
                return "sqlserver";
            }
            else if (parametro.Equals(Parametros.SIGLA_UF))
            {
                return "sigla.uf";
            }
            return null;
        }

        public static Sgbds GetSgbdPorParametro(Parametros parametro)
        {
            return ConfiguracaoSistema.GetSgbd(ConfigurationManager.AppSettings[ConfiguracaoSistema.GetParametroChave(parametro)].ToString());
        }
    }

    /// <summary>
    /// Constante que define os tipos de protocolos
    /// </summary>
    public class TipoProtocolo
    {
        public enum Tipos
        {
            /// <summary>
            /// Protocolo do Requerimento Eletronico.
            /// Codigo: 1
            /// </summary>
            REQUERIMENTO,
            /// <summary>
            /// Protocolo de viabilidade.
            /// Codigo: 5
            /// </summary>
            VIABILIDADE,
            /// <summary>
            /// Protocolo de Legalizacao (Alvara).
            /// Codigo: 3
            /// </summary>
            LEGALIZACAO,
            /// <summary>
            /// Protocolo de Micro Empreendedor Individual.
            /// Codigo: 7
            /// </summary>
            MEI,
            /// <summary>
            /// Protocolo de Viabilidade de Micro Empreendedor Individual.
            /// Codigo: 8
            /// </summary>
            VIABILIDADE_MEI,
            /// <summary>
            /// Tipo de protocolo DBE da RFB.
            /// Codigo: 9
            /// </summary>
            DBE,
            /// <summary>
            /// Tipo de protocolo nao encontrado. Somente utilizado quando nao for possivel identificaro o tipo do protocolo.
            /// Codigo: 0
            /// </summary>
            NAO_ENCONTRADO
        }

        /// <summary>
        /// Nome do tipo de protocolo baseado na constante
        /// </summary>
        /// <param name="tipoProtocolo">Constante de tipos de protocolos</param>
        /// <returns>Nome do tipo de protocolo informado</returns>
        public static String GetNome(Tipos tipoProtocolo)
        {
            if (tipoProtocolo.Equals(Tipos.REQUERIMENTO))
            {
                return "Requerimento";
            }
            else if (tipoProtocolo.Equals(Tipos.VIABILIDADE))
            {
                return "Viabilidade";
            }
            else if (tipoProtocolo.Equals(Tipos.LEGALIZACAO))
            {
                return "Legalização";
            }
            else if (tipoProtocolo.Equals(Tipos.MEI))
            {
                return "MEI";
            }
            else if (tipoProtocolo.Equals(Tipos.VIABILIDADE_MEI))
            {
                return "Viabilidade MEI";
            }
            else if (tipoProtocolo.Equals(Tipos.DBE))
            {
                return "DBE";
            }
            else if (tipoProtocolo.Equals(Tipos.NAO_ENCONTRADO))
            {
                return "Não encontrado";
            }
            return null;
        }

        /// <summary>
        /// Codigo do tipo de protocolo
        /// </summary>
        /// <param name="tipoProtocolo">Constante de tipos de protocolos</param>
        /// <returns>Codigo do tipo de protocolo informado</returns>
        public static int GetCodigo(Tipos tipoProtocolo)
        {
            if (tipoProtocolo.Equals(Tipos.REQUERIMENTO))
            {
                return 1;
            }
            else if (tipoProtocolo.Equals(Tipos.VIABILIDADE))
            {
                return 5;
            }
            else if (tipoProtocolo.Equals(Tipos.LEGALIZACAO))
            {
                return 3;
            }
            else if (tipoProtocolo.Equals(Tipos.LEGALIZACAO))
            {
                return 3;
            }
            else if (tipoProtocolo.Equals(Tipos.MEI))
            {
                return 7;
            }
            else if (tipoProtocolo.Equals(Tipos.VIABILIDADE_MEI))
            {
                return 8;
            }
            else if (tipoProtocolo.Equals(Tipos.DBE))
            {
                return 9;
            }
            else if (tipoProtocolo.Equals(Tipos.NAO_ENCONTRADO))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Constante de tipo de protocolo baseado no codigo
        /// </summary>
        /// <param name="codigo">Inteiro que representa o codigo do tipo de protocolo</param>
        /// <returns>Constante do tipo de protocolo</returns>
        public static Tipos GetTipoPorCodigo(int codigo)
        {
            if (GetCodigo(Tipos.LEGALIZACAO).Equals(codigo))
            {
                return Tipos.LEGALIZACAO;
            }
            else if (GetCodigo(Tipos.VIABILIDADE).Equals(codigo))
            {
                return Tipos.VIABILIDADE;
            }
            else if (GetCodigo(Tipos.MEI).Equals(codigo))
            {
                return Tipos.MEI;
            }
            else if (GetCodigo(Tipos.VIABILIDADE_MEI).Equals(codigo))
            {
                return Tipos.VIABILIDADE_MEI;
            }
            else if (GetCodigo(Tipos.DBE).Equals(codigo))
            {
                return Tipos.DBE;
            }
            else
            {
                return Tipos.NAO_ENCONTRADO;
            }
        }
    }

    public class ConstantesServicoWeb
    {
        public enum CodigosRetorno
        {
            SUCESSO = 0,
            NAO_ENCONTRADO = 1,
            ERRO = 9
        }
    }

    public class FontesInformacao
    {
        public enum TipoFonteInformacao
        {
            REGIN = 1,
            ORGAO_REGISTRO = 2
        }

        public static TipoFonteInformacao GetTipoFonte(int codigo)
        {
            if (((int)TipoFonteInformacao.REGIN) == codigo)
            {
                return TipoFonteInformacao.REGIN;
            }
            else if (((int)TipoFonteInformacao.ORGAO_REGISTRO) == codigo)
            {
                return TipoFonteInformacao.ORGAO_REGISTRO;
            }
            else
            {
                return TipoFonteInformacao.REGIN;
            }
        }

        public static string GetNome(TipoFonteInformacao constante)
        {
            if (TipoFonteInformacao.REGIN.Equals(constante))
            {
                return "REGIN";
            }
            else if (TipoFonteInformacao.REGIN.Equals(constante))
            {
                return "ORGÃO DE REGISTRO";
            }
            else
            {
                return "REGIN";
            }
        }
    }

    public class Versao
    {
        public static string GetVersao()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

}
