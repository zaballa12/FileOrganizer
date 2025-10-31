using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados
{
    internal class BuscarDuplicadosPorNome : BuscadorDuplicadosBase
    {
        /// <summary>
        /// Tenta descobrir quais arquivos são duplicados comparando o nome base (ignora sufixos do windows ex: "- Copia (1)") de cada arquivo.
        /// Retorna grupos de arquivos que possuem o mesmo hash.
        /// </summary>
        public override List<GrupoDuplicados> Agrupar(List<ArquivoModel> arquivos)
        {
            Dictionary<string, List<ArquivoModel>> mapa = new Dictionary<string, List<ArquivoModel>>(StringComparer.OrdinalIgnoreCase);

            foreach (ArquivoModel arquivo in arquivos)
            {
                string nomeOriginal = RetornaNomeBase(arquivo);

                if (!mapa.ContainsKey(nomeOriginal))
                    mapa[nomeOriginal] = new List<ArquivoModel>();

                mapa[nomeOriginal].Add(arquivo);
            }

            List<GrupoDuplicados> grupos = new List<GrupoDuplicados>();

            foreach (KeyValuePair<string, List<ArquivoModel>> par in mapa)
            {
                List<ArquivoModel> lista = par.Value;
                if (lista != null && lista.Count >= 2)
                {
                    lista = lista.OrderBy(a => a.DataCriacao).ToList();
                    lista.First().Selecionado = false;
                    GrupoDuplicados grupo = new GrupoDuplicados();
                    grupo.Chave = "NOME";
                    grupo.Valor = par.Key;
                    grupo.Arquivos = lista;
                    grupos.Add(grupo);
                }
            }

            return grupos;
        }
        private string RemoverSufixoCopiaWindows(string nomeArquivoSemExt)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivoSemExt))
                return nomeArquivoSemExt;

            string resultado = nomeArquivoSemExt.Trim();

            // Padrões:
            // 1) " - Cópia" / " - Copia" / " - Copy" com ou sem "(n)"
            resultado = Regex.Replace(
                resultado,
                @"\s*-\s*(c[oó]pia|copia|copy)\s*(\(\d+\))?$",
                "",
                RegexOptions.IgnoreCase
            );

            // 2) Sufixo simples "(n)" no final, ex.: "foto (2)"
            resultado = Regex.Replace(
                resultado,
                @"\s*\(\d+\)$",
                "",
                RegexOptions.IgnoreCase
            );

            return resultado.Trim();
        }

        private string RetornaNomeBase(ArquivoModel arquivo)
        {
            string ext = Path.GetExtension(arquivo.Nome);
            string semExt = Path.GetFileNameWithoutExtension(arquivo.Nome);

            string baseLimpa = RemoverSufixoCopiaWindows(semExt);

            string nomeBase = baseLimpa + (string.IsNullOrEmpty(ext) ? "" : ext.ToLowerInvariant());
            return NormalizarNome(nomeBase);
        }
    }
}
