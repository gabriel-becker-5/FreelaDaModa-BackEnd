using _02_Application.DTOs.Freelancer;
using _02_Application.Interfaces;
using _03_Infrastructure.Data;
using _04_Domain.Entities.ObjectsFields;
using Microsoft.EntityFrameworkCore;

namespace _03_Infrastructure.Repositories
{
    public class FreelancerFieldsRepository : IFreelancerFieldsRepository
    {
        private readonly AppDbContext _context;

        public FreelancerFieldsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AvailableTimeExistsAsync(string availableTimeName)
        {
            AvailableTime? result = await _context.AvailableTimes
                                    .Where(at => at.AvailableTimeName == availableTimeName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<AvailableTime> CreateAvailableTimeAsync(AvailableTime field)
        {
            _context.AvailableTimes.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<AverageRevenue> CreateAverageRevenueAsync(AverageRevenue field)
        {
            _context.AverageRevenues.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<BusinessType> CreateBusinessTypeAsync(BusinessType field)
        {
            _context.BusinessTypes.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<ExperienceYears> CreateExperienceYearsAsync(ExperienceYears field)
        {
            _context.ExperienceYears.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<FreelancerPreferences> CreateFreelancerPreferencesAsync(FreelancerPreferences field)
        {
            _context.FreelancerPreferences.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<HowUsuallyArrangeServices> CreateHowUsuallyArrangeServicesAsync(HowUsuallyArrangeServices field)
        {
            _context.HowUsuallyArrangeServices.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<WorkshopSize> CreateWorkshopSizeAsync(WorkshopSize field)
        {
            _context.WorkshopSizes.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<Specialty> CreateSpecialtyAsync(Specialty field)
        {
            _context.Specialties.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<OwnMachine> CreateOwnMachineAsync(OwnMachine field)
        {
            _context.OwnMachines.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<FreelancerProfileDto?> GetProfileFieldsNames(int freelancerId)
        {
            return await _context.FreelancersProfiles
                           .Where(f => f.Id == freelancerId)
                           .Select(f => new FreelancerProfileDto
                           {
                               Id = f.Id,
                               BusinessTypeName = f.BusinessType.BusinessTypeName,
                               ExperienceYearsName = f.ExperienceYears.ExperienceYearsName,
                               WorkshopSizeName = f.WorkshopSize.WorkshopSizeName,
                               HowUsuallyArrangeServicesName = f.HowUsuallyArrangeServices.HowUsuallyArrangeServicesName,
                               AvailableTimeName = f.AvailableTime.AvailableTimeName,
                               FreelancerPreferencesName = f.FreelancerPreferences.FreelancerPreferencesName,
                               AverageRevenueName = f.AverageRevenue.AverageRevenueName,
                               HasFixedProducer = f.HasFixedProducer,
                               HasOwnCar = f.HasOwnCar,
                               Specialties = f.FreelancerSpecialties
                                                   .Select(fs => fs.Specialty.SpecialtyName)
                                                   .ToList(),
                               OwnMachines = f.FreelancerOwnMachines
                                                   .Select(fom => fom.OwnMachine.OwnMachineName)
                                                   .ToList(),
                               BirthDate = f.BirthDate
                           }).FirstOrDefaultAsync();
        }



        public async Task<bool> AvailableTimeExistsAsync(int availableTimeId)
        {
            AvailableTime? result = await _context.AvailableTimes.FindAsync(availableTimeId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> BusinessTypeExistsAsync(string businessTypeName)
        {
            BusinessType? result = await _context.BusinessTypes
                                    .Where(bt => bt.BusinessTypeName == businessTypeName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> BusinessTypeExistsAsync(int businessTypeId)
        {
            BusinessType? result = await _context.BusinessTypes.FindAsync(businessTypeId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ExperienceYearsExistsAsync(string experienceYearsName)
        {
            ExperienceYears? result = await _context.ExperienceYears
                                    .Where(ey => ey.ExperienceYearsName == experienceYearsName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ExperienceYearsExistsAsync(int experienceYearsId)
        {
            ExperienceYears? result = await _context.ExperienceYears.FindAsync(experienceYearsId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> WorkshopSizeExistsAsync(string workshopSizeName)
        {
            WorkshopSize? result = await _context.WorkshopSizes
                                    .Where(ws => ws.WorkshopSizeName == workshopSizeName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> WorkshopSizeExistsAsync(int workshopSizeId)
        {
            WorkshopSize? result = await _context.WorkshopSizes.FindAsync(workshopSizeId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AverageRevenueExistsAsync(string averageRevenueName)
        {
            AverageRevenue? result = await _context.AverageRevenues
                                    .Where(ar => ar.AverageRevenueName == averageRevenueName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AverageRevenueExistsAsync(int averageRevenueId)
        {
            AverageRevenue? result = await _context.AverageRevenues.FindAsync(averageRevenueId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HowUsuallyArrangeServicesExistsAsync(string howUsuallyArrangeServicesName)
        {
            HowUsuallyArrangeServices? result = await _context.HowUsuallyArrangeServices
                                    .Where(huas => huas.HowUsuallyArrangeServicesName == howUsuallyArrangeServicesName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HowUsuallyArrangeServicesExistsAsync(int howUsuallyArrangeServicesId)
        {
            HowUsuallyArrangeServices? result = await _context.HowUsuallyArrangeServices.FindAsync(howUsuallyArrangeServicesId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> FreelancerPreferencesExistsAsync(string freelancerPreferencesName)
        {
            FreelancerPreferences? result = await _context.FreelancerPreferences
                                    .Where(fp => fp.FreelancerPreferencesName == freelancerPreferencesName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> FreelancerPreferencesExistsAsync(int freelancerPreferencesId)
        {
            FreelancerPreferences? result = await _context.FreelancerPreferences.FindAsync(freelancerPreferencesId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SpecialtyExistsAsync(string specialtyRegisterName)
        {
            Specialty? result = await _context.Specialties
                                    .Where(s => s.SpecialtyName == specialtyRegisterName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SpecialtyExistsAsync(int specialtyRegisterId)
        {
            Specialty? result = await _context.Specialties.FindAsync(specialtyRegisterId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SpecialtyExistsAsync(ICollection<int> specialtyRegisterId)
        {
            List<Specialty>? result = await _context.Specialties
                                                    .Where(s => specialtyRegisterId.Contains(s.Id))
                                                    .ToListAsync();

            if (result.Count != specialtyRegisterId.Count)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> OwnMachinesExistsAsync(string OwnMachineName)
        {
            OwnMachine? result = await _context.OwnMachines
                                    .Where(s => s.OwnMachineName == OwnMachineName)
                                    .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> OwnMachinesExistsAsync(int ownMachinesId)
        {
            OwnMachine? result = await _context.OwnMachines.FindAsync(ownMachinesId);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> OwnMachinesExistsAsync(ICollection<int> ownMachinesId)
        {
            List<OwnMachine>? result = await _context.OwnMachines
                                                    .Where(om => ownMachinesId.Contains(om.Id))
                                                    .ToListAsync();

            if (result.Count != ownMachinesId.Count)
            {
                return false;
            }

            return true;
        }

        public async Task<ICollection<AvailableTime>> GetAllAvailableTimes()
        {
            List<AvailableTime> result = await _context.AvailableTimes.ToListAsync();
            return result;
        }

        public async Task<ICollection<AverageRevenue>> GetAllAverageRevenues()
        {
            List<AverageRevenue> result = await _context.AverageRevenues.ToListAsync();
            return result;
        }

        public async Task<ICollection<BusinessType>> GetAllBusinessTypes()
        {
            List<BusinessType> result = await _context.BusinessTypes.ToListAsync();
            return result;
        }

        public async Task<ICollection<ExperienceYears>> GetAllExperienceYears()
        {
            List<ExperienceYears> result = await _context.ExperienceYears.ToListAsync();
            return result;
        }

        public async Task<ICollection<FreelancerPreferences>> GetAllFreelancerPreferences()
        {
            List<FreelancerPreferences> result = await _context.FreelancerPreferences.ToListAsync();
            return result;
        }

        public async Task<ICollection<HowUsuallyArrangeServices>> GetAllHowUsuallyArrangeServices()
        {
            List<HowUsuallyArrangeServices> result = await _context.HowUsuallyArrangeServices.ToListAsync();
            return result;
        }

        public async Task<ICollection<Specialty>> GetAllSpecialties()
        {
            List<Specialty> result = await _context.Specialties.ToListAsync();
            return result;
        }

        public async Task<ICollection<OwnMachine>> GetAllOwnMachines()
        {
            List<OwnMachine> result = await _context.OwnMachines.ToListAsync();
            return result;
        }

        public async Task<ICollection<WorkshopSize>> GetAllWorkshopSizes()
        {
            List<WorkshopSize> result = await _context.WorkshopSizes.ToListAsync();
            return result;
        }


        public async Task<AvailableTime?> GetAvailableTimeByIdAsync(int id)
        {
            AvailableTime? result = await _context.AvailableTimes.FindAsync(id);
            return result;
        }

        public async Task<AverageRevenue?> GetAverageRevenueByIdAsync(int id)
        {
            AverageRevenue? result = await _context.AverageRevenues.FindAsync(id);
            return result;
        }

        public async Task<BusinessType?> GetBusinessTypeByIdAsync(int id)
        {
            BusinessType? result = await _context.BusinessTypes.FindAsync(id);
            return result;
        }

        public async Task<ExperienceYears?> GetExperienceYearsByIdAsync(int id)
        {
            ExperienceYears? result = await _context.ExperienceYears.FindAsync(id);
            return result;
        }

        public async Task<FreelancerPreferences?> GetFreelancerPreferenceByIdAsync(int id)
        {
            FreelancerPreferences? result = await _context.FreelancerPreferences.FindAsync(id);
            return result;
        }

        public async Task<HowUsuallyArrangeServices?> GetHowUsuallyArrangeServiceByIdAsync(int id)
        {
            HowUsuallyArrangeServices? result = await _context.HowUsuallyArrangeServices.FindAsync(id);
            return result;
        }

        public async Task<Specialty?> GetSpecialtyByIdAsync(int id)
        {
            Specialty? result = await _context.Specialties.FindAsync(id);
            return result;
        }

        public async Task<OwnMachine?> GetOwnMachineByIdAsync(int id)
        {
            OwnMachine? result = await _context.OwnMachines.FindAsync(id);
            return result;
        }

        public async Task<WorkshopSize?> GetWorkshopSizeByIdAsync(int id)
        {
            WorkshopSize? result = await _context.WorkshopSizes.FindAsync(id);
            return result;
        }


        public async Task UpdateAvailableTimeAsync(AvailableTime availableTime)
        {
            _context.AvailableTimes.Update(availableTime);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAverageRevenueAsync(AverageRevenue averageRevenue)
        {
            _context.AverageRevenues.Update(averageRevenue);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBusinessTypeAsync(BusinessType businessType)
        {
            _context.BusinessTypes.Update(businessType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExperienceYearsAsync(ExperienceYears experienceYears)
        {
            _context.ExperienceYears.Update(experienceYears);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFreelancerPreferencesAsync(FreelancerPreferences freelancerPreferences)
        {
            _context.FreelancerPreferences.Update(freelancerPreferences);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHowUsuallyArrangeServicesAsync(HowUsuallyArrangeServices howUsuallyArrangeServices)
        {
            _context.HowUsuallyArrangeServices.Update(howUsuallyArrangeServices);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOwnMachineAsync(OwnMachine ownMachine)
        {
            _context.OwnMachines.Update(ownMachine);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSpecialtyAsync(Specialty specialty)
        {
            _context.Specialties.Update(specialty);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWorkshopSizeAsync(WorkshopSize workshopSize)
        {
            _context.WorkshopSizes.Update(workshopSize);
            await _context.SaveChangesAsync();
        }


        public async Task<bool?> DeleteAvailableTimeAsync(int id)
        {
            AvailableTime? result = await _context.AvailableTimes.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }            
        }

        public async Task<bool?> DeleteAverageRevenueAsync(int id)
        {
            AverageRevenue? result = await _context.AverageRevenues.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteBusinessTypeAsync(int id)
        {
            BusinessType? result = await _context.BusinessTypes.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteExperienceYearAsync(int id)
        {
            ExperienceYears? result = await _context.ExperienceYears.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteFreelancerPreferenceAsync(int id)
        {
            FreelancerPreferences? result = await _context.FreelancerPreferences.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteHowUsuallyArrangeServiceAsync(int id)
        {
            HowUsuallyArrangeServices? result = await _context.HowUsuallyArrangeServices.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteSpecialtyAsync(int id)
        {
            Specialty? result = await _context.Specialties.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteOwnMachineAsync(int id)
        {
            OwnMachine? result = await _context.OwnMachines.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool?> DeleteWorkshopSizeAsync(int id)
        {
            WorkshopSize? result = await _context.WorkshopSizes.FindAsync(id);

            if (result == null)
            {
                return null;
            }

            try
            {
                _context.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}