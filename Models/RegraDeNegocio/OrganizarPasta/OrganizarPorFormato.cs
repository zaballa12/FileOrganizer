using FileOrganizer.Models.Model;
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
    public class OrganizarPorFormato : IOrganizarPasta
    {
        public string Caminho { get; set; }
        public List<SugestaoMovimentacaoModel> SugestaoMovimentacaos { get;set; }

        public void ConfirmarAlteracoes()
        {
            if (!SugestaoMovimentacaos.Any())
                return;

           foreach (SugestaoMovimentacaoModel sugestao in SugestaoMovimentacaos)
            {
                try
                {
                    if (!File.Exists(sugestao.CaminhoOrigem))
                        continue;

                    if (!Directory.Exists(sugestao.CaminhoDestino))
                        Directory.CreateDirectory(sugestao.CaminhoDestino);

                    string nomeFinal = sugestao.NomeCompleto;
                    if (string.IsNullOrWhiteSpace(nomeFinal))
                        nomeFinal = Path.GetFileName(sugestao.CaminhoOrigem);

                    string destinoCompleto = Path.Combine(sugestao.CaminhoDestino, nomeFinal);

                    // se já existe um arquivo com o mesmo nome, gera um nome único: "nome (1).ext", "nome (2).ext"...
                    if (File.Exists(destinoCompleto))
                    {
                        string nomeBase = Path.GetFileNameWithoutExtension(nomeFinal);
                        string extensao = Path.GetExtension(nomeFinal);
                        destinoCompleto = GerarDestinoUnico(sugestao.CaminhoDestino, nomeBase, extensao);
                    }

                    // move o arquivo
                    File.Move(sugestao.CaminhoOrigem, destinoCompleto);
                }
                catch (UnauthorizedAccessException)
                {
                    // sem permissão;
                    continue;
                }
                catch (PathTooLongException)
                {
                    // caminho grande demais;
                    continue;
                }
                catch (IOException)
                {
                    // erro de E/S (arquivo em uso, etc.);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public List<SugestaoMovimentacaoModel> GerarPrevia()
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
            this.SugestaoMovimentacaos = sugestoes;
            return sugestoes;
        }

        private string GerarDestinoUnico(string pastaDestino, string nomeBase, string extensao)
        {
            // incrementa contador até achar um nome livre
            int i = 1;
            while (true)
            {
                string sugestao = string.Format("{0} ({1}){2}", nomeBase, i, extensao);
                string caminho = Path.Combine(pastaDestino, sugestao);
                if (!File.Exists(caminho))
                {
                    return caminho;
                }
                i++;
            }
        }
    }
}
