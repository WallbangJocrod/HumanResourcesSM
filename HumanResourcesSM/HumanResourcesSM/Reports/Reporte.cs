﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Datos;
using Metodos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Reporting.WinForms;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace HumanResourcesSM.Reports
{
    public class Reporte : Conexion
    {

        public void ExportPDF<T>(List<T> MethodReport, string ReportName, string DetailName = "")
        {
            try
            {

                string deviceInfo = @"<DeviceInfo>
                                    <EmbedFonts>None</EmbedFonts>
                                    </DeviceInfo>"; //tamaño de la pagina, default
                string[] streamIds;
                Warning[] warnings;

                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                ReportViewer viewer = new ReportViewer();
                viewer.ProcessingMode = ProcessingMode.Local;
                viewer.LocalReport.ReportPath = @"..\..\Reports\rpt" + ReportName + ".rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource((ReportName + "DS"), MethodReport));

                viewer.RefreshReport();
                var bytes = viewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string fileName = RouteSavePDF(SetReportName(ReportName, DetailName));
                if (fileName != SetReportName(ReportName, DetailName))
                {
                    File.WriteAllBytes(fileName, bytes);
                    Process.Start(fileName);
                }
                else
                    System.Windows.MessageBox.Show("Operación Cancelada", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException e) { System.Windows.MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }
        //C:\Users\ramon\Desktop\HumanResourcesSM\HumanResourcesSM\HumanResourcesSM\Reports\

        public void ExportPDFTwoArguments<T, F>(List<T> MethodReport, string ReportName, List<F> SecondMethodReport, string SecondReportName, bool SaveConfirmation = false, string DetailName = "")
        {
            try
            {
                string deviceInfo = @"<DeviceInfo>
                                    <EmbedFonts>None</EmbedFonts>
                                    </DeviceInfo>"; //tamaño de la pagina, default
                string[] streamIds;
                Warning[] warnings;

                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                ReportViewer viewer = new ReportViewer();
                viewer.ProcessingMode = ProcessingMode.Local;
                viewer.LocalReport.ReportPath = @"..\..\Reports\rpt" + ReportName + ".rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource((ReportName + "DS"), MethodReport));
                viewer.LocalReport.DataSources.Add(new ReportDataSource((SecondReportName + "DS"), SecondMethodReport));

                viewer.RefreshReport();
                var bytes = viewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string fileName = SaveConfirmation ? RouteSavePDF(SetReportName(ReportName, DetailName)) : ReportName;
                if (fileName != SetReportName(ReportName, DetailName))
                {
                    File.WriteAllBytes(fileName, bytes);
                    Process.Start(fileName);
                }
                else
                    System.Windows.MessageBox.Show("Operación Cancelada", "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException e) { System.Windows.MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }

        private string RouteSavePDF(string Name)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Pdf|*.pdf";
            saveFileDialog.Title = "Guardar archivo";
            saveFileDialog.FileName = Name;
            saveFileDialog.ShowDialog();

            return saveFileDialog.FileName;
        }

        private string SetReportName(string ReportName, string DetailName)
        {
            if (DetailName != "")
                return (ReportName + "-" + DetailName);
            else
                return ReportName;
        }
    }
}
