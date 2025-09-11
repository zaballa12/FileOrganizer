using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio
{
    public class OrganizarPorFormato : IOrganizarPastaRN
    {
        public string Caminho { get; set; }
        private List<ArquivoModel> Arquivos { get; set; } 
        private List<PastaModel> Pastas { get; set; } 

        

        public void ObterArquivos()
        {

            throw new NotImplementedException();
        }

        public void ObterPastas()
        {
            throw new NotImplementedException();
        }

        public string Organizar()
        {
            ObterArquivos();
            ObterPastas();
            throw new NotImplementedException();
        }
    }
}
