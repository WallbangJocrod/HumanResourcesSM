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
using System.Windows.Shapes;

using Datos;
using Metodos;

namespace HumanResourcesSM.Windows
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public static DUsuario ActUsuario;
        public Menu(DUsuario User)
        {
            InitializeComponent();
            ActUsuario = User;
        }

        public Border LastSelected;
        public int LastIndex = -1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            if (index == LastIndex)
                return;

            var par = VisualTreeHelper.GetParent((Button)e.Source) as UIElement;
            Border PBord = (par as Border);

            PBord.BorderThickness = new Thickness(0, 0, 0, 5);
            
            if(LastIndex > -1)
            {
                LastSelected.BorderThickness = new Thickness(0, 0, 0, 0);
            }

            LastIndex = index;
            LastSelected = PBord;

            switch (index)
            {
                case 0:
                    GestionMenu frm2 = new GestionMenu();
                    ContentFrame.Content = frm2;
                    
                    break;
                case 1:
                    RelacionesLaboralesDG frm1 = new RelacionesLaboralesDG();
                    ContentFrame.Content = frm1;
                    break;
                case 2:
                    TipoTramiteDG frm = new TipoTramiteDG();
                    ContentFrame.Content = frm;
                    break;
                case 3:
                    UsuarioDG DG = new UsuarioDG();
                    ContentFrame.Content = DG;
                    break;
                case 4:
                    break;

            }

            
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (LastIndex > -1)
            {
                LastSelected.BorderThickness = new Thickness(0, 0, 0, 0);
            }

            LastIndex = -1;
            LastSelected = null;

            AjustesMenu frm = new AjustesMenu();
            ContentFrame.Content = frm;
        }
    }
}
