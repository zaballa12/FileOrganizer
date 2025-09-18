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
    public class OrganizarPorData : OrganizadorBase
    {
        public override List<SugestaoMovimentacaoModel> GerarPrevia()
        {
            LeitorArquivosService leitor = new LeitorArquivosService(Caminho);
            List<ArquivoModel> arquivos = leitor.Ler();

            List<SugestaoMovimentacaoModel> sugestoes = new List<SugestaoMovimentacaoModel>();

            foreach (ArquivoModel arquivo in arquivos)
            {
                DateTime data = arquivo.DataCriacao;
                string ano = data.Year.ToString();
                string pastaDestino;

                string mes = data.Month.ToString("00");
                // Ex.: C:\Selecionada\2024\03
                pastaDestino = Path.Combine(Caminho, ano, mes);

                sugestoes.Add(new SugestaoMovimentacaoModel
                {
                    CaminhoOrigem = arquivo.Caminho, // caminho completo
                    CaminhoDestino = pastaDestino,    // só a pasta
                    NomeCompleto = arquivo.Nome     // nome + extensão (sem renomear)
                });
            }

            SugestaoMovimentacoes = sugestoes;
            return SugestaoMovimentacoes;
        }
    }
}

