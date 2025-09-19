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
        public string Caminho { get; set; }

        public LeitorArquivosService(string caminho)
        {
            Caminho = caminho;
        }

        /// <summary>
        /// Le arquivos da pasta em questão;
        /// </summary>
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
                    FileInfo infoArquivo = new FileInfo(caminhoArquivo);

                    // Pula arquivo de sistema/oculto
                    if ((infoArquivo.Attributes & FileAttributes.System) != 0)
                        continue;
                    if ((infoArquivo.Attributes & FileAttributes.Hidden) != 0)
                        continue;

                    ArquivoModel arquivoModel = new ArquivoModel();

                    arquivoModel.Nome = infoArquivo.Name;

                    arquivoModel.DataCriacao = infoArquivo.CreationTime;
                    arquivoModel.DataAlteracao = infoArquivo.LastWriteTime;

                    arquivoModel.Tamanho = FormatarTamanho(infoArquivo.Length);

                    arquivoModel.Caminho = infoArquivo.FullName;
                    arquivoModel.Versao = string.Empty; // se não usar, deixe vazio

                    string ext = infoArquivo.Extension; // ex.: ".pdf"
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

        /// <summary>
        /// Le arquivos da pasta em questão e arquivos que estão nas subpastas, de forma recusiva;
        /// </summary>
        public List<ArquivoModel> LerRecursivo()
        {
            List<ArquivoModel> lista = new List<ArquivoModel>();

            // Iterativo (stack) para evitar recursão profunda
            Stack<string> pilha = new Stack<string>();
            pilha.Push(Caminho);

            while (pilha.Count > 0)
            {
                string pastaAtual = pilha.Pop();

                // 1) Arquivos da pasta atual
                try
                {
                    IEnumerable<string> arquivos = Directory.EnumerateFiles(pastaAtual, "*", SearchOption.TopDirectoryOnly);

                    foreach (string caminhoArquivo in arquivos)
                    {
                        try
                        {
                            FileInfo infoArquivo = new FileInfo(caminhoArquivo);

                            // pula arquivos ocultos/sistema
                            if ((infoArquivo.Attributes & FileAttributes.System) != 0) continue;
                            if ((infoArquivo.Attributes & FileAttributes.Hidden) != 0) continue;

                            ArquivoModel modelo = new ArquivoModel();
                            modelo.Nome = infoArquivo.Name;
                            modelo.DataCriacao = infoArquivo.CreationTime;
                            modelo.DataAlteracao = infoArquivo.LastWriteTime;
                            modelo.Tamanho = FormatarTamanho(infoArquivo.Length);
                            modelo.Caminho = infoArquivo.FullName;
                            modelo.Versao = string.Empty;

                            string ext = infoArquivo.Extension;
                            if (!string.IsNullOrEmpty(ext))
                                modelo.Formato = ext.TrimStart('.').ToLowerInvariant();
                            else
                                modelo.Formato = string.Empty;

                            lista.Add(modelo);
                        }
                        catch
                        {
                            // problema ao ler este arquivo específico → ignora e continua
                            continue;
                        }
                    }
                }
                catch
                {
                    // sem permissão ou erro ao enumerar arquivos desta pasta → ignora e segue
                }

                // 2) Subpastas (empilha para continuar varredura)
                try
                {
                    IEnumerable<string> subpastas = Directory.EnumerateDirectories(pastaAtual, "*", SearchOption.TopDirectoryOnly);

                    foreach (string subpasta in subpastas)
                    {
                        try
                        {
                            DirectoryInfo infoDiretorio = new DirectoryInfo(subpasta);

                            // pula pastas ocultas/sistema e reparse points (symlink/junction) para evitar loops
                            FileAttributes atributos = infoDiretorio.Attributes;
                            if ((atributos & FileAttributes.ReparsePoint) != 0) continue;
                            if ((atributos & FileAttributes.System) != 0) continue;
                            if ((atributos & FileAttributes.Hidden) != 0) continue;

                            pilha.Push(subpasta);
                        }
                        catch
                        {
                            // problema ao acessar esta subpasta → ignora e continua
                            continue;
                        }
                    }
                }
                catch
                {
                    // erro ao enumerar subpastas → ignora e segue
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
