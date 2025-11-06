using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos;
using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos.Model;
using FileOrganizer.Models.Services;
using System.Collections.Generic;
using System.IO;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.Models.RegraDeNegocio.Renomear
{
    public class RenomearConfiguravel : RenomeadorBase
    {
        public override List<SugestaoRenomeacaoModel> GerarPrevia()
        {
            List<SugestaoRenomeacaoModel> lista = new List<SugestaoRenomeacaoModel>();
            LeitorArquivosService leitor = new LeitorArquivosService(Caminho);
            List<ArquivoModel> arquivos = leitor.Ler();

            foreach (var arquivo in arquivos) { 

                string nomeAtual = arquivo.Nome; // ex.: "colonia.png"
                string nomeSemExt = Path.GetFileNameWithoutExtension(nomeAtual);
                string extensao = Path.GetExtension(nomeAtual); // ex.: ".png"

                string prefixo = CalcularPrefixo(arquivo);
                string sufixo = CalcularSufixo(arquivo);

                string novoSemExt = string.Concat(prefixo, nomeSemExt, sufixo);
                string novoNome = string.Concat(novoSemExt, extensao);

                SugestaoRenomeacaoModel sugestao = new SugestaoRenomeacaoModel();
                sugestao.CaminhoCompleto = arquivo.Caminho;
                sugestao.NomeAtual = nomeAtual;
                sugestao.NomeNovo = novoNome;

                lista.Add(sugestao);
            }

            this.SugestaoMovimentacoes = lista;
            return lista;
        }

        private string CalcularPrefixo(ArquivoModel arquivo)
        {
            if (ModoPrefixo == ModoRenomear.Texto)
            {
                if (string.IsNullOrEmpty(PrefixoTexto))
                    return string.Empty;
                return PrefixoTexto; // o usuário decide se coloca "_" no fim
            }

            if (ModoPrefixo == ModoRenomear.Extensao)
            {
                MapeadorTipoArquivoService map = new MapeadorTipoArquivoService(arquivo.Formato);
                TipoArquivo tipoArquivo = map.Mapear();
                string rotulo = RotuloCurtoPorTipo(tipoArquivo);
                if (string.IsNullOrEmpty(rotulo))
                    return string.Empty;
                return rotulo + "_";
            }

            return string.Empty;
        }

        private string CalcularSufixo(ArquivoModel arquivo)
        {
            if (ModoSufixo == ModoRenomear.Texto)
            {
                if (string.IsNullOrEmpty(SufixoTexto))
                    return string.Empty;
                return SufixoTexto; // o usuário decide se coloca "_" no começo
            }

            if (ModoSufixo == ModoRenomear.Extensao)
            {
                MapeadorTipoArquivoService map = new MapeadorTipoArquivoService(arquivo.Formato);
                TipoArquivo tipoArquivo = map.Mapear();
                string rotulo = RotuloCurtoPorTipo(tipoArquivo);
                if (string.IsNullOrEmpty(rotulo))
                    return string.Empty;
                return "_" + rotulo;
            }

            return string.Empty;
        }

        private string RotuloCurtoPorTipo(TipoArquivo tipo)
        {
            if (tipo == TipoArquivo.Imagens) return "img";
            if (tipo == TipoArquivo.Documentos) return "doc";
            if (tipo == TipoArquivo.Audios) return "aud";
            if (tipo == TipoArquivo.Videos) return "vid";
            if (tipo == TipoArquivo.Compactados) return "zip";
            return "out";
        }
    }
}
