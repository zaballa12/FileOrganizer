using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados
{
    internal class BuscarDuplicadosPorNome : BuscadorDuplicadosBase
    {
        public override List<GrupoDuplicados> Agrupar(List<ArquivoModel> arquivos)
        {
            Dictionary<string, List<ArquivoModel>> mapa = new Dictionary<string, List<ArquivoModel>>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < arquivos.Count; i++)
            {
                ArquivoModel a = arquivos[i];
                string chave = NormalizarNome(a.Nome);

                if (!mapa.ContainsKey(chave))
                    mapa[chave] = new List<ArquivoModel>();

                mapa[chave].Add(a);
            }

            List<GrupoDuplicados> grupos = new List<GrupoDuplicados>();

            foreach (KeyValuePair<string, List<ArquivoModel>> par in mapa)
            {
                List<ArquivoModel> lista = par.Value;
                if (lista != null && lista.Count >= 2)
                {
                    GrupoDuplicados g = new GrupoDuplicados();
                    g.Chave = "NOME";
                    g.Valor = par.Key;
                    g.Arquivos = lista;
                    grupos.Add(g);
                }
            }

            return grupos;
        }
    }
}
