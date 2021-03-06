﻿using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Metodos
{
    public class MTipoTramite:DTipoTramite
    {
        #region QUERIES
        private string queryInsert = @"
            INSERT INTO [TipoTramite] (
                nombre,
                statusCambio,
                descripcion,
                estado
            ) VALUES (
                @nombre,
                @statusCambio,
                @descripcion,
                1
            );
	    ";

        private string queryUpdate = @"
            UPDATE [TipoTramite] SET
                nombre = @nombre,
                statusCambio = @statusCambio,
                descripcion = @descripcion
            WHERE idTipoTramite = @idTipoTramite;
        ";

        private string queryDelete = @"
            UPDATE [TipoTramite] SET
                estado = 0
            WHERE idTipoTramite = @idTipoTramite;
        ";

        private string queryListName = @"
            SELECT * FROM [TipoTramite] 
            WHERE nombre LIKE @nombre + '%' AND estado <> 0
            ORDER BY nombre;
        ";

        private string queryListStatus = @"
            SELECT * 
            FROM [TipoTramite] where estado <> 0
        ";

        private string queryListID = @"
            SELECT * FROM [TipoTramite] 
            WHERE idTipoTramite = @idTipoTramite AND estado <> 0;
        ";
        #endregion

        public string Insertar(DTipoTramite TipoTramite)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", TipoTramite.nombre);
                comm.Parameters.AddWithValue("@descripcion", TipoTramite.descripcion);
                comm.Parameters.AddWithValue("@statusCambio", TipoTramite.statusCambio);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro del Tipo de Tramite";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public string Editar(DTipoTramite TipoTramite)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", TipoTramite.nombre);
                comm.Parameters.AddWithValue("@statusCambio", TipoTramite.statusCambio);
                comm.Parameters.AddWithValue("@descripcion", TipoTramite.descripcion);
                comm.Parameters.AddWithValue("@idTipoTramite", TipoTramite.idTipoTramite);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del tipo de tramite";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public string Eliminar(int IdTipoTramite)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTipoTramite", IdTipoTramite);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Registro del Tipo de Trámite";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public List<DTipoTramite> Mostrar(string Nombre)
        {
            List<DTipoTramite> ListaGenerica = new List<DTipoTramite>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListName, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DTipoTramite
                    {
                        idTipoTramite = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        statusCambio = reader.GetString(2),
                        descripcion = reader.GetString(3)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public List<DTipoTramite> MostrarStatus()
        {
            List<DTipoTramite> ListaGenerica = new List<DTipoTramite>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListStatus, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DTipoTramite
                    {
                        idTipoTramite = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        statusCambio = reader.GetString(2),
                        descripcion = reader.GetString(3)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public List<DTipoTramite> Encontrar(int IdTipoTramite)
        {
            List<DTipoTramite> ListaGenerica = new List<DTipoTramite>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idTipoTramite", IdTipoTramite);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DTipoTramite
                    {
                        idTipoTramite = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        statusCambio = reader.GetString(2),
                        descripcion = reader.GetString(3)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }
    }
}
