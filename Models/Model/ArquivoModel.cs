using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.Models.Model
{
    public class ArquivoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Formato { get; set; }
        public string Tamanho { get; set; }
        public string Caminho { get; set; }
        public string Versao { get; set; }
        public bool Selecionado { get; set; } = true; // default true

    }
}
