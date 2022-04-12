USE eCommerce

SELECT * FROM Usuarios  U 
	INNER JOIN Contatos C
ON U.Id = C.UsuarioId
WHERE U.Id = 1;