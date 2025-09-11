using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileOrganizer.Services
{
    public class MensagemService
    {
        public void Aviso(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Erro(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Info(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
