﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Datos;
using Metodos;

namespace HumanResourcesSM.Windows
{
    /// <summary>
    /// Interaction logic for DepartamentoDG.xaml
    /// </summary>
    public partial class ServiciosFrm : Page
    {
        MUsuario Metodos = new MUsuario();

        public ServiciosFrm()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRestauracion_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SQL SERVER database backup files|*.bak";
            dlg.Title = "Restaurar Base de Datos";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.FileName;
                var resp = MUsuario.Restore(path);

                if (resp.Equals("OK"))
                    MAuditoria.Insertar(new DAuditoria(
                                        Menu.ActUsuario.idUsuario,
                                        DAuditoria.Restore,
                                        "Se ha realizado una restauración de la base de datos"));
            }
        }

        private void BtnRespaldo_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.SelectedPath;
                var resp = MUsuario.Backup(path);

                if(resp.Equals("OK"))
                MAuditoria.Insertar(new DAuditoria(
                                    Menu.ActUsuario.idUsuario,
                                    DAuditoria.Backup,
                                    "Se ha realizado un respaldo de la base de datos"));
            }
        }

        private void BtnAuditoria_Click(object sender, RoutedEventArgs e)
        {
            AuditoriaDG Dg = new AuditoriaDG();
            Dg.ShowDialog();
        }
    }
}
