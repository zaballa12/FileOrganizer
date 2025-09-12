using FileOrganizer.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.OrganizarPasta
{
    public interface IOrganizarPasta
    {
        //Propriedades
        public string Caminho { get; set; }
        public List<SugestaoMovimentacaoModel> SugestaoMovimentacaos { get; set; }

        //Métodos
        List<SugestaoMovimentacaoModel> GerarPrevia();
        void ConfirmarAlteracoes();

    }
}
