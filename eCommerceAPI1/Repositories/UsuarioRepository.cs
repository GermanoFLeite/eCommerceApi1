using eCommerceAPI1.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System;

namespace eCommerceAPI1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;
        public UsuarioRepository()
        {
            _connection =  new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
        //ADO.NET > Dapper: Micro-ORM(MER <-> POO)
        public List<Usuario> Get() 
        {
            // _connection.Query<Usuario>("SELECT * FROM Usuarios").ToList();
            List<Usuario> usuarios = new List<Usuario>();

            string sql = "SELECT* FROM Usuarios AS U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id;";
            _connection.Query<Usuario, Contato, EnderecoEntrega, Usuario>(sql, 
                (usuario, contato, enderecoEntrega) => {

                    if(usuarios.SingleOrDefault(a => a.Id == usuario.Id) == null)
                    {
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(a => a.Id == usuario.Id);
                    }

                    usuario.EnderecosEntrega.Add(enderecoEntrega);
                    return usuario;
                });

            return usuarios;
        }



        public Usuario Get(int id) 
        {
            List<Usuario> usuarios = new List<Usuario>();

            string sql = "SELECT* FROM Usuarios AS U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id WHERE U.Id = @Id;";
            _connection.Query<Usuario, Contato, EnderecoEntrega, Usuario>(sql,
                (usuario, contato, enderecoEntrega) => {

                    if (usuarios.SingleOrDefault(a => a.Id == usuario.Id) == null)
                    {
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(a => a.Id == usuario.Id);
                    }

                    usuario.EnderecosEntrega.Add(enderecoEntrega);
                    return usuario;
                }, new { Id = id});

            return usuarios.SingleOrDefault();
        }  

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                string sql = "INSERT INTO Usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES (@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                 usuario.Id = _connection.Query<int>(sql, usuario, transaction).FirstOrDefault();

                if (usuario.Contato != null)
                {
                    usuario.Contato.UsuarioId = usuario.Id;
                    string sqlContato = "INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    usuario.Contato.Id = _connection.Query<int>(sqlContato, usuario.Contato, transaction).Single();
                }

                if (usuario.EnderecosEntrega != null && usuario.EnderecosEntrega.Count > 0 )
                {
                    foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = usuario.Id;
                        string sqlEndereco = "INSERT INTO EnderecoEntrega(UsuairoId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuairoId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        enderecoEntrega.Id = _connection.Query<int>(sqlEndereco, enderecoEntrega, transaction).Single();
                    }

                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
            }
            finally
            {
                _connection.Close();
            }                       
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            var transanction = _connection.BeginTransaction();            
            try
            {
                string sql = "UPDATE usuarios SET Nome = @Nome, Email = @Email , Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _connection.Execute(sql, usuario, transanction);

                if (usuario.Contato != null)
                {
                    string sqlContato = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id; ";
                    _connection.Execute(sqlContato, usuario.Contato, transanction);
                }


                transanction.Commit();
            }                
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transanction.Rollback();
            }
            finally
            {
                _connection.Close();
            }

        }
        public void Delete(int id) 
        {
            _connection.Execute("DELETE FROM Usuarios WHERE Id = @id", new { id });
        }
    }
}
