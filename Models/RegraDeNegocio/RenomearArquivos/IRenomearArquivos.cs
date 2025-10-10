using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.Models.RegraDeNegocio.RenomearArquivos
{
    public interface IRenomearArquivos
    {
        //Propriedades
        public string Caminho { get; set; }
        public string PrefixoTexto { get; set; }
        public string SufixoTexto { get; set; }
        public ModoRenomear ModoPrefixo { get; set; }
        public ModoRenomear ModoSufixo { get; set; }
        public List<SugestaoRenomeacaoModel> SugestaoMovimentacoes { get; set; }

        //Métodos
        List<SugestaoRenomeacaoModel> GerarPrevia();
        void ConfirmarAlteracoes();

    }
}
