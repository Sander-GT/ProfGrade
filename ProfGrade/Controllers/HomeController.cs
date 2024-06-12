using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using System;
using ProfGrade.Models;

namespace ProfGrade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public IActionResult Index()
        {

            return View();
        }



        // Método para manejar todas las operaciones en una sola vista
        [HttpGet]
        public IActionResult Curso()
        {
            DataTable dtCursos = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string selectQuery = "SELECT ID_CURSO, NOMBRE_CURSO FROM Curso";
                using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtCursos);
                }
            }

            return View(dtCursos);
        }


        // Método para insertar un nuevo curso
        [HttpPost]
        public IActionResult CrearCurso(string nombreCurso)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO Curso (NOMBRE_CURSO) VALUES (@NombreCurso)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@NombreCurso", nombreCurso);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ViewBag.alerta = "Curso registrado correctamente";
            return RedirectToAction("Curso");
        }


        // Método para editar curso
        [HttpPost]
        public IActionResult EditCurso(int id, string nombreCurso)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE Curso SET NOMBRE_CURSO = @NombreCurso WHERE ID_CURSO = @Id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@NombreCurso", nombreCurso);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ViewBag.alerta = "Curso actualizado correctamente";
            return RedirectToAction("Curso");
        }



        // Método para eliminar un curso
        [HttpPost]
        public IActionResult EliminarCurso(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM Curso WHERE ID_CURSO = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ViewBag.alerta = "Curso eliminado correctamente";
            return RedirectToAction("Curso");
        }


















        ////////////////////////// VISTA DONDE SALE EL PERSONAL//// MODIFICAR//ELIMINAR//BUSCAR////////////

        public IActionResult Personal()
        {

            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                string query = "SELECT * FROM Usuario";


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario
                            {
                                Rut = reader["Rut"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Rol = reader["Rol"].ToString(),
                                Clave = reader["Clave"].ToString()
                            };

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            DataTable dataTable = Tabla(usuarios);
            return View(dataTable);
        }


        ///////////////////////// DATA TABLE CON LOS USUARIOS REGISTRADOS //////////////////////////
        private DataTable Tabla(List<Usuario> usuarios)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Rut", typeof(string));
            table.Columns.Add("Nombre", typeof(string));
            table.Columns.Add("Correo", typeof(string));
            table.Columns.Add("Telefono", typeof(string));
            table.Columns.Add("Rol", typeof(string));
            table.Columns.Add("Clave", typeof(string));

            foreach (var usuario in usuarios)
            {
                table.Rows.Add(usuario.Rut, usuario.Nombre, usuario.Correo, usuario.Telefono, usuario.Rol, usuario.Clave);
            }

            return table;
        }


















        public ActionResult IniciarSesion(string rut, string clave)
        {
            // Verifico si se ingresaron datos
            if (string.IsNullOrEmpty(rut) || string.IsNullOrEmpty(clave))
            {
                ViewBag.mensaje = "Ingrese sus credenciales.";
                return View("Index");
            }

            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                string query = "SELECT Rut, Rol FROM Usuario WHERE Rut = @Rut AND Clave = @Clave";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Rut", SqlDbType.NVarChar).Value = rut;
                    cmd.Parameters.Add("@Clave", SqlDbType.NVarChar).Value = clave;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string? rol = reader["Rol"].ToString();

                            // Usuario autenticado

                            switch (rol)
                            {
                                case "Administrador":
                                    return RedirectToAction("Menu_Administrador");
                                case "Director":
                                    return RedirectToAction("Menu_Administrador");
                                case "Profesor":
                                    return RedirectToAction("Menu_Administrador");
                                default:
                                    ViewBag.mensaje = "Rol no reconocido.";
                                    return View("Index");
                            }
                        }
                    }
                }
            }


            ViewBag.mensaje = "Credenciales incorrectas. Inténtalo nuevamente.";
            return View("Index");
        }




        //Vista Admin
        public IActionResult Menu_Administrador()
        {
            return View();
        }



        ///////////////////////// VISTA REGISTRO DE USUARIOS //////////////////////////

        public IActionResult Registro(string Rut, string Nombre, string Correo, string Telefono, string Rol, string Clave)
        {
            // Verifico si se ingresaron datos
            if (string.IsNullOrEmpty(Rut) || string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Rol) || string.IsNullOrEmpty(Clave))
            {
                ViewBag.alerta = "Ingrese sus Datos.";
                return View("Registro");
            }

            // Verifico la seguridad de la clave
            if (!EsClaveSegura(Clave))
            {
                ViewBag.alerta = "La clave no es segura. Debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales.";
                return View("Registro");
            }

            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                // Verifico si el Rut ya existe
                string rutExistenteQuery = "SELECT COUNT(*) FROM Usuario WHERE Rut = @Rut";
                using (SqlCommand rutExistenteCmd = new SqlCommand(rutExistenteQuery, con))
                {
                    rutExistenteCmd.Parameters.AddWithValue("@Rut", Rut);
                    int rutExistenteCount = (int)rutExistenteCmd.ExecuteScalar();

                    if (rutExistenteCount > 0)
                    {
                        ViewBag.alerta = "Este Rut ya está registrado.";
                        return View("Registro");
                    }
                }

                // Inserto el nuevo usuario si el Rut no está registrado
                string insertQuery = "INSERT INTO Usuario (Rut, Nombre, Correo, Telefono, Rol, Clave) VALUES (@Rut, @Nombre, @Correo, @Telefono, @Rol, @Clave)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@Rut", Rut);
                    cmd.Parameters.AddWithValue("@Nombre", Nombre);
                    cmd.Parameters.AddWithValue("@Correo", Correo);
                    cmd.Parameters.AddWithValue("@Telefono", Telefono);
                    cmd.Parameters.AddWithValue("@Rol", Rol);
                    cmd.Parameters.AddWithValue("@Clave", Clave);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ViewBag.alerta = "Registro OK";
                    }
                    else
                    {
                        ViewBag.alerta = "Error al registrar el usuario";
                    }
                }
            }

            return View();
        }



        ////////////// FUNCION CLAVE SEGURA //////////////
        private bool EsClaveSegura(string clave)
        {
            if (clave.Length < 8)
                return false;
            if (!clave.Any(char.IsUpper))
                return false;
            if (!clave.Any(char.IsLower))
                return false;
            if (!clave.Any(char.IsDigit))
                return false;
            if (!clave.Any(ch => !char.IsLetterOrDigit(ch))) // Verifica si hay algún carácter especial
                return false;

            return true;
        }





        ////////////////////////////////// MOSTRAR DATOS DE LOS USUARIOS ///////////////////////////
        private List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                string query = "SELECT * FROM Usuario";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario
                            {
                                Rut = reader["Rut"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Rol = reader["Rol"].ToString(),
                                Clave = reader["Clave"].ToString()
                            };

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return usuarios;
        }



        public class Usuario
        {
            public string? Rut { get; set; }
            public string? Nombre { get; set; }
            public string? Correo { get; set; }
            public string? Telefono { get; set; }
            public string? Rol { get; set; }
            public string? Clave { get; set; }
        }




        ///////////////////////////////////////// OBTENER USUARIO POR EL RUT ///////////////////////////
        private Usuario ObtenerUsuarios(string rut)
        {
            Usuario? usuario = null;

            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                string query = "SELECT * FROM Usuario WHERE Rut = @Rut";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Rut", rut);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Rut = reader["Rut"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Rol = reader["Rol"].ToString(),
                                Clave = reader["Clave"].ToString()
                            };
                        }
                    }
                }
            }

            return usuario;
        }







        ////////////////// METODO PARA BUSCAR UN USUARIO POR RUT ///////////////////////
        [HttpPost]
        public ActionResult BuscarUsuario(string rut)
        {
            Usuario usuario = ObtenerUsuarios(rut);

            if (usuario != null)
            {
                // Muestra los datos del usuario encontrado
                ViewBag.Rut = usuario.Rut;
                ViewBag.Nombre = usuario.Nombre;
                ViewBag.Correo = usuario.Correo;
                ViewBag.Telefono = usuario.Telefono;
                ViewBag.Rol = usuario.Rol;
                ViewBag.Clave = usuario.Clave;

                ViewBag.Mensaje = "Usuario encontrado.";
            }
            else
            {
                // Limpiar los datos si no se encuentra el usuario
                LimpiarDatos();
                ViewBag.Mensaje = "Usuario no encontrado.";
            }

            // Recargar la lista de usuarios
            List<Usuario> usuarios = ObtenerUsuarios();
            ViewBag.Usuarios = usuarios;

            return View("Personal");
        }



        /////////////////////////////// ELIMINAR USUARIO/////////////////////////////////
        public IActionResult Eliminar(string RutEliminar)
        {
            if (!string.IsNullOrEmpty(RutEliminar))
            {
                using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                {
                    con.Open();

                    string sql = "DELETE FROM Usuario WHERE Rut = @rut";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@rut", RutEliminar));

                        int Registro = cmd.ExecuteNonQuery();

                        if (Registro > 0)
                        {
                            ViewBag.Message = "Registro eliminado exitosamente.";
                        }
                        else
                        {
                            ViewBag.Message = "No se encontró ningún registro con el RUT ingresado.";
                        }
                    }
                }
            }
            else
            {
                ViewBag.Message = "Debe proporcionar un RUT para eliminar un registro.";
            }


            ViewBag.Usuarios = ObtenerUsuarios();

            return View("/Views/Home/Personal.cshtml");
        }


        // Método para limpiar los datos en ViewBag
        private void LimpiarDatos()
        {
            ViewBag.Rut = string.Empty;
            ViewBag.Nombre = string.Empty;
            ViewBag.Correo = string.Empty;
            ViewBag.Telefono = string.Empty;
            ViewBag.Rol = string.Empty;
            ViewBag.Clave = string.Empty;
        }




        ///////////////////////////////// MODIFICAR USUARIOS ///////////////////////////////////////////////

        [HttpPost]
        public ActionResult ModificarUsuario(string Rut, string Nombre, string Correo, string Telefono, string Rol, string Clave)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BD_ProfGrade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                con.Open();

                SqlCommand sentencia = new SqlCommand();
                sentencia.Connection = con;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "UPDATE Usuario SET Nombre = @Nombre, Correo = @Correo, Telefono = @Telefono, Rol = @Rol, Clave = @Clave WHERE Rut = @Rut";
                sentencia.Parameters.AddWithValue("@Rut", Rut);
                sentencia.Parameters.AddWithValue("@Nombre", Nombre);
                sentencia.Parameters.AddWithValue("@Correo", Correo);
                sentencia.Parameters.AddWithValue("@Telefono", Telefono);
                sentencia.Parameters.AddWithValue("@Rol", Rol);
                sentencia.Parameters.AddWithValue("@Clave", Clave);

                var result = sentencia.ExecuteNonQuery();
                var mensaje = (result > 0) ? "Registro Modificado" : "No se encontró el usuario con el Rut";

                ViewBag.mensajeMod = mensaje;

                con.Close();
            }

            ViewBag.Usuarios = ObtenerUsuarios(); //actualizo los usuarios

            return View("Personal");
        }






















        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}