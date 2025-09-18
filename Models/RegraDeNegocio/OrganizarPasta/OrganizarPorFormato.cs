using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.OrganizarPasta.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.Models.RegraDeNegocio.OrganizarPasta
{
    public class OrganizarPorFormato : OrganizadorBase
    {
        public override List<SugestaoMovimentacaoModel> GerarPrevia()
        {
            LeitorArquivosService leitor = new LeitorArquivosService(Caminho);

            List<ArquivoModel> arquivos = leitor.Ler();

            List<SugestaoMovimentacaoModel> sugestoes = new List<SugestaoMovimentacaoModel>();

            foreach (ArquivoModel arquivo in arquivos) 
            {
                MapeadorTipoArquivoService mapeador = new MapeadorTipoArquivoService(arquivo.Formato);
                TipoArquivo tipoArquivo = mapeador.Mapear();

                string pastaDestino = Path.Combine(Caminho, tipoArquivo.ToString());
                SugestaoMovimentacaoModel sugestao = new SugestaoMovimentacaoModel { 
                CaminhoOrigem = arquivo.Caminho,
                CaminhoDestino = pastaDestino,
                NomeCompleto = arquivo.Nome
                };

                sugestoes.Add(sugestao);
            }

            SugestaoMovimentacoes = sugestoes;
            return SugestaoMovimentacoes;
        }
    }
}
