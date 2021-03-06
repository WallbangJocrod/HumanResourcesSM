﻿using System;
using System.Collections.Generic;
using System.Text;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Windows;


namespace Metodos
{
    public class MPago:DPago
    {

        #region QUERIES
        //insertar
        private string queryInsertPay = @"
            INSERT INTO [Pago] (
                idEmpleado,
                fechaPago,
                banco,
                numeroReferencia,
                cantidadHoras,
                periodoInicio,
                periodoFinal,
                montoTotal,
                totalAsignacion,
                totalDeduccion,
                totalSalario,
                estado
            ) OUTPUT Inserted.idPago 
            VALUES (
                @idEmpleado,
                @fechaPago,
                @banco,
                @numeroReferencia,
                @cantidadHoras,
                @periodoInicio,
                @periodoFinal,
                @montoTotal,
                @totalAsignacion,
                @totalDeduccion,
                @totalSalario,
                1
            );
	    ";

        private string queryInsertPayDetail = @"
            INSERT INTO [DetallePago] (
                idPago,
                concepto,
                cantidad,
                salario,
                asignacion,
                deduccion      
            ) VALUES (
                @idPago,
                @concepto,
                @cantidad,
                @salario,
                @asignacion,
                @deduccion 
            );
	    ";

        private string queryUpdateDebt = @"
            UPDATE [Deuda] SET
                pagado = pagado + @pagado
            WHERE idDeuda = @idDeuda;
	    ";

        private string queryUpdateDebtComplete = @"
            UPDATE [Deuda] SET
                status = 2
            WHERE pagado = monto AND idDeuda = @idDeuda;
        ";

        //anular
        private string queryNullPay = @"
            UPDATE [Pago] SET
                estado = 0
            WHERE idPago = @idPago;
	    ";

        //mostrar Ultimo
        private string queryLast = @"
           SELECT TOP 1 
                *
            FROM [Pago] p ORDER BY idPago DESC
        ";

        //mostrar
        private string queryListPay = @"
           SELECT 
                p.idPago, 
                p.fechaPago, 
                e.cedula, 
                e.apellido + ' ' + e.nombre as NombreCompleto, 
                p.numeroReferencia, 
                p.banco, 
                p.periodoInicio,
                p.periodoFinal,
                p.montoTotal, 
                p.totalAsignacion,
                p.totalDeduccion,
                p.totalSalario,
                p.estado 
            FROM [Pago] p 
                INNER JOIN [Empleado] e ON p.idEmpleado=e.idEmpleado 
            WHERE p.idPago = @idPago
            ORDER BY p.numeroReferencia;
        ";

        //mostrar
        private string queryListByEmployee = @"
           SELECT 
                p.idPago,
				p.periodoInicio,
				p.periodoFinal
            FROM [Pago] p 
            WHERE p.idEmpleado = @idEmpleado AND p.estado <> 0
            ORDER BY p.idPago DESC;
        ";

        private string queryListDetailPay = @"
            SELECT 
				dp.idDetallePago,
				dp.concepto,
				dp.cantidad,
                dp.salario,
                dp.asignacion,
                dp.deduccion
            FROM [DetallePago] dp
				INNER JOIN [Pago] p ON p.idPago=dp.idPago
			WHERE p.idPago=@idPago;
        ";

        private string queryListEmployee = @"
            SELECT 
                e.idEmpleado,
                (e.apellido + ' ' + e.nombre) AS NombreCompleto, 
				d.nombre,
                ISNULL(c.sueldo, 0) AS sueldoFinal, 
				ISNULL((
					SELECT TOP 1
						montoTotal
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY idPago DESC), 0) AS ultimoPago,
				ISNULL((
					SELECT TOP 1
						periodoInicio
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY idPago DESC), null) AS ultimoPeriodoInicio,
				ISNULL((
					SELECT TOP 1
						periodoFinal
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY idPago DESC), null) AS ultimoPeriodoFinal,
				e.status
            FROM [Empleado] e
				INNER JOIN [Contrato] c ON e.idEmpleado = c.idEmpleado
				INNER JOIN [Departamento] d ON e.idDepartamento = d.idDepartamento
            WHERE e.nombre LIKE @nombre + '%' 
            ORDER BY e.nombre
        ";

        private string queryListEmpleyeeDetail = @"
            SELECT 
				e.idEmpleado,
                e.nombre, 
				e.apellido,
                ISNULL(c.sueldo, 0) AS sueldo, 
				d.nombre,
				e.email,
				e.telefono,
				ISNULL((
					SELECT TOP 1
						p.fechaPago
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY p.idPago DESC), null) AS ultimoPagoFecha,
				ISNULL((
					SELECT TOP 1
						p.periodoInicio
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY p.idPago DESC), null) AS ultimoPeriodoInicio,
				ISNULL((
					SELECT TOP 1
						p.periodoFinal
					FROM [Pago] p
					where p.idEmpleado = e.idEmpleado and p.estado <> 0
					ORDER BY p.idPago DESC), null) AS ultimoPeriodoFinal,
				e.status
            FROM [Empleado] e
				INNER JOIN [Contrato] c ON e.idEmpleado = c.idEmpleado
				INNER JOIN [Departamento] d ON e.idDepartamento = d.idDepartamento
            WHERE e.idEmpleado = @idEmpleado 
            ORDER BY e.idEmpleado
        ";
        #endregion


        public string Insertar(DPago Pago, List<DDetallePago> DetallePago)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryInsertPay, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idEmpleado", Pago.idEmpleado);
                comm.Parameters.AddWithValue("@fechaPago", Pago.fechaPago);
                comm.Parameters.AddWithValue("@banco", Pago.banco);
                comm.Parameters.AddWithValue("@numeroReferencia", Pago.numeroReferencia);
                comm.Parameters.AddWithValue("@cantidadHoras", Pago.cantidadHoras);
                comm.Parameters.AddWithValue("@periodoInicio", Pago.periodoInicio);
                comm.Parameters.AddWithValue("@periodoFinal", Pago.periodoFinal);
                comm.Parameters.AddWithValue("@montoTotal", Pago.montoTotal);
                comm.Parameters.AddWithValue("@totalAsignacion", Pago.totalAsignacion);
                comm.Parameters.AddWithValue("@totalDeduccion", Pago.totalDeduccion);
                comm.Parameters.AddWithValue("@totalSalario", Pago.totalSalario);

                Pago.idPago = (int)comm.ExecuteScalar();

                string respuesta = !String.IsNullOrEmpty(idPago.ToString()) ? "OK" : "No se Ingresó el Registro del Pago";

                if (!respuesta.Equals("OK")) return "No se ingreso el Registro del pago";

                respuesta = InsertarDetallePago(Pago.idPago, DetallePago);
                if (!respuesta.Equals("OK")) return "No se ingreso el Detalle del pago";

                return ActualizarDeuda(Pago.idEmpleado);
            }
            catch (SqlException e) { return e.Message; }
            catch (Exception ex) { return ex.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }

        private string InsertarDetallePago(int IdPago, List<DDetallePago> DetallePago)
        {
            string respuesta = "";
            int i = 0;

            foreach (DDetallePago det in DetallePago)
            {

                using SqlCommand comm = new SqlCommand(queryInsertPayDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idPago", IdPago);
                comm.Parameters.AddWithValue("@concepto", DetallePago[i].concepto);
                comm.Parameters.AddWithValue("@cantidad", DetallePago[i].cantidad);
                comm.Parameters.AddWithValue("@salario", DetallePago[i].salario);
                comm.Parameters.AddWithValue("@asignacion", DetallePago[i].asignacion);
                comm.Parameters.AddWithValue("@deduccion", DetallePago[i].deduccion);

                respuesta = comm.ExecuteNonQuery() == 1 ? "OK" : "No se ingresaron los detalles del pago";

                if (!respuesta.Equals("OK"))
                    throw new NullReferenceException("Error en el Registro del Detalle del Pago");

                i++;
            }

            return respuesta; 
        }

        private string ActualizarDeuda(int IdEmpleado)
        {
            string queryUpdateAllDebt = @"
                UPDATE [Deuda] SET
                    deuda.status = 2
			    FROM [Deuda] d
				    INNER JOIN [Empleado] e ON d.idEmpleado = e.idEmpleado
                WHERE e.idEmpleado = @idEmpleado AND d.status = 1 AND d.repetitivo = 0;
            ";

            using SqlCommand comm = new SqlCommand(queryUpdateAllDebt, Conexion.ConexionSql);
            comm.Parameters.AddWithValue("@idEmpleado", IdEmpleado);

            return comm.ExecuteNonQuery() == 1 ? "OK" : "OK";
        }


        public string Anular(int IdPago)
        {
            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryNullPay, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idPago", IdPago);

                return comm.ExecuteNonQuery() == 1 ? "OK" : "No se elimino el Registro del pago";
            }
            catch (SqlException e) { return e.Message; }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }
        }


        public List<DPago> Mostrar(int IdPago)
        {
            List<DPago> ListaGenerica = new List<DPago>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListPay, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idPago", IdPago);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DPago
                    {
                        idPago = reader.GetInt32(0),
                        fechaPago = reader.GetDateTime(1),
                        cedula = reader.GetString(2),
                        nombre = reader.GetString(3),
                        numeroReferencia = reader.GetString(4),
                        banco = reader.GetString(5),
                        periodoInicio = reader.GetDateTime(6),
                        periodoFinal = reader.GetDateTime(7),
                        montoTotal = (double)reader.GetDecimal(8),
                        totalAsignacion = reader.GetDouble(9),
                        totalDeduccion = reader.GetDouble(10),
                        totalSalario = reader.GetDouble(11),
                        estado = reader.GetInt32(12),
                        usuarioEmisor = Globals.USUARIO_SISTEMA
                    });
                }
                
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;

        }

        public List<DPago> MostrarUltimo()
        {
            List<DPago> ListaGenerica = new List<DPago>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryLast, Conexion.ConexionSql);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DPago
                    {
                        idPago = reader.GetInt32(0)
                    });
                }

            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;

        }


        public List<DDetallePago> MostrarDetalle(int IdPago)
        {
            List<DDetallePago> ListaGenerica = new List<DDetallePago>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListDetailPay, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idPago", IdPago);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DDetallePago
                    {
                        idDetallePago = reader.GetInt32(0),
                        concepto = reader.GetString(1),
                        cantidad = reader.GetString(2),
                        salario = reader.GetString(3),
                        asignacion = reader.GetString(4),
                        deduccion = reader.GetString(5)
                    });
                }

            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;

        }

        public List<DEmpleado> MostrarEmpleado(string Nombre)
        {
            List<DEmpleado> ListaGenerica = new List<DEmpleado>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListEmployee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@nombre", Nombre);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    string periodoPago;
                    if (reader.IsDBNull(5) || reader.IsDBNull(6))
                        periodoPago = "N/A";
                    else
                    {
                        DateTime FechaInicio = reader.GetDateTime(5);
                        DateTime FechaFinal = reader.GetDateTime(6);
                        if (DateTime.Now <= FechaFinal)
                            continue;
                        periodoPago = FechaInicio + " - " + FechaFinal;
                    }

                    ListaGenerica.Add(new DEmpleado
                    {
                        idEmpleado = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        nombreDepartamento = reader.GetString(2),
                        sueldo = (double)reader.GetDecimal(3),
                        ultimoPago = (double)reader.GetDecimal(4),
                        periodo = periodoPago,
                        status = reader.GetInt32(7)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }


        public List<DEmpleado> MostrarEmpleadoDetalle(int IdEmpleado)
        {
            List<DEmpleado> ListaGenerica = new List<DEmpleado>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListEmpleyeeDetail, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idEmpleado", IdEmpleado);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    string periodoPago;
                    if (reader.IsDBNull(8) || reader.IsDBNull(9))
                        periodoPago = "N/A";
                    else
                        periodoPago = reader.GetDateTime(8) + " - " + reader.GetDateTime(9);

                    string ultimopago = "";
                    if (!reader.IsDBNull(7))
                        ultimopago = reader.GetDateTime(7).ToShortDateString();
                    else
                        ultimopago = "N/A";

                    ListaGenerica.Add(new DEmpleado
                    {
                        idEmpleado = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellido = reader.GetString(2),
                        sueldo = (double)reader.GetDecimal(3),
                        nombreDepartamento = reader.GetString(4),
                        email = reader.GetString(5),
                        telefono = reader.GetString(6),
                        ultimoPagoFecha = ultimopago,
                        periodo = periodoPago,
                        status = reader.GetInt32(10)
                    });
                }
            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;
        }

        public List<DPago> MostrarByEmpleado(int IdEmpleado)
        {
            List<DPago> ListaGenerica = new List<DPago>();

            try
            {
                Conexion.ConexionSql.Open();

                using SqlCommand comm = new SqlCommand(queryListByEmployee, Conexion.ConexionSql);
                comm.Parameters.AddWithValue("@idEmpleado", IdEmpleado);

                using SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    ListaGenerica.Add(new DPago
                    {
                        idPago = reader.GetInt32(0),
                        periodoInicio = reader.GetDateTime(1),
                        periodoFinal = reader.GetDateTime(2)
                    });
                }

            }
            catch (SqlException e) { MessageBox.Show(e.Message, "SwissNet", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { if (Conexion.ConexionSql.State == ConnectionState.Open) Conexion.ConexionSql.Close(); }

            return ListaGenerica;

        }



    }
}
