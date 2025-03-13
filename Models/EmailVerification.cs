
using System;

public class EmailVerification
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Code { get; set; } // كود التحقق
    public DateTime ExpiryTime { get; set; } // وقت انتهاء صلاحية الكود
}
