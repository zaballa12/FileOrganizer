using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio
{
    public interface IOrganizarPastaRN
    {
        public string Caminho { get; set; }

        string Organizar();
        void ObterArquivos();
        void ObterPastas();

    }
}
