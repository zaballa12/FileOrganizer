using FileOrganizer.Models.Core;
using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace FileOrganizer.ViewModels
{
    public class BuscarDuplicadosViewModel : ViewModelBase
    {
        private readonly ISelecionadorPastasService _selecionador;
        private readonly ProtetorPastasSistemaService _protetor;
        private readonly MensagemService _msg;

        public BuscarDuplicadosViewModel()
        {
            _selecionador = new SelecionadorPastasService();
            _protetor = new ProtetorPastasSistemaService();
            _msg = new MensagemService();

            _porNome = true;      // padrão
            _porConteudo = false; // padrão

            _itens = new ObservableCollection<ArquivoDuplicadoModel>();

            SelecionarPastaCommand = new RelayCommand(SelecionarPasta);
            PesquisarCommand = new RelayCommand(Pesquisar, PodePesquisar);
            MarcarTodosCommand = new RelayCommand(_ => MarcarTodos(true));
            DesmarcarTodosCommand = new RelayCommand(_ => MarcarTodos(false));
        }

        private string _pastaSelecionada;
        public string PastaSelecionada
        {
            get { return _pastaSelecionada; }
            set
            {
                if (_pastaSelecionada == value)
                    return;
                _pastaSelecionada = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // reavalia CanExecute
            }
        }

        private bool _porNome;
        public bool PorNome
        {
            get { return _porNome; }
            set
            {
                if (_porNome == value)
                    return;
                _porNome = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _porConteudo;
        public bool PorConteudo
        {
            get { return _porConteudo; }
            set
            {
                if (_porConteudo == value)
                    return;
                _porConteudo = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private ObservableCollection<ArquivoDuplicadoModel> _itens;
        public ObservableCollection<ArquivoDuplicadoModel> Itens
        {
            get { return _itens; }
            set
            {
                if (_itens == value)
                    return;
                _itens = value;
                OnPropertyChanged();
            }
        }

        private bool _selecionarTodos;
        public bool SelecionarTodos
        {
            get { return _selecionarTodos; }
            set
            {
                if (_selecionarTodos == value)
                    return;
                _selecionarTodos = value;
                OnPropertyChanged();
                MarcarTodos(value);
            }
        }

        public ICommand SelecionarPastaCommand { get; private set; }
        public ICommand PesquisarCommand { get; private set; }
        public ICommand MarcarTodosCommand { get; private set; }
        public ICommand DesmarcarTodosCommand { get; private set; }

        private void SelecionarPasta(object _)
        {
             string caminho = _selecionador.SelecionaPasta();
            if (string.IsNullOrWhiteSpace(caminho))
                return;

            if (_protetor.IsPastaSistema(caminho))
            {
                _msg.Aviso("Essa pasta é do sistema e não pode ser utilizada. Selecione outra pasta.");
                return;
            }

            if (!Directory.Exists(caminho))
            {
                _msg.Aviso("A pasta selecionada não existe mais no disco.");
                return;
            }

            PastaSelecionada = caminho;
        }

        private bool PodePesquisar(object _)
        {
            return !string.IsNullOrWhiteSpace(PastaSelecionada)
                   && Directory.Exists(PastaSelecionada)
                   && (_porNome || _porConteudo);
        }

        private void Pesquisar(object _)
        {
            try
            {
                if (!PodePesquisar(null))
                {
                    _msg.Aviso("Selecione a pasta e pelo menos um critério (Nome e/ou Conteúdo).");
                    return;
                }

                IBuscarDuplicados buscaDuplicados;

                if (_porNome && _porConteudo)
                {
                    buscaDuplicados = new BuscarDuplicadosSequencial(new BuscadorDuplicadosBase[]
                    {
                        new BuscarDuplicadosPorNome(),
                        new BuscarDuplicadosPorConteudo()
                    });
                    buscaDuplicados.Caminho = PastaSelecionada;
                }
                else if (_porNome)
                {
                    var busca = new BuscarDuplicadosPorNome();
                    busca.Caminho = PastaSelecionada;
                    buscaDuplicados = busca;
                }
                else
                {
                    var busca = new BuscarDuplicadosPorConteudo();
                    busca.Caminho = PastaSelecionada;
                    buscaDuplicados = busca;
                }

                var grupos = buscaDuplicados.Buscar();

                _itens.Clear();

                if (grupos == null || grupos.Count == 0)
                {
                    _msg.Info("Nenhum arquivo duplicado encontrado.");
                    return;
                }

                foreach (GrupoDuplicados grupo in grupos)
                {
                    string rotulo;
                    if (grupo.Chave == "NOME") rotulo = "Nome";
                    else if (grupo.Chave == "HASH") rotulo = "Conteúdo";
                    else if (grupo.Chave == "NOME|HASH") rotulo = "Nome + Conteúdo";
                    else rotulo = grupo.Chave;

                    foreach (ArquivoModel arq in grupo.Arquivos)
                    {
                        ArquivoDuplicadoModel linha = new ArquivoDuplicadoModel
                        {
                            Selecionado = true,
                            Arquivo = arq,
                            GrupoRotulo = rotulo
                        };

                        _itens.Add(linha);
                    }
                }

                SelecionarTodos = true;
            }
            catch (UnauthorizedAccessException)
            {
                _msg.Erro("Sem permissão para ler uma ou mais pastas/arquivos.");
            }
            catch (Exception)
            {
                _msg.Erro("Falha ao executar a busca de duplicados.");
            }
        }

        private void MarcarTodos(bool valor)
        {
            for (int i = 0; i < _itens.Count; i++)
            {
                _itens[i].Selecionado = valor;
            }
        }
    }
}
