using FileOrganizer.Models.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.Services
{
    internal class LeitorArquivosService
    {
        //Propriedades
        public string Caminho { get; set; }

        //Construtor
        public LeitorArquivosService(string caminho)
        {
            Caminho = caminho;
        }
        //Métodos
        public List<ArquivoModel> Ler()
        {
            List<ArquivoModel> lista = new List<ArquivoModel>();

            IEnumerable<string> arquivos;
            try
            {
                // Apenas nível atual (sem subpastas)
                arquivos = Directory.EnumerateFiles(Caminho, "*", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                throw new Exception("Erro ao ler arquivo da pasta");
            }

            foreach (string caminhoArquivo in arquivos)
            {
                try
                {
                    FileInfo fi = new FileInfo(caminhoArquivo);

                    // Pula arquivo de sistema/oculto
                    if ((fi.Attributes & FileAttributes.System) != 0)
                        continue;
                    if ((fi.Attributes & FileAttributes.Hidden) != 0)
                        continue;

                    ArquivoModel arquivoModel = new ArquivoModel();

                    arquivoModel.Nome = fi.Name;

                    arquivoModel.DataCriacao = fi.CreationTime;
                    arquivoModel.DataAlteracao = fi.LastWriteTime;

                    arquivoModel.Tamanho = FormatarTamanho(fi.Length);

                    arquivoModel.Caminho = fi.FullName;
                    arquivoModel.Versao = string.Empty; // se não usar, deixe vazio

                    string ext = fi.Extension; // ex.: ".pdf"
                    if (!string.IsNullOrEmpty(ext))
                    {
                        arquivoModel.Formato = ext.TrimStart('.').ToLowerInvariant();
                    }
                    else
                    {
                        arquivoModel.Formato = string.Empty;
                    }

                    lista.Add(arquivoModel);
                }
                catch
                {
                    throw new Exception("Erro ao ler informações do arquivo.");
                }
            }
            return lista;
        }

        private string FormatarTamanho(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB) return string.Format("{0:0.##} GB", (double)bytes / GB);
            if (bytes >= MB) return string.Format("{0:0.##} MB", (double)bytes / MB);
            if (bytes >= KB) return string.Format("{0:0.##} KB", (double)bytes / KB);
            return bytes + " B";
        }
    }
}
