using BlazorServerExpensesTracker.Data.Configuration;
using BlazorServerExpensesTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerExpensesTracker.Data.Services
{
    public class ServiceImplementation : ServiceInterface
    {
        private readonly AppDbContext conn;

        public ServiceImplementation(AppDbContext conn)
        {
            this.conn = conn;
        }


        // add new expenses
        public async Task<(bool flag, string message)> AddOrUpdateExpensesAsync(Expenses expenses)
        {
            if (expenses.Id == 0)
            {
                conn.Expenses.Add(expenses);
                await conn.SaveChangesAsync();
                return (true, "Saved");
            }
            else
            {
                var result = await conn.Expenses.FirstOrDefaultAsync(_ => _.Id == expenses.Id);
                result.Name = expenses.Name;
                result.Amount = expenses.Amount;
                result.DateAdded = expenses.DateAdded;
                await conn.SaveChangesAsync();
                return (true, "Updated");
            }
        }

        //Get all expenses
        public async Task<List<Expenses>> GetAllExpensesAsync() => await conn.Expenses.ToListAsync();

        public async Task<bool> DeleteExpensesAsync(int id)
        {
            var result = await conn.Expenses.FirstOrDefaultAsync(_ => _.Id == id);
            if (result is null)
                return false;

            conn.Expenses.Remove(result);
            await conn.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddFundAsync(Fund fund)
        {
            try
            {
                var alreadyFund = await conn.Funds.Where(_ => _.Amount >= 0).FirstOrDefaultAsync();
                if (alreadyFund is not null)
                {

                    decimal curerntamount = alreadyFund.Amount + fund.Amount;
                    if (curerntamount >= 0)
                    {
                        alreadyFund.Amount = curerntamount;
                        await conn.SaveChangesAsync();
                        return 2;
                    }
                    return 3;

                }

                if (fund.Amount < 0)
                    return 4;

                conn.Funds.Add(fund);
                await conn.SaveChangesAsync();
                return 1;
            }
            catch (Exception) { return -1; }
        }

        public async Task<decimal> GetAvailableFund()
        {
            var fund = await conn.Funds.ToListAsync();
            return fund.Select(_ => _.Amount).Sum();
        }

        public async Task<(bool flag, string message)> AddNoteAsync(Note note)
        {
            conn.Notes.Add(note);
            await conn.SaveChangesAsync();
            return (true, "Saved");
        }

        public async Task<(bool flag, string message)> DeleteNoteAsync(int id)
        {
            var getNote = await conn.Notes.FirstOrDefaultAsync(_ => _.Id == id);
            if (getNote != null)
            {
                conn.Notes.Remove(getNote);
                await conn.SaveChangesAsync();
                return (true, "Delete");
            }
            return (false, "Note not found");
        }

        public async Task<List<Note>> GetNoteAsync() => await conn.Notes.ToListAsync();
    }
}
