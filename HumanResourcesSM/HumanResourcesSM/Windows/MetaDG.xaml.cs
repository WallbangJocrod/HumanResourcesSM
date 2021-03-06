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

    public partial class MetaDG : Page
    {
        MMeta Metodos = new MMeta();

        SearchType searchType = SearchType.Departamento;

        public MetaDG()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            if (searchType == SearchType.Departamento)
            {
                int id = CbDepartamento.SelectedIndex > -1 ? (int)CbDepartamento.SelectedValue : -1;
                var items = Metodos.MostrarTodoByDepartamento(id, CbFechaInicio.SelectedDate, CbFechaFinal.SelectedDate);

                dgDepartamento.ItemsSource = items;
            }
            else if(searchType == SearchType.Empleado)
            {
                int id = CbEmpleado.SelectedIndex > -1 ? (int)CbEmpleado.SelectedValue : -1;
                var items = Metodos.MostrarTodoByEmpleado(id, CbFechaInicio.SelectedDate, CbFechaFinal.SelectedDate);

                dgEmpleado.ItemsSource = items;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //contentsp.Children.Clear();

            var res = new MDepartamento().Mostrar("");

            CbDepartamento.ItemsSource = res;
            CbDepartamento.DisplayMemberPath = "nombre";
            CbDepartamento.SelectedValuePath = "idDepartamento";

            RBDepartamento.IsChecked = true;
            DateTime StartofWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            CbFechaInicio.SelectedDate = StartofWeek;
            CbFechaFinal.SelectedDate = StartofWeek.AddDays(6);
            Refresh();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MetaFrm frmMeta = new MetaFrm();
            bool Resp = frmMeta.ShowDialog() ?? false;
            Refresh();
        }

        private void txtVer_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            DMeta Meta = EncontrarMeta(id);

            if(Meta != null)
            {
                MetaType TipoMeta = searchType == SearchType.Departamento ? MetaType.Departamento : MetaType.Empleado;
                MetaFrm frmMeta = new MetaFrm(TypeForm.Read, TipoMeta, Meta);

                MAuditoria.Insertar(new DAuditoria(
                                    Menu.ActUsuario.idUsuario,
                                    DAuditoria.Ver,
                                    "Se ha visualzado la Meta Nº" + Meta.idMeta));

                bool Resp = frmMeta.ShowDialog() ?? false;
                Refresh();
            }
        }
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;

            DMeta Meta = EncontrarMeta(id);

            if (Meta != null)
            {
                MetaType TipoMeta = searchType == SearchType.Departamento ? MetaType.Departamento : MetaType.Empleado;
                MetaFrm frmMeta = new MetaFrm(TypeForm.Update, TipoMeta, Meta);
                bool Resp = frmMeta.ShowDialog() ?? false;
                Refresh();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Resp = MessageBox.Show("¿Seguro que quieres eliminar este item?", "Magicolor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Resp != MessageBoxResult.Yes)
                return;

            int id = (int)((Button)sender).CommandParameter;
            var resp = Metodos.Eliminar(id);

            if (resp.Equals("OK"))
                MAuditoria.Insertar(new DAuditoria(
                                    Menu.ActUsuario.idUsuario,
                                    DAuditoria.Eliminar,
                                    "Se ha Eliminado la Meta Nº" + id));
            Refresh();
        }

        DMeta EncontrarMeta(int idMeta)
        {
            switch (searchType)
            {
                case SearchType.Departamento:
                    return Metodos.EncontrarByDepartamento(idMeta)[0];
                case SearchType.Empleado:
                    return Metodos.EncontrarByEmpleado(idMeta)[0];
                default:
                    return null;
            }
        }



        private void RBEmpleado_Checked(object sender, RoutedEventArgs e)
        {
            GridCBEmpleado.Visibility = Visibility.Visible;
            dgEmpleado.Visibility = Visibility.Visible;
            searchType = SearchType.Empleado;
            Refresh();
        }

        private void RBDepartamento_Checked(object sender, RoutedEventArgs e)
        {
            GridCBEmpleado.Visibility = Visibility.Collapsed;
            dgEmpleado.Visibility = Visibility.Collapsed;
            searchType = SearchType.Departamento;
            Refresh();
        }

        private void CbDepartamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbDepartamento.SelectedIndex > -1)
            {
                PlaceDepartamento.Text = "";

                var resp = new MSeleccion().MostrarEmpleadoByDepartamento((int)CbDepartamento.SelectedValue);

                CbEmpleado.ItemsSource = resp;
                CbEmpleado.DisplayMemberPath = "nombre";
                CbEmpleado.SelectedValuePath = "idEmpleado";

                CbEmpleado.IsEnabled = true;
                Refresh();
            }
            else
            {
                PlaceDepartamento.Text = "Seleccionar Departamento";
                CbEmpleado.IsEnabled = false;
                CbEmpleado.SelectedIndex = -1;
            }
        }

        private void CbEmpleado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbEmpleado.SelectedIndex > -1)
            {
                PlaceEmpleado.Text = "";
                Refresh();
            }
            else
            {
                PlaceEmpleado.Text = "Seleccionar Empleado";
            }
        }

        private void txtLimpiar_Click(object sender, RoutedEventArgs e)
        {
            CbFechaInicio.SelectedDate = null;
            CbFechaFinal.SelectedDate = null;
            CbDepartamento.SelectedIndex = -1;
        }

        private void CbFechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbFechaInicio.SelectedDate != null)
            {
                PlaceFechaInicio.Text = "";

                CbFechaFinal.DisplayDateStart = CbFechaInicio.SelectedDate?.Date.AddDays(1);
                Refresh();

            }
            else
            {
                PlaceFechaInicio.Text = "Fecha Inicio";

                CbFechaFinal.DisplayDateStart = null;
            }
        }

        private void CbFechaFinal_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbFechaFinal.SelectedDate != null)
            {
                PlaceFechaFinal.Text = "";

                CbFechaInicio.DisplayDateEnd = CbFechaFinal.SelectedDate?.Date.AddDays(-1);
                Refresh();

            }
            else
            {
                CbFechaInicio.Text = "Fecha Final";

                CbFechaInicio.DisplayDateEnd = null;
            }
        }

        enum SearchType
        {
            Empleado,
            Departamento
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}
