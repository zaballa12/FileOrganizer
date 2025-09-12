using FileOrganizer.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.Services
{
    public class MapeadorTipoArquivoService
    {
        public string FormatoArquivo { get; set; }

        #region Lista das extenções
        private static readonly HashSet<string> ExtImagens = new HashSet<string>
        {
            "jpg", "jpeg", "png", "gif", "bmp", "tiff", "webp"
        };

        private static readonly HashSet<string> ExtDocumentos = new HashSet<string>
        {
            "pdf", "doc", "docx", "txt", "rtf", "csv", "xls", "xlsx", "ppt", "pptx"
        };

        private static readonly HashSet<string> ExtAudios = new HashSet<string>
        {
            "mp3", "wav", "aac", "flac", "ogg", "m4a"
        };

        private static readonly HashSet<string> ExtVideos = new HashSet<string>
        {
            "mp4", "mkv", "avi", "mov", "wmv"
        };

        private static readonly HashSet<string> ExtCompactados = new HashSet<string>
        {
            "zip", "rar", "7z", "tar", "gz"
        };
        #endregion

        public MapeadorTipoArquivoService(string formatoArquivo)
        {
            FormatoArquivo = formatoArquivo;
        }
        public Enumerados.TipoArquivo Mapear()
        {
            if (string.IsNullOrWhiteSpace(FormatoArquivo))
            {
                return Enumerados.TipoArquivo.Outros;
            }
            string ext = FormatoArquivo.TrimStart('.').ToLowerInvariant();

            if (ExtImagens.Contains(ext))
            {
                return Enumerados.TipoArquivo.Imagens;
            }
            else if (ExtDocumentos.Contains(ext))
            {
                return Enumerados.TipoArquivo.Documentos;
            }
            else if (ExtAudios.Contains(ext))
            {
                return Enumerados.TipoArquivo.Audios;
            }
            else if (ExtVideos.Contains(ext))
            {
                return Enumerados.TipoArquivo.Videos;
            }
            else if (ExtCompactados.Contains(ext))
            {
                return Enumerados.TipoArquivo.Compactados;
            }
            else
            {
                return Enumerados.TipoArquivo.Outros;
            }

        }
    }
}
