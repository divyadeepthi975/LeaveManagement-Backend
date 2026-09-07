using LeaveManagement.Data;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeavetypeService
    {
        private readonly AppDbContext _db;
        public LeavetypeService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Leavetype> AddAsync(Leavetype leavetype)
        {
            _db.Leavetypes.Add(leavetype);
            await _db.SaveChangesAsync();
            return leavetype;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var objLeavetype = await _db.Leavetypes.FirstOrDefaultAsync(l => l.leavetypeid == id);
            if (objLeavetype == null)
            {
                return false;
            }
            _db.Leavetypes.Remove(objLeavetype);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Leavetype>> GetAllLeavetypeAsync()
        {
            return await _db.Leavetypes.ToListAsync();
        }

        public async Task<Leavetype?> GetLeavetypeAsync(int id)
        {
            return await _db.Leavetypes.FindAsync(id);

        }

        public async Task<Leavetype?> UpdateAsync(Leavetype leavetype)
        {
            var objLeavetype = await _db.Leavetypes.FirstOrDefaultAsync(l => l.leavetypeid == leavetype.leavetypeid);
            if (objLeavetype == null)
            {
                return null;
            }
            objLeavetype.leavetypeid = leavetype.leavetypeid;
            objLeavetype.leavetypename = leavetype.leavetypename;
            objLeavetype.maximumdays = leavetype.maximumdays;
            await _db.SaveChangesAsync();
            return objLeavetype;

        }
    }
}
