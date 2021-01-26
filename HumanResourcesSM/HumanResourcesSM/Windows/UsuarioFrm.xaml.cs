﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Datos;
using Metodos;

namespace HumanResourcesSM.Windows
{

    /// <summary>
    /// Interaction logic for DepartamentoFrm.xaml
    /// </summary>
    public partial class UsuarioFrm : Window
    {
        public UsuarioFrm()
        {
            InitializeComponent();

            MRol METR = new MRol();

            var res = METR.Mostrar();

            CbRol.ItemsSource = res;
            CbRol.DisplayMemberPath = "nombre";
            CbRol.SelectedValuePath = "idRol";
        }


        public TypeForm Type;
        public DUsuario DataFill;

        public DUsuario UForm;

        public MUsuario Metodos = new MUsuario();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Read)
            {
                txtTitulo.Text = "Ver";
                fillForm(DataFill);
                SetEnable(false);
                btnEnviar.Visibility = Visibility.Collapsed;
            }
            else if (Type == TypeForm.Update)
            {
                txtTitulo.Text = "Editar";
                BgTitulo.Background = (Brush)new BrushConverter().ConvertFrom("#2A347B");
                btnEnviar.Content = "Editar";
                btnEnviar.Foreground = (Brush)new BrushConverter().ConvertFrom("#2A347B");
                btnEnviar.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#2A347B");
                fillForm(DataFill);
            }
        }



        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idRol = (int)CbRol.SelectedValue;
            string usuario = txtUsuario.txt.Text;
            string password = txtContraseña.Password;
            string confirmacion = txtConfirmacion.txt.Text;

            UForm = new DUsuario(0, 
                                 idRol,
                                 usuario,
                                 password,
                                 confirmacion);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;
            string response = Metodos.Insertar(UForm);
            MessageBox.Show(response);
            if (response == "OK")
            {
                this.DialogResult = true;
                this.Close();
            }

        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;
            UForm.idUsuario = DataFill.idUsuario;
            string response = Metodos.Editar(UForm);
            MessageBox.Show(response);
            if (response == "OK")
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        void SetEnable(bool Enable)
        {
            txtUsuario.IsEnabled = Enable;
            CbRol.IsEnabled = Enable;
            txtContraseña.IsEnabled = Enable;
            txtConfirmacion.IsEnabled = Enable;
        }
        void fillForm(DUsuario Data)
        {
            if (Data != null)
            {
                txtUsuario.SetText(Data.usuario);
                CbRol.SelectedValue = Data.idRol;
                txtConfirmacion.SetText(Data.confirmacion);
                grdContraseña.Visibility = Visibility.Collapsed;
            }
        }
        #region Validation
        bool Validate()
        {
            if (txtUsuario.txt.Text == "")
            {
                MessageBox.Show("Debes llenar el campo Usuario!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUsuario.txt.Focus();
                return true;
            }

            if (CbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar un Rol!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                CbRol.Focus();
                return true;
            }

            if (txtContraseña.Password == "")
            {
                MessageBox.Show("Debes llenar el campo Contraseña!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtContraseña.Focus();
                return true;
            }

            if (txtConfirmacion.txt.Text == "")
            {
                MessageBox.Show("Debes llenar el campo Codigo de Confirmación!", "Magicolor", MessageBoxButton.OK, MessageBoxImage.Error);
                txtConfirmacion.txt.Focus();
                return true;
            }

            return false;
        }
        #endregion

        private void CbRol_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(CbRol.SelectedIndex > -1)
            {
                PlaceRol.Text = "";
            }
            else
            {
                PlaceRol.Text = "Rol";
            }
        }

        private void txtContraseña_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtContraseña.Password == "")
            {
                PlaceContraseña.Text = "";
            }
        }

        private void txtContraseña_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtContraseña.Password == "")
            {
                PlaceContraseña.Text = "Contraseña";
            }
        }
    }
}