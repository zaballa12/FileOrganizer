using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.RenomearArquivos.Model
{
    public class SugestaoRenomeacaoModel
    {
        public string CaminhoCompleto { get; set; }
        public string NomeAtual { get; set; }  // ex.: "foto.png"
        public string NomeNovo { get; set; }  // ex.: "img_foto.png"

    }
}
