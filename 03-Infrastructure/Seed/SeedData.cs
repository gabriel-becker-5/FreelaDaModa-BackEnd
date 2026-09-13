using _02_Application.Authorization;
using _02_Application.DTOs;
using _02_Application.DTOs.Freelancer;
using _02_Application.DTOs.User;
using _02_Application.Interfaces;
using _04_Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace _03_Infrastructure.Seed
{
    public class SeedData
    {
        public static async Task Initializer(IRoleService roleService,
                                     IUserService userService, IFreelancerFieldsService freelancerFieldsService)
        {
            // Cria todos os Cargos/Roles, se não existirem
            for (int i = 0; i < Roles.All.Count; i++)
            {
                await roleService.CreateRoleAsync(Roles.All[i]);
            }

            // Cria o usuário inicial de Admin
            UserDto adminUser = new()
            {
                LegalResponsibleFullName = "Administrador do Sistema",
                LegalResponsibleDocument = "00000000000",
                Email = "admin@admin.com",
                ContactNumber = "0000000000",
                PostalCode = "00000000",
                Address = "N/A",
                AddressNumber = 0,
                Quarter = "N/A",
                City = "Blumenau",
                State = "SC"
            };

            int? user = await userService.CreateAdminUserAsync(adminUser);

            // Atribui o cargo de “Admin” ao usuário inicial
            int? roleAdminId = await roleService.GetRoleIdByNameAsync(Roles.Admin);

            if (user != null && roleAdminId != null)
            {
                await userService.CreateUserRoleAsync((int)user, (int)roleAdminId);
            }

            // Cria AvailableTimes | Disponibilidade de Tempo
            List<FieldsFreelancerDto> availableTimes = new()
            {
                new() { RegisterName = "Período integral" },
                new() { RegisterName = "Meio período" },
                new() { RegisterName = "Fins de semana" },
                new() { RegisterName = "Sob demanda" }
            };

            for (int i = 0; i < availableTimes.Count; i++)
            {
                await freelancerFieldsService.CreateAvailableTimeAsync(availableTimes[i]);
            }

            // Cria BusinessType | Tipo de Negócio
            List<FieldsFreelancerDto> businessTypes = new()
            {
                new() { RegisterName = "MEI" },
                new() { RegisterName = "ME" },
                new() { RegisterName = "SEM CNPJ" }
            };

            for (int i = 0; i < businessTypes.Count; i++)
            {
                await freelancerFieldsService.CreateBusinessTypeAsync(businessTypes[i]);
            }

            // Cria ExperienceYears | Tempo de Experiência
            List<FieldsFreelancerDto> experienceYears = new()
            {
                new() { RegisterName = "Menos de 1 ano" },
                new() { RegisterName = "1 a 3 anos" },
                new() { RegisterName = "3 a 5 anos" },
                new() { RegisterName = "5 a 10 anos" },
                new() { RegisterName = "mais de 10 anos" },
            };

            for (int i = 0; i < experienceYears.Count; i++)
            {
                await freelancerFieldsService.CreateExperienceYearsAsync(experienceYears[i]);
            }

            // Cria WorkshopSize | Tamanho da Oficina
            List<FieldsFreelancerDto> workshopSizes = new()
            {
                new() { RegisterName = "Apenas eu" },
                new() { RegisterName = "2 a 3 pessoas" },
                new() { RegisterName = "4 a 6 pessoas" },
                new() { RegisterName = "mais de 6 pessoas" }
            };

            for (int i = 0; i < workshopSizes.Count; i++)
            {
                await freelancerFieldsService.CreateWorkshopSizeAsync(workshopSizes[i]);
            }

            // Cria AverageRevenue | Faturamento médio atual
            List<FieldsFreelancerDto> averageRevenue = new()
            {
                new() { RegisterName = "até R$ 1000" },
                new() { RegisterName = "de R$ 1000 a R$ 3000 mil" },
                new() { RegisterName = "de R$ 3000 a R$ 6000" },
                new() { RegisterName = "acima de R$ 6000" },
                new() { RegisterName = "Prefiro não informar" },
            };

            for (int i = 0; i < averageRevenue.Count; i++)
            {
                await freelancerFieldsService.CreateAverageRevenueAsync(averageRevenue[i]);
            }

            // Cria HowUsuallyArrangeServices | Como costuma fechar serviços
            List<FieldsFreelancerDto> howUsuallyArrangeServices = new()
            {
                new() { RegisterName = "Grupos whatsapp" },
                new() { RegisterName = "Indicação" },
                new() { RegisterName = "Redes sociais" },
                new() { RegisterName = "Plataformas online" },
                new() { RegisterName = "Outros (cite)" }
            };

            for (int i = 0; i < howUsuallyArrangeServices.Count; i++)
            {
                await freelancerFieldsService.CreateHowUsuallyArrangeServicesAsync(howUsuallyArrangeServices[i]);
            }

            // Cria FreelancerPreferences | Preferências do Freelancer
            List<FieldsFreelancerDto> freelancerPreferences = new()
            {
                new() { RegisterName = "Serviços pontuais" },
                new() { RegisterName = "Contrato fixo" },
                new() { RegisterName = "Grandes lotes" },
                new() { RegisterName = "Peças exclusivas/autorais" }
            };

            for (int i = 0; i < freelancerPreferences.Count; i++)
            {
                await freelancerFieldsService.CreateFreelancerPreferencesAsync(freelancerPreferences[i]);
            }

            // Cria Specialties | Especialidades
            List<FieldsFreelancerDto> specialties = new()
            {
                new() { RegisterName = "Costura reta" },
                new() { RegisterName = "Overloque" },
                new() { RegisterName = "Modelagem" },
                new() { RegisterName = "Peça piloto" },
                new() { RegisterName = "Amostras" },
                new() { RegisterName = "Acabamento" },
                new() { RegisterName = "Bordado" },
                new() { RegisterName = "Estamparia" }
            };

            for (int i = 0; i < specialties.Count; i++)
            {
                await freelancerFieldsService.CreateSpecialtyAsync(specialties[i]);
            }

            // Cria OwnMachines | Máquinas que possui
            List<FieldsFreelancerDto> ownMachines = new()
            {
                new() { RegisterName = "Reta" },
                new() { RegisterName = "Overloque" },
                new() { RegisterName = "Galoneira" },
                new() { RegisterName = "Travete" },
                new() { RegisterName = "Interlock" },
                new() { RegisterName = "Máquina de corte" }
            };

            for (int i = 0; i < ownMachines.Count; i++)
            {
                await freelancerFieldsService.CreateOwnMachineAsync(ownMachines[i]);
            }
        }
    }
}