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

namespace HumanResourcesSM.Windows
{
    /// <summary>
    /// Interaction logic for SeleccionFrm.xaml
    /// </summary>
    public partial class GestionMenu : Page
    {
        public GestionMenu()
        {
            InitializeComponent();

            SeleccionFrm frm = new SeleccionFrm();
            ContentFrame.Content = frm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button thisbtn = (Button)e.Source;

            int index = int.Parse(thisbtn.Uid);

            var par = VisualTreeHelper.GetParent((Button)e.Source) as UIElement;
            Grid PGrid = (par as Grid);

            var ParBoord = VisualTreeHelper.GetParent(SelectedBord);
            Grid LastPGrid = ParBoord as Grid;

            LastPGrid.Children.Remove(SelectedBord);

            PGrid.Children.Add(SelectedBord);

            switch (index)
            {
                case 0:
                    SeleccionFrm frm = new SeleccionFrm();
                    ContentFrame.Content = frm;
                    break;
                case 1:
                    RelacionesLaboralesDG frm1 = new RelacionesLaboralesDG();
                    ContentFrame.Content = frm1;
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

        }
    }
}