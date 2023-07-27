using regin_app_mobile.Constante;
using regin_app_mobile.GeracaoXml;
using System.Collections.Generic;
using System.Data;

namespace regin_app_mobile.Dao
{
    /// <summary>
    /// Interface utilizada para padronizar a consulta de processos nas juntas comerciais devido a diferenca de SGBDs
    /// </summary>
    public interface IReginDao
    {
        bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao);

        TipoProtocolo.Tipos PesquisaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexaov);

        List<Processo> PesquisaProtocoloRegin(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao);

        List<Processo> PesquisaProtocoloOrgaoRegistro(string siglaUf, string protocolo, IDbCommand iDbCommand, bool fechaConexao);

        Processo PesquisaEventos(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao);

        Processo PesquisaEventosOrgaoRegistro(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao);

        Processo PesquisaOrgaoRegistroAnalise(string uf, Processo processo, IDbCommand iDbCommand, bool fechaConexao);

        InstituicaoAnalise PesquisaOrgaoRegistroAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao);

        Processo PesquisaInstituicoesAnalise(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao);

        InstituicaoAnalise PesquisaInstituicoesAndamento(string protocolo, InstituicaoAnalise instituicaoAnalise, IDbCommand iDbCommand, bool fechaConexao);

        Processo PesquisaExigenciasProcesso(string protocolo, Processo processo, IDbCommand iDbCommand, bool fechaConexao);

        Exigencia PesquisaExigenciasOutrosProcesso(string protocolo, string sequenciaExigencia, Exigencia exigencia, IDbCommand iDbCommand, bool fechaConexao);

        Andamento PesquisaExigenciasAndamento(string protocolo, Andamento andamento, IDbCommand iDbCommand, bool fechaConexao);

    }

    /// <summary>
    /// Fabrica de instancias de implementacoes da interface IReginDao
    /// </summary>
    public static class FabricaReginDao
    {
        public static IReginDao GetFabricaDao(ConfiguracaoSistema.Sgbds sgbd)
        {
            if (sgbd.Equals(ConfiguracaoSistema.Sgbds.ORACLE))
            {
                return new ReginOracleImplDao();
            }
            else if (sgbd.Equals(ConfiguracaoSistema.Sgbds.SQL_SERVER))
            {
                return new ReginSqlServerImplDao();
            }
            else
            {
                return null;
            }
        }
    }

    public interface IRequerimentoDao
    {
        bool ExisteProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao);

        TipoProtocolo.Tipos ConsultaTipoProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao);

        Requerimento ConsultaProtocolo(string protocolo, IDbCommand iDbCommand, bool fechaConexao);
    }

    /// <summary>
    /// Fabrica de instancias de implementacoes da interface IReginDao
    /// </summary>
    public static class FabricarequerimentoDao
    {
        public static IRequerimentoDao GetFabricaDao(ConfiguracaoSistema.Sgbds sgbd)
        {
            if (sgbd.Equals(ConfiguracaoSistema.Sgbds.MYSQL))
            {
                return new RequerimentoMySQLImplDao();
            }
            else
            {
                return null;
            }
        }
    }
}
