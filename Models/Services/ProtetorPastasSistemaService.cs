using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace FileOrganizer.Models.Services
{
    public class ProtetorPastasSistemaService 
    {
        private static readonly string[] PastasBloqueadas = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            Environment.GetFolderPath(Environment.SpecialFolder.SystemX86),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            //Environment.GetFolderPath(Environment.SpecialFolder.ProgramData),
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        };

        public bool IsPastaSistema(string caminho)
        {
            try
            {
                FileAttributes atributos = File.GetAttributes(caminho);

                if ((atributos & FileAttributes.System) != 0)
                {
                    return true;
                }

                if ((atributos & FileAttributes.ReparsePoint) != 0) // junction/symlink
                {
                    return true;
                }

                string absoluto = Path.GetFullPath(caminho)
                    .TrimEnd(Path.DirectorySeparatorChar)
                    .ToLowerInvariant();

                string raizSistema = Path.GetPathRoot(Environment.SystemDirectory)
                    .TrimEnd(Path.DirectorySeparatorChar)
                    .ToLowerInvariant();

                if (absoluto == raizSistema)
                {
                    return true; // ex.: C:\
                }

                foreach (string pasta in PastasBloqueadas)
                {
                    if (string.IsNullOrWhiteSpace(pasta))
                    {
                        continue;
                    }

                    string bloq = pasta.TrimEnd(Path.DirectorySeparatorChar).ToLowerInvariant();
                    if (absoluto.StartsWith(bloq))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        public bool PastaTemArquivos(string caminho)
        {
            try
            {
                return Directory.EnumerateFiles(caminho, "*", SearchOption.TopDirectoryOnly).Any();
            }
            catch
            {
                return false;
            }
        }
    }

}
