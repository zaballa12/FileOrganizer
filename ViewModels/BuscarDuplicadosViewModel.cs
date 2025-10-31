using FileOrganizer.Models.Core;
using FileOrganizer.Models.Model;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados;
using FileOrganizer.Models.RegraDeNegocio.BuscarDuplicados.Model;
using FileOrganizer.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace FileOrganizer.ViewModels
{
    public class BuscarDuplicadosViewModel : ViewModelBase
    {
        // Serviços
        private readonly ISelecionadorPastasService _selecionador;
        private readonly ProtetorPastasSistemaService _protetor;
        private readonly MensagemService _msg;
        private readonly ExcluirArquivosService _exclusor;

        // Estado de seleção para habilitar/desabilitar o botão Excluir
        private int _quantidadeSelecionados;
        public bool TemSelecionado
        {
            get { return _quantidadeSelecionados > 0; }
        }

        public BuscarDuplicadosViewModel()
        {
            _selecionador = new SelecionadorPastasService();
            _protetor = new ProtetorPastasSistemaService();
            _msg = new MensagemService();
            _exclusor = new ExcluirArquivosService();

            _porNome = true;   // padrão
            _porConteudo = false;  // padrão

            _itens = new ObservableCollection<ArquivoDuplicadoModel>();
            _itens.CollectionChanged += Itens_CollectionChanged;

            SelecionarPastaCommand = new RelayCommand(SelecionarPasta);
            PesquisarCommand = new RelayCommand(Pesquisar, PodePesquisar);
            ExcluirSelecionadosCommand = new RelayCommand(ExcluirSelecionados); // habilita pelo TemSelecionado (XAML)
        }

        private string _pastaSelecionada;
        public string PastaSelecionada
        {
            get { return _pastaSelecionada; }
            set
            {
                if (_pastaSelecionada == value) return;
                _pastaSelecionada = value;
                OnPropertyChanged();
            }
        }

        private bool _porNome;
        public bool PorNome
        {
            get { return _porNome; }
            set
            {
                if (_porNome == value) return;
                _porNome = value;
                OnPropertyChanged();
            }
        }

        private bool _porConteudo;
        public bool PorConteudo
        {
            get { return _porConteudo; }
            set
            {
                if (_porConteudo == value) return;
                _porConteudo = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ArquivoDuplicadoModel> _itens;
        public ObservableCollection<ArquivoDuplicadoModel> Itens
        {
            get { return _itens; }
            set
            {
                if (_itens == value) return;

                // desassina da antiga
                if (_itens != null)
                    _itens.CollectionChanged -= Itens_CollectionChanged;

                _itens = value ?? new ObservableCollection<ArquivoDuplicadoModel>();
                _itens.CollectionChanged += Itens_CollectionChanged;

                // reconta seleção na coleção nova
                RecontarSelecionados();

                OnPropertyChanged();
                OnPropertyChanged(nameof(TemSelecionado));
            }
        }

        public ICommand SelecionarPastaCommand { get; private set; }
        public ICommand PesquisarCommand { get; private set; }
        public ICommand MarcarTodosCommand { get; private set; }
        public ICommand DesmarcarTodosCommand { get; private set; }
        public ICommand ExcluirSelecionadosCommand { get; private set; }

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
                   && (PorNome || PorConteudo);
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

                if (PorNome && PorConteudo)
                {
                    buscaDuplicados = new BuscarDuplicadosSequencial(new BuscadorDuplicadosBase[]
                    {
                        new BuscarDuplicadosPorNome(),
                        new BuscarDuplicadosPorConteudo()
                    });
                    buscaDuplicados.Caminho = PastaSelecionada;
                }
                else if (PorNome)
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
                _quantidadeSelecionados = 0;

                if (grupos == null || grupos.Count == 0)
                {
                    OnPropertyChanged(nameof(TemSelecionado));
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
                        var item = new ArquivoDuplicadoModel
                        {
                            Selecionado = arq.Selecionado,
                            Arquivo = arq,
                            GrupoRotulo = rotulo,
                        };

                        _itens.Add(item);
                        if (item.Selecionado)
                            _quantidadeSelecionados++;
                    }
                }

                //SelecionarTodos = true;
                OnPropertyChanged(nameof(TemSelecionado));
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

        private void ExcluirSelecionados(object _)
        {
            try
            {
                var selecionados = _itens.Where(i => i.Selecionado).ToList();
                if (selecionados == null || selecionados.Count == 0)
                {
                    _msg.Aviso("Nenhum item selecionado para excluir.");
                    return;
                }
                
                if (!_msg.Confirmacao($"Confirma a exclusão de {selecionados.Count} arquivo(s) selecionado(s)?"))
                    return;

                var caminhos = selecionados
                    .Select(s => s.Arquivo?.Caminho)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToList();

                var (ok, falhas) = _exclusor.EnviarParaLixeira(caminhos);

                // remove da grade os que foram efetivamente apagados
                for (int i = selecionados.Count - 1; i >= 0; i--)
                {
                    string caminho = selecionados[i].Arquivo?.Caminho;
                    if (!string.IsNullOrWhiteSpace(caminho) && !File.Exists(caminho))
                    {
                        _itens.Remove(selecionados[i]);
                    }
                }

                // reconta após a exclusão
                RecontarSelecionados();
                OnPropertyChanged(nameof(TemSelecionado));

                _msg.Info($"Exclusão concluída. Sucesso: {ok}. Falhas: {falhas}.");
            }
            catch
            {
                _msg.Erro("Falha ao excluir os arquivos selecionados.");
            }
        }

        private void Itens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var obj in e.NewItems)
                {
                    if (obj is ArquivoDuplicadoModel item)
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                        if (item.Selecionado) _quantidadeSelecionados++;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var obj in e.OldItems)
                {
                    if (obj is ArquivoDuplicadoModel item)
                    {
                        item.PropertyChanged -= Item_PropertyChanged;
                        if (item.Selecionado && _quantidadeSelecionados > 0) _quantidadeSelecionados--;
                    }
                }
            }

            OnPropertyChanged(nameof(TemSelecionado));
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ArquivoDuplicadoModel.Selecionado))
            {
                if (sender is ArquivoDuplicadoModel item)
                {
                    RecontarSelecionados();
                    OnPropertyChanged(nameof(TemSelecionado));
                }
            }
        }

        private void RecontarSelecionados()
        {
            _quantidadeSelecionados = 0;
            for (int i = 0; i < _itens.Count; i++)
            {
                if (_itens[i].Selecionado) _quantidadeSelecionados++;
            }
        }
    }
}
