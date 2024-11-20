using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mime;

class Program
{
    
    // Log file location (from App.config)
    private static readonly string logFileLoc = ConfigurationManager.AppSettings["log_file_loc"];

    static async Task Main(string[] args)
    {
        // Call the method to check and send birthday emails
        await CheckAndSendBirthdayEmailsAsync();
    }

    public static async Task CheckAndSendBirthdayEmailsAsync()
    {
        var today = DateTime.Today;

        // Query the database for users whose birthday is today
        var usersWithBirthdayToday = GetUsersWithBirthday(today);

        // Send birthday emails to each user
        foreach (var user in usersWithBirthdayToday)
        {
            await SendBirthdayEmailAsync(user.Email, user.Name);
        }
    }

    private static List<User> GetUsersWithBirthday(DateTime date)
    {
        var users = new List<User>();

        // Query the database for users whose birthday is today
        string query = "SELECT Id, Name, Email, DateOfBirth FROM BUsers WHERE MONTH(DateOfBirth) = @month AND DAY(DateOfBirth) = @day";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@day", date.Day);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            DateOfBirth = (DateTime)reader["DateOfBirth"]
                        });
                    }
                }
            }
        }

        return users;
    }

    public static async Task SendBirthdayEmailAsync(string toEmail, string toName)
    {
        var fromEmail = new MailAddress(smtpUser, "Birthday Notifier");
        var toAddress = new MailAddress(toEmail, toName);
        var subject = "Happy Birthday!";

        // HTML body with an embedded image
        var body = $@"
    <html>
        <body>
            <p>Dear {toName},</p>
            <p>Happy Birthday! Wishing you a wonderful year ahead.</p>
            <p>Best wishes,<br>Your Birthday Notifier</p>
            <img src='cid:BirthdayImage' />
        </body>
    </html>";

        using (var smtp = new SmtpClient(smtpHost, smtpPort))
        {
            smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            smtp.EnableSsl = false;

            try
            {
                // Enable logging to capture more information
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = 30000; // Set a longer timeout if needed

                using (var message = new MailMessage(fromEmail, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    // Dynamically set the image path
                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BirthdayEmailer\img\Birthday.jpg");

                    // Ensure the file exists before proceeding
                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"Image not found at {imagePath}");
                        return;
                    }

                    // Add the image as an embedded attachment
                    var imageResource = new LinkedResource(imagePath)
                    {
                        ContentId = "BirthdayImage", // This is the ID referenced in the HTML body
                        TransferEncoding = TransferEncoding.Base64,
                        ContentType = new System.Net.Mime.ContentType("image/jpg")
                    };

                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    htmlView.LinkedResources.Add(imageResource);
                    message.AlternateViews.Add(htmlView);

                    // Send the email asynchronously
                    await smtp.SendMailAsync(message);
                    Console.WriteLine($"Birthday email sent to {toEmail}");
                    LogEmailSent(toEmail); // Log the action
                }
            }
            catch (SmtpException smtpEx)
            {
                // More detailed error information for SMTP issues
                Console.WriteLine($"SMTP error sending email to {toEmail}: {smtpEx.Message}");
                LogError($"SMTP error sending email to {toEmail}: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                // General error handling
                Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
                LogError($"Error sending email to {toEmail}: {ex.Message}");
            }
        }
    }


    private static void LogEmailSent(string email)
    {
        string logMessage = $"Email sent to: {email} at {DateTime.Now}";
        WriteLog(logMessage);
    }

    private static void LogError(string errorMessage)
    {
        string logMessage = $"ERROR: {errorMessage} at {DateTime.Now}";
        WriteLog(logMessage);
    }

    private static void WriteLog(string message)
    {
        try
        {
            // Append the log message to the log file
            using (StreamWriter sw = new StreamWriter(logFileLoc, true))
            {
                sw.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            // Handle logging failure (e.g., file not accessible)
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }

    // User class to represent the user in the database
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
