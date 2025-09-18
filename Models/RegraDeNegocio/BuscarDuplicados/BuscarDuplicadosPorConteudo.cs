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
        private readonly HashService _hash;

        public BuscarDuplicadosPorConteudo()
            : this(new HashService())
        {
        }

        public BuscarDuplicadosPorConteudo(HashService hashService)
        {
            _hash = hashService;
        }

        public override List<GrupoDuplicados> Agrupar(List<ArquivoModel> arquivos)
        {
            Dictionary<string, List<ArquivoModel>> mapa = new Dictionary<string, List<ArquivoModel>>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < arquivos.Count; i++)
            {
                ArquivoModel a = arquivos[i];

                string valorHash;
                try
                {
                    valorHash = _hash.HashCompletoSha256(a.Caminho);
                }
                catch
                {
                    // Falhou ao hashear (sem permissão, em uso etc.). Ignora este arquivo.
                    continue;
                }

                if (!mapa.ContainsKey(valorHash))
                    mapa[valorHash] = new List<ArquivoModel>();

                mapa[valorHash].Add(a);
            }

            List<GrupoDuplicados> grupos = new List<GrupoDuplicados>();

            foreach (KeyValuePair<string, List<ArquivoModel>> par in mapa)
            {
                List<ArquivoModel> lista = par.Value;
                if (lista != null && lista.Count >= 2)
                {
                    GrupoDuplicados g = new GrupoDuplicados();
                    g.Chave = "HASH";
                    g.Valor = par.Key;
                    g.Arquivos = lista;
                    grupos.Add(g);
                }
            }

            return grupos;
        }
    }
}
