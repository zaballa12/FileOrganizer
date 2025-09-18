using FileOrganizer.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model
{
    public class GrupoDuplicados
    {
        public string Chave { get; set; }              // ex.: "NOME", "HASH", "NOME|HASH"
        public string Valor { get; set; }              // o valor daquela chave (ex.: "RELATORIO.PDF", ou o hash)
        public List<ArquivoModel> Arquivos { get; set; } = new List<ArquivoModel>();
        public int Quantidade
        {
            get
            {
                return Arquivos == null ? 0 : Arquivos.Count;
            }
        }
    }

}
