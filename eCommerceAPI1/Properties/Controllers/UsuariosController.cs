using eCommerceAPI1.Models;
using eCommerceAPI1.Repositories;
using Microsoft.AspNetCore.Mvc;
/* CRUD
 * - GET -> Obter a lista de usuários,
 * - GET -> Obter o usuário por Id,
 * - POST -> Cadastrar um usuário,
 * - PUT -> Atualizar um usuário,
 * - DELETE -> Remover um usuário. 
 *  
 * METHOD HTTP: www.minhaapi.com.br/api/Usuarios
 * www.minhaapi.com.br/api/Usuarios/2
 */

namespace eCommerceAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;

        public UsuariosController(IUsuarioRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repository.Get()); //HTTP - 200 - OK
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuario = _repository.Get(id);

            if (usuario == null)
            {
                return NotFound(); //ERRO HTTP: 404 - Not Found
            }
            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Insert([FromBody] Usuario usuario)
        {
            _repository.Insert(usuario);
            return Ok(usuario);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Usuario usuario)
        {
            _repository.Update(usuario);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return Ok();
        }
    }
}
