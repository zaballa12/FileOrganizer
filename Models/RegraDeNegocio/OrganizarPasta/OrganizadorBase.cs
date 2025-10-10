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
    public abstract class OrganizadorBase : IOrganizarPasta
    {
        
        public string Caminho { get; set; }
        public List<SugestaoMovimentacaoModel> SugestaoMovimentacoes { get; set; } = new List<SugestaoMovimentacaoModel>();

        public abstract List<SugestaoMovimentacaoModel> GerarPrevia();

        public void ConfirmarAlteracoes()
        {
            if (!SugestaoMovimentacoes.Any())
                return;

            foreach (SugestaoMovimentacaoModel sugestao in SugestaoMovimentacoes)
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

                    if (File.Exists(destinoCompleto))
                    {
                        string nomeBase = Path.GetFileNameWithoutExtension(nomeFinal);
                        string extensao = Path.GetExtension(nomeFinal);
                        destinoCompleto = NomeArquivoService.GerarDestinoUnico(sugestao.CaminhoDestino, nomeBase, extensao);
                    }

                    File.Move(sugestao.CaminhoOrigem, destinoCompleto);
                }
                catch (UnauthorizedAccessException) { continue; }
                catch (PathTooLongException) { continue; }
                catch (IOException) { continue; }
                catch (Exception) { continue; }
            }
        }
    }
}

