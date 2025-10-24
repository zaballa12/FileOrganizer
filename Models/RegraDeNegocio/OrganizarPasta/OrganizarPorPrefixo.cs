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
        private static readonly char[] Delimitadores = new[] { '_', ' ', '-' };
        private string GrupoCurto = "OUTROS";

        public override List<SugestaoMovimentacaoModel> GerarPrevia()
        {
            LeitorArquivosService leitor = new LeitorArquivosService(Caminho);
            List<ArquivoModel> arquivos = leitor.Ler();

            List<SugestaoMovimentacaoModel> sugestoes = new List<SugestaoMovimentacaoModel>();

            foreach (ArquivoModel arquivo in arquivos)
            {
                string nomeSemExt = Path.GetFileNameWithoutExtension(arquivo.Nome);
                string prefixo = ObterPrefixo(nomeSemExt);
                string grupo = string.IsNullOrWhiteSpace(prefixo) ? GrupoCurto : prefixo.ToUpperInvariant();

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

        private string ObterPrefixo(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            // remove delimitadores iniciais e espaços sobrando
            string limpo = nome.Trim().TrimStart(Delimitadores);
            if (string.IsNullOrEmpty(limpo))
                return null;

            // tenta até o primeiro delimitador interno
            int pos = limpo.IndexOfAny(Delimitadores);
            if (pos > 0)
                return limpo.Substring(0, pos);

            if (limpo.Length >= Tamanho)
                return limpo.Substring(0, Tamanho);

            // muito curto → manda para OUTROS
            return null;

        }
    }
}
