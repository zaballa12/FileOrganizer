using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados
{
    public interface IBuscarDuplicados
    {
        string Caminho { get; set; }

        // Executa a busca e devolve grupos de duplicados
        List<GrupoDuplicados> Buscar();
    }
}
