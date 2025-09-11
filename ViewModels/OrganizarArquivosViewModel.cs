using System;
using System.Windows.Input;
using FileOrganizer.Services;

namespace FileOrganizer.ViewModels
{
    public class OrganizarArquivosViewModel : ViewModelBase
    {
        private readonly ISelecionadorPastasService _selecionador;
        private readonly ProtetorPastasSistemaService _protetor;
        private readonly MensagemService _msg;

        private string _pastaSelecionada;

        // Propriedade que vai ser usada para exibir a pasta selecionada
        public string PastaSelecionada
        {
            get { return _pastaSelecionada; }
            set
            {
                if (_pastaSelecionada == value)
                    return;

                _pastaSelecionada = value;

                // avisa a UI que a propriedade mudou (INotifyPropertyChanged)
                OnPropertyChanged();
            }
        }

        // comando para abrir o seletor de pastas e aplicar as validações
        // é a ponte entre o button do view e um método no ViewModel
        // Ligamos isso no xaml assim <Button Content="Selecionar" Command="{Binding SelecionaPastaCommand}"/>
        public ICommand SelecionaPastaCommand { get; }

        // construtor 
        public OrganizarArquivosViewModel()
        {

            _selecionador = new SelecionadorPastasService();
            _protetor = new ProtetorPastasSistemaService();
            _msg = new MensagemService();

            SelecionaPastaCommand = new RelayCommand(ExecutarSelecaoDePasta, null);
        }

        private void ExecutarSelecaoDePasta(object parametro)
        {
            var caminho = _selecionador.SelecionaPasta();
            if (string.IsNullOrWhiteSpace(caminho))
                return;

            if (_protetor.IsPastaSistema(caminho))
            {
                _msg.Aviso("Essa pasta é do sistema e não pode ser utilizada. Selecione outra pasta.");
                return;
            }

            if (!_protetor.PastaTemArquivos(caminho))
            {
                _msg.Aviso("A pasta selecionada não possui arquivos no nível atual (apenas subpastas). Escolha uma pasta que contenha ao menos um arquivo.");
                return;
            }

            PastaSelecionada = caminho;
        }
    }
}
