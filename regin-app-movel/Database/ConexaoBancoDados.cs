using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using regin_app_movel.Constante;

namespace regin_app_movel.Database
{
    /// <summary>
    /// Fabrica de instancias necessarias para conexao a um banco de dados.
    /// </summary>
    public static class FabricaBancoDados
    {
        /// <summary>
        /// Fabrica conexoes com o banco de dados especifica baseado no SGBD informado.
        /// </summary>
        /// <param name="sgbd">Constante que indica o SGBD que se deseja criar a conexao</param>
        /// <returns>Conexao com o sgbd escolhido</returns>
        public static IDbConnection FabricaConexao(ConfiguracaoSistema.Sgbds sgbd)
        {
            if (ConfiguracaoSistema.Sgbds.ORACLE.Equals(sgbd))
            {
                return new OracleConnection(ConfigurationManager.ConnectionStrings[ConfiguracaoSistema.GetParametroChaveSgbd(sgbd)].ConnectionString);
            }
            else if (ConfiguracaoSistema.Sgbds.MYSQL.Equals(sgbd))
            {
                return new MySqlConnection(ConfigurationManager.ConnectionStrings[ConfiguracaoSistema.GetParametroChaveSgbd(sgbd)].ConnectionString);
            }
            else if (ConfiguracaoSistema.Sgbds.SQL_SERVER.Equals(sgbd))
            {
                return new MySqlConnection(ConfigurationManager.ConnectionStrings[ConfiguracaoSistema.GetParametroChaveSgbd(sgbd)].ConnectionString);
            }
            else
            {
                return null;
            }
        }

    }

    /// <summary>
    /// Gerencia a conexao com o banco de dados
    /// </summary>
    public class ConexaoBancoDados
    {
        /// <summary>
        /// Constante que define o SGBD do banco de dados a que essa coenxao pertence.
        /// </summary>
        private readonly ConfiguracaoSistema.Sgbds sgbd;

        private readonly IDbConnection conexao = null;

        private IDbTransaction transacao = null;

        public static ConexaoBancoDados MontaConexao(ConfiguracaoSistema.Sgbds sgbd)
        {
            return new ConexaoBancoDados(sgbd);
        }

        /// <summary>
        /// Construtor responsavel por inicializar os objetos necessarios para conexao com o banco de dados.
        /// </summary>
        /// <param name="sgbd">Constante que indica o SGBD que se deseja criar a conexao</param>
        private ConexaoBancoDados(ConfiguracaoSistema.Sgbds sgbd)
        {
            this.sgbd = sgbd;
            this.conexao = FabricaBancoDados.FabricaConexao(this.sgbd);
        }

        /// <summary>
        /// Abre a conexao com o banco de dados
        /// </summary>
        public void AbreConexao()
        {
            conexao.Open();
        }

        /// <summary>
        /// Inicia uma transacao no banco de dados
        /// </summary>
        public void IniciaTransacao()
        {
            transacao = conexao.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Criar uma instancia de execucao de comandos no banco de dados
        /// </summary>
        /// <returns></returns>
        public IDbCommand CriaComando()
        {
            return conexao.CreateCommand();
        }

        /// <summary>
        /// Comita as alteracoes realizadas no banco de dados
        /// </summary>
        public void ComitaTransacao()
        {
            transacao.Commit();
        }

        /// <summary>
        /// Desfaz (rollback) as alteracoes realizadas no banco de dados
        /// </summary>
        public void DesfazTransacao()
        {
            transacao.Rollback();
        }

        /// <summary>
        /// Fecha a conexao com o banco de dados
        /// </summary>
        public void FecharConexao()
        {
            conexao.Close();
        }

        public IDbConnection Conexao
        {
            get { return conexao; }
        }
    }
}
