using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados
{
    internal class BuscarDuplicadosPorConteudo : BuscadorDuplicadosBase
    {
        //Pode mudar a forma como o hash é calculado sem alterar a classe.
        private readonly HashService _hash;

        public BuscarDuplicadosPorConteudo()
            : this(new HashService())
        {
        }

        public BuscarDuplicadosPorConteudo(HashService hashService)
        {
            _hash = hashService;
        }

        /// <summary>
        /// Tenta descobrir quais arquivos são duplicados comparando o hash de cada arquivo.
        /// Retorna grupos de arquivos que possuem o mesmo hash.
        /// </summary>
        public override List<GrupoDuplicados> Agrupar(List<ArquivoModel> arquivos)
        {
            // O mapa é um dicionário, onde a chave é o hash e o valor é a lista de arquivos que têm esse mesmo hash.
            // Foi usado dicionário para deixa a busca de um hash mais rápida, ContainsKey().
            Dictionary<string, List<ArquivoModel>> mapa = new Dictionary<string, List<ArquivoModel>>(StringComparer.OrdinalIgnoreCase);

            foreach (ArquivoModel arquivo in arquivos)
            {
                string valorHash;
                try
                {
                    valorHash = _hash.HashCompletoSha256(arquivo.Caminho);
                }
                catch
                {
                    // Falhou ao hashear (sem permissão, em uso etc.). Ignora este arquivo.
                    continue;
                }

                if (!mapa.ContainsKey(valorHash))
                    mapa[valorHash] = new List<ArquivoModel>();

                mapa[valorHash].Add(arquivo);
            }

            // Transforma o mapa (dicionário) em uma lista de grupos duplicados, caso dois ou mais aquivos possuam o mesmo hash.
            List<GrupoDuplicados> grupos = new List<GrupoDuplicados>();

            foreach (KeyValuePair<string, List<ArquivoModel>> par in mapa)
            {
                List<ArquivoModel> lista = par.Value;
                if (lista != null && lista.Count >= 2)
                {
                    lista = lista.OrderBy(a => a.DataCriacao).ToList();
                    lista.First().Selecionado = false; 

                    GrupoDuplicados grupo = new GrupoDuplicados();
                    grupo.Chave = "HASH";
                    grupo.Valor = par.Key;
                    grupo.Arquivos = lista;
                    grupos.Add(grupo);
                }
            }

            return grupos;
        }
    }
}
