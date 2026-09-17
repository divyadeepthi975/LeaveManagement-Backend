using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeavetypeService :ILeavetypeService
    {
        private readonly AppDbContext _db;
        public LeavetypeService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Leavetype> AddAsync(LeavetypeDTO leavetype)
        {
            bool exists = await _db.Leavetypes
            .AnyAsync(l => l.leavetypename.ToLower() == leavetype.leavetypename.ToLower());
            if (exists)
            {
                throw new Exception("Leave type with this name already exists.");
            }

            Leavetype leave = new Leavetype();
            leave.leavetypename = leavetype.leavetypename;
            leave.maximumdays= leavetype.maximumdays;
            _db.Leavetypes.Add(leave);
            await _db.SaveChangesAsync();

            var employees = await _db.Employees 
            .Where(e => e.IsActive==true)
            .ToListAsync();
            foreach (var employee in employees)
            {
                Leavebalance balance = new Leavebalance();

                balance.employeeid = employee.EmployeeId;
                balance.leavetypeid = leave.leavetypeid;
                balance.totaldays = leave.maximumdays;
                balance.useddays = 0;

                _db.Leavebalances.Add(balance);
            }
            await _db.SaveChangesAsync();

            return leave;
        }

        public async Task<string> DeleteAsync(int id)
        {
            var objLeavetype = await _db.Leavetypes.FirstOrDefaultAsync(l => l.leavetypeid == id);
            if (objLeavetype == null)
            {
                return "leavetype with this ID doesnt exists";
            }
            if (objLeavetype.IsActive == false)
            {
                return "leavetype with this id is inactive";
            }
            objLeavetype.IsActive = false;
            await _db.SaveChangesAsync();
            return $"leavetype with the id-{id} is successfully deleted";
        }

        public async Task<IEnumerable<Leavetype>> GetAllLeavetypeAsync()
        {
            return await _db.Leavetypes.ToListAsync();
        }

        public async Task<Leavetype?> GetLeavetypeAsync(int id)
        {
            Leavetype? leavetype=await _db.Leavetypes.FindAsync(id);
            if(leavetype == null)
            {
                return null;
            }
            return leavetype;

        }

        public async Task<Leavetype?> UpdateLeavetypeAsync(LeavetypeIsactiveDTO leavetype)
        {
            var objLeavetype = await _db.Leavetypes.FirstOrDefaultAsync(l => l.leavetypeid == leavetype.leavetypeid);
            if (objLeavetype == null)
            {
                return null;
            }
            objLeavetype.leavetypeid = leavetype.leavetypeid;
            objLeavetype.leavetypename = leavetype.leavetypename;
            objLeavetype.maximumdays = leavetype.maximumdays;
            objLeavetype.IsActive = leavetype.IsActive;
            await _db.SaveChangesAsync();
            return objLeavetype;

        }
    }
}
