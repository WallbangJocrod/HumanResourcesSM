﻿using System;
using System.Collections.Generic;
using System.Text;
using Datos;

using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.IO;
using System.Security.Cryptography;

namespace Metodos
{
    public class Encripter
    {
        // set permutations
        public const String strPermutation = "ouiveyxaqtd";
        public const Int32 bytePermutation1 = 0x19;
        public const Int32 bytePermutation2 = 0x59;
        public const Int32 bytePermutation3 = 0x17;
        public const Int32 bytePermutation4 = 0x41;

        // encoding
        public static string Encrypt(string strData)
        {

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));
            // reference https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx

        }


        // decoding
        public static string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));
            // reference https://msdn.microsoft.com/en-us/library/system.convert.frombase64string(v=vs.110).aspx

        }

        // encrypt
        public static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Encripter.strPermutation,
            new byte[] { Encripter.bytePermutation1,
                         Encripter.bytePermutation2,
                         Encripter.bytePermutation3,
                         Encripter.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        public static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Encripter.strPermutation,
            new byte[] { Encripter.bytePermutation1,
                         Encripter.bytePermutation2,
                         Encripter.bytePermutation3,
                         Encripter.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }
    }


    public class MUsuario : DUsuario
    { 
        #region QUERIES 
        private string queryInsert = @"
            INSERT INTO [Usuario] (
                idRol,
                usuario,
                contraseña,
                entrevistando
            ) OUTPUT Inserted.idUsuario 
            VALUES (
                @idRol,
                @usuario,
                @contraseña,
                0
            );
	    ";

        private string queryUpdate = @"
            UPDATE [Usuario] SET 
                idRol = @idRol,
                usuario = @usuario,
                contraseña = @contraseña
            WHERE idUsuario = @idUsuario;
        ";

        private string queryDelete = @"
            DELETE FROM [Usuario] 
            WHERE idUsuario = @idUsuario
        ";

        private string queryList = @"
            SELECT * FROM [Usuario] 
            WHERE usuario LIKE @usuario + '%' AND idUsuario <> 1 
            ORDER BY usuario";

        private string queryLogin = @"
            SELECT * FROM [Usuario] 
            WHERE usuario = @usuario AND contraseña = @contraseña AND idUsuario <> 1
        ";

        private string queryListID = @"
            SELECT * FROM [Usuario] 
            WHERE idUsuario = @idUsuario
        ";

        private string queryInterview = @"
            UPDATE [Usuario] SET
                entrevistando = @entrevistando
            WHERE idUsuario = @idUsuario;
        ";

        private string queryInsertSecurity = @"
            INSERT INTO [seguridad] (
                idUsuario,
                pregunta,
                respuesta
            ) VALUES (
                @idUsuario,
                @pregunta,
                @respuesta
            );
        ";

        private string queryDeleteSecurity = @"
            DELETE FROM [seguridad]
            WHERE idUsuario = @idUsuario
	    ";
        #endregion

        public string Insertar(DUsuario Usuario, List<DSeguridad> Seguridad)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryInsert, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idRol", Usuario.idRol);
                comm.Parameters.AddWithValue("@usuario", Usuario.usuario);
                comm.Parameters.AddWithValue("@contraseña", Usuario.contraseña);
                Usuario.idUsuario = (int)comm.ExecuteScalar();

                string respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro del Usuario";
                if (respuesta.Equals("OK"))
                    respuesta = InsertarSeguridad(Seguridad, Usuario.idUsuario);

                return respuesta;
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public string Editar(DUsuario Usuario, List<DSeguridad> Seguridad)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryUpdate, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idRol", Usuario.idRol);
                comm.Parameters.AddWithValue("@usuario", Usuario.usuario);
                comm.Parameters.AddWithValue("@contraseña", Usuario.contraseña);
                comm.Parameters.AddWithValue("@idUsuario", Usuario.idUsuario);

                string respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Actualizó el Registro del Usuario";
                if (respuesta.Equals("OK"))
                    respuesta = BorrarSeguridad(Usuario, Seguridad);

                return respuesta;
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }

        private string InsertarSeguridad(List<DSeguridad> Detalle, int IdUsuario)
        {
            int i = 0;
            string respuesta = "";

            foreach (DSeguridad det in Detalle)
            {
                using SqlCommand comm = new SqlCommand(queryInsertSecurity, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idUsuario", IdUsuario);
                comm.Parameters.AddWithValue("@pregunta", Detalle[i].pregunta);
                comm.Parameters.AddWithValue("@respuesta", Detalle[i].respuesta);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se Ingresó el Registro de la Seguridad";
                if (!respuesta.Equals("OK")) break;

                i++;
            }

            return respuesta;
        }

        private string BorrarSeguridad(DUsuario Usuario, List<DSeguridad> Seguridad)
        {
            using SqlCommand comm = new SqlCommand(queryDeleteSecurity, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idUsuario", Usuario.idUsuario);

            string respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "OK";

            if (respuesta.Equals("OK"))
                InsertarSeguridad(Seguridad, Usuario.idUsuario);

            return respuesta;
        }


        public string Eliminar(int IdUsuario)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryDelete, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idUsuario", IdUsuario);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se Eliminó el Registro del Usuario";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public List<DUsuario> Mostrar(string Usuario)
        {
            List<DUsuario> ListaGenerica = new List<DUsuario>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryList, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Usuario);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DUsuario
                    {
                        idUsuario = reader.GetInt32(0),
                        idRol = reader.GetInt32(1),
                        usuario = reader.GetString(2),
                        contraseña = reader.GetString(3),
                        entrevistando = reader.GetInt32(4)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public List<DUsuario> Login(string Usuario, string Contraseña)
        {
            List<DUsuario> ListaGenerica = new List<DUsuario>();


            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryLogin, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@usuario", Usuario);
                comm.Parameters.AddWithValue("@contraseña", Contraseña);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DUsuario
                    {
                        idUsuario = reader.GetInt32(0),
                        idRol = reader.GetInt32(1),
                        usuario = reader.GetString(2),
                        contraseña = reader.GetString(3),
                        entrevistando = reader.GetInt32(4)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public List<DUsuario> Encontrar(int IdUsuario)
        {
            List<DUsuario> ListaGenerica = new List<DUsuario>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListID, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idUsuario", IdUsuario);

                using SqlDataReader reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    ListaGenerica.Add(new DUsuario
                    {
                        idUsuario = reader.GetInt32(0),
                        idRol = reader.GetInt32(1),
                        usuario = reader.GetString(2),
                        contraseña = reader.GetString(3),
                        entrevistando = reader.GetInt32(4)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public string Entrevistando(int IdUsuario, bool Entrevistando)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryInterview, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@entrevistando", Entrevistando ? 1 : 0);
                comm.Parameters.AddWithValue("@idUsuario", IdUsuario);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se actualizo el Registro del usuario";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }

    }
}
