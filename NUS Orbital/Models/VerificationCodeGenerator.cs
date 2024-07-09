namespace NUS_Orbital.Models
{
    public class VerificationCodeGenerator
    {
        public static string GenerateCode()
        {
            Random random = new Random();
            int code = random.Next(100000, 999999);
            return code.ToString();
        }
    }
}
