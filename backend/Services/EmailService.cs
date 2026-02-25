using System.Net;
using System.Net.Mail;

public class EmailService
{
    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("mshiyas9526@gmail.com", "opxd elro xhbl zhhi"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("yourgmail@gmail.com"),
            Subject = "Password Reset OTP",
            Body = $"Your OTP is: {otp}. Valid for 5 minutes.",
            IsBodyHtml = false,
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
