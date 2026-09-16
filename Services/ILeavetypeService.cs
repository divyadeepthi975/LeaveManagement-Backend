using LeaveManagement.Models.Entities;
using LeaveManagement.DTO;

namespace LeaveManagement.Services
{
    public interface ILeavetypeService
    {
        Task<Leavetype> AddAsync(LeavetypeDTO leavetype);

        Task<string> DeleteAsync(int id);

        Task<IEnumerable<Leavetype>> GetAllLeavetypeAsync();

        Task<Leavetype?> GetLeavetypeAsync(int id);

        Task<Leavetype?> UpdateLeavetypeAsync(Leavetype leavetype);
    }
}