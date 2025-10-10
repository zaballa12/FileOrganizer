using System;
using System.IO;

namespace FileOrganizer.Models.Services
{
    public static class NomeArquivoService
    {
        public static string GerarDestinoUnico(string pastaDestino, string nomeBase, string extensao)
        {
            if (string.IsNullOrWhiteSpace(pastaDestino))
                throw new ArgumentException("Pasta destino inválida.", nameof(pastaDestino));

            if (!Directory.Exists(pastaDestino))
                Directory.CreateDirectory(pastaDestino);

            int contador = 1;
            while (true)
            {
                string candidato = string.Format("{0} ({1}){2}", nomeBase, contador, extensao);
                string caminhoCandidato = Path.Combine(pastaDestino, candidato);

                if (!File.Exists(caminhoCandidato))
                    return caminhoCandidato;

                contador = contador + 1;
            }
        }

        public static string SanitizarNomeArquivo(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return nomeArquivo;

            char[] invalidos = Path.GetInvalidFileNameChars();
            string resultado = nomeArquivo;

            for (int i = 0; i < invalidos.Length; i++)
            {
                string caractere = invalidos[i].ToString();
                resultado = resultado.Replace(caractere, "_");
            }

            if (resultado.Length > 240)
                resultado = resultado.Substring(0, 240);

            return resultado;
        }
    }
}
