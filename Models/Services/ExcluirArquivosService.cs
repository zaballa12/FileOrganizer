using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO; 

namespace FileOrganizer.Models.Services
{
    public class ExcluirArquivosService
    {
        /// <summary>
        /// Envia arquivos/pastas para a Lixeira.
        /// Retorna (sucessos, falhas).
        /// </summary>
        public (int Sucesso, int Falhas) EnviarParaLixeira(IEnumerable<string> caminhos)
        {
            if (caminhos == null) return (0, 0);

            int sucesso = 0;
            int falhas = 0;

            foreach (var caminho in caminhos)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(caminho))
                    {
                        falhas++;
                        continue;
                    }

                    if (File.Exists(caminho))
                    {
                        FileSystem.DeleteFile(
                            caminho,
                            UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin
                        );
                        sucesso++;
                    }
                    else if (Directory.Exists(caminho))
                    {
                        FileSystem.DeleteDirectory(
                            caminho,
                            UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin
                        );
                        sucesso++;
                    }
                    else
                    {
                        // caminho não existe mais
                        falhas++;
                    }
                }
                catch
                {
                    falhas++;
                }
            }

            return (sucesso, falhas);
        }
    }
}
