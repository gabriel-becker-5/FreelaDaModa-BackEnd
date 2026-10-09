using _02_Application.DTOs;
using _02_Application.DTOs.Company;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _02_Application.Services;
using _04_Domain.Entities;
using _04_Domain.Entities.Identity;
using _04_Domain.Entities.Profiles;
using _04_Domain.Enums;
using _04_Domain.Interfaces;

namespace _05_Tests
{
    public class OrdemServicoServiceTests
    {
        private const string EmailEmpresaLogada = "empresa@teste.com";
        private const int IdEmpresaLogada = 1;

        private static OrdemServicoService CriarServico(
            out FakeOrdemServicoRepository ordemServicoRepository,
            out FakeUserRepository userRepository,
            User? freelancer = null)
        {
            ordemServicoRepository = new FakeOrdemServicoRepository();
            userRepository = new FakeUserRepository();

            User empresaLogada = new()
            {
                Id = IdEmpresaLogada,
                Email = EmailEmpresaLogada,
                Roles = new List<Roles> { Roles.Company }
            };
            userRepository.Adicionar(empresaLogada);

            if (freelancer != null)
            {
                userRepository.Adicionar(freelancer);
            }

            FakeUserService userService = new(empresaLogada, EmailEmpresaLogada);

            return new OrdemServicoService(ordemServicoRepository, userService, userRepository);
        }

        private static OrdemServicoDto CriarDtoValido(int? freelancerId = null)
        {
            return new OrdemServicoDto
            {
                Titulo = "Confecção de peças",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                FreelancerId = freelancerId
            };
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoFreelancerNaoExiste()
        {
            OrdemServicoService servico = CriarServico(out _, out _);
            OrdemServicoDto dto = CriarDtoValido(freelancerId: 999);

            await Assert.ThrowsAsync<ArgumentException>(() => servico.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoFreelancerEstaExcluido()
        {
            User freelancerExcluido = new()
            {
                Id = 2,
                Email = "freelancer@teste.com",
                IsDeleted = true,
                Roles = new List<Roles> { Roles.Freelancer }
            };

            OrdemServicoService servico = CriarServico(out _, out _, freelancerExcluido);
            OrdemServicoDto dto = CriarDtoValido(freelancerId: freelancerExcluido.Id);

            await Assert.ThrowsAsync<ArgumentException>(() => servico.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoUsuarioNaoTemPapelDeFreelancer()
        {
            User usuarioEmpresa = new()
            {
                Id = 3,
                Email = "outraempresa@teste.com",
                Roles = new List<Roles> { Roles.Company }
            };

            OrdemServicoService servico = CriarServico(out _, out _, usuarioEmpresa);
            OrdemServicoDto dto = CriarDtoValido(freelancerId: usuarioEmpresa.Id);

            await Assert.ThrowsAsync<ArgumentException>(() => servico.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_DeveCriarComFreelancerValido_EStatusSempreEmAndamento()
        {
            User freelancerValido = new()
            {
                Id = 4,
                Email = "freelancer2@teste.com",
                IsDeleted = false,
                Roles = new List<Roles> { Roles.Freelancer }
            };

            OrdemServicoService servico = CriarServico(out _, out _, freelancerValido);
            OrdemServicoDto dto = CriarDtoValido(freelancerId: freelancerValido.Id);
            dto.Status = "qualquer coisa que o cliente mande";

            OrdemServico criada = await servico.CreateAsync(dto);

            Assert.Equal(freelancerValido.Id, criada.FreelancerId);
            Assert.Equal("Em andamento", criada.Status);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarArgumentException_QuandoTentaTrocarFreelancerId()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 10,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Em andamento",
                FreelancerId = 5
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido(freelancerId: 999);
            dto.Id = existente.Id;

            await Assert.ThrowsAsync<ArgumentException>(() => servico.UpdateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarArgumentException_QuandoOsNaoTemFreelancerEDtoInformaUm()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 15,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Em andamento",
                FreelancerId = null
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido(freelancerId: 999);
            dto.Id = existente.Id;
            dto.Titulo = "Tentativa de alteração";

            ArgumentException excecao =
                await Assert.ThrowsAsync<ArgumentException>(() => servico.UpdateAsync(dto));

            Assert.Equal(
                "O freelancer da ordem de serviço não pode ser alterado por esta operação.",
                excecao.Message);
            Assert.Null(existente.FreelancerId);
            Assert.Equal("Título original", existente.Titulo);
            Assert.Equal("Em andamento", existente.Status);
        }

        [Fact]
        public async Task UpdateAsync_DeveManterFreelancerId_QuandoDtoEnviaOMesmoValor()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 11,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Em andamento",
                FreelancerId = 5
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido(freelancerId: 5);
            dto.Id = existente.Id;

            bool resultado = await servico.UpdateAsync(dto);

            Assert.True(resultado);
            Assert.Equal(5, existente.FreelancerId);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarArgumentException_QuandoStatusInvalido()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 12,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Em andamento"
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido();
            dto.Id = existente.Id;
            dto.Status = "xpto";

            await Assert.ThrowsAsync<ArgumentException>(() => servico.UpdateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_DeveManterStatusAtual_QuandoDtoStatusForNulo()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 13,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Concluída"
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido();
            dto.Id = existente.Id;
            dto.Status = null;

            bool resultado = await servico.UpdateAsync(dto);

            Assert.True(resultado);
            Assert.Equal("Concluída", existente.Status);
        }

        [Fact]
        public async Task UpdateAsync_DeveNormalizarStatus_QuandoDtoEnviaValorValido()
        {
            OrdemServicoService servico = CriarServico(out FakeOrdemServicoRepository ordemServicoRepository, out _);

            OrdemServico existente = new()
            {
                Id = 14,
                UserId = IdEmpresaLogada,
                Titulo = "Título original",
                Categoria = "Costura",
                Modalidade = "Remoto",
                Cidade = "São Paulo",
                Valor = 100,
                Prazo = DateTime.Now.AddDays(10),
                Status = "Em andamento"
            };
            ordemServicoRepository.Adicionar(existente);

            OrdemServicoDto dto = CriarDtoValido();
            dto.Id = existente.Id;
            dto.Status = "Concluída";

            bool resultado = await servico.UpdateAsync(dto);

            Assert.True(resultado);
            Assert.Equal("Concluída", existente.Status);
        }

        private sealed class FakeUserService : IUserService
        {
            private readonly User _usuarioLogado;
            private readonly string _emailLogado;

            public FakeUserService(User usuarioLogado, string emailLogado)
            {
                _usuarioLogado = usuarioLogado;
                _emailLogado = emailLogado;
            }

            public string GetLoggedUserEmailAddress() => _emailLogado;

            public Task<User?> GetUserByEmailAsync(string email)
            {
                User? usuario = string.Equals(email, _emailLogado, StringComparison.OrdinalIgnoreCase)
                    ? _usuarioLogado
                    : null;

                return Task.FromResult(usuario);
            }

            public Task<CreateUserResult> CreateFreelancerAsync(CreateFreelancerDto dto) => throw new NotImplementedException();
            public Task<CreateUserResult> CreateCompanyAsync(CreateCompanyDto dto) => throw new NotImplementedException();
            public Task<UserDto?> GetUserByIdAsync(int id) => throw new NotImplementedException();
            public Task<int?> GetUserIdByEmailAsync(string email) => throw new NotImplementedException();
            public Task<PagedResult<UserDto>> GetAllUsersAsync(int page, int pageSize) => throw new NotImplementedException();
            public Task<PagedResult<GetFreelancerDto>> GetAllFreelancersAsync(int skip, int take) => throw new NotImplementedException();
            public Task<GetFreelancerDto?> GetFreelancerProfileByIdAsync(int userId) => throw new NotImplementedException();
            public Task<FreelancerPublicProfileDto?> GetFreelancerPublicProfileByIdAsync(int userId) => throw new NotImplementedException();
            public Task<GetCompanyDto?> GetCompanyProfileByIdAsync(int userId) => throw new NotImplementedException();
            public Task<CompanyPublicProfileDto?> GetCompanyPublicProfileByIdAsync(int userId) => throw new NotImplementedException();
            public Task<ProfileUpdateResult> UpdateUserFreelancerAsync(UpdateFreelancerDto dto, int userId) => throw new NotImplementedException();
            public Task<ProfileUpdateResult> UpdateUserCompanyAsync(UpdateCompanyDto dto, int userId) => throw new NotImplementedException();
            public Task<bool> DeleteUserByIdAsync(int id) => throw new NotImplementedException();
            public Task<AuthenticatedUserDto?> AuthenticateAsync(string email, string password) => throw new NotImplementedException();
        }

        private sealed class FakeUserRepository : IUserRepository
        {
            private readonly Dictionary<int, User> _usuarios = new();

            public void Adicionar(User usuario) => _usuarios[usuario.Id] = usuario;

            public Task<User?> GetUserByIdAsync(int id)
            {
                _usuarios.TryGetValue(id, out User? usuario);
                return Task.FromResult(usuario);
            }

            public Task<User> CreateUserAsync(User user) => throw new NotImplementedException();
            public Task<int?> CreateFreelancerUserProfileAsync(User user, FreelancerProfile profile) => throw new NotImplementedException();
            public Task<int?> CreateCompanyUserProfileAsync(User user, CompanyProfile profile) => throw new NotImplementedException();
            public Task<int> CountUsersAsync() => throw new NotImplementedException();
            public Task<ICollection<User>> GetAllUsersAsync(int skip, int take) => throw new NotImplementedException();
            public Task<ICollection<User>> GetAllFreelancersAsync(int skip, int take) => throw new NotImplementedException();
            public Task<int> CountFreelancersAsync() => throw new NotImplementedException();
            public Task<User?> GetUserByEmailAsync(string email) => throw new NotImplementedException();
            public Task<User?> GetByEmailAsync(string email) => throw new NotImplementedException();
            public Task<User?> GetFreelancerProfileAsync(int userId) => throw new NotImplementedException();
            public Task<User?> GetCompanyProfileAsync(int userId) => throw new NotImplementedException();
            public Task<ICollection<Roles>> GetUserRolesAsync(int userId) => throw new NotImplementedException();
            public Task UpdateFreelancerAsync(User user, FreelancerProfile profile) => throw new NotImplementedException();
            public Task UpdateCompanyAsync(User user, CompanyProfile profile) => throw new NotImplementedException();
            public Task AddRoleToUserAsync(User user, int roleId) => throw new NotImplementedException();
            public Task RemoveRoleFromUserAsync(User user, int roleId) => throw new NotImplementedException();
            public Task RemoveAllRolesFromUserAsync(int userId) => throw new NotImplementedException();
            public Task DeleteCurrentUserAsync(User user) => throw new NotImplementedException();
            public Task<bool> IsEmailRegistered(string email) => throw new NotImplementedException();
            public Task<bool> IsCpfRegistered(string cpf) => throw new NotImplementedException();
            public Task<bool> IsCnpjRegistered(string cnpj) => throw new NotImplementedException();
            public Task<string?> GetProfileImageKeyAsync(int userId) => throw new NotImplementedException();
            public Task<bool> TrySetProfileImageKeyAsync(int userId, string? expectedKey, string? newKey) => throw new NotImplementedException();
        }

        private sealed class FakeOrdemServicoRepository : IOrdemServicoRepository
        {
            private readonly Dictionary<int, OrdemServico> _ordens = new();
            private int _proximoId = 1000;

            public void Adicionar(OrdemServico ordem)
            {
                if (ordem.Id == 0)
                {
                    ordem.Id = _proximoId++;
                }

                _ordens[ordem.Id] = ordem;
            }

            public Task<List<OrdemServico>> ListAllAsync() => Task.FromResult(_ordens.Values.ToList());

            public Task<List<OrdemServico>> ListByUserIdAsync(int userId) => throw new NotImplementedException();

            public Task<OrdemServico?> GetByIdAsync(int id)
            {
                _ordens.TryGetValue(id, out OrdemServico? ordem);
                return Task.FromResult(ordem);
            }

            public Task<OrdemServico> CreateAsync(OrdemServico ordemServico)
            {
                Adicionar(ordemServico);
                return Task.FromResult(ordemServico);
            }

            public Task UpdateAsync(OrdemServico ordemServico)
            {
                _ordens[ordemServico.Id] = ordemServico;
                return Task.CompletedTask;
            }

            public Task DeleteAsync(OrdemServico ordemServico) => throw new NotImplementedException();

            public Task<List<OrdemServico>> ListByFreelancerIdAsync(int freelancerId) => throw new NotImplementedException();

            public Task<List<OrdemServico>> ListByEmpresaIdAsync(int empresaUserId) => throw new NotImplementedException();
        }
    }
}
