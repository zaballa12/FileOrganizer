using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.OrganizarPasta.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.OrganizarPasta
{
    public class OrganizarPorPrefixo : OrganizadorBase
    {
        // Configurações da estratégia
        private int Tamanho = 3; 

        private string GrupoCurto = "OUTROS";

        public override List<SugestaoMovimentacaoModel> GerarPrevia()
        {
            LeitorArquivosService leitor = new LeitorArquivosService(Caminho);
            List<ArquivoModel> arquivos = leitor.Ler();

            List<SugestaoMovimentacaoModel> sugestoes = new List<SugestaoMovimentacaoModel>();

            foreach(ArquivoModel arquivo in arquivos)
            {
                string nomeSemExt = Path.GetFileNameWithoutExtension(arquivo.Nome);
                string grupo;

                if (string.IsNullOrWhiteSpace(nomeSemExt) || nomeSemExt.Length < Tamanho)
                {
                    grupo = GrupoCurto;
                }
                else
                {
                    grupo = nomeSemExt.Substring(0, Tamanho);
                    grupo = grupo.ToUpperInvariant();
                }
                string pastaDestino = Path.Combine(Caminho, grupo);

                SugestaoMovimentacaoModel sugestao = new SugestaoMovimentacaoModel();
                sugestao.CaminhoOrigem = arquivo.Caminho; // caminho completo do arquivo
                sugestao.CaminhoDestino = pastaDestino;    // só a pasta
                sugestao.NomeCompleto = arquivo.Nome;    // nome + extensão (sem renomear)

                sugestoes.Add(sugestao);
            }

            SugestaoMovimentacoes = sugestoes;
            return SugestaoMovimentacoes;
        }
    }
}
