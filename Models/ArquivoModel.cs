using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models
{
    public class ArquivoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public FormatoArquivo Formato { get; set; }
        public string Tamanho { get; set; }
        public string Caminho { get; set; }
        public string Versao { get; set; }  

        public enum FormatoArquivo
        {
            png,
            jpg,
            pdf,
            doc,
            dll,
            log,
            xml,
            config,
            txt,
            xls,
            mp3,
            mp4,
            bat,
            docx
        }
    }
}
