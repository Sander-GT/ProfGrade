﻿CREATE TABLE USUARIO (
    RUT VARCHAR(10) PRIMARY KEY,
    NOMBRE VARCHAR(50) NOT NULL,
    CORREO VARCHAR(50) NOT NULL,
    TELEFONO VARCHAR(15),
    ROL VARCHAR(20) NOT NULL,
    CLAVE VARCHAR(30) NOT NULL
);

SELECT * FROM USUARIO;


DELETE FROM Usuario
WHERE RUT = '2-9'
  AND Nombre = 'CLIENTE DOS'
  AND Correo = 'prueba2@gmail.com'
  AND Telefono = '+56911111111'
  AND Rol = 'Profesor'
  AND Clave = '@Prueba123';





