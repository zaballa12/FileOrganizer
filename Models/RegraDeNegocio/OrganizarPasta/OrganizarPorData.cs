using FileOrganizer.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.OrganizarPasta
{
    public class OrganizarPorData : IOrganizarPasta
    {
        public string Caminho { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<SugestaoMovimentacaoModel> SugestaoMovimentacaos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ConfirmarAlteracoes()
        {
            throw new NotImplementedException();
        }

        public List<SugestaoMovimentacaoModel> GerarPrevia()
        {
            throw new NotImplementedException();
        }
    }
}

