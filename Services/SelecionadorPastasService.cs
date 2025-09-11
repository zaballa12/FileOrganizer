using System;
using System.Net.NetworkInformation;
using WinForms = System.Windows.Forms;

namespace FileOrganizer.Services
{
    public class SelecionadorPastasService : ISelecionadorPastasService
    {
        public string SelecionaPasta()
        {
            using var dialogo = new FolderBrowserDialog
            {
                Description = "Selecione uma pasta",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false,
                RootFolder = Environment.SpecialFolder.Desktop
            };

            DialogResult resultado = dialogo.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                return dialogo.SelectedPath;
            }

            return null;
        }
    }
}
