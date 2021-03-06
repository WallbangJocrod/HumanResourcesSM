﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
    public partial class DespidoDG : Page
    {
        MSeleccion Metodos = new MSeleccion();

        public DespidoDG()
        {
            InitializeComponent();
        }

        public void Refresh(string search)
        {

            var items = Metodos.MostrarDespedirDG(search);


            dgOperaciones.ItemsSource = items;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            Refresh(txtBuscar.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DepartamentoFrm frmTrab = new DepartamentoFrm();
            bool Resp = frmTrab.ShowDialog() ?? false;
            Refresh(txtBuscar.Text);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Refresh(txtBuscar.Text);
        }


        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "";
            }

        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "")
            {
                txtBucarPlaceH.Text = "Buscar...";
            }

        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            //int id = (int)((Button)sender).CommandParameter;
            //var response = Metodos.Encontrar(id);

            //DepartamentoFrm frmTrab = new DepartamentoFrm();
            //frmTrab.Type = TypeForm.Read;
            //frmTrab.DataFill = response[0];
            //bool Resp = frmTrab.ShowDialog() ?? false;
            //Refresh(txtBuscar.Text);

            //MessageBox.Show(response[0].fechaNacimiento.ToString());
        }

        private void BtnFire_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            DespidoVista Vista = new DespidoVista(id);

            Vista.ShowDialog();
            Refresh(txtBuscar.Text);
        }
    }
}
