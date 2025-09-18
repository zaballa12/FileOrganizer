using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados
{
    public abstract class BuscadorDuplicadosBase : IBuscarDuplicados
    {
        public string Caminho { get; set; }
        public List<GrupoDuplicados> Grupos { get; set; } = new List<GrupoDuplicados>();

        protected virtual string NormalizarNome(string nomeComExtensao)
        {
            if (string.IsNullOrWhiteSpace(nomeComExtensao))
                return string.Empty;

            return nomeComExtensao.Trim().ToUpperInvariant();
        }

        protected virtual List<ArquivoModel> LerTodosArquivos()
        {
            var leitor = new LeitorArquivosService(Caminho);
            return leitor.LerRecursivo();
        }

        public List<GrupoDuplicados> Buscar()
        {
            try
            {
                List<ArquivoModel> arquivos = LerTodosArquivos();

                if (arquivos == null || arquivos.Count == 0)
                    return new List<GrupoDuplicados>();

                List<GrupoDuplicados> grupos = Agrupar(arquivos);
                if (grupos == null)
                    grupos = new List<GrupoDuplicados>();

                return grupos;
            }
            catch
            {
                return new List<GrupoDuplicados>();
            }
        }

        public abstract List<GrupoDuplicados> Agrupar(List<ArquivoModel> arquivos);
    }
}
