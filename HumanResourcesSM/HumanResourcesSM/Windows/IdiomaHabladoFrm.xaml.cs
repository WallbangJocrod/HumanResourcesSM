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
    public partial class IdiomaHabladoFrm : Window
    {
        public IdiomaHabladoFrm(SeleccionFrm Par)
        {
            InitializeComponent();

            ParentFrm = Par;
            isSelection = true;
        }

        public IdiomaHabladoFrm(DEmpleado empleado)
        {
            InitializeComponent();

            Empleado = empleado;
            isSelection = false;
        }


        public TypeForm Type;
        public DIdiomaHablado DataFill;

        public DIdiomaHablado UForm;

        public MIdiomaHablado Metodos = new MIdiomaHablado();

        public SeleccionFrm ParentFrm;
        public DEmpleado Empleado;
        bool isSelection;

        MIdiomaHablado methodIH = new MIdiomaHablado();


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type == TypeForm.Update)
                Update();
            else
                Create();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var resp = Metodos.MostrarIdioma();

            foreach(DIdioma item in resp)
            {
                item.nombre = item.nombre.ToUpper();
            }

            CbIdioma.ItemsSource = resp;
            CbIdioma.DisplayMemberPath = "nombre";
            CbIdioma.SelectedValuePath = "idIdioma";

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

        public void InsertIdioma(DIdiomaHablado idiomaHablado)
        {
            idiomaHablado.idEmpleado = Empleado.idEmpleado;
            var resp = methodIH.Insertar(idiomaHablado);
            if (resp == "OK")
            {
                MAuditoria.Insertar(new DAuditoria(
                                    Menu.ActUsuario.idUsuario,
                                    DAuditoria.Registrar,
                                    "Se ha registrado un Idioma Hablado del empleado Nº" + Empleado.idEmpleado));

                MessageBox.Show("Registro completado!", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void EditIdioma(DIdiomaHablado idiomaHablado)
        {
            idiomaHablado.idEmpleado = Empleado.idEmpleado;
            var resp = methodIH.Editar(idiomaHablado);

            if (resp == "OK")
            {
                MAuditoria.Insertar(new DAuditoria(
                                    Menu.ActUsuario.idUsuario,
                                    DAuditoria.Editar,
                                    "Se ha Editado un Idioma Hablado del empleado Nº" + Empleado.idEmpleado));

                MessageBox.Show("Edición completada!", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }


        void fillData()
        {
            if (Validate())
            {
                UForm = null;
                return;
            }

            int idIdioma = (int)CbIdioma.SelectedValue;
            int Nivel = CbNivel.SelectedIndex;

            UForm = new DIdiomaHablado(0,
                                        idIdioma,
                                        0,
                                        Nivel);
        }

        void Create()
        {
            fillData();
            if (UForm == null)
                return;

            if (isSelection)
                ParentFrm.InsertIdioma(UForm);
            else
                InsertIdioma(UForm);

            this.DialogResult = true;
            this.Close();

        }

        void Update()
        {
            fillData();
            if (UForm == null)
                return;

            UForm.idIdiomaHablado = DataFill.idIdiomaHablado;
            if (isSelection)
                ParentFrm.EditIdioma(UForm, UForm.idIdiomaHablado);
            else
                EditIdioma(UForm);

            this.DialogResult = true;
            this.Close();
        }


        void SetEnable(bool Enable)
        {
            CbIdioma.IsEnabled = Enable;
            CbNivel.IsEnabled = Enable;
        }

        void fillForm(DIdiomaHablado Data)
        {
            if (Data != null)
            {
                CbIdioma.SelectedValue = Data.idIdioma;
                CbNivel.SelectedIndex = Data.nivel;
            }
        }

        #region Validation
        bool Validate()
        {
            if (CbIdioma.SelectedIndex == -1)
            {
                MessageBox.Show("Debes Seleccionar un Idioma!", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error);
                CbIdioma.Focus();
                return true;
            }
            if (CbNivel.SelectedIndex == -1)
            {
                MessageBox.Show("Debes Seleccionar un Nivel!", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error);
                CbNivel.Focus();
                return true;
            }

            return false;
        }
        #endregion
    }
}
