using LeaveManagement.Models.Entities;

namespace LeaveManagement.Services
{
    public interface ILeavetypeService
    {
        Task<Leavetype> AddAsync(Leavetype leavetype);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Leavetype>> GetAllLeavetypeAsync();

        Task<Leavetype?> GetLeavetypeAsync(int id);

        Task<Leavetype?> UpdateAsync(Leavetype leavetype);
    }
}