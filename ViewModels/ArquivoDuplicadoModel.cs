using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using FileOrganizer.Models.Model;

namespace FileOrganizer.ViewModels
{
    public class ArquivoDuplicadoModel : ViewModelBase
    {
        private bool _selecionado = true;
        public bool Selecionado
        {
            get { return _selecionado; }
            set
            {
                if (_selecionado == value)
                    return;
                _selecionado = value;
                OnPropertyChanged();
            }
        }

        private ArquivoModel _arquivo;
        public ArquivoModel Arquivo
        {
            get { return _arquivo; }
            set
            {
                if (_arquivo == value)
                    return;
                _arquivo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Nome));
                OnPropertyChanged(nameof(Caminho));
                OnPropertyChanged(nameof(Tamanho));
            }
        }

        public string Nome
        {
            get
            {
                return _arquivo.Nome;
            }
        }

        public string Caminho
        {
            get
            {
                return Path.GetDirectoryName(_arquivo.Caminho);
            }
        }
        
        public string Tamanho
        {
            get
            {
                return _arquivo.Tamanho;
            }
        }

        public string GrupoRotulo { get; set; }
    }
}
