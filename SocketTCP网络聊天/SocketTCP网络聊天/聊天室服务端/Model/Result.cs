using System;
using System.Collections.Generic;
using System.Text;

public class Result
{

    public Result(int id, int userId, int totalAmount, int winAmount)
    {
        this.Id = id;
        this.UserId = userId;
        this.TotalAmount = totalAmount;
        this.WinAmount = winAmount;
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public int TotalAmount { get; set; }
    public int WinAmount { get; set; }
   
}

