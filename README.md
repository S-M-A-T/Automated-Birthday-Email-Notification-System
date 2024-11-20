# Automated Birthday Email Notification System

A robust console application built using C# that automatically sends personalized birthday emails to users. The application fetches user data from a SQL Server database, composes birthday emails with an embedded image, and logs all email-sending activities for tracking and troubleshooting.

---

## Features

### 🎂 Birthday Email Notifications
- Automatically identifies users with birthdays on the current date.
- Sends personalized HTML emails with the recipient's name and an embedded image.
- Uses SMTP for reliable and secure email delivery.

### 🗄️ Database Integration
- Connects to a SQL Server database to retrieve user details.
- Filters users based on the current month and day to identify birthdays.

### 📝 Logging
- Logs all email-sending events to a specified file for transparency.
- Captures and logs errors (e.g., SMTP failures) with detailed descriptions.

### ⚙️ Configurable
- Easily configurable SMTP and database settings using the `App.config` file.
- Dynamic paths for embedded images and log files.

---

## Technologies Used

- **Programming Language:** C# (.NET Framework)
- **Database:** SQL Server
- **Email Service:** SMTP (System.Net.Mail)
- **Configuration Management:** App.config for settings (SMTP, database connection, log file path)

---

## System Architecture

1. **Database Query:**
   - Retrieves user details (`Id`, `Name`, `Email`, `DateOfBirth`) from the `BUsers` table.
   - Filters based on the current date using SQL queries.

2. **Email Composition:**
   - Creates an HTML email with a birthday message and an embedded image.
   - Embeds the image as a `LinkedResource` to ensure proper rendering in email clients.

3. **Email Sending:**
   - Utilizes the `SmtpClient` class for sending emails.
   - Handles SMTP errors gracefully and retries if necessary.

4. **Logging:**
   - Records all email-sending activities in a log file.
   - Logs errors with timestamps for debugging and tracking.

---

## Installation

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/S-M-A-T/birthday-email-notification.git
   cd birthday-email-notification
   ```

2. **Configure Settings:**
   - Open the `App.config` file.
   - Update the following settings:
     - **SMTP Host**: Set your SMTP server details.
     - **SMTP Port**: Specify the SMTP port.
     - **SMTP Credentials**: Add your email address and password.
     - **Database Connection String**: Configure the SQL Server connection.

3. **Build and Run:**
   - Open the project in Visual Studio.
   - Build and run the solution.
     
---

## Usage

- Run the application to automatically send birthday emails to users whose birthdays match the current date.
- Emails are sent asynchronously to ensure non-blocking operations.
- Check the log file for details about sent emails and any errors.

---

## Example Log Output

```plaintext
Email sent to: user@example.com at 2024-11-20 10:30:45
Email sent to: anotheruser@example.com at 2024-11-20 10:31:12
ERROR: SMTP error sending email to invaliduser@example.com: Unable to connect to server at 2024-11-20 10:32:01
```



---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

Feel free to fork, contribute, or suggest improvements! 🎉

