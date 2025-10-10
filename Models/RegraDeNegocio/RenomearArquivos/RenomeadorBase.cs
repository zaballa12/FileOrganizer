using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.Models.RegraDeNegocio.RenomearArquivos
{
    public abstract class RenomeadorBase : IRenomearArquivos
    {
        public string Caminho { get; set; }
        public List<SugestaoRenomeacaoModel> SugestaoMovimentacoes { get; set; } = new List<SugestaoRenomeacaoModel>();
        public ModoRenomear ModoPrefixo { get; set; }

        public ModoRenomear ModoSufixo { get; set; }

        public string PrefixoTexto { get; set; }

        public string SufixoTexto { get; set; }

        public abstract List<SugestaoRenomeacaoModel> GerarPrevia();

        public void ConfirmarAlteracoes()
        {
            if (!SugestaoMovimentacoes.Any())
                return;

            foreach (SugestaoRenomeacaoModel sugestao in SugestaoMovimentacoes)
            {
                try
                {
                    string nomeFinalSanitizado = NomeArquivoService.SanitizarNomeArquivo(sugestao.NomeNovo);

                    string pastaOrigem = Path.GetDirectoryName(sugestao.CaminhoCompleto);
                    string caminhoDestino = Path.Combine(pastaOrigem, nomeFinalSanitizado);
                    // Se já existe arquivo com o nome final, gera nome único "nome (1).ext", "nome (2).ext", ...
                    if (File.Exists(caminhoDestino))
                    {
                        string nomeBase = Path.GetFileNameWithoutExtension(nomeFinalSanitizado);
                        string extensao = Path.GetExtension(nomeFinalSanitizado);
                        caminhoDestino = NomeArquivoService.GerarDestinoUnico(pastaOrigem, nomeBase, extensao);
                    }

                    File.Move(sugestao.CaminhoCompleto, caminhoDestino);
                }
                catch (UnauthorizedAccessException) { continue; }
                catch (PathTooLongException) { continue; }
                catch (IOException) { continue; }
                catch (Exception) { continue; }
            }
        }
    }
}
