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
    public class BuscarDuplicadosSequencial : IBuscarDuplicados
    {
        public string Caminho { get; set; }
        private readonly List<BuscadorDuplicadosBase> _etapas;

        public BuscarDuplicadosSequencial(IEnumerable<BuscadorDuplicadosBase> etapas)
        {
            _etapas = etapas != null ? etapas.ToList() : new List<BuscadorDuplicadosBase>();
        }

        public List<GrupoDuplicados> Buscar()
        {
            if  (_etapas.Count == 0)
                return new List<GrupoDuplicados>();

            var leitor = new LeitorArquivosService(Caminho);
            var arquivos = leitor.LerRecursivo(); 

            if (arquivos == null || arquivos.Count == 0) 
                return new List<GrupoDuplicados>();

            var grupos = _etapas[0].Agrupar(arquivos);

            for (int i = 1; i < _etapas.Count; i++)
            {
                var proximo = new List<GrupoDuplicados>();
                foreach (var grupo in grupos)
                {
                    if (grupo.Arquivos == null || grupo.Arquivos.Count < 2)
                        continue;

                    var sub = _etapas[i].Agrupar(grupo.Arquivos);
                    foreach (var subgrupo in sub)
                    {
                        proximo.Add(new GrupoDuplicados
                        {
                            Chave = string.IsNullOrEmpty(grupo.Chave) ? subgrupo.Chave : $"{subgrupo.Chave}|{subgrupo.Chave}",
                            Valor = string.IsNullOrEmpty(grupo.Valor) ? subgrupo.Valor : $"{subgrupo.Valor}|{subgrupo.Valor}",
                            Arquivos = subgrupo.Arquivos
                        });
                    }
                }
                grupos = proximo;
                if (grupos.Count == 0) 
                    break;
            }

            return grupos != null ? grupos : new List<GrupoDuplicados>();
        }
    }
}
