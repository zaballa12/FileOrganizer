using FileOrganizer.Models.Core;
using FileOrganizer.Models.RegraDeNegocio.OrganizarPasta;
using FileOrganizer.Models.RegraDeNegocio.OrganizarPasta.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace FileOrganizer.ViewModels
{
    public class OrganizarArquivosViewModel : ViewModelBase
    {
        private readonly ISelecionadorPastasService _selecionador;
        private readonly ProtetorPastasSistemaService _protetor;
        private readonly MensagemService _msg;
        private IOrganizarPasta _organizar;
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
        // Propriedades que controlam o tipo da organização
        private bool _porFormato = true;  // padrão
        public bool PorFormato
        {
            get { return _porFormato; }
            set { if (_porFormato == value) return; _porFormato = value; OnPropertyChanged(); }
        }

        private bool _porPrefixo;
        public bool PorPrefixo
        {
            get { return _porPrefixo; }
            set { if (_porPrefixo == value) return; _porPrefixo = value; OnPropertyChanged(); }
        }

        private bool _porData;
        public bool PorData
        {
            get { return _porData; }
            set { if (_porData == value) return; _porData = value; OnPropertyChanged(); }
        }

        // Prévia para o DataGrid
        public ObservableCollection<SugestaoMovimentacaoModel> ItensPrevia { get; }
            = new ObservableCollection<SugestaoMovimentacaoModel>();


        // comando para abrir o seletor de pastas e aplicar as validações
        // é a ponte entre o button do view e um método no ViewModel
        // Ligamos isso no xaml assim <Button Content="Selecionar" Command="{Binding SelecionaPastaCommand}"/>
        public ICommand SelecionaPastaCommand { get; }
        public ICommand GerarPreviaCommand { get; }
        public ICommand ConfirmarCommand { get; }


        // construtor 
        public OrganizarArquivosViewModel()
        {

            _selecionador = new SelecionadorPastasService();
            _protetor = new ProtetorPastasSistemaService();
            _msg = new MensagemService();

            SelecionaPastaCommand = new RelayCommand(ExecutarSelecaoDePasta, null);
            GerarPreviaCommand = new RelayCommand(ExecutarGerarPrevia, PodeGerarPrevia);
            ConfirmarCommand = new RelayCommand(ExecutarConfirmar, PodeConfirmar);
        }

        private void ExecutarSelecaoDePasta(object _)
        {
            string caminho = _selecionador.SelecionaPasta();
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

            if (!Directory.Exists(caminho))
            {
                _msg.Aviso("A pasta selecionada não existe mais no disco.");
                return;
            }

            PastaSelecionada = caminho;
        }
        private bool PodeGerarPrevia(object p)
        {
            return !string.IsNullOrWhiteSpace(PastaSelecionada);
        }

        private void ExecutarGerarPrevia(object p)
        {
            try
            {
                _organizar = CriarEstrategiaAtual();
                _organizar.Caminho = PastaSelecionada;

                ItensPrevia.Clear();

                var lista = _organizar.GerarPrevia();
                ItensPrevia.Clear();

                if (lista == null || lista.Count == 0)
                {
                    _msg.Info("A pasta não possui arquivos no nível atual.");
                    return;
                }

                foreach (var s in lista)
                {
                    ItensPrevia.Add(s);
                }
            }
            catch (UnauthorizedAccessException)
            {
                _msg.Erro("Sem permissão para ler a pasta.");
            }
            catch (Exception)
            {
                _msg.Erro("Falha ao gerar a prévia.");
            }
        }

        private bool PodeConfirmar(object p)
        {
            return ItensPrevia.Count > 0;
        }

        private void ExecutarConfirmar(object p)
        {
            try
            {
                if (_organizar == null)
                {
                    _msg.Info("Gere a prévia antes de confirmar.");
                    return;
                }

                // garanta que a estratégia aplique exatamente o que está na grade
                _organizar.SugestaoMovimentacoes = new List<SugestaoMovimentacaoModel>(ItensPrevia);

                _organizar.ConfirmarAlteracoes();

                _msg.Info("Organização concluída com sucesso!");
            }
            catch (Exception)
            {
                _msg.Erro("Falha ao aplicar as alterações.");
            }
        }

        private IOrganizarPasta CriarEstrategiaAtual()
        {
            if (PorFormato)
                return new OrganizarPorFormato();

            if (PorPrefixo)
                return new OrganizarPorPrefixo();

            if (PorData)
                return new OrganizarPorData();

            return null;
        }
    }
}
