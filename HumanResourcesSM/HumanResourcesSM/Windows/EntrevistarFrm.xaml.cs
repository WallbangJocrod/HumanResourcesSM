﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Metodos;
using Datos;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HumanResourcesSM.Windows
{
    public partial class EntrevistarFrm : Page
    {
        DEmpleado Empleado = new DEmpleado();

        GestionMenu Parent = new GestionMenu();

        public EntrevistarFrm(DEmpleado empleado, GestionMenu parent)
        {
            InitializeComponent();

            Empleado = empleado;
            Parent = parent;
        }

        DEmpleado EmpleadoEntrevistado;
        DSeleccion EmpleadoSelEntrevistado;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FetchEmpleado();
        }

        void FetchEmpleado()
        {
            MSeleccion SelMetodo = new MSeleccion();


            var Resp2 = SelMetodo.EncontrarSeleccion(Empleado.idEmpleado);


            if (Resp2.Count < 1)
            {
                NotAvailablePanel.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                NotAvailablePanel.Visibility = Visibility.Collapsed;
            }

            var DatosSeleccion = Resp2[0];

            EmpleadoEntrevistado = Empleado;
            EmpleadoSelEntrevistado = DatosSeleccion;

            txtNombre.Text = Empleado.nombre;
            txtApellido.Text = Empleado.apellido;
            txtDireccion.Text = Empleado.direccion;

            if (!Regex.IsMatch(Empleado.curriculum, @"^https?:\/\/", RegexOptions.IgnoreCase))
                Empleado.curriculum = "http://" + Empleado.curriculum;

            UrlCurriculo.NavigateUri = new Uri(Empleado.curriculum);
            txtDocumento.Text = Empleado.cedula;

            int edad = (DateTime.Today.Year - Empleado.fechaNacimiento.Year);
            if (Empleado.fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            txtFechaNac.Text = Empleado.fechaNacimiento.ToShortDateString() + " (" + edad + " Años)";

            txtPaisNac.Text = Empleado.nacionalidad; // por cambiar, debe verse "España - ES"
            txtEstadoLegal.Text = Empleado.estadoLegal;
            txtEmail.Text = Empleado.email;
            txtTelf.Text = Empleado.telefono;

            var res = new MDepartamento().Encontrar(Empleado.idDepartamento)[0];


            txtNombrePosicion.Text = DatosSeleccion.nombrePuesto;
            txtDepartamento.Text = res.nombre;
            txtFechaApl.Text = DatosSeleccion.fechaAplicacion.ToShortDateString();


            RefreshDGIdiomas();
            RefreshDGEducacion();
        }

        //***********************CRUD IDIOMAS*********************************

        public List<DIdiomaHablado> IdiomaHablado = new List<DIdiomaHablado>();
        public List<ModelIdiomaHablado> ModelHI = new List<ModelIdiomaHablado>();

        MIdiomaHablado methodIH = new MIdiomaHablado();

        public void DeleteIdioma(int id)
        {
            var resp = methodIH.Eliminar(id);

            RefreshDGIdiomas();
        }

        private void BtnAgregarIdioma_Click(object sender, RoutedEventArgs e)
        {
            IdiomaHabladoFrm Frm = new IdiomaHabladoFrm(EmpleadoEntrevistado);

            Frm.ShowDialog();

            RefreshDGIdiomas();

        }

        private void BtnIdiDelete_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ELIMINANDO" + ((Button)e.Source).CommandParameter);
            DeleteIdioma((int)((Button)e.Source).CommandParameter);
        }

        private void BtnIdiUpdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ACTUALIZANDO" + ((Button)e.Source).CommandParameter);

            DIdiomaHablado resp = methodIH.Encontrar((int)((Button)e.Source).CommandParameter)[0];

            IdiomaHabladoFrm frm = new IdiomaHabladoFrm(EmpleadoEntrevistado);
            frm.Type = TypeForm.Update;
            frm.DataFill = resp;
            frm.ShowDialog();

            RefreshDGIdiomas();

        }

        public void RefreshDGIdiomas()
        {
            IdiomaHablado.Clear();
            ModelHI.Clear();

            

            var IdiomasHablados = methodIH.Mostrar(EmpleadoEntrevistado.idEmpleado);
            IdiomaHablado = IdiomasHablados;

            foreach(DIdiomaHablado item in IdiomasHablados)
            {
                var idi = methodIH.EncontrarIdioma(item.idIdioma)[0];
                ModelHI.Add(new ModelIdiomaHablado(idi.nombre, item.nivel));
            }

            IdiomaList.Children.Clear();

            for (int i = 0; i < IdiomaHablado.Count; i++)
            {
                IdiomaList.Children.Add(CreateRow(ModelHI[i], IdiomaHablado[i]));
            }


        }

        Border CreateRow(ModelIdiomaHablado Model, DIdiomaHablado DIH)
        {
            string Nivel = DIH.nivel == 0 ? "Básico" :
                           DIH.nivel == 1 ? "Intermedio" :
                           DIH.nivel == 2 ? "Avanzado" :
                           DIH.nivel == 3 ? "Fluido" : "ERROR";



            Border MainBord = new Border();
            MainBord.BorderBrush = Brushes.Black;
            MainBord.BorderThickness = new Thickness(0);

            Grid Divider = new Grid();
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                }
            );
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                }
            );
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                }
            );

            Button DeleteButton = new Button();
            DeleteButton.SetValue(Grid.ColumnProperty, 0);
            DeleteButton.Background = DeleteButton.BorderBrush = Brushes.Transparent;
            DeleteButton.Margin = new Thickness(10, 2, 10, 2);
            DeleteButton.Padding = new Thickness(0);
            DeleteButton.Height = 17;
            Image delImg = new Image();
            string packUri = @"/Img/delete.png";
            delImg.Source = new BitmapImage(new Uri(packUri, UriKind.Relative));
            RenderOptions.SetBitmapScalingMode(delImg, BitmapScalingMode.HighQuality);
            DeleteButton.Click += new RoutedEventHandler(BtnIdiDelete_Click);
            DeleteButton.CommandParameter = DIH.idIdiomaHablado;
            DeleteButton.Content = delImg;

            Divider.Children.Add(DeleteButton);


            StackPanel SPContent = new StackPanel();
            SPContent.SetValue(Grid.ColumnProperty, 1);
            SPContent.MaxHeight = 50;

            Divider.Children.Add(SPContent);

            TextBlock NameText = new TextBlock();
            NameText.Text = Model.nombreIdioma.ToUpper();
            NameText.MaxHeight = 35;
            NameText.TextWrapping = TextWrapping.Wrap;
            NameText.TextTrimming = TextTrimming.CharacterEllipsis;
            NameText.FontWeight = FontWeights.Bold;
            NameText.FontSize = 12;
            NameText.Margin = new Thickness(5, 0, 5, 0);

            SPContent.Children.Add(NameText);

            TextBlock LevelText = new TextBlock();
            LevelText.Text = Nivel;
            LevelText.FontSize = 10;
            LevelText.TextWrapping = TextWrapping.Wrap;
            LevelText.TextTrimming = TextTrimming.CharacterEllipsis;
            LevelText.Margin = new Thickness(5, 0, 5, 0);

            SPContent.Children.Add(LevelText);


            Button EditButton = new Button();
            EditButton.SetValue(Grid.ColumnProperty, 2);
            EditButton.Background = EditButton.BorderBrush = Brushes.Transparent;
            EditButton.Margin = new Thickness(10, 3, 10, 3);
            EditButton.Padding = new Thickness(0);
            EditButton.Height = 17;
            Image editImg = new Image();
            string packUri2 = @"/Img/edit.png";
            editImg.Source = new BitmapImage(new Uri(packUri2, UriKind.Relative));
            EditButton.Click += new RoutedEventHandler(BtnIdiUpdate_Click);
            EditButton.CommandParameter = DIH.idIdiomaHablado;
            EditButton.Content = editImg;

            Divider.Children.Add(EditButton);

            MainBord.Child = Divider;

            return MainBord;

        }

        //*********************** END CRUD IDIOMAS*********************************

        //***********************CRUD EDUCACIÓN*********************************

        public List<DEducacion> ListaEducacion = new List<DEducacion>();

        MEducacion methodEdu = new MEducacion();

        public void DeleteEducacion(int id)
        {
            var resp = methodEdu.Eliminar(id);

            RefreshDGEducacion();
        }

        private void BtnAgregarEducacion_Click(object sender, RoutedEventArgs e)
        {
            EducacionFrm Frm = new EducacionFrm(EmpleadoEntrevistado);

            Frm.ShowDialog();
            RefreshDGEducacion();

        }

        private void BtnEduDelete_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ELIMINANDO" + ((Button)e.Source).CommandParameter);
            DeleteEducacion((int)((Button)e.Source).CommandParameter);
        }

        private void BtnEduUpdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ACTUALIZANDO" + ((Button)e.Source).CommandParameter);

            DEducacion resp = methodEdu.Encontrar((int)((Button)e.Source).CommandParameter)[0];

            EducacionFrm frm = new EducacionFrm(EmpleadoEntrevistado);
            frm.Type = TypeForm.Update;
            frm.DataFill = resp;
            frm.ShowDialog();
            RefreshDGEducacion();

        }

        public void RefreshDGEducacion()
        {
            var Educacion = new MEducacion().Mostrar(EmpleadoEntrevistado.idEmpleado);
            ListaEducacion = Educacion;

            EducacionList.Children.Clear();

            for (int i = 0; i < ListaEducacion.Count; i++)
            {
                EducacionList.Children.Add(CreateRowEdu(ListaEducacion[i]));
            }


        }

        Border CreateRowEdu(DEducacion DED)
        {

            Border MainBord = new Border();
            MainBord.BorderBrush = Brushes.Black;
            MainBord.BorderThickness = new Thickness(0);

            Grid Divider = new Grid();
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                }
            );
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                }
            );
            Divider.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                }
            );

            Button DeleteButton = new Button();
            DeleteButton.SetValue(Grid.ColumnProperty, 0);
            DeleteButton.Background = DeleteButton.BorderBrush = Brushes.Transparent;
            DeleteButton.Margin = new Thickness(10, 2, 10, 2);
            DeleteButton.Padding = new Thickness(0);
            DeleteButton.Height = 17;
            Image delImg = new Image();
            string packUri = @"/Img/delete.png";
            delImg.Source = new BitmapImage(new Uri(packUri, UriKind.Relative));
            RenderOptions.SetBitmapScalingMode(delImg, BitmapScalingMode.HighQuality);
            DeleteButton.Click += new RoutedEventHandler(BtnEduDelete_Click);
            DeleteButton.CommandParameter = DED.idEducacion;
            DeleteButton.Content = delImg;
            DeleteButton.Foreground = Brushes.Black;

            Divider.Children.Add(DeleteButton);

            StackPanel SPContent = new StackPanel();
            SPContent.SetValue(Grid.ColumnProperty, 1);
            SPContent.MaxHeight = 50;

            Divider.Children.Add(SPContent);

            TextBlock NameText = new TextBlock();
            NameText.Text = DED.nombreCarrera;
            NameText.MaxHeight = 35;
            NameText.TextWrapping = TextWrapping.Wrap;
            NameText.TextTrimming = TextTrimming.CharacterEllipsis;
            NameText.FontWeight = FontWeights.Bold;
            NameText.FontSize = 12;
            NameText.Margin = new Thickness(5, 0, 5, 0);

            SPContent.Children.Add(NameText);

            string egreso = DED.fechaEgreso == null ? "Actualidad" : DED.fechaEgreso?.Year.ToString();

            TextBlock DateText = new TextBlock();
            DateText.Text = DED.fechaIngreso.Year + " - " + egreso;
            DateText.FontSize = 10;
            DateText.TextWrapping = TextWrapping.Wrap;
            DateText.TextTrimming = TextTrimming.CharacterEllipsis;
            DateText.Margin = new Thickness(5, 0, 5, 0);

            SPContent.Children.Add(DateText);


            Button EditButton = new Button();
            EditButton.SetValue(Grid.ColumnProperty, 3);
            EditButton.Background = EditButton.BorderBrush = Brushes.Transparent;
            EditButton.Margin = new Thickness(10, 3, 10, 3);
            EditButton.Padding = new Thickness(0);
            EditButton.Height = 23;
            Image editImg = new Image();
            string packUri2 = @"/Img/edit.png";
            editImg.Source = new BitmapImage(new Uri(packUri2, UriKind.Relative));
            EditButton.Click += new RoutedEventHandler(BtnEduUpdate_Click);
            EditButton.CommandParameter = DED.idEducacion;
            EditButton.Content = editImg;
            EditButton.Foreground = Brushes.Black;

            Divider.Children.Add(EditButton);

            MainBord.Child = Divider;

            return MainBord;

        }

        //*********************** END CRUD EDUCACIÓN*********************************

        private void BtnInfoUpdate_Click(object sender, RoutedEventArgs e)
        {
            SeleccionVista vista = new SeleccionVista(EmpleadoEntrevistado, EmpleadoSelEntrevistado);
            vista.ShowDialog();
            FetchEmpleado();
        }



        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Está Seguro que Desea No Contratar al Seleccionado?", "SwissNet", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (Resp == MessageBoxResult.No)
                return;

            ContratoFrm frm = new ContratoFrm(EmpleadoEntrevistado, EmpleadoSelEntrevistado, false);
            bool resp = frm.ShowDialog() ?? false;

            if (resp)
                Parent.RegresarDetalleSeleccionado();

        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Está Seguro que Desea Contratar al Seleccionado?", "SwissNet", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (Resp == MessageBoxResult.No)
                return;

            ContratoFrm frm = new ContratoFrm(EmpleadoEntrevistado, EmpleadoSelEntrevistado);
            bool resp = frm.ShowDialog() ?? false;

            if (resp)
                Parent.RegresarDetalleSeleccionado();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            Parent.RegresarDetalleSeleccionado();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FetchEmpleado();
        }
    }
}
