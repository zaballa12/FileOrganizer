using FileOrganizer.Models.Core;
using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.Renomear;
using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos;
using FileOrganizer.Models.RegraDeNegocio.RenomearArquivos.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using static FileOrganizer.Models.Model.Enumerados;

namespace FileOrganizer.ViewModels
{
    public class RenomearArquivosViewModel : ViewModelBase
    {
        private readonly ISelecionadorPastasService _selecionador;
        private readonly ProtetorPastasSistemaService _protetor;
        private readonly MensagemService _mensagens;

        public RenomearArquivosViewModel()
        {
            _selecionador = new SelecionadorPastasService();
            _protetor = new ProtetorPastasSistemaService();
            _mensagens = new MensagemService();

            _modoPrefixo = ModoRenomear.Texto;
            _modoSufixo = ModoRenomear.Extensao; // defina como preferir
            // Inicializa proxies a partir dos enums
            _modoPrefixoTexto = _modoPrefixo == ModoRenomear.Texto ? "Texto" : "Extensão";
            _modoSufixoTexto = _modoSufixo == ModoRenomear.Texto ? "Texto" : "Extensão";
            _itensPrevia = new ObservableCollection<SugestaoRenomeacaoModel>();

            SelecionarPastaCommand = new RelayCommand(ExecutarSelecionarPasta);
            GerarPreviaCommand = new RelayCommand(ExecutarGerarPrevia, PodeGerarPrevia);
            ConfirmarCommand = new RelayCommand(ExecutarConfirmar, PodeConfirmar);
        }

        // ----------------- PROPRIEDADES -----------------

        private string _pastaSelecionada;
        public string PastaSelecionada
        {
            get { return _pastaSelecionada; }
            set
            {
                if (_pastaSelecionada == value) return;
                _pastaSelecionada = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private ModoRenomear _modoPrefixo;
        public ModoRenomear ModoPrefixo
        {
            get { return _modoPrefixo; }
            set
            {
                if (_modoPrefixo == value) return;
                _modoPrefixo = value;
                // sincronia com o proxy
                ModoPrefixoTexto = _modoPrefixo == ModoRenomear.Texto ? "Texto" : "Extensão";
                OnPropertyChanged();
            }
        }

        private ModoRenomear _modoSufixo;
        public ModoRenomear ModoSufixo
        {
            get { return _modoSufixo; }
            set
            {
                if (_modoSufixo == value) return;
                _modoSufixo = value;
                // sincronia com o proxy
                ModoSufixoTexto = _modoSufixo == ModoRenomear.Texto ? "Texto" : "Extensão";
                OnPropertyChanged();
            }
        }
        // ==== PROXIES em STRING usados pelos ComboBox no XAML ====
        private string _modoPrefixoTexto;
        public string ModoPrefixoTexto
        {
            get => _modoPrefixoTexto;
            set
            {
                if (_modoPrefixoTexto == value) return;
                _modoPrefixoTexto = value;
                // mapeia para o enum
                _modoPrefixo = string.Equals(value, "Texto", StringComparison.OrdinalIgnoreCase)
                    ? ModoRenomear.Texto
                    : ModoRenomear.Extensao;
                OnPropertyChanged();                 // notifica o próprio proxy
                OnPropertyChanged(nameof(ModoPrefixo)); // mantém ambos em sincronia
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _modoSufixoTexto;
        public string ModoSufixoTexto
        {
            get => _modoSufixoTexto;
            set
            {
                if (_modoSufixoTexto == value) return;
                _modoSufixoTexto = value;
                // mapeia para o enum
                _modoSufixo = string.Equals(value, "Texto", StringComparison.OrdinalIgnoreCase)
                    ? ModoRenomear.Texto
                    : ModoRenomear.Extensao;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModoSufixo));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _prefixoTexto;
        public string PrefixoTexto
        {
            get { return _prefixoTexto; }
            set
            {
                if (_prefixoTexto == value) return;
                _prefixoTexto = value;
                OnPropertyChanged();
            }
        }

        private string _sufixoTexto;
        public string SufixoTexto
        {
            get { return _sufixoTexto; }
            set
            {
                if (_sufixoTexto == value) return;
                _sufixoTexto = value;
                OnPropertyChanged();
            }
        }

        // Helpers para o XAML mostrar/ocultar textbox
        public bool MostrarPrefixoTexto
        {
            get { return _modoPrefixo == ModoRenomear.Texto; }
        }

        public bool MostrarSufixoTexto
        {
            get { return _modoSufixo == ModoRenomear.Texto; }
        }

        private ObservableCollection<SugestaoRenomeacaoModel> _itensPrevia;
        public ObservableCollection<SugestaoRenomeacaoModel> ItensPrevia
        {
            get { return _itensPrevia; }
            set
            {
                if (_itensPrevia == value) return;
                _itensPrevia = value;
                OnPropertyChanged();
            }
        }

        // ----------------- COMMANDS -----------------

        public ICommand SelecionarPastaCommand { get; private set; }
        public ICommand GerarPreviaCommand { get; private set; }
        public ICommand ConfirmarCommand { get; private set; }

        // ----------------- MÉTODOS -----------------

        private void ExecutarSelecionarPasta(object parametro)
        {
            string caminho = _selecionador.SelecionaPasta();
            if (string.IsNullOrWhiteSpace(caminho))
                return;

            if (_protetor.IsPastaSistema(caminho))
            {
                _mensagens.Aviso("Essa pasta é do sistema e não pode ser utilizada. Selecione outra pasta.");
                return;
            }

            if (!Directory.Exists(caminho))
            {
                _mensagens.Aviso("A pasta selecionada não existe mais no disco.");
                return;
            }

            PastaSelecionada = caminho;
        }

        private bool PodeGerarPrevia(object parametro)
        {
            return !string.IsNullOrWhiteSpace(PastaSelecionada)
                   && Directory.Exists(PastaSelecionada);
        }

        private void ExecutarGerarPrevia(object parametro)
        {
            try
            {
                RenomearConfiguravel renomeador = new RenomearConfiguravel();
                renomeador.Caminho = PastaSelecionada;
                renomeador.ModoPrefixo = _modoPrefixo;
                renomeador.ModoSufixo = _modoSufixo;
                renomeador.PrefixoTexto = _prefixoTexto;
                renomeador.SufixoTexto = _sufixoTexto;

                var previa = renomeador.GerarPrevia();

                ItensPrevia.Clear();
                if (previa != null)
                {
                    for (int i = 0; i < previa.Count; i++)
                        ItensPrevia.Add(previa[i]);
                }

                // guarda a última estratégia para confirmar depois
                _ultimoRenomeador = renomeador;
                CommandManager.InvalidateRequerySuggested();
            }
            catch
            {
                _mensagens.Erro("Falha ao gerar a pré-visualização.");
            }
        }

        private bool PodeConfirmar(object parametro)
        {
            return _ultimoRenomeador != null && ItensPrevia.Count > 0;
        }

        private void ExecutarConfirmar(object parametro)
        {
            try
            {
                _ultimoRenomeador.ConfirmarAlteracoes();
                _mensagens.Info("Arquivos renomeados com sucesso.");
                ItensPrevia.Clear();
            }
            catch
            {
                _mensagens.Erro("Falha ao aplicar as alterações.");
            }
        }

        // Mantém a instância usada na prévia para reaplicar as mesmas regras
        private IRenomearArquivos _ultimoRenomeador;
    }
}
